env = "dev"
location = "UK South"
environment_tag="Development"
stub_blob_storage_connection_string="DefaultEndpointsProtocol=https;AccountName=sadevcmsdocumentservices;AccountKey=06beksVS54Cw5YqSLpvKrJStK8yYMsSui1cPO3MT4+pnHys6sCBFqBq17ix5ZGXuL5cHxnBIslXzZsL24ZRa7g==;EndpointSuffix=core.windows.net"

app_service_plan_sku = {
    size = "B1"
    tier = "Basic"
}

rumpole_webapp_details = {
    valid_audience = "https://CPSGOVUK.onmicrosoft.com/fa-rumpole-dev-gateway"
	valid_scopes = "user_impersonation"
	valid_roles = ""
}

core_data_api_details = {
    api_url = "https://core-data.dev.cpsdigital.co.uk/graphql"
    api_scope = "api://5f1f433a-41b3-45d3-895e-927f50232a47/case.confirm"
}