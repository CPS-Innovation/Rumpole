env = "dev"
location = "UK South"
environment_tag="Development"
rumpole_pipeline_coordinator_function_app_key="58mN4H3J0zfMJE0NyJBtb4AnzLfED/WtXYWvUvADeDhUcqFxmieLJQ=="
stub_blob_storage_connection_string="DefaultEndpointsProtocol=https;AccountName=sadevcmsdocumentservices;AccountKey=06beksVS54Cw5YqSLpvKrJStK8yYMsSui1cPO3MT4+pnHys6sCBFqBq17ix5ZGXuL5cHxnBIslXzZsL24ZRa7g==;EndpointSuffix=core.windows.net"
search_client_authorization_key="vROU0E80Q2KcFdtmtUAJTKxOUs4loMacLht2QXky6gAzSeDSXCgj"
rumpole_pipeline_redact_pdf_function_app_key="Q9dIC77OtbyN1YPB9NgbOKpp-5hO916dMkZaGriUcEFfAzFuUV_87g=="

app_service_plan_sku = {
    size = "B1"
    tier = "Basic"
}

rumpole_webapp_details = {
	client_id = "ecc7fb92-796a-4421-816f-d385b9e4bbfc"
	calling_scopes = "https://CPSGOVUK.onmicrosoft.com/fa-rumpole-dev-gateway"
}

core_data_api_details = {
    api_url = "https://core-data.dev.cpsdigital.co.uk/graphql"
    api_scope = "api://5f1f433a-41b3-45d3-895e-927f50232a47/case.confirm"
}

coordinator_scope_details = {
    app_registration_application_id = "b8f25b3d-d89c-4d2a-a010-31e426e5eb99"
    user_impersonation_scope_id = "42b54fc3-3b35-4109-95f5-e62d23f739d8"
}

redact_pdf_scope_details = {
    app_registration_application_id = "6cda2834-224f-4578-9b4f-2792102411c9"
    user_impersonation_scope_id = "a7aca134-aee0-424e-860e-d25ee1559fa3"
}