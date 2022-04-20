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

variable "rumpole_pipeline_coordinator_function_app_key" {
  type        = string
}