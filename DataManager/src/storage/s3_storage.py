import io

from minio import Minio, S3Error
from minio.deleteobjects import DeleteObject
from tenacity import retry, wait_random_exponential, stop_after_attempt

from src.configuration.configuration_manager import ConfigurationManager
from src.logging.logger_factory import create_logger
from src.storage.base_storage import BaseStorage


class S3Storage(BaseStorage):
    def __init__(self):
        self.logger = create_logger(__name__)
        self.bucket_name = ConfigurationManager.get_s3_bucket_name()
        self.client = Minio(endpoint=ConfigurationManager.get_s3_url(),
                            access_key=ConfigurationManager.get_s3_access_key(),
                            secret_key=ConfigurationManager.get_s3_secret_key(),
                            secure=ConfigurationManager.get_is_s3_url_secure())

    @retry(reraise=True, wait=wait_random_exponential(min=1, max=5), stop=stop_after_attempt(3))
    def get_object(self, path: str) -> bytes:
        response = None
        try:
            self.logger.info("Document %s is getting from storage", path)
            response = self.client.get_object(bucket_name=self.bucket_name, object_name=path)
            return response.data
        finally:
            if response:
                response.close()
                response.release_conn()

    @retry(reraise=True, wait=wait_random_exponential(min=1, max=5), stop=stop_after_attempt(3))
    def put_object(self, path: str, data: bytes) -> None:
        self.logger.info("Document %s is putting into storage", path)
        self.client.put_object(bucket_name=self.bucket_name,
                                        object_name=path,
                                        data=io.BytesIO(data),
                                        length=len(data))

    def __list_objects(self, path: str) -> list[str]:
        self.logger.info("Getting object list in path %s", path)
        response = self.client.list_objects(bucket_name=self.bucket_name, prefix=f"{path.rstrip("/")}/", recursive=True)
        return list(map(lambda x: x.object_name, response))

    @retry(reraise=True, wait=wait_random_exponential(min=1, max=5), stop=stop_after_attempt(3))
    def object_exists(self, path: str) -> bool:
        response = None
        try:
            response = self.client.get_object(bucket_name=self.bucket_name, object_name=path, offset=0, length=1)
            return True
        except S3Error as e:
            if e.code == "NoSuchKey":
                return False
            raise
        finally:
            if response:
                response.close()
                response.release_conn()

    @retry(reraise=True, wait=wait_random_exponential(min=1, max=5), stop=stop_after_attempt(3))
    def delete_objects(self, path: str) -> None:
        self.logger.info("Path %s is deleting from storage", path)
        obj_list = self.__list_objects(path)
        if len(obj_list) > 0:
            delete_objects = []
            for obj in obj_list:
                delete_objects.append(DeleteObject(obj))
            errors = self.client.remove_objects(bucket_name=self.bucket_name, delete_object_list=delete_objects)
            for error in errors:
                print("error occurred when deleting object", error)
