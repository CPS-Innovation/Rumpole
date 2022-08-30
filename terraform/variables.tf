#################### Variables ####################

variable "resource_name_prefix" {
  type = string
  default = "rumpole"
}

variable "env" {
  type = string 
}

variable "location" {
  description = "The location of this resource"
  type        = string
}

variable "app_service_plan_sku" {
  type = object({
    tier = string
    size = string
  })
}

variable "core_data_api_details" {
  type = object({
    api_url = string
    api_scope = string
  })
}

variable "environment_tag" {
  type        = string
  description = "Environment tag value"
}

# TODO get rid of this as it will change every time coordinator is rebuilt
variable "rumpole_pipeline_coordinator_function_app_key" {
  type = string
}

# TODO get rid of this as it will change every time pdf generator is rebuilt
variable "rumpole_pipeline_redact_pdf_function_app_key" {
  type = string
}

# TODO get rid of this as it will change every time coordinator is rebuilt
variable "coordinator_scope_details" {
  type = object({
    app_registration_application_id = string
    user_impersonation_scope_id = string
  })
}

# TODO get rid of this as it will change every time coordinator is rebuilt
variable "redact_pdf_scope_details" {
  type = object({
    app_registration_application_id = string
    user_impersonation_scope_id = string
  })
}

variable "rumpole_webapp_details" {
  type = object({
    client_id = string
    calling_scopes = string
  })
}

variable "stub_blob_storage_connection_string" {
  type = string
}

# TODO get rid of this as it will change every time search index service is rebuilt
variable "search_client_authorization_key" {
  type = string
}