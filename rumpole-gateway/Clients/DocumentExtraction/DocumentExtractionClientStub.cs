using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using RumpoleGateway.Domain.DocumentExtraction;

namespace RumpoleGateway.Clients.DocumentExtraction
{
    [ExcludeFromCodeCoverage]
    public class DocumentExtractionClientStub : IDocumentExtractionClient
    {
        private readonly string _blobStorageConnectionString;

        public DocumentExtractionClientStub(string blobStorageConnectionString)
        {
            _blobStorageConnectionString = blobStorageConnectionString;
        }

        public Task<Case> GetCaseDocumentsAsync(string caseId, string accessToken)
        {
            return Task.FromResult(caseId switch
            {
                "18846" => McLoveCase(caseId),
                "1000000" => McLoveCase(caseId),
                "1000001" => MultipleFileTypeCase(caseId),
                _ => null
            });
        }

        public async Task<Stream> GetDocumentAsync(string documentId, string fileName, string accessToken)
        {
            var blobClient = new BlobClient(_blobStorageConnectionString, "cms-documents", fileName);

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var blob = await blobClient.DownloadContentAsync();

            return blob.Value.Content.ToStream();
        }

        private static Case McLoveCase(string caseId)
        {
            return new Case
            {
                CaseId = caseId,
                CaseDocuments = new[]
                {
                    new CaseDocument
                    {
                        DocumentId = "MG12",
                        FileName = "MG12.doc",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG12",
                            Name = "MG12 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "stmt Shelagh McLove MG11",
                        FileName = "stmt Shelagh McLove MG11.docx",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG11",
                            Name = "MG11 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "MG00",
                        FileName = "MG00.doc",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG00",
                            Name = "MG00 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "stmt JONES 1989 1 JUNE mg11",
                        FileName = "stmt JONES 1989 1 JUNE mg11.docx",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG11",
                            Name = "MG11 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "MG20 10 JUNE",
                        FileName = "MG20 10 JUNE.doc",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG20",
                            Name = "MG20 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "UNUSED 1 - STORM LOG 1881 01.6.20 - EDITED 2020-11-23 MCLOVE",
                        FileName = "UNUSED 1 - STORM LOG 1881 01.6.20 - EDITED 2020-11-23 MCLOVE.docx",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG11",
                            Name = "MG11 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "Shelagh McLove VPS mg11",
                        FileName = "Shelagh McLove VPS mg11.docx",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG11",
                            Name = "MG11 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "UNUSED 6 - DA CHECKLIST MCLOVE",
                        FileName = "UNUSED 6 - DA CHECKLIST MCLOVE.docx",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG6",
                            Name = "MG6 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "MG0",
                        FileName = "MG0.docx",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "MG06 3 June",
                        FileName = "MG06 3 June.doc",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG06",
                            Name = "MG06 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "SDC items to be Disclosed (1-6) MCLOVE",
                        FileName = "SDC items to be Disclosed (1-6) MCLOVE.doc",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG11",
                            Name = "MG11 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "stmt BLAYNEE 2034 1 JUNE mg11",
                        FileName = "stmt BLAYNEE 2034 1 JUNE mg11.docx",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG11",
                            Name = "MG11 File"
                        }
                    },
                    new CaseDocument
                    {
                       DocumentId = "PRE CONS D",
                        FileName = "PRE CONS D.docx",
                        CreatedDate = DateTime.Parse("09-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG00",
                            Name = "MG00 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "MG05 MCLOVE",
                        FileName = "MG05 MCLOVE.doc",
                        CreatedDate = DateTime.Parse("11-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG05",
                            Name = "MG05 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "MG20 5 JUNE",
                        FileName = "MG20 5 JUNE.doc",
                        CreatedDate = DateTime.Parse("11-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG20",
                            Name = "MG20 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "MG02 SHELAGH MCLOVE",
                        FileName = "MG02 SHELAGH MCLOVE.doc",
                        CreatedDate = DateTime.Parse("11-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG02",
                            Name = "MG02 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "MG06 10 june",
                        FileName = "MG06 10 june.doc",
                        CreatedDate = DateTime.Parse("13-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG06",
                            Name = "MG06 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "stmt Lucy Doyle MG11",
                        FileName = "stmt Lucy Doyle MG11.docx",
                        CreatedDate = DateTime.Parse("13-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG11",
                            Name = "MG11 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "MCLOVE MG3",
                        FileName = "MCLOVE MG3.docx",
                        CreatedDate = DateTime.Parse("13-feb-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG3",
                            Name = "MG3 File"
                        }
                    }
                }
            };
        }

        private static Case MultipleFileTypeCase(string caseId)
        {
            return new Case
            {
                CaseId = caseId,
                CaseDocuments = new[]
                {
                    new CaseDocument
                    {
                        DocumentId = "docCDE",
                        FileName = "docCDE.doc",
                        CreatedDate = DateTime.Parse("09-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "docxCDE",
                        FileName = "docxCDE.docx",
                        CreatedDate = DateTime.Parse("09-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "docmCDE",
                        FileName = "docmCDE.docm",
                        CreatedDate = DateTime.Parse("09-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "xlsxCDE",
                        FileName = "xlsxCDE.xlsx",
                        CreatedDate = DateTime.Parse("09-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "xlsCDE",
                        FileName = "xlsCDE.xls",
                        CreatedDate = DateTime.Parse("09-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "pptCDE",
                        FileName = "pptCDE.ppt",
                        CreatedDate = DateTime.Parse("09-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "pptxCDE",
                        FileName = "pptxCDE.pptx",
                        CreatedDate = DateTime.Parse("09-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "htmlCDE",
                        FileName = "htmlCDE.html",
                        CreatedDate = DateTime.Parse("09-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "msgCDE",
                        FileName = "msgCDE.msg",
                        CreatedDate = DateTime.Parse("09-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "vsdCDE",
                        FileName = "vsdCDE.vsd",
                        CreatedDate = DateTime.Parse("09-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "bmpCDE",
                        FileName = "bmpCDE.bmp",
                        CreatedDate = DateTime.Parse("09-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "gifCDE",
                        FileName = "gifCDE.gif",
                        CreatedDate = DateTime.Parse("11-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "jpgCDE",
                        FileName = "jpgCDE.jpg",
                        CreatedDate = DateTime.Parse("11-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "pngCDE",
                        FileName = "pngCDE.png",
                        CreatedDate = DateTime.Parse("11-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "tiffCDE",
                        FileName = "tiffCDE.tiff",
                        CreatedDate = DateTime.Parse("13-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "rtfCDE",
                        FileName = "rtfCDE.rtf",
                        CreatedDate = DateTime.Parse("13-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    },
                    new CaseDocument
                    {
                        DocumentId = "txtCDE",
                        FileName = "txtCDE.txt",
                        CreatedDate = DateTime.Parse("13-jun-2022"),
                        CmsDocType = new CmsDocType
                        {
                            Code = "MG0",
                            Name = "MG0 File"
                        }
                    }
                }
            };
        }
    }
}