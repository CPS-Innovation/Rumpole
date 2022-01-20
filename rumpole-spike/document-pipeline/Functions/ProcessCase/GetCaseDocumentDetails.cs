using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Services.CmsService;

namespace Functions
{
  public class GetCaseDocumentDetails
  {
    private readonly CmsService _cmsService;
    public GetCaseDocumentDetails(CmsService cmsService)
    {
      _cmsService = cmsService;
    }

    [FunctionName("GetCaseDocumentDetails")]
    public async Task<List<CmsCaseDocumentDetails>> Run([ActivityTrigger] int caseId, ILogger log)
    {
      return await _cmsService.GetCaseDocumentDetails(caseId);
    }
  }
}