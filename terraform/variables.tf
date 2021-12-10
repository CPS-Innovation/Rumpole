#################### Variables ####################

variable "resource_name_prefix" {
  type = string
  default = "rumpole"
}

variable "env" {
  type = string 
}

variable "app_service_plan_sku" {
  type = object({
    tier = string
    size = string
  })
}

variable "core_data_api_url" {
  type = string
}