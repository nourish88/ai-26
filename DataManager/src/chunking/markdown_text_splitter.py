from langchain_text_splitters import MarkdownHeaderTextSplitter, RecursiveCharacterTextSplitter

from src.chunking.base_splitter import BaseSplitter
from src.logging.logger_factory import create_logger


class MarkdownTextSplitter(BaseSplitter):
    def __init__(self):
        self.logger = create_logger(__name__)
        headers_to_split_on = [
            ("#", "Header 1"),
            ("##", "Header 2"),
            ("###", "Header 3")
        ]
        self.markdown_splitter = MarkdownHeaderTextSplitter(
            headers_to_split_on=headers_to_split_on, strip_headers=False
        )

    def split(self, document: str, chunk_size: int, chunk_overlap: int) -> list[str]:
        md_header_splits = self.markdown_splitter.split_text(document)

        text_splitter = RecursiveCharacterTextSplitter(
            chunk_size=chunk_size, chunk_overlap=chunk_overlap
        )

        splits = text_splitter.split_documents(md_header_splits)
        self.logger.info("Markdown document has been split into %s chunks", len(splits))

        return list(map(lambda x: x.page_content, splits))
