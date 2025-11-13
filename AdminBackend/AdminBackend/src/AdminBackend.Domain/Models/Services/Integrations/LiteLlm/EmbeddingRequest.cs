namespace AdminBackend.Domain.Models.Services.Integrations.LiteLlm
{
    public class EmbeddingRequest
    {
        public string model { get; set; }
        public string input { get; set; }
        public string baseUrl {  get; set; }
        public string apiKey { get; set; }
    }
}
