resource catalog_service 'Microsoft.App/containerApps@2023-05-01' = {
  name: 'catalog-service'
  location: 'West Europe'
  properties: {
    provisioningState: 'Succeeded'
    runningStatus: 'Running'
    managedEnvironmentId: '/subscriptions/78033352-805c-4acd-af80-f8f95083268d/resourceGroups/az-native-dev/providers/Microsoft.App/managedEnvironments/acaenv-az-native-dev'
    environmentId: '/subscriptions/78033352-805c-4acd-af80-f8f95083268d/resourceGroups/az-native-dev/providers/Microsoft.App/managedEnvironments/acaenv-az-native-dev'
    workloadProfileName: 'Consumption'
    outboundIpAddresses: [
      '51.137.8.84'
      '51.137.8.250'
      '40.114.177.194'
      '40.114.177.222'
      '4.175.88.205'
      '4.175.89.31'
      '4.175.89.19'
      '4.175.89.7'
      '20.103.135.29'
      '20.103.134.29'
      '20.103.132.54'
      '20.103.134.224'
    ]
    latestRevisionName: 'catalog-service--a9iyvvw'
    latestReadyRevisionName: 'catalog-service--a9iyvvw'
    latestRevisionFqdn: 'catalog-service--a9iyvvw.greendesert-0c4350be.westeurope.azurecontainerapps.io'
    customDomainVerificationId: '1387F783D35B4E628CFECBEF659048D3844B3364AFAB806A32607A7B0D3AA59F'
    configuration: {
      secrets: [
        {
          name: 'aznativecontainersdevazurecrio-aznativecontainersdev'
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        fqdn: 'catalog-service.greendesert-0c4350be.westeurope.azurecontainerapps.io'
        external: true
        targetPort: 80
        exposedPort: 0
        transport: 'Auto'
        traffic: [
          {
            weight: 100
            latestRevision: true
          }
        ]
        customDomains: null
        allowInsecure: false
        ipSecurityRestrictions: null
        corsPolicy: {
          allowedOrigins: [
            'angular.json'
            'config'
            'db.json'
            'dockerfile'
            'package-lock.json'
            'package.json'
            'src'
            'tsconfig.app.json'
            'tsconfig.json'
            'tsconfig.spec.json'
          ]
          allowedMethods: null
          allowedHeaders: [
            '*'
          ]
          exposeHeaders: null
          maxAge: null
          allowCredentials: true
        }
        clientCertificateMode: null
        stickySessions: null
      }
      registries: [
        {
          server: 'aznativecontainersdev.azurecr.io'
          username: 'aznativecontainersdev'
          passwordSecretRef: 'aznativecontainersdevazurecrio-aznativecontainersdev'
          identity: ''
        }
      ]
      dapr: null
      maxInactiveRevisions: null
      service: null
    }
    template: {
      revisionSuffix: ''
      terminationGracePeriodSeconds: null
      containers: [
        {
          image: 'aznativecontainersdev.azurecr.io/catalog-service'
          name: 'catalog-service'
          env: [
            {
              name: 'App__UseSQLite'
              value: 'true'
            }
            {
              name: 'ApplicationInsights__ConnectionString'
              value: ''
            }
          ]
          resources: {
            cpu: '0.5'
            memory: '1Gi'
            ephemeralStorage: '2Gi'
          }
        }
      ]
      initContainers: null
      scale: {
        minReplicas: 0
        maxReplicas: 1
        rules: null
      }
      volumes: null
      serviceBinds: null
    }
    eventStreamEndpoint: 'https://westeurope.azurecontainerapps.dev/subscriptions/78033352-805c-4acd-af80-f8f95083268d/resourceGroups/az-native-dev/containerApps/catalog-service/eventstream'
  }
  identity: {
    type: 'None'
  }
}