namespace Services.SearchDataStorageService
{
    public class SearchDataStorageOptions
    {
        public bool Enabled { get; set; }

        public string EndpointUrl { get; set; }

        public string AuthorizationKey { get; set; }

        public string DatabaseName { get; set; }

        public string ContainerName { get; set; }
    }
}