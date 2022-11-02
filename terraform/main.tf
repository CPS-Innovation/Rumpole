terraform {
  backend "azurerm" {
    storage_account_name = "__terraform_storage_account__"
    container_name       = "__terraform_container_name__"
    key                  = "__terraform_key__"
    access_key           = "__storage_key__"
  }
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
}

provider "azurerm" {
  features {
    # key_vault {
    #   purge_soft_delete_on_destroy = true
    # }
  }
}

data "azurerm_client_config" "current" {}

data "azuread_service_principal" "terraform_service_principal" {
  application_id = "__terraform_service_principal_app_id__"
  // application_id = "ab6f55a4-543f-4f76-bf0a-13bdbd6c324b" // Dev 
  // application_id = "b92f19b6-be30-4292-9763-d4b3340a8a64" // uat
}

data "azurerm_subscription" "current" {}

locals {
  env_name_suffix = "${var.env != "prod" ? "-${var.env}" : ""}"
  resource_name = var.env != "prod" ? "${var.resource_name_prefix}-${var.env}" : var.resource_name_prefix
}
