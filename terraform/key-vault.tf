#################### Key Vault ####################

resource "azurerm_key_vault" "kv_rumpole" {
  name                = "kv-${local.resource_name}"
  location            = azurerm_resource_group.rg_rumpole.location
  resource_group_name = azurerm_resource_group.rg_rumpole.name
  tenant_id           = data.azurerm_client_config.current.tenant_id

  sku_name = "standard"
}

resource "azurerm_key_vault_access_policy" "kvap_rumpole_fa" {
  key_vault_id = azurerm_key_vault.kv_rumpole.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_function_app.fa_rumpole.identity[0].principal_id

  secret_permissions = [
    "Get",
  ]
}

resource "azurerm_key_vault_access_policy" "kvap_terraform_sp" {
  key_vault_id = azurerm_key_vault.kv_rumpole.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = "4acc9fb2-3e32-4109-b3d1-5fcd3a253e4e"#data.azuread_service_principal.terraform_service_principal.object_id

  secret_permissions = [
    "Get",
    "Set"
  ]
}

