#################### Resource Group ####################

resource "azurerm_resource_group" "rg_rumpole" {
  name     = "rg-${local.resource_name}"
  location = "UK South"
}
