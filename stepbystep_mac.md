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
---
## Phase 4: Scalability

### 1. Install KEDA
KEDA (Kubernetes Event-Driven Autoscaling) allows scaling based on external events like Prometheus metrics.

If not already installed:
```bash
helm repo add kedacore https://kedacore.github.io/charts
helm repo update
helm install keda kedacore/keda --namespace keda --create-namespace
```

### 2. Deploy the ScaledObject
This object tells KEDA how to scale your application based on Prometheus metrics. Ici, nous configurons un seuil de **30 requêtes sur 5 minutes par pod**.

```bash
kubectl apply -f k8s/app-scaledobject.yaml
```

> [!NOTE]
> **Paramètres de temps par défaut :**
> - **Polling Interval (30s)** : Fréquence de vérification des métriques.
> - **Cooldown Period (300s / 5min)** : Temps d'attente avant de supprimer un pod après une baisse de charge.

### 3. Verify Autoscaling
Wait for some traffic, then check the status:
```bash
# Watch the pods scaling
kubectl get pods -w

# Check KEDA's view of the metrics
kubectl get scaledobject kubekata-app-scaler
```

---
## Phase 5: Asynchronous Processing (RabbitMQ & Worker)

### 1. Deploy RabbitMQ
RabbitMQ serves as our message broker.

```bash
# Create the isolated namespace
kubectl create namespace queue

# Deploy RabbitMQ
kubectl apply -f k8s/rabbitmq-deployment.yaml
```

### 2. Build and Deploy the Worker
The worker consumes messages from the queue and handles them idempotently.

```bash
# Publish and build Docker image
cd worker/KubeKataWorker
dotnet publish -c Release
eval $(minikube docker-env)
docker build -t kubekata-worker -f Dockerfile .
cd ../..

# Deploy Worker and its scaling rules
kubectl apply -f k8s/worker-deployment.yaml
kubectl apply -f k8s/worker-scaledobject.yaml
```

### 3. Verify the Async Flow
1. Check the logs of the worker:
   ```bash
   kubectl logs -l app=kubekata-worker -f
   ```
2. Create an admin via the API (Producer) and see it appearing in the worker's logs (Consumer).

---
## Phase 6: Resource Governance (Priorities & Quotas)

### 1. Apply Priority and Quotas
This ensures the API has priority over the Worker and limits the total pods.

```bash
kubectl apply -f k8s/resource-governance.yaml
```

### 2. Update Deployments (Add Priority)
Pour que Kubernetes prenne en compte ces priorités, tu dois éditer tes fichiers YAML pour y ajouter le `priorityClassName`.

#### A. Éditer `k8s/app-deployment.yaml`
Ajoute `priorityClassName: apps-high-priority` dans la section `spec.template.spec`:
```yaml
    spec:
      priorityClassName: apps-high-priority # <-- Ajoute cette ligne
      containers:
      - name: kubekata-app
```

#### B. Éditer `k8s/worker-deployment.yaml`
Ajoute `priorityClassName: apps-low-priority`:
```yaml
    spec:
      priorityClassName: apps-low-priority # <-- Ajoute cette ligne
      containers:
      - name: kubekata-worker
```

#### C. Apply the changes
```bash
kubectl apply -f k8s/app-deployment.yaml
kubectl apply -f k8s/worker-deployment.yaml
```

### 3. Verify Quotas
Check that your namespace has a limit:
```bash
kubectl get quota pod-quota
```
---
## Phase 7: Orchestration with Helm

Helm permet de déployer toute l'architecture d'un seul coup.

### 1. Installation complète
Depuis la racine du repo :
```bash
helm install kubekata ./helm/kubekata
```

### 2. Vérification
Vérifie que tous les composants sont démarrés :
```bash
kubectl get pods -A
```

### 3. Gestion des mises à jour
Si tu modifies le `values.yaml` :
```bash
helm upgrade kubekata ./helm/kubekata
```

### 4. Suppression totale
```bash
helm uninstall kubekata
```
