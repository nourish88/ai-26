import logging
from src.utils import request_context
class RequestContextFilter(logging.Filter):
    def filter(self, record: logging.LogRecord) -> bool:
        ctx = request_context.get(None) 
        if ctx is not None:            
            record.request = ctx
        return True
