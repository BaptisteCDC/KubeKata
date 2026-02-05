# KubeKata Prerequisites

This document lists the tools required for the current phase of the Kubernetes Kata. Prerequisites will be added incrementally as we progress.

## Tools Required (Current Phase)

- **Docker**: Container runtime to run Minikube.
- **Minikube**: Local Kubernetes cluster.
- **Kubectl**: Kubernetes CLI.

## Verification

Ensure the core tools are accessible in your environment:

```bash
minikube version
kubectl version --client
docker version
```

> [!NOTE]
> Additional tools (monitoring, load testing, etc.) will be added to this list as we reach those phases of the kata.

Run the following to check your environment:

```bash
minikube version
kubectl version --client
docker version
```

> [!IMPORTANT]
> Ensure your user has the necessary permissions to run `brew` and `docker`. If you encounter permission issues during installation, you may need to adjust ownership of Homebrew's directories (usually `/usr/local` or `/opt/homebrew`).

---
[Next Step: Start Minikube](https://minikube.sigs.k8s.io/docs/start/)
