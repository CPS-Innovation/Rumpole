namespace RumpoleGateway.Domain.Config
{
    public class BlobOptions
    {
        public string BlobContainerName { get; set; }

        public int BlobExpirySecs { get; set; }

        public int UserDelegationKeyExpirySecs { get; set; }
    }
}
