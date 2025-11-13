namespace AdminBackend.Domain.Models.Services.Integrations.LiteLlm
{
    public class EmbeddingResponse
    {
        public string model {  get; set; }
        public Datum[] data { get; set; }
        public string _object {  get; set; }
        public Usage usage { get; set; }
    }

    public class Datum
    {
        public string _object { get; set; }
        public int index { get; set; }
        public float[] embedding { get; set; }
    }

    public class Usage
    {
        public int completion_tokens {  get; set; }
        public int prompt_tokens {  get; set; }
    }


}
