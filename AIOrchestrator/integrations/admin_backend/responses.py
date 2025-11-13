from pydantic import BaseModel, Field
from typing import List, Optional,Literal
from datetime import datetime
from . import ChatDetectionType

class SemanticSearchResult(BaseModel):
    id: str
    parentid: str
    datasourceid: Optional[str] = None
    title: str
    soruceurl: Optional[str] = None
    content: str
    sentiment: Optional[str] = None

class SemanticSearchResponse(BaseModel):
    response: Optional[List[SemanticSearchResult]] = []


class McpServer(BaseModel):
    id : str
    identifier : str
    uri : str

class LlmModel(BaseModel):
    id : str
    name : str
    topP : float
    temperature : float
    maxTokens : int
    enableThinking : bool
    url : str

class Agent(BaseModel):
    id : int
    name : str
    indentifier : str
    description : str
    type : Literal["REACT","CHATBOT","AGENTIC_RAG","REFLECTIVE_RAG","MCP_POWERED_AGENTIC_RAG","CUSTOM"]
    outputType : Literal["BLOCK","STREAMING","BOTH"]
    prompt : str
    memoryType : Literal["MEMORY","MONGO","POSTGRE"]
    enableGuardRails : bool
    checkHallucination : bool
    mcpservers : Optional[List[McpServer]] = []
    model : Optional[LlmModel]

class AgentConfigurationResponse(BaseModel):
    isSuccess: bool
    error: Optional[str] = None
    agent: Optional[Agent] = None

class ApplicationForUI(BaseModel):
    #id: int
    name: str
    identifier: str
    has_application_file: bool = Field(..., alias='hasApplicationFile')
    has_user_file: bool = Field(..., alias='hasUserFile')    
    description: str    

    class Config:
        validate_by_name = True
        str_strip_whitespace = True

class ApplicationForUIResultInfo(BaseModel):
    items: List[ApplicationForUI]

    class Config:
        validate_by_name = True

class ApplicationForUIResponse(BaseModel):
    result: ApplicationForUIResultInfo

class UserFile(BaseModel):
    id: int
    title: str
    file_name: str = Field(..., alias='fileName')
    file_extension: str = Field(..., alias='fileExtension')
    description: str
    ingestion_status_type_id: int = Field(..., alias='ingestionStatusTypeId')
    error_detail: Optional[str] = Field(None, alias='errorDetail')

    class Config:
        validate_by_name = True
        str_strip_whitespace = True

class UserFilesResponse(BaseModel):
    result: Optional[List[UserFile]]

    class Config:
        validate_by_name = True

class DeleteUserFileResponse(BaseModel):
    result:bool
    error_message:Optional[str]= Field(None, alias='errorMessage')

class ChatDetectionResponse(BaseModel):
    id: int
    applicationIdentifier: str
    chatDetectionTypeId: ChatDetectionType 
    threadId: str
    messageId: str
    userMessage: str
    sources: Optional[str] = None
    reason: Optional[str] = None
