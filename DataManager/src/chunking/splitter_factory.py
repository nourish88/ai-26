from src.chunking.base_splitter import BaseSplitter
from src.chunking.html_header_text_splitter import HtmlHeaderTextSplitter
from src.chunking.markdown_text_splitter import MarkdownTextSplitter
from src.chunking.recursive_character_text_splitter import RecursiveCharacterTextSplitter
from src.chunking.splitter_types import SplitterTypes
from src.logging.logger_factory import create_logger


class SplitterFactory:
    def __init__(self):
        self.__splitters = {SplitterTypes.MARKDOWN: MarkdownTextSplitter(),
                            SplitterTypes.FIXED_SIZE:RecursiveCharacterTextSplitter(),
                            SplitterTypes.HTML:HtmlHeaderTextSplitter()}
        self.logger = create_logger(__name__)

    def get_splitter(self, text_splitters: SplitterTypes) -> BaseSplitter:
        result = self.__splitters.get(text_splitters)
        if result:
            self.logger.info("%s splitter will be used", text_splitters)
            return result

        raise ValueError("Unsupported splitter type")
