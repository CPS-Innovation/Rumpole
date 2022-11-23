﻿using System;
using RumpoleGateway.Domain.CoreDataApi.CaseDetails;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RumpoleGateway.Clients.CoreDataApi
{
    public interface ICoreDataApiClient
    {
        Task<CaseDetails> GetCaseDetailsByIdAsync(string caseId, string accessToken, Guid correlationId);

        Task<IList<CaseDetails>> GetCaseInformationByUrnAsync(string urn, string accessToken, Guid correlationId);
    }
}
