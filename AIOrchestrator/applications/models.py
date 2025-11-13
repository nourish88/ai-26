from pydantic import BaseModel
from typing import Literal,Optional,List
from typing_extensions import Any
from langgraph.graph.state import CompiledStateGraph
from enum import Enum
from langchain_openai import ChatOpenAI
from log import create_logger
from server.configuration_manager import ConfigurationManager

class AppType(str, Enum):
    REACT          = "REACT"
    CHATBOT            = "CHATBOT"
    AGENTIC_RAG    = "AGENTIC_RAG"
    REFLECTIVE_RAG = "REFLECTIVE_RAG"
    MCP_POWERED_AGENTIC_RAG = "MCP_POWERED_AGENTIC_RAG"
    CUSTOM         = "CUSTOM"

class LlmModel(BaseModel):
    id : str
    name : str
    topP : float
    temperature : float
    maxTokens : int
    enableThinking : bool
    url : str

class McpServer(BaseModel):
    id : str
    identifier : str
    uri : str

class App(BaseModel):
    id : int
    userId : Optional[str]
    name : str
    indentifier : str
    description : str
    type : Literal["REACT","CHATBOT","AGENTIC_RAG","REFLECTIVE_RAG","MCP_POWERED_AGENTIC_RAG","CUSTOM"]
    outputType : Literal["BLOCK","STREAMING","BOTH"]
    prompt : str
    memoryType : Literal["MEMORY","MONGO","POSTGRE"]
    enableGuardRails : bool
    checkHallucination : bool
    model : Optional[LlmModel]
    mcpservers : Optional[List[McpServer]] = []
    _graph: Optional[CompiledStateGraph]=None

def create_model(llmModel:LlmModel) -> ChatOpenAI:
        """Get the model based on the provided name."""
        logger = create_logger(__name__)
        logger.info(f"Creating {llmModel.name}")
        model = ChatOpenAI(
            model=llmModel.name
            ,temperature=llmModel.temperature 
            ,streaming=True
            ,extra_body= {"chat_template_kwargs":{"enable_thinking":llmModel.enableThinking}}
            ,openai_api_base=llmModel.url
            ,api_key=ConfigurationManager.get_litellm_api_key()
            )
    
        return model