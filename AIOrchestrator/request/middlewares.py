from fastapi import Request
from fastapi.responses import JSONResponse
from .request_context import RequestContext
from starlette.middleware.base import BaseHTTPMiddleware
from server.configuration_manager import ConfigurationManager
from .token_validator import TokenValidator
from typing import Any
from cache import CacheHelper,UserCacheItem
from starlette.types import ASGIApp
from log import create_logger
import traceback
from datetime import datetime
import json
from utils import extract_trace,request_context

EXCLUDED_ENDPOINTS = ["/docs","/openapi.json","/static"]
EXCLUDED_ENDPOINTS_FOR_APP_IDENTIFIER = ["/applications"]

class RequestContextMiddleware(BaseHTTPMiddleware):
    async def dispatch(self, request: Request, call_next):
        if not request.url.path.startswith(tuple(EXCLUDED_ENDPOINTS)):
            
            app_identifier = request.headers.get("app-identifier", "")
            if not app_identifier and not request.url.path.startswith(tuple(EXCLUDED_ENDPOINTS_FOR_APP_IDENTIFIER)):
                return JSONResponse(content={"error": "Missing app-identifier header"}, status_code=401) 
            
            if not app_identifier:
                app_identifier = "fake"

            if ConfigurationManager.get_auth_enabled():
                auth_header = request.headers.get("Authorization")
                if not auth_header or not auth_header.startswith("Bearer "):
                    return JSONResponse(content={"error": "Missing or invalid Authorizaiton header."}, status_code=401)
                
                token_validator = TokenValidator()
                token = auth_header.split(" ")[1]
                token_validation_result : dict[str,Any] | None = token_validator.validate_token(token=token)
                if token_validation_result:        
                    client_id = token_validation_result.get("preferred_username")
                    client_name = token_validation_result.get("name")
                    user_code = token_validation_result.get("user_code")
                    email =  token_validation_result.get("email")
                    corporate_user = token_validation_result.get("kurumsalkullanici")
                    troop_code =  token_validation_result.get("troop_code")
                    district_code = token_validation_result.get("district_code")
                    identity_number = token_validation_result.get("identity_number")
                    user_roles = token_validation_result.get("user_roles")
                    user_projects = token_validation_result.get("user_projects")

                    user_id = str(client_id)
                    request.state.context = RequestContext(
                        app_identifier=app_identifier
                        ,user_id= user_id
                        ,client_name=client_name if str(client_name) else None
                        ,user_code=user_code if str(client_name) else None
                        ,email=email if str(client_name) else None
                        ,corporate_user=corporate_user if str(client_name) else None
                        ,troop_code=troop_code if str(client_name) else None
                        ,district_code=district_code if str(client_name) else None
                        ,identity_number=identity_number if str(client_name) else None
                        ,user_roles=user_roles if str(client_name) else None
                        ,user_projects=user_projects if str(client_name) else None
                        )
                    #Cache user cache item
                    user_cache_item = UserCacheItem(user_id=user_id,request_token=token,app_identifier=app_identifier)
                    await CacheHelper.set(user_id,user_cache_item)
                else:
                    return JSONResponse(content={"error": "Invalid authorization token."}, status_code=401)
            else:                   
                # Create RequestContext instance with the extracted value
                request.state.context = RequestContext(
                                                       app_identifier=app_identifier
                                                       ,user_id="default_user"
                                                       ,client_name = None
                                                       ,user_code= None
                                                       ,email= None
                                                       ,corporate_user= None
                                                       ,troop_code= None
                                                       ,district_code= None
                                                       ,identity_number= None
                                                       ,user_roles= None
                                                       ,user_projects= None)
        response = await call_next(request)
        return response
    
class CustomExceptionMiddleware(BaseHTTPMiddleware):    

    def __init__(self, app):
        super().__init__(app)
        self.logger = create_logger("app.exception")   # JSON logger

    async def dispatch(self, request: Request, call_next):
        # 2️⃣ context (isteğe bağlı) ekle
        ctx = get_request_context_dict(request=request) 
        req_info = {
            "path": request.url.path,
            "method": request.method,
            "client_ip": request.client.host if request.client else None,
            "context":ctx if ctx else None
        }
        request_context.set(req_info)  
        
        try:
            response = await call_next(request)
            if response.status_code and response.status_code!=200:
                extra={"status_code":response.status_code}
                self.logger.warning("Unsuccess response!",extra=extra)
            return response
        except Exception as exc:
            # 1️⃣ Trace’i kısalt ve filtrele
            trace = extract_trace(exc, limit=1000)            
            
            # 3️⃣ Log’lama
            # record.extra’yi kullanalım → logger’a ek alan geç
            self.logger.error(
                f"Unhandled exception: {exc}",
                extra={"trace": trace}
            )

            # 4️⃣ Kullanıcıya standart 500
            return JSONResponse(
                status_code=500,
                content={"detail": "Internal server error"}
            )
    
async def get_request_context(request: Request) -> RequestContext:
    return request.state.context

def get_request_context_dict(request : Request)->Any|None:
    ctx = getattr(request.state, "context", None)   
    if ctx:
        # context için dict dönüştürme
        ctx_dict = getattr(ctx, "to_dict", lambda: ctx.__dict__)()
        return ctx_dict
    else:
        return None