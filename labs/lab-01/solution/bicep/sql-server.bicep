param sqlServerName string
param adminLogin string = 'aznativeadmin'
param rgLocation string = resourceGroup().location

resource aznativesql 'Microsoft.Sql/servers@2023-05-01-preview' = {
  properties: {
    administratorLogin: adminLogin
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    administrators: {
      administratorType: 'ActiveDirectory'
      principalType: 'User'
      azureADOnlyAuthentication: false
    }
    restrictOutboundNetworkAccess: 'Disabled'
  }
  location: rgLocation
  tags: {}
  name: sqlServerName
}
