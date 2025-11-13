from enum import Enum


class SplitterTypes(str, Enum):
    MARKDOWN = "markdown"
    FIXED_SIZE = "fixed-size"
    HTML = "html"
