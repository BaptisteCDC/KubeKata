# KubeKata: Step by Step (macOS)

Follow these steps to set up your environment and start your first local Kubernetes cluster.

## Phase 1: Environment Setup

### 1. Install Minikube and Kubectl
Run the following command in your terminal:
```bash
brew install minikube kubectl
```

### 2. Verify Installation
Ensure both tools are correctly installed:
```bash
minikube version
kubectl version --client
```

### 3. Start Minikube
Launch your local Kubernetes cluster using the Docker driver (default):
```bash
minikube start
```

---
## Phase 2: Deploy Custom Application

### 1. Configure Shell for Minikube's Docker
Run this to build images directly inside Minikube's Docker daemon:
```bash
eval $(minikube docker-env)
```

### 2. Build the Docker Image
Navigate to the application folder and build the image:
```bash
cd application
docker build -t kubekata-app:latest .
cd ..
```

### 3. Deploy to Kubernetes
Apply the deployment and service manifests:
```bash
kubectl apply -f k8s/app-deployment.yaml
```

### 4. Verify the Deployment
Check if the pods are running and the service is available:
```bash
kubectl get pods
kubectl get services
```

### 5. Access the Application
Use Minikube to get the URL for your service:
```bash
minikube service kubekata-service --url
```

---
Next Phase: [Phase 3: Monitoring]
