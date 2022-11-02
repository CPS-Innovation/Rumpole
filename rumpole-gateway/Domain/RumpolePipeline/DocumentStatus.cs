namespace RumpoleGateway.Domain.RumpolePipeline
{
	public enum DocumentStatus
	{
		None,
		PdfUploadedToBlob,
		Indexed,
		NotFoundInCDE,
		UnableToConvertToPdf,
		UnexpectedFailure,
		OcrAndIndexFailure,
		DocumentEvaluated,
		DocumentRemovedFromSearchIndex,
		UnexpectedSearchIndexRemovalFailure,
		SearchIndexUpdateFailure,
		UnableToEvaluateDocument
	}
}

