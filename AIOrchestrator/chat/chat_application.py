from log import create_logger
from typing import Optional,Any,AsyncGenerator,List
from applications import App,LlmModel, ApplicationFactory,check_message_guard_rails
from .models import ChatMessage
from datetime import datetime
from langgraph.graph.state import CompiledStateGraph
from .chat_store_mongo import ChatStoreMongo
from uuid import uuid4
from langchain_core.runnables import RunnableConfig
from langchain_core.messages import AIMessageChunk
from langfuse import get_client,observe
from langfuse.langchain import CallbackHandler
from server import ConfigurationManager
from integrations.admin_backend import AdminBackendService,ChatDetectionType

async def check_message(llm_model : LlmModel | None,chat_message:ChatMessage)->bool :
    if not llm_model or not chat_message.query or not chat_message.message_id or not chat_message.thread_id or not chat_message.user_id:
        return True
    result = await check_message_guard_rails(llm_model,chat_message.query)
    if not result:
        admin_service = AdminBackendService()
        appsResponse = await admin_service.chat_detection(
            chat_detection_type=ChatDetectionType.GuardRail
            ,message_id=chat_message.message_id
            ,reason="UNSAFE"
            ,sources=None
            ,thread_id=chat_message.thread_id
            ,user_id=chat_message.user_id
            ,user_message=chat_message.query
        )
    return result

class ChatApplication():
    is_initialize : bool
    application_identifier : str
    user_id : str
    app : Optional[App]
    chat_store : ChatStoreMongo
    def __init__(self,application_identifier:str, user_id : str):
        self.is_initialize = False
        self.logger = create_logger(__name__)
        self.application_identifier = application_identifier
        self.user_id = user_id        
    
    async def initialize(self)->None:
        if self.is_initialize:
            raise ValueError("Chat application is already initialized.")
        await self.set_application()
        self.chat_store = ChatStoreMongo(self.application_identifier)
        self.is_initialize = True

    @observe
    async def chat(self, chat_message : ChatMessage)->ChatMessage:
         self.check_is_initialized()
         self.prepeare_message(chat_message = chat_message)
         config,query = self.prepeare_context(chatMessage=chat_message)
         if not self.app:
              raise ValueError(f"Application not found for app {self.application_identifier}")
         
         if not self.app._graph:
              raise ValueError(f"Application graph not found for app {self.application_identifier}")
         check_message_result = True
         has_error = False
         if self.app.enableGuardRails:             
            check_message_result = await check_message(llm_model = self.app.model,chat_message=chat_message)
         if check_message_result:             
            graph : CompiledStateGraph = self.app._graph            
            try:
                response = await graph.ainvoke(
                    {
                        "messages": [query],
                        "user_id":self.user_id,
                        "thread_id":chat_message.thread_id,
                        "documents":chat_message.fileIdentifiers,
                        "message_id":chat_message.message_id,
                        "llm":self.app.model,
                        "retrieval_query":""
                    }
                    ,config=config
                    )
                ai_message = self.get_ai_message_from_response(response=response)         
            except Exception as ex:
                has_error = True
                ai_message = "Beklenmedik bir hata oluştu."
                self.logger.error("Graph invoke failed.")
                self.logger.error(ex)
         else:
             ai_message = "Mesajınız kurallarımıza uymamaktadır. İşleme devam edemiyorum."
             
         chat_message.response = ai_message
         chat_message.response_time_stamp = datetime.now()
         if not has_error:
            await self.chat_store.store_message(chat_massage=chat_message)
         return chat_message
    
    @observe(name="chat")
    async def chat_stream(self, chat_message : ChatMessage)->AsyncGenerator[str,None]:
         self.check_is_initialized()
         self.prepeare_message(chat_message = chat_message)
         config,query = self.prepeare_context(chatMessage=chat_message)
         if not self.app:
              raise ValueError(f"Application not found for app {self.application_identifier}")
         
         if not self.app._graph:
              raise ValueError(f"Application graph not found for app {self.application_identifier}")
         graph : CompiledStateGraph = self.app._graph
         message=''
         check_message_result = True
         if self.app.enableGuardRails:             
            check_message_result = await check_message(llm_model = self.app.model,chat_message=chat_message)
         if check_message_result:
            async for ai_message ,meta in graph.astream(
                {
                    "messages": [query],
                    "user_id":self.user_id,
                    "thread_id":chat_message.thread_id,
                    "documents":chat_message.fileIdentifiers,
                    "message_id":chat_message.message_id,
                    "llm":self.app.model,
                    "retrieval_query":""
                }
                    ,config =config,stream_mode="messages"
                ):
                if ai_message: # and graph_message["content"]:
                    if isinstance(ai_message,AIMessageChunk):
                        ai_chunk_message : AIMessageChunk =ai_message
                        if ai_chunk_message.content:
                            newMessage = str(ai_chunk_message.content)
                            message+=newMessage
                            chat_message.response = newMessage
                            chat_message.response_time_stamp = datetime.now()
                            yield chat_message.model_dump_json()
                        if 'finish_reason' in ai_chunk_message.response_metadata and ai_chunk_message.response_metadata['finish_reason'] == 'stop':
                            chat_message.response_time_stamp = datetime.now()
                            chat_message.response=''
                            yield chat_message.model_dump_json()
                            chat_message.response=message
                            await self.chat_store.store_message(chat_massage=chat_message)
         else:
             chat_message.response = "Mesajınız kurallarımıza uymamaktadır. İşleme devam edemiyorum."
             yield chat_message.model_dump_json()
             await self.chat_store.store_message(chat_massage=chat_message)
         
    
    async def chat_history(self)->list[ChatMessage]:
        self.check_is_initialized()
        return await self.chat_store.get_user_chat_history(self.user_id)
    
    async def chat_history_detail(self,thread_id:str)->list[ChatMessage]:
        self.check_is_initialized()
        return await self.chat_store.get_user_chat_history_detail(userId= self.user_id,thread_id=thread_id)
    
    async def delete_thread(self,thread_id:str)->None:
        self.check_is_initialized()
        return await self.chat_store.delete_thread(thread_id=thread_id)
    
    async def like_message(self,message_id:str)->int:
        self.check_is_initialized()
        return await self.chat_store.like_message(message_id=message_id)
    
    async def dislike_message(self,message_id:str)->int:
        self.check_is_initialized()
        return await self.chat_store.dislike_message(message_id=message_id)
    
    def prepeare_message(self, chat_message:ChatMessage)->ChatMessage:

        if not chat_message.thread_id:
            chat_message.thread_id = str(uuid4())
        
        chat_message.user_id = self.user_id
        chat_message.message_id  = get_client().get_current_trace_id() if ConfigurationManager.get_enable_agent_observability() else str(uuid4())
        chat_message.application_identifier = f"{self.application_identifier}"
        chat_message.query_time_stamp  = datetime.now()
        return chat_message

    def prepeare_context(self,chatMessage:ChatMessage)->tuple[RunnableConfig,str]:
        if not chatMessage.thread_id:
            raise ValueError("Thread id missing")
        
        if not chatMessage.user_id:
            raise ValueError("User id missing")
        
        if not chatMessage.message_id:
            raise ValueError("Message id missing")
        
        if not chatMessage.query:
            raise ValueError("Query missing")
        langfuse_handler = CallbackHandler()
        file_identifiers : List[str] = []
        if chatMessage.fileIdentifiers:
            file_identifiers = chatMessage.fileIdentifiers
        config : RunnableConfig = {"configurable": 
                                   {
                                       "thread_id": chatMessage.thread_id,
                                       "user_id":chatMessage.user_id,
                                       "message_id":chatMessage.message_id
                                    },
                                    "callbacks": [langfuse_handler],
                                    "metadata": {
                                        "langfuse_session_id": chatMessage.thread_id,
                                        "langfuse_user_id": self.user_id,
                                        "langfuse_tags":[self.application_identifier] + file_identifiers
                                    }
                                }
        
        query  = chatMessage.query
        return config,query

    def get_ai_message_from_response(self, response : dict[str,Any]):
        return response["messages"][-1].content

    async def set_application(self)->None:
            self.app = await ApplicationFactory().get_app(self.application_identifier,self.user_id)
            return
    
    def check_is_initialized(self)->None:
        if not self.is_initialize:
             raise ValueError("Chat application not initialized. Call initialize() first")


