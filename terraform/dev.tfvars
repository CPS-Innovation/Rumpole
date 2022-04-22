env = "dev"
location = "UK South"
environment_tag="Development"
rumpole_pipeline_coordinator_function_app_key="58mN4H3J0zfMJE0NyJBtb4AnzLfED/WtXYWvUvADeDhUcqFxmieLJQ=="

app_service_plan_sku = {
    size = "B1"
    tier = "Basic"
}

core_data_api_details = {
    api_url = "https://core-data.dev.cpsdigital.co.uk/graphql"
    api_scope = "api://5f1f433a-41b3-45d3-895e-927f50232a47/case.confirm"
}

coordinator_scope_details = {
    app_registration_application_id = "b8f25b3d-d89c-4d2a-a010-31e426e5eb99"
    user_impersonation_scope_id = "3090db55-29c9-222c-027f-3408db21d9e5"
}
