# Karls Repos

See https://dev.azure.com/CPSDTS/Innovation/_git/cognitive-search

| Repo                | Early Date  | Late Date   | Start Rank | End Rank | April burst | Use                                                 |
| ------------------- | ----------- | ----------- | ---------- | -------- | ----------- | --------------------------------------------------- |
| document-metadata   | 25 Apr 2021 | 26 Apr 2021 | 9          | 6        | 1           | Persists and indexes metadata (search service)      |
| document-pipelines  | 29 Apr 2021 | 5 May 2021  | 10         | 8        | 1           | (5506_Project_migration): orchestrator for all else |
| document-processing | 17 Nov 2020 | 21 Apr 2021 | 3          | 4        | 1           |
| document-retriever  | 4 Feb 2021  | 8 Mar 2021  | 5          | 3        | ?           | Get docs from CMS (the original spike)              |
| rumpole             | 27 Nov 2020 | 28 Apr 2021 | 4          | 7        | 1           | UI                                                  |
| terraform           | 22 Apr 2021 | 13 May 2021 | 6          | 10       | 1           | Terraform                                           |
| text-services       | 22 Apr 2021 | 22 Apr 2021 | 6          | 5        | 1           | NER (named entity recognition) sport addresses etc  |
| vision-services     | 22 Apr 2021 | 30 Apr 2021 | 6          | 9        | 1           | OCR                                                 |
|                     |             |             |            |          |             |
| storage-manager     | 31 Jul 2020 | 30 Sep 2020 | 1          | 1        |             |
| cognitive-search    | 7 Sep 2020  | 11 Jan 2021 | 2          | 2        |             |

# Original Docs for McLove

https://cpsgovuk.sharepoint.com/:f:/s/Innovation/EjwxrjT-uYRFo_wbdXHQZVQB7UGxOatDN3biVAWoZlwvmQ?e=Z4kPn7

# terraform

Four modules:

- rumpole
  - app service
- cps-document-metadata
  - cosmos
  - fa persist (document-metadata)
  - fa indexer (document-metadata)
  - search service
- document-pipelines
  - APIM stuff
  - Image processing; event triggering; storage (sacpsdocumentpipelines)

# PDF files https://medium.com/medialesson/convert-files-to-pdf-using-microsoft-graph-azure-functions-20bc84d2adc4

# Gettting Sharepoint site ID

Graph explorer

https://graph.microsoft.com/v1.0/sites/cpsgovuk.sharepoint.com/:/sites/Innovation/?$select=id

{
"@odata.context": "https://graph.microsoft.com/v1.0/$metadata#sites(id)/$entity",
"id": "cpsgovuk.sharepoint.com,fe402897-95c0-489b-8b34-e5d87354055b,99d6fc24-d53a-46a6-9839-dcc30ecd4996"
}

# App Registration

AD client id: 6cc88b25-ecea-41bc-9ca5-c4d993a2df10
tenant: 00dd0d1d-d7e6-4338-ac51-565339c7088c
master secret: ~447Q~grKT.gw3GudlSfYjsP5YoE4s.1.9xBy (id: e6537899-4047-4764-84ff-c1f9339e25ec)
