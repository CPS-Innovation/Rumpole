using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RumpoleGateway.Domain.CaseData;
using RumpoleGateway.Domain.CaseData.Args;
using RumpoleGateway.Factories;
using RumpoleGateway.CaseDataImplementations.Tde.Clients;
using RumpoleGateway.Services;
using RumpoleGateway.CaseDataImplementations.Tde.Mappers;

namespace RumpoleGateway.CaseDataImplementations.Tde.Services
{
    public class TdeCaseDataService : ICaseDataService
    {
        private readonly ITdeClient _tdeClient;
        private readonly ICaseDataArgFactory _caseDataServiceArgFactory;
        private readonly ICaseDetailsMapper _caseDetailsMapper;

        public TdeCaseDataService(ITdeClient tdeClient, ICaseDataArgFactory caseDataServiceArgFactory, ICaseDetailsMapper caseDetailsMapper)
        {
            _tdeClient = tdeClient;
            _caseDataServiceArgFactory = caseDataServiceArgFactory;
            _caseDetailsMapper = caseDetailsMapper;
        }

        public async Task<IEnumerable<CaseDetails>> ListCases(UrnArg arg)
        {
            var caseIdentifiers = await _tdeClient.ListCaseIdsAsync(arg);

            var calls = caseIdentifiers.Select(async caseIdentifier =>
                 await _tdeClient.GetCaseAsync(_caseDataServiceArgFactory.CreateCaseArgFromUrnArg(arg, caseIdentifier.Id)));

            var cases = await Task.WhenAll(calls);

            return cases.Select(@case => _caseDetailsMapper.MapCaseDetails(@case));
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