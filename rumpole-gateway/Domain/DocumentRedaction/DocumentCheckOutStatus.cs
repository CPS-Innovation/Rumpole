namespace RumpoleGateway.Domain.DocumentRedaction
{
    public enum DocumentCheckOutStatus
    {
        CheckedOut = 1,
        AlreadyCheckedOut = 2,
        NotFound = 0,
        SystemError = 3
    }
}
