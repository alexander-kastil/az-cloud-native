resource aznativesql 'Microsoft.Sql/servers@2023-05-01-preview' = {
  kind: 'v12.0'
  properties: {
    administratorLogin: 'aznativeadmin'
    version: '12.0'
    state: 'Ready'
    fullyQualifiedDomainName: 'aznativesql.database.windows.net'
    privateEndpointConnections: []
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    administrators: {
      administratorType: 'ActiveDirectory'
      principalType: 'User'
      login: 'alexander.pajer@integrations.at'
      sid: '25853297-1418-4fc4-96ec-22f8bc83a64b'
      tenantId: 'd92b247e-90e0-4469-a129-6a32866c0d0a'
      azureADOnlyAuthentication: false
    }
    restrictOutboundNetworkAccess: 'Disabled'
    externalGovernanceStatus: 'Disabled'
  }
  location: 'westeurope'
  tags: {}
  name: 'aznativesql'
}