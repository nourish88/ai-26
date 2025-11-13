from fastapi import FastAPI, HTTPException, Depends
from fastapi.staticfiles import StaticFiles
from fastapi.responses import StreamingResponse
from server.openapi_utils import setup_custom_openapi
from request import RequestContextMiddleware,CustomExceptionMiddleware, RequestContext, get_request_context
import sys
import asyncio
import uvicorn
from server.configuration_manager import ConfigurationManager
from log.logger_factory import create_logger
from chat import ChatMessage,ChatApplication
from contextlib import asynccontextmanager
from cache import init_cache_helper,close_cache_helper
from integrations.admin_backend import AdminBackendService
from langfuse import Langfuse,get_client


logger = create_logger(__name__)

@asynccontextmanager
async def lifespan(app: FastAPI):
    # Startup
    await init_cache_helper()
    if ConfigurationManager.get_enable_agent_observability():
        Langfuse(
            public_key=ConfigurationManager.get_langfuse_public_key(),
            secret_key=ConfigurationManager.get_langfuse_secret_key(),
            host=ConfigurationManager.get_langfuse_address()
    )
    yield
    # Shutdown
    await close_cache_helper()
    if ConfigurationManager.get_enable_agent_observability():
        get_client().flush()

fast_api = FastAPI(lifespan=lifespan)
# Override the default OpenAPI schema
setup_custom_openapi(fast_api)


# Add middleware
fast_api.add_middleware(CustomExceptionMiddleware)
fast_api.add_middleware(RequestContextMiddleware)


fast_api.mount("/static", StaticFiles(directory="static"), name="static")

async def create_chat_application(application_identifier : str, user_id : str)->ChatApplication:
    chat_application = ChatApplication(application_identifier=application_identifier,user_id= user_id)
    await chat_application.initialize()
    return chat_application

@fast_api.post("/chat/blocking")
async def chat_block(chatMessage:ChatMessage,context: RequestContext = Depends(get_request_context)):    
    app_id = context.app_identifier
    user_id = context.user_id

    chat_application = await create_chat_application(application_identifier=app_id,user_id= user_id)
    message =await chat_application.chat(chat_message=chatMessage)
    return message

@fast_api.post("/chat/streaming")
async def chat_stream(chatMessage:ChatMessage,context: RequestContext = Depends(get_request_context))->StreamingResponse:
    app_id = context.app_identifier
    user_id = context.user_id

    chat_application = await create_chat_application(application_identifier=app_id,user_id= user_id)
    return StreamingResponse(chat_application.chat_stream(chat_message=chatMessage),media_type="text/event-stream")

@fast_api.get("/chat_history", response_model=list[ChatMessage], tags=["Chat History"])
async def chat_history(context: RequestContext = Depends(get_request_context)):
    app_identifier = context.app_identifier
    user_id = context.user_id

    chat_application = await create_chat_application(application_identifier=app_identifier,user_id= user_id)
    messages = await chat_application.chat_history()
    return messages

@fast_api.get("/chat_history_detail", response_model=list[ChatMessage], tags=["Chat History"])
async def chat_history_detail(thread_id: str, context: RequestContext = Depends(get_request_context)):
    app_identifier = context.app_identifier
    user_id = context.user_id

    chat_application = await create_chat_application(application_identifier=app_identifier,user_id= user_id)
    messages = await chat_application.chat_history_detail(thread_id)
    return messages

@fast_api.delete("/chat_history_detail", tags=["Chat History"])
async def delete_chat_history_detail(thread_id: str, context: RequestContext = Depends(get_request_context)):
    app_identifier = context.app_identifier
    user_id = context.user_id
    if not thread_id:
        raise HTTPException(status_code=404, detail="Thread Id is missing.")

    chat_application = await create_chat_application(application_identifier=app_identifier,user_id= user_id)
    await chat_application.delete_thread(thread_id=thread_id)
    return {}

@fast_api.post("/like_message", tags=["Message Actions"])
async def like_message(message_id: str, comment : str|None, context: RequestContext = Depends(get_request_context)):
    app_identifier = context.app_identifier
    user_id = context.user_id
    if not message_id:
        raise HTTPException(status_code=404, detail="Message Id is missing.")
    

    chat_application = await create_chat_application(application_identifier=app_identifier,user_id= user_id)
    result = await chat_application.like_message(message_id=message_id)    

    if result == 0:
        raise HTTPException(status_code=404, detail="Message not found.")
    
    if ConfigurationManager.get_enable_agent_observability():
        score_name = "helpfulness-user-comment"
        get_client().create_score(
            name=score_name,
            value=1,
            trace_id = message_id,
            data_type="BOOLEAN", 
            comment = comment if comment else  "Correct answer",
            score_id= message_id+score_name
        )

    return {}

@fast_api.post("/report_message", tags=["Message Actions"])
async def report_message(message_id: str, comment : str|None, context: RequestContext = Depends(get_request_context)):
    app_identifier = context.app_identifier
    user_id = context.user_id
    if not message_id:
        raise HTTPException(status_code=404, detail="Message Id is missing.")
    

    chat_application = await create_chat_application(application_identifier=app_identifier,user_id= user_id)
    result = await chat_application.dislike_message(message_id=message_id)
    if result == 0:
        raise HTTPException(status_code=404, detail="Message not found.")
    
    if ConfigurationManager.get_enable_agent_observability():
        score_name = "helpfulness-user-comment"
        get_client().create_score(
            name=score_name,
            value=0,
            trace_id = message_id,
            data_type="BOOLEAN", 
            comment = comment if comment else  "Incorrect answer",
            score_id= message_id+score_name
        )

    return {}

@fast_api.get("/applications", tags=["Application Operations"])
async def applications(context: RequestContext = Depends(get_request_context)):
    user_id = context.user_id
    admin_service = AdminBackendService()
    appsResponse = await admin_service.applications_for_ui(user_id=user_id)
    if not appsResponse or not appsResponse.result or not appsResponse.result.items:
        return []
    else:
        return appsResponse.result.items
    
@fast_api.get("/files/user", tags=["User Files"])
async def user_files(context: RequestContext = Depends(get_request_context)):
    user_id = context.user_id
    admin_service = AdminBackendService()
    response = await admin_service.application_user_files(user_id=user_id)
    if not response or not response.result:
        return []
    else:
        return response.result
    
@fast_api.delete("/files/user", tags=["Delete User File"])
async def delete_user_file(file_id: int, context: RequestContext = Depends(get_request_context)):
    app_identifier = context.app_identifier
    user_id = context.user_id
    if not file_id:
        raise HTTPException(status_code=404, detail="File Id is missing.")

    admin_service = AdminBackendService()
    response = await admin_service.delete_user_file(user_id=user_id,file_id=file_id)
    if not response:
        return {}
    else:
        return response

if __name__ == "__main__":
    if sys.platform=="win32":
        asyncio.set_event_loop_policy(asyncio.WindowsSelectorEventLoopPolicy()) 

    port = ConfigurationManager.get_app_port()
    uvicorn.run(fast_api, host="0.0.0.0", port=port)

