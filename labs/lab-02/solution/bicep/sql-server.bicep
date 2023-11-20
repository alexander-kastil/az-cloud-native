param sqlServerName string
param sqlAdminLogin string = 'aznativeadmin'
param rgLocation string = resourceGroup().location

resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  properties: {
    administratorLogin: sqlAdminLogin
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'    
    restrictOutboundNetworkAccess: 'Disabled'
  }
  location: rgLocation
  tags: {}
  name: sqlServerName
}
