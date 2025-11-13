from abc import abstractmethod, ABC


class BaseStorage(ABC):
    @abstractmethod
    def get_object(self, path: str) -> bytes:
        pass

    @abstractmethod
    def put_object(self, path: str, data: bytes) -> None:
        pass

    @abstractmethod
    def delete_objects(self, path: str) -> None:
        pass

    @abstractmethod
    def object_exists(self, path: str) -> bool:
        pass
