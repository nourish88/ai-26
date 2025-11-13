import io

from markitdown import MarkItDown, DocumentConverterResult

from src.extraction.base_extractor import BaseExtractor
from src.extraction.content_types import ContentTypes
from src.logging.logger_factory import create_logger


class MarkItDownExtractor(BaseExtractor):
    def __init__(self):
        self.logger = create_logger(__name__)
        self.markitdown = MarkItDown(enable_plugins=True)

    def is_supported_content_type(self, file_extension: str) -> bool:
        content_type = BaseExtractor.convert_to_content_type(file_extension)
        return content_type in [ContentTypes.PDF, ContentTypes.TXT, ContentTypes.DOCX]

    def extract(self, data: bytes, file_extension: str) -> str:
        if not self.is_supported_content_type(file_extension):
            raise ValueError("Unsupported content type")

        self.logger.info("Document is converting to markdown")
        result: DocumentConverterResult = self.markitdown.convert_stream(io.BytesIO(data))
        self.logger.info("Document is converted to markdown")
        return result.markdown
