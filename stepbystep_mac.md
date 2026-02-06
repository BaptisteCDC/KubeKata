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
// TODO: Revoir cette partie pour faire du multi langage
### 2. Build and Dockerize the Application
To avoid SSL issues during the Docker build, we will publish the application locally and then copy the binaries into a lean Docker image.

#### A. Publish locally
```bash
cd application
dotnet publish -c Release
```

#### B. Build the Docker Image
Assure-toi d'être dans le dossier `application` :
```bash
cd application
eval $(minikube docker-env)
docker build -t kubekata-app -f Dockerfile .
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
## Phase 3: Monitoring

### 1. Install Prometheus and Grafana
We use the `kube-prometheus-stack` to install everything at once:

```bash
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update
helm upgrade --install prom prometheus-community/kube-prometheus-stack -n monitoring --create-namespace
```

### 2. Enable Metrics Server
Wait for the monitoring pods to be ready, then enable the metrics server:
```bash
minikube addons enable metrics-server
```

### 3. Access Grafana
#### A. Create a Tunnel
```bash
kubectl port-forward -n monitoring service/prom-grafana 3000:80
```

#### B. Get Login Credentials
In another terminal:
```bash
# User: admin
# Password:
kubectl get secret -n monitoring prom-grafana -o jsonpath="{.data.admin-password}" | base64 --decode ; echo ""
```

#### C. Login
Open [http://localhost:3002](http://localhost:3002) and explore the default dashboards.

### 4. Intégrer ton Application dans Grafana

Maintenant que ton application expose des métriques (via `/metrics`), voici comment les visualiser :

#### B. Vérifier la connexion Prometheus (Debug)
Si tes données n'apparaissent pas, vérifie que Prometheus "voit" bien ton application :
1. Lance un tunnel vers Prometheus :
   ```bash
   kubectl port-forward -n monitoring prometheus-prom-kube-prometheus-stack-prometheus-0 9090
   ```
2. Ouvre [http://localhost:9090/targets](http://localhost:9090/targets).
3. Cherche `kubekata-app-monitor`. Il doit être en état **UP**. Si ce n'est pas le cas, vérifie les labels de ton Service.

> [!TIP]
> **Que veut dire "Vérifier les labels" ?**
> Kubernetes utilise des étiquettes (Labels) pour lier les objets. Pour que le monitoring fonctionne :
> 1. Ton **Service** doit avoir le label `app: kubekata` (dans `metadata.labels`).
> 2. Ton **ServiceMonitor** doit avoir le sélecteur `matchLabels: app: kubekata` qui pointe vers ce service.
> - **Vérifier en ligne de commande** : `kubectl get svc --show-labels`
> - **Vérifier dans le code** : Compare `k8s/app-deployment.yaml` et `k8s/app-servicemonitor.yaml`.

#### C. Créer ton premier graphique
1. Va dans **Dashboards** > **New** > **New Dashboard**.
2. Clique sur **Add visualization**.
3. Sélectionne la source **Prometheus**.
4. Dans le champ **Query**, entre : `kubekata_admins_created_total`.
5. Dans l'onglet **Options** à droite, change le titre en "Total Admins Created".
6. Clique sur **Save** en haut à droite.

---
Next Phase: [Phase 4: Scalability]
