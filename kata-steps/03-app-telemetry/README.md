# Étape 3 : Télémétrie Applicative

1. Déploie le ServiceMonitor pour Prometheus :

```bash
kubectl apply -f app-servicemonitor.yaml
```
2. Vérifie dans Prometheus que les métriques `kubekata_http_requests_total` remontent.
