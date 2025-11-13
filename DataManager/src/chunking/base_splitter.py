from abc import ABC, abstractmethod


class BaseSplitter(ABC):
    @abstractmethod
    def split(self, document: str, chunk_size: int, chunk_overlap: int) -> list[str]:
        pass
