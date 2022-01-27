namespace Services.SearchDataStorageService
{
    public class SearchDataIndexOptions
    {
        public bool Enabled { get; set; }
        
        public string EndpointUrl { get; set; }

        public string AuthorizationKey { get; set; }

        public string IndexName { get; set; }
    }
}