using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Services.PdfService
{
    // taken from https://github.com/GrillPhil/ServerlessPDFConversionDemo
    public class PdfService
    {
        private readonly PdfOptions _pdfOptions;
        private readonly AuthenticationService _authenticationService;
        private HttpClient _httpClient = new HttpClient();

        private string _token;

        public PdfService(AuthenticationService authenticationService, IOptions<PdfOptions> pdfOptions)
        {
            _authenticationService = authenticationService;
            _pdfOptions = pdfOptions.Value;
        }

        // private async Task<HttpClient> CreateAuthorizedHttpClient()
        // {
        //     if (_httpClient != null)
        //     {
        //         return _httpClient;
        //     }

        //     var token = await _authenticationService.GetAccessTokenAsync();
        //     _httpClient = new HttpClient();
        //     _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        //     return _httpClient;
        // }

        public async Task<MemoryStream> GetPdfStream(Stream inputStream, string contentType)
        {
            var path = $"{_pdfOptions.GraphEndpoint}sites/{_pdfOptions.SiteId}/drive/items/";

            var fileId = await UploadStreamAsync(path, inputStream, contentType.ToString());
            var pdfStream = await DownloadConvertedFileAsync(path, fileId, "pdf");
            await DeleteFileAsync(path, fileId);

            return pdfStream;
        }

        private async Task<string> UploadStreamAsync(string path, Stream content, string contentType)
        {
            string tmpFileName = $"{Guid.NewGuid().ToString()}.{MimeTypes.MimeTypeMap.GetExtension(contentType)}";
            string requestUrl = $"{path}root:/{tmpFileName}:/content";
            var requestContent = new StreamContent(content);
            requestContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var response = await SendAsync(HttpMethod.Put, requestUrl, requestContent);
            if (response.IsSuccessStatusCode)
            {
                dynamic file = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                return file?.id;
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Upload file failed with status {response.StatusCode} and message {message}");
            }
        }

        private async Task<MemoryStream> DownloadConvertedFileAsync(string path, string fileId, string targetFormat)
        {

            var requestUrl = $"{path}{fileId}/content?format={targetFormat}";
            var response = await SendAsync(HttpMethod.Get, requestUrl);
            if (response.IsSuccessStatusCode)
            {
                await response.Content.LoadIntoBufferAsync();
                var fileContent = (MemoryStream)await response.Content.ReadAsStreamAsync();
                return fileContent;
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Download of converted file {path} failed with status {response.StatusCode} and message {message}");
            }
        }

        private async Task DeleteFileAsync(string path, string fileId)
        {
            var requestUrl = $"{path}{fileId}";
            var response = await SendAsync(HttpMethod.Delete, requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Delete file failed with status {response.StatusCode} and message {message}");
            }
        }

        private async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string requestUri, HttpContent content = null)
        {

            Func<string, Task<HttpResponseMessage>> sendAsync = (string token) =>
            {
                var request = new HttpRequestMessage(httpMethod, requestUri);
                request.Content = content;
                request.Headers.Add("Authorization", $"Bearer {token}");
                return _httpClient.SendAsync(request);
            };

            if (_token == null)
            {
                _token = await _authenticationService.GetAccessTokenAsync();
            }

            var response = await sendAsync(_token);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // token has expired
                _token = await _authenticationService.GetAccessTokenAsync();
                return await sendAsync(_token);
            }

            return response;

        }
    }
}
