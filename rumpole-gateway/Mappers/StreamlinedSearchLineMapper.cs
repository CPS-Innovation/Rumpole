using System.Collections.Generic;
using RumpoleGateway.Domain.RumpolePipeline;

namespace RumpoleGateway.Mappers
{
    public class StreamlinedSearchLineMapper : IStreamlinedSearchLineMapper
    {
        public StreamlinedSearchLine Map(SearchLine searchLine)
        {
            var streamlinedSearchLine = new StreamlinedSearchLine
            {
                Text = searchLine.Text,
                CaseId = searchLine.CaseId,
                DocumentId = searchLine.DocumentId,
                Id = searchLine.Id,
                LineIndex = searchLine.LineIndex,
                PageIndex = searchLine.PageIndex,
                PageHeight = searchLine.PageHeight,
                PageWidth = searchLine.PageWidth,
                Words = new List<StreamlinedWord>()
            };

            return streamlinedSearchLine;
        }
    }
}
