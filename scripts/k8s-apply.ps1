param(
  [Parameter(Mandatory=$false)][string]$Namespace = "drivesale",
  [switch]$CreateNamespace
)

Write-Host "Applying Kubernetes manifests (namespace: $Namespace)..." -ForegroundColor Cyan

if ($CreateNamespace) {
  Write-Host "Ensuring namespace exists..." -ForegroundColor Yellow
  kubectl get ns $Namespace 2>$null 1>$null
  if ($LASTEXITCODE -ne 0) {
    kubectl create ns $Namespace
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
  }
}

$k8sDir = Join-Path $PSScriptRoot "..\k8s"
if (!(Test-Path $k8sDir)) {
  Write-Error "k8s directory not found."
  exit 1
}

kubectl apply -k $k8sDir -n $Namespace
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Done. Use port-forward to access the API:" -ForegroundColor Green
Write-Host "kubectl -n $Namespace port-forward svc/drivesale-api 8080:80" -ForegroundColor DarkGreen

