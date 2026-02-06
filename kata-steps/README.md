# 🪜 KubeKata : La Progression Pas à Pas

Ce dossier contient les étapes successives pour construire l'architecture KubeKata. Chaque dossier est autonome et représente un état stable de l'infrastructure.

## 📋 Sommaire des Étapes

1.  **[01-basic-app](./01-basic-app)** : Déploiement d'une API .NET simple sur Minikube.
2.  **[02-monitoring-infra](./02-monitoring-infra)** : Installation de Prometheus/Grafana et observation des logs techniques.
3.  **[03-app-telemetry](./03-app-telemetry)** : Instrumentation de l'application et exposition des métriques métier.
4.  **[04-keda-autoscaling](./04-keda-autoscaling)** : Mise en place de l'autoscaling dynamique basé sur le trafic HTTP.
5.  **[05-async-worker](./05-async-worker)** : Introduction de RabbitMQ et du Worker pour le traitement asynchrone.
6.  **[06-governance-scaling](./06-governance-scaling)** : Optimisation finale (PriorityClasses, Quotas et autoscaling du Worker).

---

## 🛠️ Comment utiliser ces étapes ?

Chaque sous-dossier contient un `README.md` avec les instructions spécifiques. Il est recommandé de les suivre dans l'ordre pour bien comprendre comment chaque brique s'ajoute aux précédentes.

> [!TIP]
> **Souveraineté Informatique** : En suivant ces étapes, tu apprends à gérer chaque aspect de ton infrastructure (Scaling, Priorités, Messages) sans dépendre des services managés d'un Cloud Provider.
