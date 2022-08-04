﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using RumpoleGateway.Clients.DocumentExtraction;

namespace RumpoleGateway.Functions.DocumentExtraction
{
    public class DocumentExtractionGetDocument : BaseRumpoleFunction
    {
        private readonly IDocumentExtractionClient _documentExtractionClient;
        
        public DocumentExtractionGetDocument(IDocumentExtractionClient documentExtractionClient, ILogger<DocumentExtractionGetCaseDocuments> logger)
            : base(logger)
        {
            _documentExtractionClient = documentExtractionClient;
        }

        [FunctionName("DocumentExtractionGetDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "documents/{documentId}/{*fileName}")] HttpRequest req, string documentId, string fileName)
        {
            try
            {
                if (!req.Headers.TryGetValue(Constants.Authentication.Authorization, out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                    return AuthorizationErrorResponse();
                
                if (string.IsNullOrWhiteSpace(documentId))
                    return BadRequestErrorResponse("Document id is not supplied.");
                
                if (string.IsNullOrWhiteSpace(fileName))
                    return BadRequestErrorResponse("FileName is not supplied.");
                
                //TODO exchange access token via on behalf of?
                var document = await _documentExtractionClient.GetDocumentAsync(documentId, fileName, "accessToken");

                return document != null ? new OkObjectResult(document) 
                    : NotFoundErrorResponse($"No document found for file name '{fileName}'."); // TODO change this to document id when hooked up to CDE
            }
            catch (Exception exception)
            {
                return exception switch
                {
                    _ => InternalServerErrorResponse(exception, "An unhandled exception occurred.")
                };
            }
        }
    }
}

