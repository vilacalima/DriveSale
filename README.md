# DriveSale (API de Revenda de Veiculos)

Plataforma de API para revenda de veiculos automotores. Implementada em C# (.NET 9) com Clean Architecture, EF Core (PostgreSQL), documentacao OpenAPI/Swagger e manifests Kubernetes. Inclui Docker Compose para ambiente local com Postgres e Adminer.

## Projeto
- Endpoints para:
  - Cadastrar/editar veiculos e clientes
  - Efetuar venda (cria pagamento pendente)
  - Atualizar pagamento via webhook: status paid/canceled
  - Listar veiculos disponiveis ou vendidos, ordenados por preco (asc)
- Documentacao via Swagger (OpenAPI) e healthcheck simples.

## Como foi implementado
- Clean Architecture com quatro camadas:
  - Domain: entidades ricas (Vehicle, Sale, Payment, Client), enums e value object `Cpf`
  - Application: casos de uso com MediatR (Commands/Queries) e interfaces (Repos/UoW)
  - Infrastructure: EF Core + Npgsql (DbContext, Configurations, Repositories, UnitOfWork, Migrations)
  - WebApi: controllers finos, DI, Swagger e health
- Migracoes: migration inicial incluida e aplicada automaticamente no startup (`Database.Migrate()`).
- Docker Compose: Servicos `pgdb` (Postgres), `adminer` (UI) e `api` (WebApi container).
- Kubernetes: manifests prontos (ConfigMap, Secret, Deployment, Service).

## Estrutura do repositorio
```
DriveSale.sln
src/
  Domain/            # Entidades, Enums, ValueObjects, Base
  Application/       # Commands/Queries (MediatR), Interfaces, DTOs
  Infrastructure/    # EF Core, DbContext, Configurations, Repositories, Migrations
  WebApi/            # Controllers, Program.cs, Swagger, appsettings
k8s/                 # Manifests K8s
scripts/             # Scripts Docker Compose e EF
docker-compose.yml   # Compose (pgdb, adminer, api)
```

## Como usar localmente

Opcao A - VS Code (API no host)
- Requisitos: .NET SDK 9, Docker (para subir Postgres com Compose)
- Passos:
  - VS Code + Run Task + `compose-up` ou `compose-up-build`
  - F5 (perfil ".NET Launch WebApi")
  - Swagger: `http://localhost:5000/swagger`
  - Connection string do debug: `.vscode/launch.json` (ajuste se necessario)

Opcao B - Docker Compose (API + Postgres no Docker)
- `docker compose up -d --build` ou VS Code + Run Task + `compose-up-build`
- API: `http://localhost:8080/swagger`
- Adminer: `http://localhost:8081` (System: PostgreSQL, Server: `pgdb`, User: `postgres`, Password: `postgres`, Database: `fase2`)

Tarefas uteis (VS Code + Run Task)
- Compose: `compose-up`, `compose-up-recreate`, `compose-up-build`, `compose-down`
- EF: `ef-tool-install`, `ef-migrations-add`, `ef-database-update`, `ef-add-and-update`

## Banco de dados e Migracoes
- Ha uma migration inicial em `src/Infrastructure/Migrations/*` aplicada no startup.
- Para criar novas migracoes:
  - Task: `ef-migrations-add` (informe o nome) e depois `ef-database-update`, ou
  - Script: `scripts/ef-add-update.ps1 -InstallTool -Name MinhaMigration`

## Endpoints principais
Base: `/api`

Veiculos
- POST `/vehicles`  cria veiculo
- PUT `/vehicles/{id}`  edita (somente se disponivel)
- GET `/vehicles?status=available|sold`  lista por preco (asc)
- GET `/vehicles/{id}`  detalhe

Clientes
- POST `/clients`  cria cliente (CPF valido)
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

## Como testar
Use o Swagger (UI) ou `src/WebApi/WebApi.http`. Fluxo sugerido:

1) Criar cliente
```
POST /api/clients
{
  "name": "Joao Silva",
  "email": "joao@exemplo.com",
  "cpf": "111.444.777-35"
}
```

2) Cadastrar veiculo
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

3) Listar disponiveis
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

Observacoes
- Validacoes de dominio: CPF valido; nao editar veiculo vendido; venda somente com veiculo disponivel; webhook idempotente.
- HTTP: 201 (criado), 202 (webhook), 204 (update), 400/422 (validacoes), 404 (nao encontrado), 409 (conflito).

## Kubernetes
- Manifests em `k8s/`: `configmap.yaml`, `secret.yaml`, `deployment.yaml`, `service.yaml`.
- Ajuste a imagem no Deployment antes de aplicar.

## CI/CD (GitHub Actions)
- Workflow: `.github/workflows/docker-publish.yml`
- O pipeline compila e publica a imagem Docker nas seguintes situações:
  - Push na branch `main`/`master` (gera a tag `latest` e tag de branch)
  - Push de tags (`v*` ou `*.*.*`) — publica com o nome da tag
  - Execução manual via `workflow_dispatch`
- Configuração necessária no repositório (Settings → Secrets and variables → Actions):
  - `DOCKERHUB_USERNAME`: seu usuário do Docker Hub
  - `DOCKERHUB_TOKEN`: um Access Token do Docker Hub (ou sua senha — recomendado usar token)
- A imagem publicada segue o padrão: `docker.io/<DOCKERHUB_USERNAME>/drivesale-api:<tag>`
- O workflow usa `src/WebApi/Dockerfile` e cache de build do Docker para acelerar execuções.

## Solucao de problemas
- 28P01 (senha invalida):
  - `compose-down` (remove volumes) + `compose-up-build` para recriar banco com a senha do compose
  - Teste com Adminer (Server: `pgdb`) ou `psql` usando a mesma connection string
- Porta 5432 ocupada: altere a porta publicada no `docker-compose.yml` e atualize a connection string
- HTTPS no dev: projeto roda em HTTP no debug. Para HTTPS, gere/confie o certificado: `dotnet dev-certs https --trust`

## Arquitetura e Fluxo
![Class Diagram](docs/ClassDiagram.png)
![architecture](docs/architecture.jpg)
![architecture](docs/eks-deployment.jpg)

## Arquitetura

Visão geral (Clean Architecture)
- Domain: núcleo de negócio (Entidades, Value Objects e regras). Sem dependências externas.
- Application: orquestra casos de uso (MediatR Commands/Queries), validações de aplicação e portas (interfaces de Repositórios/UoW). Depende apenas de Domain.
- Infrastructure: detalhes técnicos (EF Core, DbContext, Repositórios, Migrations, UnitOfWork). Implementa interfaces da Application.
- WebApi: camada de entrada HTTP (Controllers, Swagger, DI, Health). Não contém regra de negócio.

Padrões e decisões
- MediatR para desacoplar Controllers de casos de uso (CQRS leve com Requests/Handlers).
- Repositórios e UoW na Infrastructure para persistência via EF Core (PostgreSQL).
- Value Object `Cpf` no domínio para garantir formato/validação.
- Migrações aplicadas no startup (Database.Migrate) apenas para ambientes controlados; em produção, preferir migrações por pipeline.
- Integração com pagamento via webhook idempotente. Envio de e‑mail não faz parte do escopo atual.

Camadas e dependências (alto nível)
- Usuário → WebApi (HTTP) → Application (Handlers)
- Application → Domain (regras e entidades)
- Application → Infrastructure (Repositórios/UoW) → Banco de Dados (EF Core)
- Application → Serviço externo de Pagamento (webhook para atualização de status)

Principais entidades e invariantes
- Vehicle: só editável quando status = disponível; ao vender, muda para vendido.
- Client: possui `Cpf` válido e e‑mail válido (campo de dados, sem envio).
- Sale: criada apenas com Vehicle disponível; calcula/preenche preço total.
- Payment: criado com Sale, inicia como pending; transita para paid/canceled via webhook.

Fluxo de venda (exemplo)
1) Criar `Client` e `Vehicle` via endpoints.
2) Criar `Sale`: gera `Payment` em status pending e retorna `paymentCode`.
3) Provedor de pagamento chama webhook: `/api/webhooks/payments/{paymentCode}` com `{ status: paid|canceled }`.
4) Application valida e atualiza `Payment` e `Sale`; se `paid`, o `Vehicle` muda para vendido.
5) Consultas: buscar `Sale` e listar `Vehicles` disponíveis/vendidos.

Persistência e mapeamento
- EF Core + Npgsql: `DbContext`, Configurations por entidade, Repositórios e UoW.
- Migração inicial incluída em `src/Infrastructure/Migrations/`.

Observabilidade e DX
- Swagger/OpenAPI habilitado em dev e compose.
- Health check simples em `/health`.

## Deployment (EKS)
- Recomendado: banco fora do cluster (RDS Postgres). A API recebe a connection string via Secret/ConfigMap e variáveis de ambiente no Deployment.
- Tráfego: ALB (Ingress) → Service (ClusterIP) → Deployment/Pods.
- Configuração: `ConfigMap` (não sensíveis) e `Secret` (credenciais); opcional, integração com Secrets Manager via External Secrets.
- Se optar por banco no cluster (menos comum): use chart (Bitnami Postgres) com StatefulSet + PVC e exponha Service interno; cuidar de backup/upgrade.
- Diagramas: `docs/architecture.drawio`, `docs/eks-deployment.drawio`.

## cURL/REST examples

Defina a base conforme o ambiente:
- VS Code (debug): `BASE=http://localhost:5000`
- Docker Compose: `BASE=http://localhost:8080`

PowerShell
```
$env:BASE="http://localhost:5000"
```

Bash
```
export BASE=http://localhost:5000
```

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

Criar veiculo
```
curl -s -X POST %BASE%/api/vehicles ^
  -H "Content-Type: application/json" ^
  -d "{\"brand\":\"Fiat\",\"model\":\"Argo\",\"year\":2022,\"color\":\"Prata\",\"price\":65000}"
```

Listar veiculos disponiveis / vendidos
```
curl -s %BASE%/api/vehicles?status=available
curl -s %BASE%/api/vehicles?status=sold
```

Obter veiculo por id
```
curl -s %BASE%/api/vehicles/{id}
```

Atualizar veiculo
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
