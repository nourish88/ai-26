from pydantic import BaseModel

from src.chunking.splitter_types import SplitterTypes
from src.extraction.extractor_types import ExtractorTypes
from src.storage.storage_types import StorageTypes


class JobRequest(BaseModel):
    application_id: int
    file_id: int
    file_store_id: int
    document_id: str
    file_extension: str
    bucket_name: str | None
    storage_type: StorageTypes
    extractor_type: ExtractorTypes
    chunk_type: SplitterTypes
    chunk_size: int
    chunk_overlap: int
