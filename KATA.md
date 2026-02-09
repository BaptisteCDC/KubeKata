# KubeKata: The Kubernetes Resilience & Scalability Challenge

## Context
You are a DevOps engineer tasked with deploying a critical "Admin Registration" system. This system consists of an API (Producer), a Message Broker (RabbitMQ), and a Worker (Consumer). The system must be resilient to high traffic and handle processing asynchronously.

## Learning Objectives
- **Local K8s Management**: Master Minikube, Docker environment, and basic kubectl.
- **Observability**: Set up Prometheus & Grafana to monitor application health and performance.
- **Autoscaling**: Use KEDA to scale workloads based on real-time metrics (HTTP load and Queue depth).
- **Resource Governance**: Implement PriorityClasses and ResourceQuotas to ensure system stability.
- **Persistence**: Move from In-Memory to persistent storage (PostgreSQL) with SOLID Dependency Inversion.
- **Open Source Focus**: Use only cross-platform, vendor-neutral tools.

## Prerequisites & Pre-flight Checks
1.  **Docker**: Must be running.
2.  **VPN**: Must be **DISABLED** (to avoid DNS/Routing issues with Minikube).
3.  **Terminal Check**:
    ```bash
    docker ps  # Should list nothing or running containers
    minikube version
    helm version
    ```

---

## Step 1: Environment Boot
1.  **Start Minikube**:
    ```bash
    minikube start --driver=docker
    ```
2.  **Configure Docker Shell**:
    ```bash
    eval $(minikube docker-env)
    ```

## Step 2: Build & Deploy
1.  **Build Application Images**:
    ```bash
    # API
    cd application && dotnet publish -c Release && docker build -t kubekata-app -f Dockerfile .
    # Worker
    cd ../worker/KubeKataWorker && dotnet publish -c Release && docker build -t kubekata-worker -f Dockerfile .
    cd ../..
    ```
2.  **Deploy via Helm**:
    ```bash
    helm install kubekata ./helm/kubekata
    ```

## Step 3: Observability Setup
1.  **Enable Metrics**: `minikube addons enable metrics-server`.
2.  **Access Grafana**:
    ```bash
    # Port forward in a separate terminal
    kubectl port-forward -n monitoring service/prom-grafana 3000:80
    ```
    - **Login**: admin
    - **Password**: `kubectl get secret -n monitoring prom-grafana -o jsonpath="{.data.admin-password}" | base64 --decode ; echo ""`
    - **Dashboard**: Import or open "KubeKata Dashboard". It includes metrics for Pods, Queue size, and Database writes.

## Step 4: Load Testing
1.  **Get Service URL**:
    ```bash
    minikube service kubekata-service --url
    ```
2.  **Run Load Test**:
    ```bash
    # Configurable URL/Port
    ./scripts/load_test_worker.sh <SERVICE_URL>
    ```
    - Observe the scaling in Grafana.
    - Watch Pods: `kubectl get pods -w`.

## Step 5: Advanced - Persistence & SOLID
1.  **Persistence**: The system now uses PostgreSQL (deployed via Helm).
    - **Table 1 (API)**: `admin_accounts` store registration data via `IAdminRepository`.
    - **Table 2 (Worker)**: `processed_messages` tracks processed IDs via `IMessageTracker` for idempotency.
2.  **Dependency Inversion**: 
    - Check `application/Program.cs` and `worker/Program.cs`. 
    - The repository and tracker implementations are swapped at startup based on configuration.
    - Neither the API nor the Worker logic knows about PostgreSQL; they only know their respective interfaces.

## Step 6: Step-by-Step Learning
- **Phase 1**: Cluster initialization & Docker isolation.
- **Phase 2**: Helm orchestration vs manual manifests.
- **Phase 3**: Scaling triggers (HPA vs KEDA).
- **Phase 4**: Resiliency via Message Queuing.
