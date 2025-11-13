from log import create_logger
from .models import App,LlmModel,McpServer,AppType,create_model
from .memory_factory import MemoryFactory
from langgraph.prebuilt import create_react_agent
from langgraph.graph.state import CompiledStateGraph
from langchain_openai import ChatOpenAI
from server.configuration_manager import ConfigurationManager
from langgraph.checkpoint.base import BaseCheckpointSaver
from .states import OrchestratorState
from .tool_factory import rag_retriever, ToolFactory
from typing import List,Optional
from .evals import detect_hallucination_hook

class AgentFactory:
    def __init__(self):
        self.logger = create_logger(__name__)   

    async def create_agent(self, app : App) -> CompiledStateGraph | None:        
        self.logger.info(f"Building app {app.indentifier} with type = {app.type}, memory = {app.memoryType}, mcpservers = {app.mcpservers}")
        memory = MemoryFactory().get_memory(app=app)
        
        if not app.model :
            raise ValueError("Application Llm model is missing")

        if app.type==AppType.REACT.value:
            return await self.get_react_agent(user_id=app.userId, llmModel = app.model,prompt=app.prompt,memory=memory,mcp_servers= app.mcpservers)
        elif app.type==AppType.CHATBOT.value:
            return self.get_chatbot_agent(llmModel= app.model,prompt=app.prompt,memory=memory)
        elif app.type==AppType.AGENTIC_RAG.value:
            return await self.get_agentic_rag_agent(user_id=app.userId, llmModel = app.model,prompt=app.prompt,memory=memory,detect_hallucination=app.checkHallucination)
        elif app.type==AppType.MCP_POWERED_AGENTIC_RAG.value:
            return await self.get_mcp_powered_agentic_rag_agent(user_id=app.userId, llmModel = app.model,prompt=app.prompt,memory=memory,mcp_servers= app.mcpservers,detect_hallucination=app.checkHallucination)
        elif app.type==AppType.REFLECTIVE_RAG.value: 
            self.logger.error(f"Not Implemented")
            return None
        elif app.type==AppType.CUSTOM.value:
            self.logger.error(f"Application agent not found for type {AppType.CUSTOM.value} and identifier {app.indentifier}")
            return None
        else:
            return None
        
    def get_chatbot_agent(self, llmModel : LlmModel, prompt : str, memory: BaseCheckpointSaver)-> CompiledStateGraph:
        """Creates a basic chatbot agent with the specified model."""
        model = create_model(llmModel)
        tools = []
        return create_react_agent(
            model=model,
            tools=tools,
            prompt=prompt,
            checkpointer=memory,
            state_schema=OrchestratorState,
        )
        
    
    async def get_react_agent(self,user_id:str|None, llmModel : LlmModel, prompt : str, memory: BaseCheckpointSaver, mcp_servers:Optional[List[McpServer]]) -> CompiledStateGraph:
        """Creates a REACT agent with the specified model and tools."""
        model = create_model(llmModel)
        tools = []
        if mcp_servers:
            tools = await ToolFactory().mcp_servers_to_tool(user_id=user_id,mcp_servers= mcp_servers)
        return create_react_agent(
            model=model,
            tools=tools,
            prompt=prompt,
            checkpointer=memory,
            state_schema=OrchestratorState    
        )



    async def get_agentic_rag_agent(
            self,
            user_id:str|None,
            llmModel : LlmModel,
            prompt : str,
            memory: BaseCheckpointSaver,
            detect_hallucination:bool)-> CompiledStateGraph:
        """Creates a basic RAG agent with the specified model and tools."""
        model = create_model(llmModel)
        tools = [rag_retriever]
        post_model_hook = None
        if detect_hallucination:
            post_model_hook = detect_hallucination_hook
        return create_react_agent(
            model=model,
            tools=tools,
            prompt=prompt,
            checkpointer=memory,
            state_schema=OrchestratorState,
            post_model_hook=post_model_hook
        ) 

    async def get_mcp_powered_agentic_rag_agent(
            self
            ,user_id:str|None
            , llmModel : LlmModel
            , prompt : str
            , memory: BaseCheckpointSaver
            , mcp_servers:Optional[List[McpServer]]
            ,detect_hallucination:bool)-> CompiledStateGraph:
        """Creates a basic RAG agent with the specified model and tools."""
        model = create_model(llmModel)
        tools = []
        if mcp_servers:
            tools = await ToolFactory().mcp_servers_to_tool(user_id=user_id, mcp_servers=mcp_servers)
        tools.append(rag_retriever)
        post_model_hook = None
        if detect_hallucination:
            post_model_hook = detect_hallucination_hook
        return create_react_agent(
            model=model,
            tools=tools,
            prompt=prompt,
            checkpointer=memory,
            state_schema=OrchestratorState,
            post_model_hook=post_model_hook
        )    