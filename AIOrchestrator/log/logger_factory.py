import logging
import json
from datetime import datetime
from .context_filters import RequestContextFilter
from .elastic_handler import PYESHandler
from server import ConfigurationManager

def create_logger(name: str,*,level: int = logging.INFO) -> logging.Logger:
    lvl = level
    logger = logging.getLogger(name)
    logger.setLevel(lvl)

    if not logger.handlers:
        stream = logging.StreamHandler()
        stream.setLevel(lvl)

        # <-- JSON formatter
        class JsonFormatter(logging.Formatter):
            def format(self, record: logging.LogRecord) -> str:
                payload = {
                    "timestamp": datetime.now().isoformat() + "Z",
                    "level": record.levelname,
                    "name": record.name,
                    "message": record.getMessage(),
                    "file": f"{record.filename}:{record.lineno}",
                    "process": record.process                    
                }

                standard_keys = {
                "name", "msg", "args", "levelname", "levelno",
                "pathname", "filename", "module", "exc_info",
                "exc_text", "stack_info", "lineno", "funcName",
                "created", "msecs", "relativeCreated", "thread",
                "threadName", "processName", "process", "message","taskName"
                }

                for key, value in record.__dict__.items():
                    if key not in standard_keys:
                        payload[key] = value                

                return json.dumps(payload)

        stream.setFormatter(JsonFormatter())
        stream.addFilter(RequestContextFilter())
        logger.addHandler(stream)

        if ConfigurationManager.get_enable_log_to_elastic():
            # ES bağlantı bilgileri (örn. .env veya CI ortam değişkenleri)
            es_host   = ConfigurationManager.get_es_host()
            es_index  = ConfigurationManager.get_es_index()

            es_handler = PYESHandler(
                hosts=[es_host]
                ,auth_type=PYESHandler.AuthType.NO_AUTH
                ,raise_on_indexing_exceptions=True
                ,buffer_size=100
                ,es_index_name=es_index,index_name_frequency=PYESHandler.IndexNameFrequency.NEVER)
            es_handler.setLevel(level)
            es_handler.setFormatter(JsonFormatter())
            es_handler.addFilter(RequestContextFilter())
            logger.addHandler(es_handler)

    return logger
