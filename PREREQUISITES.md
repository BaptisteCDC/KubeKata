# KubeKata Prerequisites

This document lists the tools required for the Kubernetes Kata.

## Tools Required

- **Docker**: Container runtime to run Minikube.
- **Minikube**: Local Kubernetes cluster.
- **Kubectl**: Kubernetes CLI tool.
- **Helm**: Package manager for Kubernetes (used for monitoring).
- **.NET 10 SDK**: To build and publish the C# application.
- **GitHub CLI (gh)**: To manage the repository and push code.

## Verification

Ensure all tools are accessible in your environment:

```bash
minikube version
kubectl version --client
docker version
helm version
dotnet --version
gh --version
```

> [!IMPORTANT]
> If you encounter permission issues during installation, you may need to adjust ownership of Homebrew's directories (usually `/opt/homebrew` on Apple Silicon).

---
[Next Step: Start Minikube](https://minikube.sigs.k8s.io/docs/start/)
