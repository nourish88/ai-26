import httpx
from tenacity import retry, wait_random_exponential, stop_after_attempt
from . import (SemanticSearchRequest
               ,SemanticSearchResponse
               ,AgentConfigurationResponse
               ,ApplicationForUIResponse
               ,UserFilesResponse
               ,DeleteUserFileResponse
               ,ChatDetectionType
               ,ChatDetectionRequest
               ,ChatDetectionResponse)
from log.logger_factory import create_logger
from server.configuration_manager import ConfigurationManager
from cache import CacheHelper,UserCacheItem

async def create_request_header(user_id:str|None):
    if not user_id:
        return None
    user_cache_item  = await CacheHelper.get_user_cache_item(user_id)    
    if not user_cache_item:
        return None
    return  {
                "Authorization" : f"Bearer {user_cache_item.request_token}",
                "app-identifier" : f"{user_cache_item.app_identifier}"
            }

class AdminBackendService:
    def __init__(self):
        self.logger = create_logger(__name__)

    #@retry(reraise=True, wait=wait_random_exponential(min=1, max=5), stop=stop_after_attempt(3))
    async def semantic_search(self, request :SemanticSearchRequest) -> SemanticSearchResponse | None:
        headers = await create_request_header(user_id=request.userId)
        response = httpx.post(f"{ConfigurationManager.get_admin_backend_url()}/search/semanticsearch", json=request.model_dump(),headers=headers)
        response.raise_for_status()
        
        #if not response.json()["success"]:
        #    self.logger.warning("Error has occurred while setting error file status: %s", response.json()["error"])
        #    #raise Exception("Error has occurred while setting error file status")
        #    return None
        
        result = SemanticSearchResponse(**response.json())
        return result
    

    #@retry(reraise=True, wait=wait_random_exponential(min=1, max=5), stop=stop_after_attempt(3))
    async def agent_configuration(self, application_identifier :str, user_id: str|None) -> AgentConfigurationResponse | None:
        headers = await create_request_header(user_id=user_id)
        response = httpx.get(f"{ConfigurationManager.get_admin_backend_url()}/agents/configuration",headers=headers)
        response.raise_for_status()       
        
        result = AgentConfigurationResponse(**response.json())
        return result
    
    async def applications_for_ui(self, user_id: str|None) -> ApplicationForUIResponse | None:
        headers = await create_request_header(user_id=user_id)
        response = httpx.get(f"{ConfigurationManager.get_admin_backend_url()}/applications?pageIndex=0&pageSize=1000",headers=headers)
        response.raise_for_status()       
        
        result = ApplicationForUIResponse(**response.json())
        return result
    
    async def application_user_files(self, user_id: str|None) -> UserFilesResponse | None:
        headers = await create_request_header(user_id=user_id)
        response = httpx.get(f"{ConfigurationManager.get_admin_backend_url()}/files/user",headers=headers)
        response.raise_for_status()       
        
        result = UserFilesResponse(**response.json())
        return result
    
    async def delete_user_file(self, user_id: str|None, file_id:int) -> DeleteUserFileResponse | None:
        headers = await create_request_header(user_id=user_id)
        response = httpx.delete(f"{ConfigurationManager.get_admin_backend_url()}/files/user/{file_id}",headers=headers)
        response.raise_for_status()       
        
        result = DeleteUserFileResponse(**response.json())
        return result
    
    async def chat_detection(self
                             , chat_detection_type : ChatDetectionType
                             ,thread_id : str
                             ,message_id : str
                             ,user_message : str
                             ,sources : str|None
                             ,reason : str | None 
                             , user_id: str|None) -> ChatDetectionResponse | None:
        headers = await create_request_header(user_id=user_id)
        request = ChatDetectionRequest(
                        chatDetectionTypeId=chat_detection_type,
                        threadId=thread_id,
                        messageId=message_id,
                        userMessage=user_message,
                        sources=sources,
                        reason=reason
)
        response = httpx.post(f"{ConfigurationManager.get_admin_backend_url()}/chatdetections",json=request.dict(),headers=headers)
        response.raise_for_status()
        
        result = ChatDetectionResponse(**response.json())
        return result
