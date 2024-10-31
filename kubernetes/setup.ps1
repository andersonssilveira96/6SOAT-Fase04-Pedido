minikube start

# Metrics para obter informações e fazer o HPA da API.
kubectl apply -f metrics-server.yaml

# Banco de dados
kubectl apply -f db-configmap.yaml
kubectl apply -f db-pv.yaml
kubectl apply -f db-pvc.yaml
kubectl apply -f db-deployment.yaml
kubectl apply -f db-svc.yaml

# API
kubectl apply -f api-secret.yaml
kubectl apply -f api-deployment.yaml
kubectl apply -f api-svc.yaml
kubectl apply -f api-hpa.yaml

pause