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