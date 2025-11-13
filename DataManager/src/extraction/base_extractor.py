from abc import ABC, abstractmethod

from src.extraction.content_types import ContentTypes


class BaseExtractor(ABC):
    @abstractmethod
    def is_supported_content_type(self, file_extension: str) -> bool:
        pass

    @abstractmethod
    def extract(self, data: bytes, file_extension: str) -> str:
        pass

    @staticmethod
    def convert_to_content_type(file_extension: str) -> ContentTypes:
        extension = file_extension.lower()
        if extension == "pdf":
            return ContentTypes.PDF
        if extension == "docx":
            return ContentTypes.DOCX
        if extension == "txt":
            return ContentTypes.TXT

        raise ValueError("Unsupported file extension")
