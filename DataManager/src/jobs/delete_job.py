from src.logging.logger_factory import create_logger
from src.storage.base_storage import BaseStorage

class DeleteJob:
    def __init__(self):
        self.logger = create_logger(__name__)

    def delete(self, document_id: str, storage: BaseStorage):
        try:
            storage.delete_objects(document_id)
            self.logger.info("All files related to document %s are deleted", document_id)
        except BaseException:
            self.logger.exception("Unable to delete files related to document %s", document_id)
