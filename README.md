# DriveSale (API de Revenda de Veículos)

Plataforma de API para revenda de veículos automotores. Implementada em C# (.NET 9) com Clean Architecture, EF Core (PostgreSQL), documentação OpenAPI/Swagger e manifests Kubernetes. Inclui Docker Compose para ambiente local com Postgres e Adminer.

## Projeto
- Endpoints para:
  - Cadastrar/editar veículos e clientes
  - Efetuar venda (cria pagamento pendente)
  - Atualizar pagamento via webhook: status paid/canceled
  - Listar veículos disponíveis ou vendidos, ordenados por preço (asc)
- Documentação via Swagger (OpenAPI) e healthcheck simples.

## Como foi implementado
- Clean Architecture com quatro camadas:
  - Domain: entidades ricas (Vehicle, Sale, Payment, Client), enums e value object `Cpf`
  - Application: casos de uso com MediatR (Commands/Queries) e interfaces (Repos/UoW)
  - Infrastructure: EF Core + Npgsql (DbContext, Configurations, Repositories, UnitOfWork, Migrations)
  - WebApi: controllers finos, DI, Swagger e health
- Migrações: migração inicial incluída e aplicada automaticamente no startup (`Database.Migrate()`).
- Docker Compose: serviços `pgdb` (Postgres), `adminer` (UI) e `api` (WebApi container).
- Kubernetes: manifests prontos (ConfigMap, Secret, Deployment, Service, StatefulSet para Postgres e Deployment para Adminer).

## Estrutura do repositório
```
DriveSale.sln
src/
  Domain/            # Entidades, Enums, ValueObjects, Base
  Application/       # Commands/Queries (MediatR), Interfaces, DTOs
  Infrastructure/    # EF Core, DbContext, Configurations, Repositories, Migrations
  WebApi/            # Controllers, Program.cs, Swagger, appsettings
k8s/                 # Manifests K8s (ConfigMap, Secret, Postgres, Adminer, Deployment, Service)
scripts/             # Scripts Compose, EF e Kubernetes
README.md            # Este arquivo
```

## Como usar localmente

Opção A - VS Code (API no host)
- Requisitos: .NET SDK 9, Docker (para subir Postgres com Compose)
- Passos:
  - VS Code + Run Task + `compose-up` ou `compose-up-build`
  - F5 (perfil ".NET Launch WebApi")
  - Swagger: `http://localhost:5000/swagger`
  - Connection string do debug: `.vscode/launch.json` (ajuste se necessário)

Opção B - Docker Compose (API + Postgres no Docker)
- `docker compose up -d --build` ou VS Code + Run Task + `compose-up-build`
- API: `http://localhost:8080/swagger`
- Adminer: `http://localhost:8081` (System: PostgreSQL, Server: `pgdb`, User: `postgres`, Password: `postgres`, Database: `fase2`)

Tarefas úteis (VS Code + Run Task)
- Compose: `compose-up`, `compose-up-recreate`, `compose-up-build`, `compose-down`
- EF: `ef-tool-install`, `ef-migrations-add`, `ef-database-update`, `ef-add-and-update`

## Banco de dados e Migrações
- Há uma migração inicial em `src/Infrastructure/Migrations/*` aplicada no startup.
- Para criar novas migrações:
  - Task: `ef-migrations-add` (informe o nome) e depois `ef-database-update`, ou
  - Script: `scripts/ef-add-update.ps1 -InstallTool -Name MinhaMigration`

## Endpoints principais
Base: `/api`

Veículos
- POST `/vehicles`  cria veículo
- PUT `/vehicles/{id}`  edita (somente se disponível)
- GET `/vehicles?status=available|sold`  lista por preço (asc)
- GET `/vehicles/{id}`  detalhe

Clientes
- POST `/clients`  cria cliente (CPF válido)
- PUT `/clients/{id}`  atualiza nome/email
- GET `/clients/{id}`  detalhe

Vendas
- POST `/sales`  cria venda + payment pendente
  - Body: `{ vehicleId, buyerCpf, saleDate? }`
  - Retorno: `{ id, totalPrice, paymentCode, paymentStatus }`
- GET `/sales/{id}`  detalhe

Webhook Pagamentos
- POST `/webhooks/payments/{paymentCode}`
  - Body: `{ "status": "paid" | "canceled", "provider"?: "..." }`
  - Resposta: 202 Accepted (idempotente)

Health
- GET `/health`

## Como testar (fluxo sugerido)
1) Criar cliente
```
POST /api/clients
{
  "name": "Joao Silva",
  "email": "joao@exemplo.com",
  "cpf": "111.444.777-35"
}
```

2) Cadastrar veículo
```
POST /api/vehicles
{
  "brand": "Fiat",
  "model": "Argo",
  "year": 2022,
  "color": "Prata",
  "price": 65000
}
```

3) Listar disponíveis
```
GET /api/vehicles?status=available
```

4) Registrar venda (guarde o `paymentCode`)
```
POST /api/sales
{
  "vehicleId": "<id do veiculo>",
  "buyerCpf": "11144477735"
}
```

5) Simular pagamento via webhook
```
POST /api/webhooks/payments/{paymentCode}
{
  "status": "paid"
}
```

6) Listar vendidos
```
GET /api/vehicles?status=sold
```

7) Consultar venda
```
GET /api/sales/{id}
```

Observações
- Validações de domínio: CPF válido; não editar veículo vendido; venda somente com veículo disponível; webhook idempotente.
- HTTP: 201 (criado), 202 (webhook), 204 (update), 400/422 (validações), 404 (não encontrado), 409 (conflito).

## Kubernetes
- Manifests em `k8s/`: `configmap.yaml`, `secret.yaml`, `postgres.yaml`, `adminer.yaml`, `deployment.yaml`, `service.yaml`, `kustomization.yaml`.
- Ajuste a imagem no Deployment antes de aplicar.

### Subir no Kubernetes (demo)
- Requisitos: `kubectl` instalado e contexto de cluster ativo.
- Ajuste a imagem em `k8s/deployment.yaml` para seu repositório Docker Hub (ex.: `docker.io/<seu_usuario>/drivesale-api:latest`).
- Subir Postgres + API + Adminer via Kustomize:
  - `./scripts/k8s-apply.ps1 -CreateNamespace` (Windows/PowerShell)
  - ou `kubectl apply -k k8s -n drivesale`
- Acessar API via port-forward: `kubectl -n drivesale port-forward svc/drivesale-api 8080:80` → `http://localhost:8080/swagger`
- Acessar Adminer: `kubectl -n drivesale port-forward svc/adminer 8081:8080` → `http://localhost:8081`
- Observação: o `postgres` roda via StatefulSet com PVC (1Gi) usando o StorageClass padrão do cluster.

## CI/CD (GitHub Actions)
- Workflow: `.github/workflows/docker-publish.yml`
- O pipeline compila e publica a imagem Docker nas seguintes situações:
  - Push na branch `main`/`master` (gera a tag `latest` e tag de branch)
  - Push de tags (`v*` ou `*.*.*`) — publica com o nome da tag
  - Execução manual via `workflow_dispatch`
- Configuração necessária no repositório (Settings → Secrets and variables → Actions):
  - `DOCKERHUB_USERNAME`: seu usuário do Docker Hub
  - `DOCKERHUB_TOKEN`: Access Token do Docker Hub (mínimo Read & Write; recomendado Read, Write, Delete)
  - (Opcional) Variable `DOCKERHUB_NAMESPACE`: namespace da imagem (ex.: organização). Se não definir, usa `DOCKERHUB_USERNAME`.
- A imagem publicada segue o padrão: `docker.io/<namespace>/drivesale-api:<tag>`
- O workflow usa `src/WebApi/Dockerfile` e cache de build do Docker para acelerar execuções.
- Sem os segredos configurados, o workflow ainda faz o build (sem push) e usa `github.repository_owner` como namespace apenas para tag local.

## Solução de problemas
- 28P01 (senha inválida):
  - `compose-down` (remove volumes) + `compose-up-build` para recriar banco com a senha do compose
  - Teste com Adminer (Server: `pgdb`) ou `psql` usando a mesma connection string
- Porta 5432 ocupada: altere a porta publicada no `docker-compose.yml` e atualize a connection string
- HTTPS no dev: projeto roda em HTTP no debug. Para HTTPS, gere/confie o certificado: `dotnet dev-certs https --trust`

## Arquitetura e Fluxo
- Visão geral (Clean Architecture)
  - Domain: núcleo de negócio (Entidades, Value Objects e regras). Sem dependências externas.
  - Application: orquestra casos de uso (MediatR Commands/Queries), validações de aplicação e portas (interfaces de Repositórios/UoW). Depende apenas de Domain.
  - Infrastructure: detalhes técnicos (EF Core, DbContext, Repositórios, Migrations, UnitOfWork). Implementa interfaces da Application.
  - WebApi: camada de entrada HTTP (Controllers, Swagger, DI, Health). Não contém regra de negócio.
- Padrões e decisões
  - MediatR para desacoplar Controllers de casos de uso (CQRS leve com Requests/Handlers).
  - Repositórios e UoW na Infrastructure para persistência via EF Core (PostgreSQL).
  - Value Object `Cpf` no domínio para garantir formato/validação.
  - Migrações aplicadas no startup (Database.Migrate) apenas para ambientes controlados; em produção, preferir migrações por pipeline.
  - Integração com pagamento via webhook idempotente. Envio de e-mail não faz parte do escopo atual.
- Camadas e dependências (alto nível)
  - Usuário → WebApi (HTTP) → Application (Handlers)
  - Application → Domain (regras e entidades)
  - Application → Infrastructure (Repositórios/UoW) → Banco de Dados (EF Core)
  - Application → Serviço externo de Pagamento (webhook para atualização de status)

  ![Class Diagram](docs/ClassDiagram.png)
  ![architecture](docs/architecture.jpg)
  ![architecture](docs/eks-deployment.jpg)

## cURL/REST exemplos rápidos
Defina a base conforme o ambiente:
- VS Code (debug): `BASE=http://localhost:5000`
- Docker Compose: `BASE=http://localhost:8080`

Criar cliente
```
curl -s -X POST %BASE%/api/clients ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Joao Silva\",\"email\":\"joao@exemplo.com\",\"cpf\":\"111.444.777-35\"}"
```

Obter cliente por id
```
curl -s %BASE%/api/clients/{id}
```

Atualizar cliente
```
curl -s -X PUT %BASE%/api/clients/{id} ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Joao Atualizado\",\"email\":\"joao@exemplo.com\"}"
```

Criar veículo
```
curl -s -X POST %BASE%/api/vehicles ^
  -H "Content-Type: application/json" ^
  -d "{\"brand\":\"Fiat\",\"model\":\"Argo\",\"year\":2022,\"color\":\"Prata\",\"price\":65000}"
```

Listar veículos disponíveis / vendidos
```
curl -s %BASE%/api/vehicles?status=available
curl -s %BASE%/api/vehicles?status=sold
```

Obter veículo por id
```
curl -s %BASE%/api/vehicles/{id}
```

Atualizar veículo
```
curl -s -X PUT %BASE%/api/vehicles/{id} ^
  -H "Content-Type: application/json" ^
  -d "{\"brand\":\"Fiat\",\"model\":\"Argo\",\"year\":2023,\"color\":\"Branco\",\"price\":64000}"
```

Criar venda (guarde o paymentCode do retorno)
```
curl -s -X POST %BASE%/api/sales ^
  -H "Content-Type: application/json" ^
  -d "{\"vehicleId\":\"{vehicleId}\",\"buyerCpf\":\"11144477735\"}"
```

Aplicar webhook de pagamento (paid/canceled)
```
curl -s -X POST %BASE%/api/webhooks/payments/{paymentCode} ^
  -H "Content-Type: application/json" ^
  -d "{\"status\":\"paid\"}"
```

Obter venda por id
```
curl -s %BASE%/api/sales/{id}
```
