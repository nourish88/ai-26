from environs import env

# Loads configuration from .env file
env.read_env()

class ConfigurationManager:
    @staticmethod
    def get_is_s3_url_secure() -> bool:
        return env.bool("IS_S3_URL_SECURE", False)

    @staticmethod
    def get_s3_url() -> str:
        return env.str("S3_URL")

    @staticmethod
    def get_s3_access_key() -> str:
        return env.str("S3_ACCESS_KEY")

    @staticmethod
    def get_s3_secret_key() -> str:
        return env.str("S3_SECRET_KEY")

    @staticmethod
    def get_s3_bucket_name() -> str:
        return env.str("S3_BUCKET_NAME")

    @staticmethod
    def get_app_host() -> str:
        return env.str("APP_HOST")

    @staticmethod
    def get_app_port() -> int:
        return env.int("APP_PORT")

    @staticmethod
    def get_admin_backend_url() -> str:
        return env.str("ADMIN_BACKEND_URL")
    
    @staticmethod
    def get_auth_issuer() -> str:
        return env.str("AUTH_ISSUER")
    
    @staticmethod
    def get_auth_audience() -> str:
        return env.str("AUTH_AUDIENCE")    
    
    @staticmethod
    def get_auth_client_secret() -> str:
        return env.str("AUTH_CLIENT_SECRET")
    
    @staticmethod
    def get_trusted_certificate_path() -> str|None:
        value = env.str("TRUSTED_CERTIFICATE_PATH")
        if not value:
            return None
        else:
            return value 
        
    @staticmethod
    def get_auth_enabled() -> bool:
        return env.bool("AUTH_ENABLED")
    
    @staticmethod
    def get_redis_url() -> str:
        return env.str("REDIS_URL")
    
    @staticmethod
    def get_redis_password() -> str|None:
        value = env.str("REDIS_PASSWORD")
        if not value:
            return None
        else:
            return value
    
    @staticmethod
    def get_default_cache_expire() -> int:
        return env.int("DEFAULT_CACHE_EXPIRE_IN_MINUTE")
    
    @staticmethod
    def get_enable_log_to_elastic() -> bool:
        return env.bool("ENABLE_LOG_TO_ELASTIC")
    
    @staticmethod
    def get_es_host() -> str:
        return env.str("ES_HOST")   
    
    @staticmethod
    def get_es_index() -> str:
        return env.str("ES_INDEX")
