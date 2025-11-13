from log import create_logger
from .models import McpServer
from langchain_mcp_adapters.client import StreamableHttpConnection,MultiServerMCPClient,StdioConnection,SSEConnection,WebsocketConnection
from typing import Annotated, Dict,List,Any
from langchain_core.tools import BaseTool, tool,InjectedToolCallId
from .states import OrchestratorState
from integrations.admin_backend import SemanticSearchRequest,AdminBackendService
from langchain_core.runnables import RunnableConfig
from langgraph.prebuilt import InjectedState
from server import ConfigurationManager
from langfuse import observe
from langgraph.types import Command
from langchain_core.messages.tool import ToolMessage

Connection = StdioConnection | SSEConnection | StreamableHttpConnection | WebsocketConnection

from cache import CacheHelper,UserCacheItem

async def create_authentication_header(user_id:str|None):
    if not user_id:
        return None
    user_cache_item  = await CacheHelper.get_user_cache_item(user_id)    
    if not user_cache_item:
        return None
    return  {
        "Authorization" : f"Bearer {user_cache_item.request_token}",
        "app-identifier" : f"{user_cache_item.app_identifier}"
        }

class ToolFactory:
    def __init__(self):
        self.logger = create_logger(__name__)


    async def  mcp_servers_to_tool(self,user_id: str|None, mcp_servers:List[McpServer]) -> list[BaseTool] :
        if mcp_servers:
            headers = await create_authentication_header(user_id=user_id)
            for server in mcp_servers:
                connection = StreamableHttpConnection(transport="streamable_http", url=server.uri,headers=headers)
                cons : Dict[str,Connection]= {}
                cons[server.id] = connection
                client = MultiServerMCPClient(connections=cons)       
                tools = await client.get_tools()
                return tools
        
        empty_tool_list : list[BaseTool] = []
        return empty_tool_list
    

@tool
@observe(as_type="retriever")
async def rag_retriever(search_query:str
                        , state: Annotated[OrchestratorState, InjectedState]
                        ,tool_call_id: Annotated[str, InjectedToolCallId],):
    
    """
    Use this tool to fetch external information for the user’s query.  
    **This tool should be used first** — retrieve relevant documents or facts.  
    If the retrieved information is incomplete, ambiguous, or insufficient to fully answer the question,  
    then the model may fall back to using its internal knowledge.  

    Args:
        search_query (str): The user’s question or a reformulated query for external search.
        state: The agent’s state (provides metadata like user_id, context, etc.).

    Returns:
        A string or structured result containing search findings (documents, summaries, etc.).
        If an error occurs, returns an error message string.
    """
    logger = create_logger(__name__)
    
    print("***********************")
    semantic_search_request  = SemanticSearchRequest(query=search_query)
    semantic_search_request.userId = state.get("user_id")
    semantic_search_request.fileIdentifiers = state.get("documents")
    service = AdminBackendService()
    message = ""
    retrieved_docs = state.get("retrieved_docs",[])
    if not retrieved_docs:
        retrieved_docs = []

    try:
        logger.info(f"Searching {search_query}...")
        result = await service.semantic_search(semantic_search_request)
        logger.info(f"SearchResult:")     
        # if ConfigurationManager.get_rag_retriever_debug_mode():
        #     logger.info(result)
        #     return result
        # else:
        #     if result:
        #         if not result.response:
        #             logger.warning("***Search result is empty***")
        #             return ""
        #         # content’leri al
        #         contents = [res.content for res in result.response if res.content is not None]
        #         # birleştir (örneğin aralarına boşluk koyarak)
        #         merged = "\n\n".join(contents)
        #         logger.info(merged)
        #         return merged
        #     else:
        #         logger.warning("***Search result is empty***")
        #         return ""

        
        if result:
            if not result.response:
                logger.warning("***Search result is empty***")                
                message = "Arama sonucunda kayıt bulunamadı."
            else:
                contents = [res.content for res in result.response if res.content is not None]
                logger.info(contents)
                retrieved_docs = retrieved_docs + contents # TODO : Merge existing documents
                message = "\n\n".join(contents)
            
                
        else:
            logger.warning("***Search result is empty***")
            message = "Arama sonucunda kayıt bulunamadı."

        return Command(update={
                        "retrieved_docs": retrieved_docs,
                        "retrieval_query" : search_query,
                        "messages": [ToolMessage(message,tool_call_id=tool_call_id)]
                })

    except Exception as e:
        logger.error(f"Failed to fetch search results for {search_query}, {e}")
        message = "Arama sırasında hata oluştu."
        return Command(update={
                        "retrieved_docs": retrieved_docs,
                        "retrieval_query" : search_query,
                        "messages": [ToolMessage(message,tool_call_id=tool_call_id)]
                }) 
        