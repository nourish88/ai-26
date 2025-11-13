from pydantic import BaseModel

class UserCacheItem(BaseModel):    
    user_id : str