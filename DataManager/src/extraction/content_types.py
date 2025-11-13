from enum import Enum


class ContentTypes(str, Enum):
    PDF = "pdf",
    TXT = "txt",
    DOCX = "docx",
