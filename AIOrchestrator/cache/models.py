from pydantic import BaseModel

class UserCacheItem(BaseModel):    
    user_id : str
    request_token:str
    app_identifier: str