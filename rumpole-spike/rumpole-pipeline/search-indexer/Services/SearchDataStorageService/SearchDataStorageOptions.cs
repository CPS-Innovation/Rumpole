namespace Services.SearchDataStorageService
{
    public class SearchDataStorageOptions
    {
        public string EndpointUrl { get; set; }

        public string AuthorizationKey { get; set; }

        public string DatabaseName { get; set; }

        public string ContainerName { get; set; }
    }
}