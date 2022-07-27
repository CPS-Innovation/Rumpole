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
                BoundingBox = searchTermLookup.Item1 ? word.BoundingBox : null,
                Weighting = searchTermLookup.Item2
            };

            return result;
        }

        public Tuple<bool, int> SearchTermIncluded(string wordText, string searchTerm)
        {
            const string pattern1 = @"[.,/#!$%^&*;:{}=\-_`~()…”]";
            
            var tidiedText = Regex.Replace(wordText, pattern1, "", RegexOptions.IgnoreCase).Replace(" ", "");
            var searchTermTidied = Regex.Replace(searchTerm, pattern1, "", RegexOptions.IgnoreCase).Replace(" ", "");

            var fuzzyWeighting = Fuzz.Ratio(tidiedText, searchTerm);
            return tidiedText.Equals(searchTermTidied, StringComparison.CurrentCultureIgnoreCase) 
                ? new Tuple<bool, int>(true, fuzzyWeighting) 
                : new Tuple<bool, int>(!string.IsNullOrEmpty(tidiedText) && fuzzyWeighting >= 90, fuzzyWeighting);
        }
    }
}
