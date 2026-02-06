# Étape 2 : Monitoring & Logs

1. Installe Prometheus/Grafana (voir README racine).
2. Vérifie les logs techniques de ton pod :

```bash
kubectl logs -l app=kubekata -f
```
