using RumpoleGateway.Domain.DocumentRedaction;

namespace RumpoleGateway.Mappers
{
    public interface IRedactPdfRequestMapper
    {
        RedactPdfRequest Map(DocumentRedactionSaveRequest saveRequest, string caseId, string documentId, string fileName);
    }
}
