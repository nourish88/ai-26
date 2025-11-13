namespace AdminBackend.Domain.Constants;

public enum IngestionStatusTypes : int
{
    ProcessingRequested = 1,
    Extracting = 2,
    Chunking = 3,
    Indexing = 4,
    Processed = 5,
    DeletingRequested = 6,
    ProcessingFailed = 7
}