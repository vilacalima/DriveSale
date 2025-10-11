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
- Migrações: migration inicial incluÃ­da e aplicada automaticamente no startup (`Database.Migrate()`).
- Docker Compose: Serviços `pgdb` (Postgres), `adminer` (UI) e `api` (WebApi container).
- Kubernetes: manifests prontos (ConfigMap, Secret, Deployment, Service).

## Estrutura do repositÃ³rio
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

Opção A - VS Code (API no host)
- Requisitos: .NET SDK 9, Docker (para subir Postgres com Compose)
- Passos:
  - VS Code â†’ Run Task â†’ `compose-up` ou `compose-up-build`
  - F5 (perfil ".NET Launch WebApi")
  - Swagger: `http://localhost:5000/swagger`
  - Connection string do debug: `.vscode/launch.json` (ajuste se necessÃ¡rio)

Opção B -  Docker Compose (API + Postgres no Docker)
- `docker compose up -d --build` ou VS Code â†’ Run Task â†’ `compose-up-build`
- API: `http://localhost:8080/swagger`
- Adminer: `http://localhost:8081` (System: PostgreSQL, Server: `pgdb`, User: `postgres`, Password: `postgres`, Database: `fase2`)

Tarefas Ãºteis (VS Code â†’ Run Task)
- Compose: `compose-up`, `compose-up-recreate`, `compose-up-build`, `compose-down`
- EF: `ef-tool-install`, `ef-migrations-add`, `ef-database-update`, `ef-add-and-update`

## Banco de dados e Migrações
- HÃ¡ uma migration inicial em `src/Infrastructure/Migrations/*` aplicada no startup.
- Para criar novas Migrações:
  - Task: `ef-migrations-add` (informe o nome) e depois `ef-database-update`, ou
  - Script: `scripts/ef-add-update.ps1 -InstallTool -Name MinhaMigration`

## Endpoints principais
Base: `/api`

veículos
- POST `/vehicles` â€” cria veÃ­culo
- PUT `/vehicles/{id}` â€” edita (somente se disponÃ­vel)
- GET `/vehicles?status=available|sold` â€” lista por preço (asc)
- GET `/vehicles/{id}` â€” detalhe

Clientes
- POST `/clients` â€” cria cliente (CPF vÃ¡lido)
- PUT `/clients/{id}` â€” atualiza nome/email
- GET `/clients/{id}` â€” detalhe

Vendas
- POST `/sales` â€” cria venda + payment pendente
  - Body: `{ vehicleId, buyerCpf, saleDate? }`
  - Retorno: `{ id, totalPrice, paymentCode, paymentStatus }`
- GET `/sales/{id}` â€” detalhe

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
  "name": "JoÃ£o Silva",
  "email": "joao@exemplo.com",
  "cpf": "111.444.777-35"
}
```

2) Cadastrar veÃ­culo
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
  "vehicleId": "<id do veÃ­culo>",
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

ObservaÃ§Ãµes
- ValidaÃ§Ãµes de domÃ­nio: CPF vÃ¡lido; nÃ£o editar veÃ­culo vendido; venda somente com veÃ­culo disponÃ­vel; webhook idempotente.
- HTTP: 201 (criado), 202 (webhook), 204 (update), 400/422 (validaÃ§Ã£o), 404 (nÃ£o encontrado), 409 (conflito).

## Kubernetes
- Manifests em `k8s/`: `configmap.yaml`, `secret.yaml`, `deployment.yaml`, `service.yaml`.
- Ajuste a imagem no Deployment antes de aplicar.

## SoluÃ§Ã£o de problemas
- 28P01 (senha invÃ¡lida):
  - `compose-down` (remove volumes) â†’ `compose-up-build` para recriar banco com a senha do compose
  - Teste com Adminer (Server: `pgdb`) ou `psql` usando a mesma connection string
- Porta 5432 ocupada: altere a porta publicada no `docker-compose.yml` e atualize a connection string
- HTTPS no dev: projeto roda em HTTP no debug. Para HTTPS, gere/confie o certificado: `dotnet dev-certs https --trust`

## Arquitetura e Fluxo
```
```

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
  -d "{\"name\":\"JoÃ£o Silva\",\"email\":\"joao@exemplo.com\",\"cpf\":\"111.444.777-35\"}"
```

Obter cliente por id
```
curl -s %BASE%/api/clients/{id}
```

Atualizar cliente
```
curl -s -X PUT %BASE%/api/clients/{id} ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"JoÃ£o Atualizado\",\"email\":\"joao@exemplo.com\"}"
```

Criar veÃ­culo
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

Obter veÃ­culo por id
```
curl -s %BASE%/api/vehicles/{id}
```

Atualizar veÃ­culo
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


