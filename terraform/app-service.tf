#################### App Service ####################

resource "azurerm_app_service" "as_web_rumpole" {
  name                = "as-web-${local.resource_name}"
  location            = azurerm_resource_group.rg_rumpole.location
  resource_group_name = azurerm_resource_group.rg_rumpole.name
  app_service_plan_id = azurerm_service_plan.asp_rumpole.id
  https_only          = true

  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY"  = azurerm_application_insights.ai_rumpole.instrumentation_key
    "REACT_APP_CLIENT_ID"             = azuread_application.as_web_rumpole.application_id
    "REACT_APP_TENANT_ID"             = data.azurerm_client_config.current.tenant_id
    "REACT_APP_GATEWAY_BASE_URL"      = "https://${azurerm_linux_function_app.fa_rumpole.name}.azurewebsites.net"
    "REACT_APP_GATEWAY_SCOPE"         = "https://CPSGOVUK.onmicrosoft.com/${azurerm_linux_function_app.fa_rumpole.name}/user_impersonation"
  }

  site_config {
    app_command_line = "node subsititute-config.js; npx serve -s"
    linux_fx_version = "NODE|14-lts"
  }

  tags = {
    environment = var.environment_tag
  }

  /*auth_settings {
    enabled                       = true
    issuer                        = "https://sts.windows.net/${data.azurerm_client_config.current.tenant_id}/"
    
    # AllowAnonymous as no need for web auth if we are hosted within CPS network, the SPA auth handles hiding UI 
    # from unauthed users. Also having web auth switched on means that Cypress automation tests don't work.
    unauthenticated_client_action = "AllowAnonymous"
    token_store_enabled           = true
    active_directory {
      client_id         = azuread_application.as_web_rumpole.application_id
      client_secret     = azuread_application_password.asap_web_rumpole_app_service.value
      allowed_audiences = ["https://CPSGOVUK.onmicrosoft.com/as-web-${local.resource_name}"]
    }
  }*/
}

resource "azuread_application" "as_web_rumpole" {
  display_name    = "as-web-${local.resource_name}"
  identifier_uris = ["https://CPSGOVUK.onmicrosoft.com/as-web-${local.resource_name}"]
  owners          = [data.azuread_service_principal.terraform_service_principal.object_id]

  single_page_application {
    redirect_uris = var.env == "dev" ? ["https://as-web-${local.resource_name}.azurewebsites.net/", "http://localhost:3000/"] : ["https://as-web-${local.resource_name}.azurewebsites.net/"]
  }
  web {
    homepage_url  = "https://as-web-${local.resource_name}.azurewebsites.net"
    implicit_grant {
      access_token_issuance_enabled = true
      //id_token_issuance_enabled     = true
    }
    redirect_uris = var.env == "dev" ? ["https://getpostman.com/oauth2/callback"] : [""]
  }

  required_resource_access {
    resource_app_id = "00000002-0000-0000-c000-000000000000" # Azure AD Graph 

    resource_access {
      id   = "311a71cc-e848-46a1-bdf8-97ff7156d8e6" # read user
      type = "Scope"
    }
  }

  required_resource_access {
    resource_app_id = module.azurerm_app_reg_fa_rumpole.client_id

    resource_access {
      id   = module.azurerm_app_reg_fa_rumpole.oauth2_permission_scope_ids["user_impersonation"]
      type = "Scope"
    }
  }
}

resource "azuread_application_password" "asap_web_rumpole_app_service" {
  application_object_id = azuread_application.as_web_rumpole.id
  end_date_relative     = "17520h"
}

module "azurerm_service_principal_sp_rumpole_web" {
  source         = "./modules/terraform-azurerm-azuread_service_principal"
  application_id = azuread_application.as_web_rumpole.application_id
  app_role_assignment_required = false
  owners         = [data.azurerm_client_config.current.object_id]
}

resource "azuread_service_principal_password" "sp_rumpole_web_pw" {
  service_principal_id = module.azurerm_service_principal_sp_rumpole_web.object_id
}

resource "azuread_service_principal_delegated_permission_grant" "rumpole_web_grant_access_to_rumpole_gateway" {
  service_principal_object_id          = module.azurerm_service_principal_sp_rumpole_web.object_id
  resource_service_principal_object_id = module.azurerm_service_principal_sp_rumpole_gateway.object_id
  claim_values                         = ["user_impersonation"]
}
 
