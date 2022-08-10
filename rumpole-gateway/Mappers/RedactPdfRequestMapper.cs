using System;
using System.Collections.Generic;
using System.Linq;
using RumpoleGateway.Domain.DocumentRedaction;

namespace RumpoleGateway.Mappers
{
    public class RedactPdfRequestMapper : IRedactPdfRequestMapper
    {
        public RedactPdfRequest Map(DocumentRedactionSaveRequest saveRequest, string caseId, string documentId, string fileName)
        {
            if (saveRequest == null) throw new ArgumentNullException(nameof(saveRequest));

            var result = new RedactPdfRequest
            {
                CaseId = caseId,
                DocumentId = documentId,
                FileName = fileName,
                RedactionDefinitions = new List<RedactionDefinition>()
            };

            foreach (var item in saveRequest.Redactions)
            {
                var redactionDefinition = new RedactionDefinition
                {
                    PageIndex = item.PageIndex,
                    Height = item.Height,
                    Width = item.Width,
                    RedactionCoordinates = new List<RedactionCoordinates>()
                };
                foreach (var redactionCoordinates in item.RedactionCoordinates.Select(coordinates => new RedactionCoordinates
                         {
                             X1 = coordinates.X1,
                             Y1 = coordinates.Y1,
                             X2 = coordinates.X2,
                             Y2 = coordinates.Y2
                         }))
                {
                    redactionDefinition.RedactionCoordinates.Add(redactionCoordinates);
                }

                result.RedactionDefinitions.Add(redactionDefinition);
            }

            return result;
        }
    }
}
