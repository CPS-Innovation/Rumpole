using System;
using System.Text.RegularExpressions;
using FuzzySharp;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using RumpoleGateway.Domain.RumpolePipeline;

namespace RumpoleGateway.Mappers
{
    public class StreamlinedSearchWordMapper : IStreamlinedSearchWordMapper
    {
        public StreamlinedWord Map(Word word, string searchTerm)
        {
            var searchTermLookup = SearchTermIncluded(word.Text, searchTerm);
            var result = new StreamlinedWord
            {
                Text = word.Text,
                BoundingBox = searchTermLookup.TermFound ? word.BoundingBox : null,
                StreamlinedMatchType = searchTermLookup.SearchMatchType
            };

            return result;
        }

        public SearchTermResult SearchTermIncluded(string wordText, string searchTerm)
        {
            var tidiedText = wordText.Replace(" ", "");
            if (searchTerm.Equals(tidiedText, StringComparison.CurrentCultureIgnoreCase))
                return new SearchTermResult(true, StreamlinedMatchType.Exact);

            var partialWeighting = Fuzz.PartialRatio(tidiedText, searchTerm);
            if (partialWeighting >= 95)
            {
                return Regex.IsMatch(wordText, @"\b" + searchTerm + @"\b", RegexOptions.IgnoreCase) 
                    ? new SearchTermResult(true, StreamlinedMatchType.Exact) 
                    : new SearchTermResult(true, StreamlinedMatchType.Fuzzy);
            }

            return new SearchTermResult(false, StreamlinedMatchType.None);
        }
    }
}
