using System.Collections.Generic;
using System.Threading.Tasks;
using RumpoleGateway.Domain.CaseData;
using RumpoleGateway.Domain.CaseData.Args;

namespace RumpoleGateway.Services
{
    public interface IDocumentService
    {
        Task CheckoutDocument(DocumentArg arg);

        Task CancelCheckoutDocument(DocumentArg arg);
    }
}