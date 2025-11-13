namespace AdminBackend.Domain.Models.Services.Search
{
    public class ElasticSearchEntity
    {
        public string id{ get; set; }
        public string parentid{ get; set; }
        public string applicationid{ get; set; }
        public string datasourceid{ get; set; }
        public string userid{ get; set; }
        public string title{ get; set; }
        public string sourceurl{ get; set; }
        public string storeidentifier{ get; set; }
        public string bucket{ get; set; }
        public string filepath{ get; set; }
        public string content{ get; set; }
        public string[] keywords{ get; set; }
        public string sentiment{ get; set; }
        public int filetype { get; set; }
        public List<float> contentvector { get; set; }
    }
}
