from environs import env

# Loads configuration from .env file
env.read_env()

class ConfigurationManager: 
    @staticmethod
    def get_admin_backend_url() -> str:
        return env.str("ADMIN_BACKEND_URL")
    
    @staticmethod
    def get_litellm_api_key() -> str:
        return env.str("LITE_LLM_API_KEY")
    
    @staticmethod
    def get_app_port() -> int:
        return env.int("APP_PORT_ADDRESS")
    
    @staticmethod
    def get_mongo_connection_string() -> str:
        return env.str("MONGO_CONN_STRING")
    
    @staticmethod
    def get_chat_database_name() -> str:
        return env.str("CHAT_DATABASE")
    
    @staticmethod
    def get_auth_issuer() -> str:
        return env.str("AUTH_ISSUER")
    
    @staticmethod
    def get_auth_audience() -> str:
        return env.str("AUTH_AUDIENCE")
    
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
    def get_rag_retriever_debug_mode() -> bool:
        return env.bool("RAG_RETRIEVER_DEBUG_MODE")
    
    @staticmethod
    def get_enable_log_to_elastic() -> bool:
        return env.bool("ENABLE_LOG_TO_ELASTIC")
    
    @staticmethod
    def get_es_host() -> str:
        return env.str("ES_HOST")   
    
    @staticmethod
    def get_es_index() -> str:
        return env.str("ES_INDEX")
    
    @staticmethod
    def get_enable_agent_observability() -> bool:
        return env.bool("ENABLE_AGENT_OBSERVABILITY")
    
    @staticmethod
    def get_langfuse_address() -> str:
        return env.str("LANGFUSE_ADDRESS")
    
    @staticmethod
    def get_langfuse_public_key() -> str:
        return env.str("LANGFUSE_PUBLIC_KEY")
    
    @staticmethod
    def get_langfuse_secret_key() -> str:
        return env.str("LANGFUSE_SECRET_KEY")
