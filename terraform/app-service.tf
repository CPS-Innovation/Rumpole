#################### App Service ####################

resource "azurerm_app_service" "as_web_rumpole" {
  name                = "as-web-${local.resource_name}"
  location            = azurerm_resource_group.rg_rumpole.location
  resource_group_name = azurerm_resource_group.rg_rumpole.name
  app_service_plan_id = azurerm_app_service_plan.asp_rumpole.id
  https_only          = true

  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.ai_rumpole.instrumentation_key
  }

  site_config {
    app_command_line = "npx serve -s"
    linux_fx_version = "NODE|14-lts"
  }

  auth_settings {
    enabled                       = true
    issuer                        = "https://sts.windows.net/${data.azurerm_client_config.current.tenant_id}/"
    unauthenticated_client_action = "RedirectToLoginPage"
    default_provider              = "AzureActiveDirectory"
    token_store_enabled = true
    active_directory {
      client_id         = azuread_application.as_web_rumpole.application_id
      client_secret     = azuread_application_password.asap_web_rumpole_app_service.value
      allowed_audiences = ["https://as-web-${local.resource_name}.azurewebsites.net"]
    }
  }
}

resource "azuread_application" "as_web_rumpole" {
  display_name               = "as-web-${local.resource_name}"
  oauth2_allow_implicit_flow = false
  identifier_uris            = ["https://CPSGOVUK.onmicrosoft.com/as-web-${local.resource_name}"]
  owners                     = ["4acc9fb2-3e32-4109-b3d1-5fcd3a253e4e"]
  reply_urls = [
    "https://as-web-${local.resource_name}.azurewebsites.net/.auth/login/aad/callback",
  ]
  homepage = "https://as-web-${local.resource_name}.azurewebsites.net"

  required_resource_access {
    resource_app_id = "00000002-0000-0000-c000-000000000000"

    resource_access {
      id   = "311a71cc-e848-46a1-bdf8-97ff7156d8e6"
      type = "Scope"
    }
  }

  required_resource_access {
    resource_app_id = "00000003-0000-0000-c000-000000000000"

    resource_access {
      id   = "5f8c59db-677d-491f-a6b8-5f174b11ec1d"
      type = "Scope"
    }
  }

  required_resource_access {
    resource_app_id = azuread_application.fa_rumpole.application_id

    resource_access {
      id   = tolist(azuread_application.fa_rumpole.oauth2_permissions)[0].id
      type = "Scope"
    }
  }

  depends_on = [
    azuread_application.fa_rumpole
  ]
}

resource "azuread_application_password" "asap_web_rumpole_app_service" {
  application_object_id = azuread_application.as_web_rumpole.id
  description           = "Default app service app password"
  end_date_relative     = "17520h"
  value                 = "__asap_web_rumpole_app_service_password__"
}
