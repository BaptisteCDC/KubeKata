# Étape 5 : Traitement Asynchrone

1. Crée le namespace queue.
2. Déploie RabbitMQ :

```bash
kubectl apply -f rabbitmq-deployment.yaml
```
3. Déploie le Worker :

```bash
kubectl apply -f worker-deployment.yaml
```
