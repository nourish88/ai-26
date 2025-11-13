from log import create_logger
from .models import App
from langgraph.checkpoint.base import BaseCheckpointSaver
from langgraph.checkpoint.memory import MemorySaver
from langgraph.checkpoint.mongodb import AsyncMongoDBSaver
from pymongo import AsyncMongoClient
from server.configuration_manager import ConfigurationManager

class MemoryFactory:
    def __init__(self):
        self.logger = create_logger(__name__)

    def get_memory(self, app:App)-> BaseCheckpointSaver: 
        self.logger.info(f"Creating {app.memoryType} memory for application {app}.]")
        if app.memoryType== "MONGO":
            return self.get_mongo_memory(appId=app.id)
        elif app.memoryType == "POSTGRE": #TODO : Postgres memory 
            return MemorySaver()
        else:
            return MemorySaver()

    def get_mongo_memory(self,appId : int) -> BaseCheckpointSaver:
        db_name = str(appId)
        connection_string = ConfigurationManager.get_mongo_connection_string()
        client = AsyncMongoClient(connection_string)
        checkpointer =  AsyncMongoDBSaver(client,db_name=db_name);
        return checkpointer