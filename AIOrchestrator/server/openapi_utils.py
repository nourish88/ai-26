# openapi_utils.py
from fastapi import FastAPI
from fastapi.openapi.models import OpenAPI
from fastapi.openapi.utils import get_openapi

def setup_custom_openapi(app: FastAPI):
    def custom_openapi():
        if app.openapi_schema:
            return app.openapi_schema

        openapi_schema = get_openapi(
            title="My API",
            version="1.0.0",
            routes=app.routes,
        )

        # Ensure components exist
        if "components" not in openapi_schema:
            openapi_schema["components"] = {}

        # Add app-identifier parameter to components (if not already present)
        if "parameters" not in openapi_schema["components"]:
            openapi_schema["components"]["parameters"] = {}
        openapi_schema["components"]["parameters"]["app-identifier"] = {
            "type": "string",
            "description": "Uygulama kimliği",
            "in": "header",
            "name": "app-identifier",
            "required": True,
        }

        # Add parameter to all operations
        for path, path_item in openapi_schema.get("paths", {}).items():
            for method, operation in path_item.items():
                if "parameters" not in operation:
                    operation["parameters"] = []
                operation["parameters"].append(
                    {"$ref": "#/components/parameters/app-identifier"}
                )

        app.openapi_schema = openapi_schema
        return app.openapi_schema

    app.openapi = custom_openapi
