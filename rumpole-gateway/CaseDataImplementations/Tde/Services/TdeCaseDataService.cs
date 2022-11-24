using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RumpoleGateway.Domain.CaseData;
using RumpoleGateway.Domain.CaseData.Args;
using RumpoleGateway.Factories;
using RumpoleGateway.CaseDataImplementations.Tde.Clients;
using RumpoleGateway.Services;

namespace RumpoleGateway.CaseDataImplementations.Tde.Services
{
    public class TdeCaseDataService : ICaseDataService
    {
        private readonly ITdeClient _tdeClient;
        private readonly ICaseDataArgFactory _caseDataServiceArgFactory;

        public TdeCaseDataService(ITdeClient tdeClient, ICaseDataArgFactory caseDataServiceArgFactory)
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