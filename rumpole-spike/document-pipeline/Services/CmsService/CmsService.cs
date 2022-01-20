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

    private readonly CmsOptions _cmsOptions;

    public CmsService(HttpClient httpClient, IOptions<CmsOptions> cmsOptions)
    {
      _httpClient = httpClient;
      _cmsOptions = cmsOptions.Value;
    }

    public async Task<List<CmsCaseDocumentDetails>> GetCaseDocumentDetails(int caseId)
    {
      var url = _cmsOptions.CmsDocumentDetailsUrl;
      var response = await _httpClient.GetAsync(url);
      var jsonString = await response.Content.ReadAsStringAsync();
      var results = JsonConvert.DeserializeObject<List<CmsCaseDocumentDetails>>(jsonString);
      results.ForEach(result => result.CaseId = caseId);

      return results;
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