# Collect Events from Azure Storage File Share in Event Hub

In this demo, we will collect events from Azure Storage File Share in Event Hub. As Azure File Share does not publish events to event grid, we will use 

![file-share-events](_images/file-share-events.png)

## Demos

- Execute `capture-fileShare-events.azcli` to create the Event Hub and the storage account

- Go to the diagnostic settings of storage account and edit the settings for file share events

    ![diagostic-settings](_images/diagnostic-settings.png)

- Send this events to the Event Hub created in the previous step

    ![diagnostic-send-to-eh](_images/diagnostic-send-to-eh.png)

- Run the demo and watch the output in the debugger. You should see the events from the file share

    ![debugger](_images/debugger.png)    

    >Note: You could now process this file using the [Claim-Check-pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/claim-check)