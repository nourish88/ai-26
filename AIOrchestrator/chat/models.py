from pydantic import BaseModel
from typing import Optional,Dict,TypedDict,List
from datetime import datetime

class ChatMessage(BaseModel):
    """Represents the chat messages for the AI Orchestrator."""
    id : Optional[str] = None
    message_id: Optional[str] = None
    application_identifier: Optional[str] = None
    thread_id:Optional[str] = None
    user_id:Optional[str] = None
    query: str|None =None
    response:Optional[str] =None
    is_liked:Optional[bool] =False
    is_disliked:Optional[bool] =False
    context:dict ={}
    query_time_stamp:Optional[datetime] = None
    response_time_stamp:Optional[datetime] =None
    fileIdentifiers: Optional[List[str]] = None

class ChatMessageEntity(TypedDict):
    id: Optional[str]
    message_id: Optional[str]
    application_identifier: Optional[str]
    thread_id: Optional[str]
    user_id: Optional[str]
    query: Optional[str]
    response: Optional[str]
    is_liked: Optional[bool]
    is_disliked: Optional[bool]
    context: Dict
    query_time_stamp: Optional[datetime]
    response_time_stamp: Optional[datetime]

