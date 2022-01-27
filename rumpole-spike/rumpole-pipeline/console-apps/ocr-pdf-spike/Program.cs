using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Services.OcrService;

namespace ocr_pdf_spike
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var ocrService = new OcrService(new OcrOptions
            {
                ServiceUrl = "https://cv-rumpole-spike.cognitiveservices.azure.com/",
                ServiceKey = "c71318040bf94551beb3d890e6533bef"
            });

            var results = await ocrService.GetOcrResults("https://sarumpolespike.blob.core.windows.net/rumpole/123/pdfs/101.pdf?sv=2020-08-04&se=2024-01-27T14%3A51%3A40Z&sr=b&sp=r&rsct=application%2Fpdf&sig=GhQ4CorvRjbp%2BdwmSpipwBWtepzEmxXTnB74IrpmG8A%3D");

            Console.Write(JsonConvert.SerializeObject(results));
        }
    }
}
