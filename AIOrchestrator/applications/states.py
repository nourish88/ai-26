from langgraph.prebuilt.chat_agent_executor import AgentState
from typing import List,NotRequired,Any

class OrchestratorState(AgentState):
    documents: NotRequired[List[str] | None]
    user_id: NotRequired[str]
    thread_id:NotRequired[str]
    message_id:NotRequired[str]
    retrieved_docs: NotRequired[List[str] | None] 
    retrieval_query: NotRequired[str | None]
    llm: NotRequired[Any] | None
