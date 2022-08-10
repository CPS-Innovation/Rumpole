﻿using System;
using System.IO;
using System.Threading.Tasks;
using Aspose.Pdf;
using Aspose.Pdf.Annotations;
using RumpoleGateway.Clients.RumpolePipeline;
using RumpoleGateway.Domain.DocumentRedaction;
using RumpoleGateway.Services;

namespace RumpoleGateway.Clients.DocumentRedaction
{
    public class DocumentRedactionClientStub : IDocumentRedactionClient
    {
        private readonly IBlobStorageClient _blobStorageClient;
        private readonly ISasGeneratorService _sasGeneratorService;

        public DocumentRedactionClientStub(IBlobStorageClient blobStorageClient, ISasGeneratorService sasGeneratorService)
        {
            _blobStorageClient = blobStorageClient;
            _sasGeneratorService = sasGeneratorService;
        }

        public Task<DocumentRedactionStatus> CheckOutDocument(string caseId, string documentId, string accessToken)
        {
            return Task.FromResult(DocumentRedactionStatus.CheckedOut);
        }

        public Task<DocumentRedactionStatus> CheckInDocument(string caseId, string documentId, string accessToken)
        {
            return Task.FromResult(DocumentRedactionStatus.CheckedIn);
        }

        public async Task<DocumentRedactionSaveResult> SaveRedactions(string caseId, string documentId, string fileName, DocumentRedactionSaveRequest saveRequest, string accessToken)
        {
            var saveResult = new DocumentRedactionSaveResult();

            var document = await _blobStorageClient.GetDocumentAsync(fileName);
            if (document == null)
            {
                saveResult.Succeeded = false;
                saveResult.Message = $"Invalid document - a document with filename '{fileName}' could not be retrieved for redaction purposes";
                return saveResult;
            }

            var fileNameWithoutExtension = fileName.IndexOf(".pdf", StringComparison.OrdinalIgnoreCase) > -1 ? fileName.Split(".pdf", StringSplitOptions.RemoveEmptyEntries)[0] : fileName;
            var newFileName = $"{fileNameWithoutExtension}_{DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper()}.pdf";

            using var doc = new Document(document);

            foreach (var redactionPage in saveRequest.Redactions)
            {
                var currentPage = redactionPage.PageIndex + 1;
                var annotationPage = doc.Pages[currentPage];
                foreach (var boxToDraw in redactionPage.RedactionCoordinates)
                {
                    var annotationRectangle = new Rectangle(boxToDraw.X1, boxToDraw.Y1, boxToDraw.X2, boxToDraw.Y2);
                    var redactionAnnotation = new RedactionAnnotation(annotationPage, annotationRectangle)
                        {
                            FillColor = Color.Black
                        };

                    doc.Pages[currentPage].Annotations.Add(redactionAnnotation, true);
                    redactionAnnotation.Redact();
                }
            }

            using var redactedDocumentStream = new MemoryStream();
            doc.Save(redactedDocumentStream);

            await _blobStorageClient.UploadDocumentAsync(redactedDocumentStream, newFileName);

            var redactedFileUri = await _sasGeneratorService.GenerateSasUrlAsync(newFileName);

            saveResult.Succeeded = true;
            saveResult.RedactedDocumentUrl = redactedFileUri;

            return saveResult;
        }
    }
}
