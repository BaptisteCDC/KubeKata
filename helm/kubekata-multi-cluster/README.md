# 🌐 Architecture Multi-Cluster KubeKata

Ce dossier contient une version dégroupée du Helm Chart KubeKata, conçue pour être installée sur **deux clusters Kubernetes distincts**.

---

## 🏗️ Structure des Charts

### 1. `app/` (Le Cluster Applicatif)
Contient l'API, le Worker, RabbitMQ et les règles de gouvernance.
- **À installer sur** : Ton cluster de Production ou de Calcul.
- **Objectif** : Zéro surcharge de monitoring locale.

### 2. `monitoring/` (Le Cluster de Supervision)
Contient uniquement les `ServiceMonitors`.
- **À installer sur** : Ton cluster de Monitoring (celui qui héberge Prometheus/Grafana).
- **Objectif** : Centraliser la supervision sans toucher aux clusters applicatifs.

---

## 🚀 Guide de déploiement Multi-Minikube

### Étape 1 : Préparer le Cluster Applicatif (Minikube A)
```bash
# Démarre le premier minikube
minikube start -p kubekata-app

# Installe uniquement l'applicatif
helm install kubekata-app ./app
```

### Étape 2 : Préparer le Cluster de Supervision (Minikube B)
```bash
# Démarre le second minikube
minikube start -p kubekata-monitoring

# Installe la stack Prometheus (si pas déjà faite)
helm install prom prometheus-community/kube-prometheus-stack --namespace monitoring --create-namespace

# Installe la configuration de monitoring de KubeKata
# Note : Tu devras configurer l'IP du Minikube A dans les ServiceMonitors pour le multi-cluster réel.
helm install kubekata-mon ./monitoring
```

---

## 📡 Comment lier les deux clusters ?

En environnement réel (On-Premise), le Prometheus du Cluster B irait chercher les métriques sur l'IP des Nodes ou l'Ingress du Cluster A.

Dans cette configuration Kata, si tu installes les deux sur le même cluster, ils se complètent parfaitement. Si tu les sépares, il faudra configurer un `additionalScrapeConfigs` dans Prometheus pour cibler l'IP exposée par Minikube A.
