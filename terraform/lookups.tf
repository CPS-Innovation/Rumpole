data "azurerm_client_config" "current" {}

data "azuread_service_principal" "terraform_service_principal" {
  application_id = "__terraform_service_principal_app_id__"
  // application_id = "ab6f55a4-543f-4f76-bf0a-13bdbd6c324b" // Dev 
  // application_id = "b92f19b6-be30-4292-9763-d4b3340a8a64" // uat
}

data "azurerm_subscription" "current" {}

data "azuread_application_published_app_ids" "well_known" {}

data "azuread_application" "fa_pipeline_coordinator" {
  display_name        = "fa-${local.pipeline_resource_name}-coordinator"
}

data "azuread_application" "fa_pipeline_pdf_generator" {
  display_name        = "fa-${local.pipeline_resource_name}-pdf-generator"
}

data "azurerm_function_app_host_keys" "fa_pipeline_coordinator_host_keys" {
  name                = "fa-${local.pipeline_resource_name}-coordinator"
  resource_group_name = "rg-${local.pipeline_resource_name}"
}

data "azurerm_function_app_host_keys" "fa_pipeline_pdf_generator_host_keys" {
  name                = "fa-${local.pipeline_resource_name}-pdf-generator"
  resource_group_name = "rg-${local.pipeline_resource_name}"
}

data "azurerm_search_service" "pipeline_ss" {
  name                = "ss-${local.pipeline_resource_name}"
  resource_group_name = "rg-${local.pipeline_resource_name}"
}

data "azuread_service_principal" "fa_pipeline_coordinator_service_principal" {
  application_id = data.azuread_application.fa_pipeline_coordinator.application_id
}

data "azuread_service_principal" "fa_pdf_generator_service_principal" {
  application_id = data.azuread_application.fa_pipeline_pdf_generator.application_id
}