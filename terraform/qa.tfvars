env = "qa"
location = "UK South"
environment_tag="QA"
rumpole_pipeline_coordinator_function_app_key = "Lb7urp7BPantjTrXosHRtMCNBVLc9OoFcYuoUiI_E8I4AzFuBt8cdg=="
stub_blob_storage_connection_string="DefaultEndpointsProtocol=https;AccountName=saqacmsdocumentservices;AccountKey=nmbdArGrAOzr2nHk1srJkzt2lURPPnFEW5pUfx/oGFlT08Ec70RC6uzdNDXOJjM/rKq5X3g/1A70Zk92HR044Q==;EndpointSuffix=core.windows.net"

app_service_plan_sku = {
    size = "B1"
    tier = "Basic"
}

core_data_api_details = {
    api_url = "https://core-data.dev.cpsdigital.co.uk/graphql"
    api_scope = "api://5f1f433a-41b3-45d3-895e-927f50232a47/case.confirm"
}

coordinator_scope_details = {
    app_registration_application_id = "db476ea0-9bec-44af-8c1c-7c570587855d"
    user_impersonation_scope_id = "f07216d7-d6fe-42aa-8034-fe6506e03a49"
}