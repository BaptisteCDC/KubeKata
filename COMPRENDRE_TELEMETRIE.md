# Comprendre la Télémétrie : Du Code C# à Grafana

Ce document explique comment les données circulent de ton application jusqu'à ton dashboard Grafana.

## 🔄 Le Flux de Données

```mermaid
graph TD
    subgraph "Application C# (.NET 10)"
        A[Code Métier / Controllers] -->|Exécution| B[MetricsMiddleware]
        B -->|Capture Action & Status| C[KubeKataMetrics Service]
        C -->|Incrémentation| D[SDK OpenTelemetry]
    end

    subgraph "Exposition (Metrics Endpoint)"
        D -->|Endpoint HTTP| E[/metrics]
    end

    subgraph "Kubernetes Infrastructure"
        F[Prometheus Operator] -->|Découvre via| G[ServiceMonitor]
        G -->|Cible| H[Service Kubernetes]
        H -->|Scrape| E
    end

    subgraph "Visualisation"
        I[Grafana] -->|Requête PromQL| J[Prometheus DB]
        J -->|Données| I
    end
```

---

## 🏗️ 1. Côté Code C# (Génération)

### Les Bibliothèques (NuGet)
Nous utilisons **OpenTelemetry**, le standard moderne pour l'observabilité :
- `OpenTelemetry.Instrumentation.AspNetCore` : Capture automatique des requêtes.
- `OpenTelemetry.Exporter.Prometheus.AspNetCore` : Transforme les données au format compris par Prometheus.

### Le Middleware (Le "Cœur" du tri)
Le `MetricsMiddleware` est celui qui permet le tri par "Action" :
1. Chaque requête HTTP traverse ce middleware.
2. Il utilise les métadonnées de routage d'ASP.NET Core pour extraire le nom de la méthode (ex: `Create`).
3. Il appelle `RecordRequest` pour enregistrer la donnée.

---

## 🌐 2. Exposition des données

Dans `Program.cs`, nous avons :
```csharp
app.UseOpenTelemetryPrometheusScrapingEndpoint();
```
Cela crée une route invisible `/metrics` sur ton application. Tu peux la voir toi-même :
`curl http://127.0.0.1:64134/metrics`

C'est ici que Prometheus vient lire les compteurs régulièrement (tous les 15s ou 30s).

---

## ☸️ 3. Infrastructure Kubernetes

Pour que Prometheus sache quoi lire, nous utilisons un **ServiceMonitor** (`k8s/app-servicemonitor.yaml`) :
- Il dit à Prometheus : "Cherche tous les Services qui ont le label `app: kubekata`".
- Une fois trouvé, il sait qu'il doit aller lire le port `8080` sur le chemin `/metrics`.

---

## 📊 4. Visualisation Grafana

Enfin, dans Grafana, on utilise le langage **PromQL**.

Quand tu tapes : `sum by (action) (kubekata_http_requests_total)`
1. Grafana demande à Prometheus : "Donne-moi le compteur `kubekata_http_requests_total`".
2. On lui demande de regrouper (`sum by`) par l'étiquette `action`.
3. Grafana reçoit une liste (Create: 5, GetAll: 12) et l'affiche sur ton graphique.

---

## 🚀 Résumé des commandes utiles

- **Voir les métriques brutes** : `curl <URL_API>/metrics`
- **Vérifier que Prometheus voit l'app** : Ouvrir `http://localhost:9090/targets`
- **Tester le tri dans Grafana** : Utiliser l'onglet **Explore** avec la métrique `kubekata_http_requests_total`.
