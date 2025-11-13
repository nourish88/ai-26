import langchain_text_splitters

from src.chunking.base_splitter import BaseSplitter
from src.logging.logger_factory import create_logger


class RecursiveCharacterTextSplitter(BaseSplitter):
    def __init__(self):
        self.logger = create_logger(__name__)

    def split(self, document: str, chunk_size: int, chunk_overlap: int) -> list[str]:
        text_splitter = langchain_text_splitters.RecursiveCharacterTextSplitter(
            chunk_size=chunk_size, chunk_overlap=chunk_overlap
        )

        splits = text_splitter.split_text(document)
        self.logger.info("Document has been split into %s chunks", len(splits))
        return splits