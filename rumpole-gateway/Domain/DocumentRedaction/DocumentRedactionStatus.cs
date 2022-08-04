namespace RumpoleGateway.Domain.DocumentRedaction
{
    public enum DocumentRedactionStatus
    {
        CheckedOut = 1,
        AlreadyCheckedOut = 2,
        NotFound = 0,
        CheckedIn = 3
    }
}
