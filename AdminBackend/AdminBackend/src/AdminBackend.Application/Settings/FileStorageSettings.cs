namespace AdminBackend.Application.Settings;

public class FileStorageSettings
{
    public const string SectionName = "FileStorageSettings";
    public Dictionary<string, string> BucketNameMappings { get; set; } = null!;
}