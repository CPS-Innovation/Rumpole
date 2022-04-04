#################### Key Vault ####################

resource "azurerm_key_vault" "kv_rumpole" {
  name                = "kv-${local.resource_name}"
  location            = azurerm_resource_group.rg_rumpole.location
  resource_group_name = azurerm_resource_group.rg_rumpole.name
  tenant_id           = data.azurerm_client_config.current.tenant_id

  sku_name = "standard"
}