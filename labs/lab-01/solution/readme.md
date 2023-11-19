# Solution - Architecture & Provisioning

- Install [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- Install [Azure CLI Tools Extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.azurecli)
- Examine [create-app.azcli](./create-app.azcli) and change the env vaiable to provide a unique name to avoid naming conflicts

    - Before:
        ```Bash
        env=dev
        loc=westeurope
        ```
    - After:
        ```Bash
        env=alex
        loc=westeurope
        ```
  >Note: Notice that we are saving most of the keys, endpoints and connection strings in Azure Key Vault. This is a best practice and allows us to pick them up in subsequent labs. If you change naming conventions, you will need to update the subsequent labs script to pick up the correct values.

- Execute `create-app.azcli` in [WSL](/setup/windows-subsystem-linux/) using  to create the Azure resources.  Do not execute the whole script at once. But instead do it resource by resource to be able to fix errors.