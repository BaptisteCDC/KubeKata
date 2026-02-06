# Étape 6 : Gouvernance & Scaling du Worker

1. Déploie la gouvernance (Priorités/Quotas) :

```bash
kubectl apply -f resource-governance.yaml
```
2. Mets à jour les déploiements avec les priorités.
3. Ajoute l'autoscaler du worker (KEDA) :

```bash
kubectl apply -f worker-scaledobject.yaml
```
