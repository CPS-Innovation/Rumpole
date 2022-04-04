#################### Functions ####################

resource "azurerm_function_app" "fa_rumpole" {
  name                       = "fa-${local.resource_name}-gateway"
  location                   = azurerm_resource_group.rg_rumpole.location
  resource_group_name        = azurerm_resource_group.rg_rumpole.name
  app_service_plan_id        = azurerm_app_service_plan.asp_rumpole.id
  storage_account_name       = azurerm_storage_account.sacpsrumpole.name
  storage_account_access_key = azurerm_storage_account.sacpsrumpole.primary_access_key
  os_type                    = "linux"
  version                    = "~4"
  app_settings = {
    "AzureWebJobsStorage"                 = azurerm_storage_account.sacpsrumpole.primary_connection_string
    "FUNCTIONS_WORKER_RUNTIME"            = "dotnet"
    "FUNCTIONS_EXTENSION_VERSION"         = "~4"
    "StorageConnectionAppSetting"         = azurerm_storage_account.sacpsrumpole.primary_connection_string
    "APPINSIGHTS_INSTRUMENTATIONKEY"      = azurerm_application_insights.ai_rumpole.instrumentation_key
    "OnBehalfOfTokenTenantId"             = data.azurerm_client_config.current.tenant_id
    "OnBehalfOfTokenClientId"             = azuread_application.fa_rumpole.application_id
    "OnBehalfOfTokenClientSecret"         = azuread_application_password.faap_rumpole_app_service.value
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = ""
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE"     = ""
    "CoreDataApiUrl"                      = var.core_data_api_details.api_url
    "CoreDataApiScope"                    = var.core_data_api_details.api_scope
  }
  site_config {
    always_on        = true
    ip_restriction   = []
    linux_fx_version = "DOTNET|6.0"
    cors {
      allowed_origins     = ["https://as-web-${local.resource_name}.azurewebsites.net", var.env == "dev" ? "http://localhost:3000" : ""]
      support_credentials = true
    }
  }

  tags = {
    environment = var.environment_tag
  }

  identity {
    type = "SystemAssigned"
  }

  auth_settings {
    enabled                       = true
    issuer                        = "https://sts.windows.net/${data.azurerm_client_config.current.tenant_id}/"
    unauthenticated_client_action = "RedirectToLoginPage"
    default_provider              = "AzureActiveDirectory"
    active_directory {
      client_id         = azuread_application.fa_rumpole.application_id
      client_secret     = azuread_application_password.faap_rumpole_app_service.value
      allowed_audiences = ["https://CPSGOVUK.onmicrosoft.com/fa-${local.resource_name}-gateway"]
    }
  }

  lifecycle {
    ignore_changes = [
      app_settings["WEBSITES_ENABLE_APP_SERVICE_STORAGE"],
      app_settings["WEBSITE_ENABLE_SYNC_UPDATE_SITE"],
    ]
  }
}

data "azuread_client_config" "current" {}

resource "random_uuid" "user_impersonation_scope_id" {}

resource "azuread_application" "fa_rumpole" {
  display_name    = "fa-${local.resource_name}-gateway"
  identifier_uris = ["https://CPSGOVUK.onmicrosoft.com/fa-${local.resource_name}-gateway"]
  owners          = [data.azuread_client_config.current.object_id]

  api {
      oauth2_permission_scope {
        admin_consent_description  = "Allow an application to access function app on behalf of the signed-in user."
        admin_consent_display_name = "Access function app"
        enabled                    = true
        id                         = random_uuid.user_impersonation_scope_id.result
        type                       = "Admin"
        value                      = "user_impersonation"
    }
  }

  required_resource_access {
    resource_app_id = "00000003-0000-0000-c000-000000000000" # Microsoft Graph

    resource_access {
      id   = "e1fe6dd8-ba31-4d61-89e7-88639da4683d" # read user
      type = "Scope"
    }


    resource_access {
      id   = "5f8c59db-677d-491f-a6b8-5f174b11ec1d" # read all groups (requires admin consent?)
      type = "Scope"
    }
  }

  web {
    redirect_uris = ["https://fa-${local.resource_name}-gateway.azurewebsites.net/.auth/login/aad/callback"]

    implicit_grant {
      access_token_issuance_enabled = false
      // id_token_issuance_enabled     = false
    }
  }

}

resource "azuread_application_password" "faap_rumpole_app_service" {
  application_object_id = azuread_application.fa_rumpole.id
  end_date_relative     = "17520h"
  depends_on = [
    azuread_application.fa_rumpole
  ]
}
