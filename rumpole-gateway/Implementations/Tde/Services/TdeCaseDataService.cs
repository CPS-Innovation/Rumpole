using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RumpoleGateway.Domain.CaseDataService;
using RumpoleGateway.Domain.CoreDataApi.CaseDetails;
using RumpoleGateway.Factories;
using RumpoleGateway.Implementations.Tde.Clients;
using RumpoleGateway.Services;

namespace RumpoleGateway.Implementations.Tde.Services
{
    public class TdeCaseDataService : ICaseDataService
    {
        private readonly ITdeClient _tdeClient;
        private readonly ICaseDataServiceArgFactory _caseDataServiceArgFactory;

        public TdeCaseDataService(ITdeClient tdeClient, ICaseDataServiceArgFactory caseDataServiceArgFactory)
        {
            _tdeClient = tdeClient;
            _caseDataServiceArgFactory = caseDataServiceArgFactory;
        }

        public async Task<IEnumerable<CaseDetails>> ListCases(UrnArg arg)
        {
            var caseIdentifiers = await _tdeClient.ListCaseIdsAsync(arg);
            throw new NotImplementedException();
            //         var calls = caseIdentifiers.Select(async caseIdentifier =>
            // {
            //     var @case = await _tdeClient.GetCaseAsync(_caseDataServiceArgFactory.CreateCaseArgFromUrnArg(arg, caseIdentifier.Id));

            //     return new UrnCase
            //     {
            //         Summary = summary,
            //         Defendants = defendants,
            //         PreChargeDecisionRequests = preChargeDecisionRequests
            //     };
            // });

            //         var cases = await Task.WhenAll(calls);

            //         return cases.Where(item => !isSplitCase || item.Summary.Urn.Split("/")[1] == urnSplitId);
        }

        public Task<CaseDetailsFull> GetCase(CaseArg arg)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<DocumentDetails>> ListDocuments(CaseArg arg)
        {
            throw new System.NotImplementedException();
        }

    }
}