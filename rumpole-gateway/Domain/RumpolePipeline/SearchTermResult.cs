namespace RumpoleGateway.Domain.RumpolePipeline;

public class SearchTermResult
{
    public SearchTermResult(bool termFound, int weighting, StreamlinedMatchType matchType)
    {
        TermFound = termFound;
        Weighting = weighting;
        SearchMatchType = matchType;
    }
    
    public bool TermFound { get; }

    public int Weighting { get; }

    public StreamlinedMatchType SearchMatchType { get; }
}