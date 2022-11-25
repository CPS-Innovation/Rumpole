namespace RumpoleGateway.Domain.RumpolePipeline
{
	public enum TrackerStatus
	{
		Initialised,
		NotStarted,
		Running,
		// ReSharper disable once InconsistentNaming
		NoDocumentsFoundInCDE,
		Completed,
		Failed,
		UnableToEvaluateExistingDocuments
	}
}

