from src.logging import create_logger
from src.configuration import ConfigurationManager
from typing import Any
import json
import requests
import jwt
from jwt import InvalidTokenError
from jwt.algorithms import RSAAlgorithm

JWKS_CACHE:Any = None

def cache_jwks(jwks : Any):
    global JWKS_CACHE
    JWKS_CACHE = jwks

class TokenValidator():
    issuer : str
    configuration_url : str
    jwks_url : str
    expected_audiance : str
    trusted_certificate_path : str|None
    def __init__(self):
        self.logger = create_logger(__name__)
        self.issuer = ConfigurationManager.get_auth_issuer()
        self.configuration_url = f"{self.issuer}/.well-known/openid-configuration"
        self.expected_audiance = ConfigurationManager.get_auth_audience()
        self.trusted_certificate_path = ConfigurationManager.get_trusted_certificate_path()

    def fetch_and_cache_jwks(self):
        try:
                            
            configuration_response = requests.get(self.configuration_url,verify=self.trusted_certificate_path)
            configuration_response.raise_for_status()
            self.jwks_url = configuration_response.json().get("jwks_uri")
            jwks_response = requests.get(self.jwks_url,verify=self.trusted_certificate_path)
            jwks_json= jwks_response.json()
            cache_jwks(jwks_json)
            self.logger.debug("JWKS fetched and cached.")
        except requests.RequestException as e:
            self.logger.exception(e)
    
    def get_jwks(self):
        if not JWKS_CACHE:
            self.fetch_and_cache_jwks()
        return JWKS_CACHE

    def get_public_key(self,token: str):
        headers = jwt.get_unverified_header(token)
        kid=headers.get("kid")
        jwks = self.get_jwks()
        if not kid or "keys" not in jwks:
            raise InvalidTokenError("Invalid token: Missing key ID or JWKS is empty.")
        
        for key in jwks["keys"]:
            if key["kid"] == kid:
                return RSAAlgorithm.from_jwk(json.dumps(key))
            
        raise InvalidTokenError("No matching public key found.")
    
    def validate_token(self,token:str)->Any|None:
        try:
            public_key = self.get_public_key(token=token)
            decoded_token = jwt.decode(jwt= token,key= public_key,algorithms=["RS256"],issuer=self.issuer,options={"verify_aud":False})
            return decoded_token
        except InvalidTokenError as e:
            self.logger.exception(e)
            return None
    

