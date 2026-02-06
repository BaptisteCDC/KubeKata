#!/bin/bash

# Configuration
URL="http://127.0.0.1:54489/api/Admin"
COUNT=${1:-20} # Par défaut 20 cycles (100 GETs total)
SLEEP_TIME=${2:-0.1} # Délai entre les groupes de requêtes

echo "🚀 Démarrage du test de charge hybride (1 POST + 5 GET par cycle) : $COUNT cycles..."

for i in $(seq 1 $COUNT)
do
  # 1. POST - Création d'un admin
  ID=$(uuidgen | head -c 8)
  USERNAME="loadtest_$ID"
  EMAIL="$USERNAME@kubekata.io"

  echo -n "P"
  curl -s -X POST "$URL" \
       -H "Content-Type: application/json" \
       -d "{
         \"username\": \"$USERNAME\",
         \"email\": \"$EMAIL\",
         \"password\": \"Password123!\"
       }" > /dev/null

  # 2. 5x GET - Récupération de la liste
  for g in {1..50}
  do
    echo -n "g"
    curl -s "$URL" > /dev/null
  done

  echo " [$i/$COUNT]"
  sleep $SLEEP_TIME
done

echo -e "\n✅ Test terminé !"
echo "Surveille les pods : kubectl get pods -w"
echo "Surveille les logs : kubectl logs -l app=kubekata-worker -f"
