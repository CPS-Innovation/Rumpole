using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.DocumentRedaction;
using RumpoleGateway.Domain.Logging;
using RumpoleGateway.Extensions;
using RumpoleGateway.Mappers;
using RumpoleGateway.Services;

namespace RumpoleGateway.Clients.DocumentRedaction
{
    [ExcludeFromCodeCoverage]
    public class DocumentRedactionClientStub : IDocumentRedactionClient
    {
        private readonly IRedactPdfRequestMapper _redactPdfRequestMapper;
        private readonly IRedactionClient _redactionClient;
        private readonly ISasGeneratorService _sasGeneratorService;
        private readonly ILogger<DocumentRedactionClientStub> _logger;

        public DocumentRedactionClientStub(IRedactPdfRequestMapper redactPdfRequestMapper, IRedactionClient redactionClient, ISasGeneratorService sasGeneratorService, ILogger<DocumentRedactionClientStub> logger)
        {
            _redactPdfRequestMapper = redactPdfRequestMapper;
            _redactionClient = redactionClient;
            _sasGeneratorService = sasGeneratorService;
            _logger = logger;
        }

        public Task<DocumentRedactionStatus> CheckOutDocumentAsync(string caseId, string documentId, string accessToken, Guid correlationId)
        {
            _logger.LogMethodEntry(correlationId, nameof(CheckOutDocumentAsync), $"CaseId: {caseId}, DocumentId: {documentId}");
            var result = Task.FromResult(DocumentRedactionStatus.CheckedOut);
            _logger.LogMethodExit(correlationId, nameof(CheckOutDocumentAsync), string.Empty);
            return result;
        }

        public Task<DocumentRedactionStatus> CheckInDocumentAsync(string caseId, string documentId, string accessToken, Guid correlationId)
        {
            _logger.LogMethodEntry(correlationId, nameof(CheckInDocumentAsync), $"CaseId: {caseId}, DocumentId: {documentId}");
            var result = Task.FromResult(DocumentRedactionStatus.CheckedIn);
            _logger.LogMethodExit(correlationId, nameof(CheckInDocumentAsync), string.Empty);
            return result;
        }

        public async Task<DocumentRedactionSaveResult> SaveRedactionsAsync(string caseId, string documentId, string fileName, DocumentRedactionSaveRequest saveRequest, string accessToken, Guid correlationId)
        {
            _logger.LogMethodEntry(correlationId, nameof(SaveRedactionsAsync), $"CaseId: {caseId}, DocumentId: {documentId}, FileName: {fileName}, SaveRedactionsCollection: {saveRequest.ToJson()}");
            
            AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);
            var saveResult = new DocumentRedactionSaveResult();

            try
            {
                var redactPdfRequest = _redactPdfRequestMapper.Map(saveRequest, caseId, documentId, fileName, correlationId);
                var redactionResult = await _redactionClient.RedactPdfAsync(redactPdfRequest, accessToken, correlationId);

                if (!redactionResult.Succeeded)
                {
                    saveResult.Succeeded = false;
                    saveResult.Message = $"Invalid document - a document with filename '{fileName}' could not be redacted - '{redactionResult.Message}'";
                    return saveResult;
                }

                var redactedFileUri = await _sasGeneratorService.GenerateSasUrlAsync(redactionResult.RedactedDocumentName, correlationId);

                saveResult.Succeeded = true;
                saveResult.RedactedDocumentUrl = redactedFileUri;

                return saveResult;
            }
            catch (Exception ex)
            {
                var message = $"Could not complete the redaction request for filename '{fileName}'";
                _logger.LogMethodError(correlationId, nameof(SaveRedactionsAsync), message, ex);
                
                saveResult.Succeeded = false;
                saveResult.Message = $"{message} - {ex.Message}";
                return saveResult;
            }
            finally
            {
                _logger.LogMethodExit(correlationId, nameof(SaveRedactionsAsync), saveResult.ToJson());
            }
        }
    }
}
