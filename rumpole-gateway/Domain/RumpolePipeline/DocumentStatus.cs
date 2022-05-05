namespace RumpoleGateway.Domain.RumpolePipeline
{
	public enum DocumentStatus
	{
		None,
		PdfUploadedToBlob,
		NotFoundInCDE,
		FailedToConvertToPdf
	}
}

