﻿using System.Collections.Generic;
using RumpoleGateway.Domain.CoreDataApi.CaseDetails;
using RumpoleGateway.Domain.CoreDataApi.ResponseTypes;

namespace RumpoleGateway.Tests.FakeData
{
    public class ResponseCaseDetailsFake
    {
        public ResponseCaseDetails GetCaseDetailsResponse_Payload()
        {
            var responseCaseDetails = new ResponseCaseDetails
            {

                CaseDetails = new CaseDetails { Id = 18868,
                    UniqueReferenceNumber = "10OF1234520",
                    CaseType = "O",
                    AppealType = "",
                    CaseStatus = new CaseStatus{Code ="LV",
                                                Description = "Live Case" }  ,
                    LeadDefendant =   new LeadDefendant {FirstNames = "Connor", 
                                                         Surname="Rich", 
                                                         OrganisationName =""},
                    Offences = new List<Offence> { new Offence {EarlyDate= "2021-12-08", 
                                                                LateDate = "2021-12-10", 
                                                                ListOrder = 1,
                                                                Code ="MDR",
                                                                ShortDescription="Short description for URN 10OF1234520 offence 1",
                                                                LongDescription="This is a very long description for URN 10OF1234520 offence 1"} } } };


            return responseCaseDetails;
        }
    }
}