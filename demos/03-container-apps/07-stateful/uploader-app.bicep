resource uploader_app 'Microsoft.App/containerApps@2023-05-01' = {
  name: 'uploader-app'
  location: 'West Europe'
  properties: {
    provisioningState: 'Succeeded'
    runningStatus: 'Running'
    managedEnvironmentId: '/subscriptions/78033352-805c-4acd-af80-f8f95083268d/resourceGroups/az-native-dev/providers/Microsoft.App/managedEnvironments/acaenv-az-native-dev'
    environmentId: '/subscriptions/78033352-805c-4acd-af80-f8f95083268d/resourceGroups/az-native-dev/providers/Microsoft.App/managedEnvironments/acaenv-az-native-dev'
    workloadProfileName: 'Consumption'
    outboundIpAddresses: [
      '20.8.196.214'
      '4.175.220.254'
      '4.175.220.253'
      '20.238.219.204'
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
    latestRevisionName: 'uploader-app--g8x8po3'
    latestReadyRevisionName: 'uploader-app--g8x8po3'
    latestRevisionFqdn: 'uploader-app--g8x8po3.mangodesert-74de3e55.westeurope.azurecontainerapps.io'
    customDomainVerificationId: '1387F783D35B4E628CFECBEF659048D3844B3364AFAB806A32607A7B0D3AA59F'
    configuration: {
      secrets: [
        {
          name: 'aznativecontainersdevazurecrio-aznativecontainersdev'
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        fqdn: 'uploader-app.mangodesert-74de3e55.westeurope.azurecontainerapps.io'
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
        corsPolicy: null
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
          image: 'aznativecontainersdev.azurecr.io/uploader-app'
          name: 'uploader-app'
          env: [
            {
              name: 'IMG_FOLDER_NAME'
              value: 'images'
            }
          ]
          resources: {
            cpu: '0.5'
            memory: '1Gi'
            ephemeralStorage: '2Gi'
          }
          volumeMounts: [
            {
              volumeName: 'azurefiles'
              mountPath: '/app/images'
            }
          ]
        }
      ]
      initContainers: null
      scale: {
        minReplicas: 0
        maxReplicas: 1
        rules: null
      }
      volumes: [
        {
          name: 'azurefiles'
          storageType: 'AzureFile'
          storageName: 'mountimages'
        }
      ]
      serviceBinds: null
    }
    eventStreamEndpoint: 'https://westeurope.azurecontainerapps.dev/subscriptions/78033352-805c-4acd-af80-f8f95083268d/resourceGroups/az-native-dev/containerApps/uploader-app/eventstream'
  }
  identity: {
    type: 'None'
  }
}