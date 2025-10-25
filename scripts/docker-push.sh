#!/usr/bin/env bash
set -euo pipefail

# Usage:
#   DOCKERHUB_USERNAME=youruser DOCKERHUB_TOKEN=... ./scripts/docker-push.sh [tag]
#   ./scripts/docker-push.sh 1.0.0

USERNAME="${DOCKERHUB_USERNAME:-}"
IMAGE="drivesale-api"
TAG="${1:-latest}"
REGISTRY="docker.io"

if [[ -z "$USERNAME" ]]; then
  echo "ERROR: Docker Hub username is required. Set DOCKERHUB_USERNAME or pass via env." 1>&2
  exit 1
fi

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
DOCKERFILE="$ROOT_DIR/src/WebApi/Dockerfile"
if [[ ! -f "$DOCKERFILE" ]]; then
  echo "ERROR: Dockerfile not found at src/WebApi/Dockerfile" 1>&2
  exit 1
fi

REPO="$REGISTRY/$USERNAME/$IMAGE"
echo "Repository: $REPO"
echo "Tag:        $TAG"

if [[ -n "${DOCKERHUB_TOKEN:-}" ]]; then
  echo "Logging in to $REGISTRY as $USERNAME (password from env)..."
  echo -n "$DOCKERHUB_TOKEN" | docker login "$REGISTRY" -u "$USERNAME" --password-stdin
else
  echo "Skipping docker login (no DOCKERHUB_TOKEN). If needed, run: docker login -u $USERNAME" 1>&2
fi

echo "Building image..."
docker build -f "$DOCKERFILE" -t "$REPO:$TAG" "$ROOT_DIR"

if [[ "$TAG" != "latest" ]]; then
  echo "Tagging also as latest..."
  docker tag "$REPO:$TAG" "$REPO:latest"
fi

echo "Pushing $REPO:$TAG ..."
docker push "$REPO:$TAG"

if [[ "$TAG" != "latest" ]]; then
  echo "Pushing $REPO:latest ..."
  docker push "$REPO:latest"
fi

echo "Done."

