# openapi_utils.py
from fastapi import FastAPI
from fastapi.openapi.models import OpenAPI
from fastapi.openapi.utils import get_openapi
from typing import Dict,Any
from src.configuration import ConfigurationManager

def setup_custom_openapi(app: FastAPI) -> Dict[str, Any]:
    def custom_openapi():
        if app.openapi_schema:
            return app.openapi_schema

        openapi_schema = get_openapi(
            title=app.title,
            version=app.version,
            description=app.description,
            routes=app.routes,
        )
        if ConfigurationManager.get_auth_enabled:

            if "components" not in openapi_schema:
                openapi_schema["components"] = {}

            openapi_schema["components"]["securitySchemes"] = {
                "OpenIDConnect": {
                    "type": "oauth2",
                    "flows": {
                        "authorizationCode": {
                            "authorizationUrl": f"{ConfigurationManager.get_auth_issuer()}/protocol/openid-connect/auth",
                            "tokenUrl": f"{ConfigurationManager.get_auth_issuer()}/protocol/openid-connect/token",
                            "x-clientId": "abc",          # <-- buraya kendi client_id
                            "x-clientSecret": "def",
                            #"scopes": {
                            #     "openid": "OpenID Connect scope",
                            #     "profile": "Profile information",
                            #     "email": "User email",
                            #     "read": "Read access",
                            #     "write": "Write access"
                            #}
                        }
                    }                    
                }
            }
        openapi_schema["security"] = [{"OpenIDConnect": ["openid"]}]
        app.openapi_schema = openapi_schema
        return app.openapi_schema

    app.openapi = custom_openapi