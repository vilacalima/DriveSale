param(
  [Parameter(Mandatory=$false)][string]$Username = $env:DOCKERHUB_USERNAME,
  [Parameter(Mandatory=$false)][string]$Image = "drivesale-api",
  [Parameter(Mandatory=$false)][string]$Tag = "latest",
  [Parameter(Mandatory=$false)][string]$Registry = "docker.io",
  [Parameter(Mandatory=$false)][string]$Password = $env:DOCKERHUB_TOKEN,
  [switch]$NoLogin
)

Write-Host "Docker: build and push WebApi image to Docker Hub" -ForegroundColor Cyan

if (-not $Username -or [string]::IsNullOrWhiteSpace($Username)) {
  Write-Error "Docker Hub username is required. Use -Username or set DOCKERHUB_USERNAME."
  exit 1
}

$root = Join-Path $PSScriptRoot ".."
$dockerfile = Join-Path $root "src/WebApi/Dockerfile"
if (!(Test-Path $dockerfile)) {
  Write-Error "Dockerfile not found at src/WebApi/Dockerfile (relative to repo root)."
  exit 1
}

$repo = "$Registry/$Username/$Image"
Write-Host "Repository: $repo" -ForegroundColor Green
Write-Host "Tag:        $Tag" -ForegroundColor Green

Push-Location $root
try {
  if (-not $NoLogin) {
    if ($Password) {
      Write-Host "Logging in to $Registry as $Username (password from env/param)..." -ForegroundColor Yellow
      $secure = ConvertTo-SecureString $Password -AsPlainText -Force
      $cred = New-Object System.Management.Automation.PSCredential($Username, $secure)
      # Use password-stdin to avoid echoing
      $plain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure))
      $plain | docker login $Registry -u $Username --password-stdin
      if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    } else {
      Write-Host "Logging in to $Registry as $Username (interactive)..." -ForegroundColor Yellow
      docker login $Registry -u $Username
      if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
  } else {
    Write-Host "Skipping docker login (NoLogin)." -ForegroundColor DarkYellow
  }

  Write-Host "Building image..." -ForegroundColor Green
  docker build -f "$dockerfile" -t "$repo:$Tag" .
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

  if ($Tag -ne "latest") {
    Write-Host "Tagging also as latest..." -ForegroundColor DarkGreen
    docker tag "$repo:$Tag" "$repo:latest"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
  }

  Write-Host "Pushing $repo:$Tag ..." -ForegroundColor Green
  docker push "$repo:$Tag"
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

  if ($Tag -ne "latest") {
    Write-Host "Pushing $repo:latest ..." -ForegroundColor Green
    docker push "$repo:latest"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
  }

  Write-Host "Done." -ForegroundColor Green
} finally {
  Pop-Location
}

