from datetime import datetime

from uuid_extensions import uuid7str
from typing import Final

from src.chunking.splitter_factory import SplitterFactory
from src.extraction.extractor_factory import ExtractorFactory
from src.jobs.delete_job import DeleteJob
from src.logging.logger_factory import create_logger
from src.models.job_request import JobRequest
from src.service.admin_backend_service import AdminBackendService
from src.service.ingestion_status_types import IngestionStatusTypes
from src.storage.base_storage import BaseStorage
from src.storage.storage_factory import StorageFactory


class ExtractChunkJob:
    RAW_FILE_TEMPLATE: Final = "{doc_id}/raw/{doc_id}.{extension}"
    EXTRACTED_FILE_TEMPLATE: Final = "{doc_id}/txt/{doc_id}.txt"
    CHUNK_FILE_TEMPLATE: Final = "{doc_id}/chunks/{chunk_id}.txt"
    JOB_COMPLETE_TEMPLATE: Final = "{doc_id}/job/completed"

    def __init__(self):
        self.storage_factory = StorageFactory()
        self.extractor_factory = ExtractorFactory()
        self.splitter_factory = SplitterFactory()
        self.logger = create_logger(__name__)
        self.delete_job = DeleteJob()
        self.admin_backend_service = AdminBackendService()

    async def _notify_chunking(self, model: JobRequest) -> None:
        self.admin_backend_service.change_file_status(model.file_id, IngestionStatusTypes.Chunking)
        self.logger.info("Chunking starting status has been notified to backend")

    async def _notify_extracting(self, model: JobRequest) -> None:
        self.admin_backend_service.change_file_status(model.file_id, IngestionStatusTypes.Extracting)
        self.logger.info("Extraction starting status has been notified to backend")

    async def _notify_indexing(self, model: JobRequest) -> None:
        self.admin_backend_service.change_file_status(model.file_id, IngestionStatusTypes.Indexing)
        self.logger.info("Completion status has been notified to backend and indexing can be started..")

    async def _notify_error(self, model: JobRequest, error: BaseException) -> None:
        self.logger.info("Error has been notified to backend")
        self.admin_backend_service.set_error_file_status(model.file_id, str(error))
        pass

    async def execute(self, model: JobRequest) -> None:
        storage = self.storage_factory.get_storage(model.storage_type)

        try:
            self.logger.info("Document %s processing is started", model.document_id)

            if await self._check_already_processed(model, storage):
                return

            if not await self._check_if_file_status_is_processing_requested(model):
                return

            await self._notify_extracting(model)
            extracted_text = await self._extract_document(model, storage)
            await self._save_extracted_text(model, storage, extracted_text)

            await self._notify_chunking(model)
            await self._process_chunks(model, storage, extracted_text)

            await self._mark_job_completed(model, storage)
        except BaseException as e:
            await self._handle_error(model, storage, e)

    async def _check_if_file_status_is_processing_requested(self, model: JobRequest):
        status = self.admin_backend_service.get_file_status(model.file_id)
        if status != IngestionStatusTypes.ProcessingRequested:
            self.logger.warning("File %s has status of %s. Skipping..", model.file_id, status.name)
            return False
        return True

    async def _check_already_processed(self, model: JobRequest, storage: BaseStorage) -> bool:
        job_complete_path = self.JOB_COMPLETE_TEMPLATE.format(doc_id=model.document_id)
        if storage.object_exists(job_complete_path):
            self.logger.warning("Document %s was already processed. Skipping..", model.document_id)
            await self._notify_indexing(model)
            return True
        return False

    async def _extract_document(self, model: JobRequest, storage: BaseStorage) -> str:
        raw_path = self.RAW_FILE_TEMPLATE.format(doc_id=model.document_id, extension=model.file_extension)
        raw_data = storage.get_object(raw_path)
        extractor = self.extractor_factory.get_extractor(model.extractor_type)
        return extractor.extract(data=raw_data, file_extension=model.file_extension)

    async def _save_extracted_text(self, model: JobRequest, storage: BaseStorage, text: str) -> None:
        extracted_path = self.EXTRACTED_FILE_TEMPLATE.format(doc_id=model.document_id)
        storage.put_object(path=extracted_path, data=text.encode("utf-8"))
        self.logger.info("Extracted document text has been written to %s", extracted_path)

    async def _process_chunks(self, model: JobRequest, storage: BaseStorage, text: str) -> None:
        splitter = self.splitter_factory.get_splitter(model.chunk_type)
        chunks = splitter.split(document=text,
                                chunk_size=model.chunk_size,
                                chunk_overlap=model.chunk_overlap)

        self.logger.info("Writing chunks to storage")
        for chunk in chunks:
            chunk_id = uuid7str()
            chunk_path = self.CHUNK_FILE_TEMPLATE.format(doc_id=model.document_id, chunk_id=chunk_id)
            storage.put_object(path=chunk_path, data=chunk.encode("utf-8"))
            self.logger.info("Chunk %s has been written", chunk_id)
        self.logger.info("All chunks have been written to storage")

    async def _mark_job_completed(self, model: JobRequest, storage: BaseStorage) -> None:
        complete_path = self.JOB_COMPLETE_TEMPLATE.format(doc_id=model.document_id)
        storage.put_object(path=complete_path, data=str(datetime.now()).encode("utf-8"))
        self.logger.info("Document %s processing is completed", model.document_id)
        await self._notify_indexing(model)

    async def _handle_error(self, model: JobRequest, storage: BaseStorage, error: BaseException) -> None:
        self.logger.exception("Document %s processing has been failed", model.document_id)
        self.delete_job.delete(model.document_id, storage)
        await self._notify_error(model, error)
