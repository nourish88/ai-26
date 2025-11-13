namespace AdminBackend.Application.Settings
{
    public class IndexSettings
    {
        public const string SectionName = "IndexSettings";
        public Dictionary<string,string> ServiceUserNameMappings {  get; set; }
        public Dictionary<string,string> ServicePasswordMappings {  get; set; }
    }
}
