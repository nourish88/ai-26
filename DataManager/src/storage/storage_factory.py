from src.logging.logger_factory import create_logger
from src.storage.base_storage import BaseStorage
from src.storage.s3_storage import S3Storage
from src.storage.storage_types import StorageTypes


class StorageFactory:
    def __init__(self):
        self.logger = create_logger(__name__)
        self.__storages = {StorageTypes.S3: S3Storage()}

    def get_storage(self, storage_type: StorageTypes) -> BaseStorage:
        result = self.__storages.get(storage_type)

        if result:
            self.logger.info("%s storage instance will be used", storage_type)
            return result

        raise ValueError("Unsupported storage type")
