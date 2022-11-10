data "azurerm_linux_function_app" "fa_pipeline_coordinator" {
  name                = "fa-${local.pipeline_resource_name}-coordinator"
  resource_group_name = "rg=${local.pipeline_resource_name}"
}

data "azurerm_windows_function_app" "fa_pipeline_pdf_generator" {
  name                = "fa-${local.pipeline_resource_name}-pdf-generator"
  resource_group_name = "rg=${local.pipeline_resource_name}"
}

data "azurerm_linux_function_app" "fa_pipeline_text_extractor" {
  name                = "fa-${local.pipeline_resource_name}-text-extractor"
  resource_group_name = "rg=${local.pipeline_resource_name}"
}

data "azurerm_function_app_host_keys" "fa_pipeline_coordinator_host_keys" {
  name                = "fa-${local.pipeline_resource_name}-coordinator"
  resource_group_name = "rg=${local.pipeline_resource_name}"
}

data "azurerm_function_app_host_keys" "fa_pipeline_pdf_generator_host_keys" {
  name                = "fa-${local.pipeline_resource_name}-pdf-generator"
  resource_group_name = "rg=${local.pipeline_resource_name}"
}

data "azurerm_function_app_host_keys" "fa_pipeline_text_extractor_host_keys" {
  name                = "fa-${local.pipeline_resource_name}-text-extractor"
  resource_group_name = "rg=${local.pipeline_resource_name}"
}

data "azurerm_search_service" "pipeline_ss" {
  name                = "ss-${local.pipeline_resource_name}"
  resource_group_name = "rg=${local.pipeline_resource_name}"
}