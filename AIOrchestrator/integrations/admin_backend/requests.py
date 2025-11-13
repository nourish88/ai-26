from pydantic import BaseModel
from typing import List,Optional
from enum import Enum

class ChatDetectionType(int, Enum):
    Hallucination = 1
    GuardRail = 2

class SemanticSearchRequest(BaseModel):
    query: str =" "
    top: int = 3
    skip: int = 0
    userId: Optional[str] = None 
    fileIdentifiers: Optional[List[str]] = None

class ChatDetectionRequest(BaseModel):
    chatDetectionTypeId: ChatDetectionType
    threadId:str
    messageId:str
    userMessage:str
    sources:Optional[str] = None 
    reason:Optional[str] = None