import langchain_text_splitters
from langchain_text_splitters import HTMLHeaderTextSplitter

from src.chunking.base_splitter import BaseSplitter
from src.logging.logger_factory import create_logger


class HtmlHeaderTextSplitter(BaseSplitter):
    def __init__(self):
        self.logger = create_logger(__name__)
        headers_to_split_on = [
            ("h1", "Header 1"),
            ("h2", "Header 2"),
            ("h3", "Header 3"),
        ]

        self.html_splitter = HTMLHeaderTextSplitter(headers_to_split_on)


    def split(self, document: str, chunk_size: int, chunk_overlap: int) -> list[str]:
        html_header_splits = self.html_splitter.split_text(document)

        text_splitter = langchain_text_splitters.RecursiveCharacterTextSplitter(
            chunk_size=chunk_size, chunk_overlap=chunk_overlap
        )

        splits = text_splitter.split_documents(html_header_splits)
        self.logger.info("Document has been split into %s chunks", len(splits))
        return list(map(lambda x: x.page_content, splits))