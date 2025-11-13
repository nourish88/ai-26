from log import create_logger
from pymongo import MongoClient
from pymongo.collection import Collection
from server import ConfigurationManager
from chat import ChatMessage
from .models import ChatMessageEntity
import typing

async def get_collection(application_identifier : str):
    connection_string = ConfigurationManager.get_mongo_connection_string()    
    client =  MongoClient(connection_string,connect=True)
    database_name = ConfigurationManager.get_chat_database_name()
    collection_name = application_identifier
    return client[database_name][collection_name]

class ChatStoreMongo():
    application_identifier : str
    collection : Collection | None
    def __init__(self,application_identifier:str):
        self.logger = create_logger(__name__)
        self.application_identifier = application_identifier
        self.collection = None


    async def store_message(self,chat_massage : ChatMessage)->ChatMessage:
        collection = await self.get_collection()
        entity=self.chat_message_to_entity(chat_message=chat_massage)
        result = collection.insert_one(entity)
        chat_massage.id = str(result.inserted_id)
        return chat_massage
    
    async def delete_thread(self,thread_id:str)->None:
        collection = await self.get_collection()
        collection.delete_many({"thread_id": thread_id,"application_identifier": self.application_identifier})
    
    
    async def get_user_chat_history(self,userId: str)->list[ChatMessage]:
        collection = await self.get_collection()
        chat_history = collection.find({"user_id": userId, "application_identifier": self.application_identifier},{'_id': False}).to_list(1000)
        return chat_history
    
    async def get_user_chat_history_detail(self,userId: str,thread_id: str)->list[ChatMessage]:
        collection = await self.get_collection()
        chat_history = collection.find({"user_id": userId,"thread_id": thread_id, "application_identifier": self.application_identifier},{'_id': False}).to_list(1000)
        return chat_history
    
    async def like_message(self,message_id:str)->int:
        collection = await self.get_collection()
        update_result =  collection.update_one(
            {"application_identifier": self.application_identifier,"message_id": message_id},
            {"$set": {"is_liked": True, "is_disliked": False}})
        return update_result.modified_count   

    async def dislike_message(self,message_id:str)->int:
        collection = await self.get_collection()
        update_result =  collection.update_one(
            {"application_identifier": self.application_identifier,"message_id": message_id},
            {"$set": {"is_liked": False, "is_disliked": True}})
        return update_result.modified_count   

    
    async def get_collection(self)->Collection:
        if self.collection == None:
            self.collection = await get_collection(application_identifier= self.application_identifier)

        return self.collection
    
    def chat_message_to_entity(self,chat_message : ChatMessage)-> ChatMessageEntity:
        data_dict = chat_message.model_dump()
        typed_dict = typing.cast(ChatMessageEntity, data_dict)
        return typed_dict
    
    def entity_to_chat_message(self,chat_message_entity : ChatMessageEntity)-> ChatMessage:
        pydantic_model = ChatMessage(**chat_message_entity)
        return pydantic_model

        
        