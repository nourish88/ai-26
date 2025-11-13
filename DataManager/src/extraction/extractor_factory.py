from src.extraction.base_extractor import BaseExtractor
from src.extraction.extractor_types import ExtractorTypes
from src.extraction.markitdown_extractor import MarkItDownExtractor
from src.logging.logger_factory import create_logger


class ExtractorFactory:
    def __init__(self):
        self.__extractors = {ExtractorTypes.MARKITDOWN: MarkItDownExtractor()}
        self.logger = create_logger(__name__)

    def get_extractor(self, extractor_type: ExtractorTypes) -> BaseExtractor:
        result = self.__extractors.get(extractor_type)

        if result:
            self.logger.info("%s extractor will be used", extractor_type)
            return result

        raise ValueError("Unsupported extractor type")
