namespace AdminBackend.Domain.Models.Services.Search
{
    public record SemanticSearchOutput
    (
        string id,
        string parentid,
        string applicationid,
        string datasourceid,
        string userid,
        string title,
        string sourceurl,
        string storeidentifier,
        string bucket,
        string filepath,
        string content,
        string[] keywords,
        string sentiment
    );
}
