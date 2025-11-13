# keycloak_oidc_sync.py
import httpx
from typing import Optional
#NOTE : Keycloak service account must be enabled
class KeycloakOidcClient:
    def __init__(self, client: Optional[httpx.Client] = None) -> None:        
        self.client = client or httpx.Client()

    def get_token(
        self,
        authority: str,
        client_id: str,
        client_secret: str,
        scope: Optional[str] = None,
    ) -> Optional[str]:        
        discovery_url = f"{authority}/.well-known/openid-configuration"
        resp = self.client.get(discovery_url, timeout=10)
        resp.raise_for_status()
        discovery = resp.json()

        token_endpoint = discovery.get("token_endpoint")
        if not token_endpoint:
            raise Exception("Token endpoint not found in discovery document")

        data = {
            "grant_type": "client_credentials",
            "client_id": client_id,
            "client_secret": client_secret,
        }
        if scope:
            data["scope"] = scope

        token_resp = self.client.post(token_endpoint, data=data, timeout=10)
        token_resp.raise_for_status()
        token_data = token_resp.json()

        return token_data.get("access_token")
