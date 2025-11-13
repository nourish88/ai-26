from log import create_logger
from .models import App,LlmModel,McpServer
from integrations.admin_backend import AdminBackendService,AgentConfigurationResponse
from .agent_factory import AgentFactory

def create_app_cache_id(application_identifier: str , user_id:str|None)->str:
    if not user_id:
        return application_identifier
    
    return f"{application_identifier}{user_id}"

class ApplicationFactory:       
    def __init__(self):
        self.logger = create_logger(__name__)
        self.app_dict = {}

    async def get_app(self, application_identifier : str,user_id : str|None) -> App|None:
        self.logger.info(f"Fetching application {application_identifier}")
        app_cache_id = create_app_cache_id(application_identifier=application_identifier,user_id=user_id)
        value = self.app_dict.get(app_cache_id)
        if value:
            self.logger.info(f"Application {application_identifier} found in application cache")
            return value
        else:
            self.logger.info(f"Application {application_identifier} not found in application cache. Creating new.")
            new_app = await self.create_app(application_identifier=application_identifier,user_id=user_id)
            if new_app:
                self.app_dict[app_cache_id] = new_app
                return new_app
            else:
                return None       
        
        

    async def create_app(self, application_identifier : str,user_id :str|None) -> App|None:
        app_configuration = await self.get_app_configuration(application_identifier=application_identifier,user_id=user_id)
        if app_configuration and app_configuration.agent and app_configuration.agent.model:
            llm_model= LlmModel(
                id = app_configuration.agent.model.id
                , name=app_configuration.agent.model.name
                , topP= app_configuration.agent.model.topP
                , temperature= app_configuration.agent.model.temperature
                ,maxTokens=app_configuration.agent.model.maxTokens
                ,enableThinking=app_configuration.agent.model.enableThinking
                ,url= app_configuration.agent.model.url)
            mcp_servers = []
            if app_configuration.agent.mcpservers:
                for server in app_configuration.agent.mcpservers:
                    mcp_server = McpServer(id=server.id,identifier=server.identifier,uri=server.uri)
                    mcp_servers.append(mcp_server)

            app = App(
                id = app_configuration.agent.id,
                userId=user_id,
                description=app_configuration.agent.description,
                indentifier=app_configuration.agent.indentifier,
                memoryType=app_configuration.agent.memoryType,
                enableGuardRails=app_configuration.agent.enableGuardRails,
                checkHallucination=app_configuration.agent.checkHallucination,
                model=llm_model,
                name=app_configuration.agent.name,
                outputType=app_configuration.agent.outputType,
                prompt=app_configuration.agent.prompt,
                type=app_configuration.agent.type,
                mcpservers=mcp_servers
                )
            '''
            data  = None;
            file_path = os.path.join(os.path.dirname(__file__), "../mocks/app.json")
            if not os.path.exists(file_path):
                raise HTTPException(status_code=404, detail= file_path +" not found")    
            with open(file_path, "r") as f:
                data = json.load(f)[0]

            app = App(**data);
            '''
            try:
                app._graph = await AgentFactory().create_agent(app=app)
            except Exception as e:
                self.logger.error("Graph initilizing failed")
                self.logger.exception(e)
            
            return app
        else:
            self.logger.warning(f"Application {application_identifier} configuration not found at admin backend.")
            return None


    async def get_app_configuration(self,application_identifier : str,user_id:str|None) -> AgentConfigurationResponse|None:
        self.logger.debug(f"Getting application {application_identifier} configuration from admin backend.")
        admin_service = AdminBackendService()
        response = await admin_service.agent_configuration(application_identifier=application_identifier,user_id=user_id)
        return response
