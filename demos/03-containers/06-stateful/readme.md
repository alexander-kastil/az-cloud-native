# Stateful Containers using Azure Storage Volume Mounts

[Storage options for applications in Azure Kubernetes Service (AKS)](https://learn.microsoft.com/en-us/azure/aks/concepts-storage)

## Demo

Create a stoage class:

```bash
kubectl apply -f azure-file-sc.yaml
```

Create a persistent volume claim:

```bash
kubectl apply -f azure-file-pvc.yaml
```

Create a pod with a volume mount:

```bash
kubectl apply -f azure-file-pod.yaml
```