using System;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Options;

namespace Services.OcrService
{
    public class OcrService
    {
        private readonly ComputerVisionClient _computerVisionClient;

        public OcrService(IOptions<OcrOptions> ocrOptions)
        {
            _computerVisionClient = Authenticate(ocrOptions.Value.ServiceUrl, ocrOptions.Value.ServiceKey);
        }

        private ComputerVisionClient Authenticate(string endpoint, string key)
        {
            var client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint, };
            return client;
        }

        public async Task<AnalyzeResults> GetOcrResults(string url)
        {
            var textHeaders = await _computerVisionClient.ReadAsync(url);

            string operationLocation = textHeaders.OperationLocation;
            await Task.Delay(500);

            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            ReadOperationResult results;

            while (true)
            {
                results = await _computerVisionClient.GetReadResultAsync(Guid.Parse(operationId));

                if (results.Status == OperationStatusCodes.Running ||
                    results.Status == OperationStatusCodes.NotStarted)
                {
                    await Task.Delay(500);
                }
                else
                {
                    break;
                }
            }


            return results.AnalyzeResult;
        }
    }
}