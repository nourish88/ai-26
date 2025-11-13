import httpx
from tenacity import retry, wait_random_exponential, stop_after_attempt
from src.oidc import KeycloakOidcClient

from src.configuration.configuration_manager import ConfigurationManager
from src.logging.logger_factory import create_logger
from src.service.ingestion_status_types import IngestionStatusTypes


class AdminBackendService:
    client : KeycloakOidcClient
    authority : str
    client_id : str
    client_secret : str
    scope : str|None
    def __init__(self):
        self.logger = create_logger(__name__)
        self.client = KeycloakOidcClient()
        self.authority = ConfigurationManager.get_auth_issuer()
        self.client_id = ConfigurationManager.get_auth_audience()
        self.client_secret = ConfigurationManager.get_auth_client_secret()

    @retry(reraise=True, wait=wait_random_exponential(min=1, max=5), stop=stop_after_attempt(3))
    def change_file_status(self, file_id: int, status: IngestionStatusTypes) -> None:
        response = httpx.put(f"{ConfigurationManager.get_admin_backend_url()}/files/status", json={
                "id": file_id,
                "status": status.value
            },headers=self.get_request_headers())
        response.raise_for_status()
        if not response.json()["success"]:
            self.logger.warning("Error has occurred while changing file status: %s", response.json()["error"])
            raise Exception("Error has occurred while changing file status")

    @retry(reraise=True, wait=wait_random_exponential(min=1, max=5), stop=stop_after_attempt(3))
    def set_error_file_status(self, file_id: int, error_detail: str) -> None:
        response = httpx.put(f"{ConfigurationManager.get_admin_backend_url()}/files/status/error", json={
            "id": file_id,
            "errorDetail": error_detail
        },headers=self.get_request_headers())
        response.raise_for_status()
        if not response.json()["success"]:
            self.logger.warning("Error has occurred while setting error file status: %s", response.json()["error"])
            raise Exception("Error has occurred while setting error file status")

    def get_file_status(self, file_id: int) -> IngestionStatusTypes:
        response = httpx.get(f"{ConfigurationManager.get_admin_backend_url()}/files/status/{file_id}",headers=self.get_request_headers())
        response.raise_for_status()
        if not response.json()["success"]:
            self.logger.warning("Error has occurred while getting file status: %s", response.json()["error"])
            raise Exception("Error has occurred while getting file status")

        return IngestionStatusTypes(response.json()["status"])
    
    def get_request_headers(self)->dict[str,str]|None:
        try:
            #TODO : Cache token
            token = self.client.get_token(self.authority,self.client_id,self.client_secret)
            if token:
                return {"Authorization" : f"Bearer {token}"}
            else:
                return None
        except Exception as e:
            self.logger.error("Get token for admin backend failed.")
            self.logger.exception(e)
            return None
