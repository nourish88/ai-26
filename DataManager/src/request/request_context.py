from pydantic import BaseModel
from typing import Optional

class RequestContext(BaseModel):
    user_id: str
    client_name: Optional[str] = None
    user_code: Optional[str] = None
    email: Optional[str] = None
    corporate_user: Optional[str] = None
    troop_code: Optional[str] = None
    district_code: Optional[str] = None
    identity_number: Optional[str] = None
    user_roles: Optional[str] = None
    user_projects: Optional[str] = None