using System.Net.Http;
using System;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Collections.Generic;
using Domain;
using Microsoft.Extensions.Options;

namespace Services.CmsService
{
    public class CmsService
    {
        private readonly HttpClient _httpClient;

        public CmsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CmsDocument> GetDocument(string url)
        {
            var response = await _httpClient.GetAsync(url);
            var contentType = response.Content.Headers.ContentType;

            return new CmsDocument
            {
                ContentType = contentType,
                Stream = await response.Content.ReadAsStreamAsync()
            };

        }
    }
}