# Postman

Arquivos prontos:
- Collection: `postman/DriveSale.postman_collection.json`
- Environment: `postman/DriveSale.postman_environment.json`

Como usar:
- Importe os dois arquivos no Postman.
- Ajuste a variável `baseUrl` no ambiente conforme onde a API estiver rodando:
  - Debug VS Code: `http://localhost:5000`
  - Docker Compose: `http://localhost:8080`
  - Padrão do arquivo: `http://localhost:5261`
- Rode a pasta "E2E Sale Flow" no Collection Runner.

Fluxo automatizado (com testes):
- Cria cliente (usa CPF válido `52998224725`), captura `clientId`.
- Cria veículo, captura `vehicleId`.
- Busca veículo por id e valida retorno 200.
- Cria venda, captura `saleId` e `paymentCode`.
- Dispara webhook de pagamento `paid` (202 Accepted).
- Busca venda por id e valida `payment.status == 1` (Paid).
- Lista veículos `sold` e valida que contém o `vehicleId` criado.

Variáveis de ambiente usadas:
- `baseUrl`, `cpf`, `clientId`, `vehicleId`, `saleId`, `paymentCode`.

VS Code Task
- Use o comando: Tasks: Run Task
- Escolha uma das tasks criadas:
- `postman:install-newman` — instala o Newman globalmente (caso não tenha).
- `postman:e2e:docker` — executa a collection contra `http://localhost:8080` (Docker Compose).
- `postman:e2e:debug` — executa a collection contra `http://localhost:5000` (debug local).

Observações
- As tasks chamam `newman.cmd` para evitar bloqueio por ExecutionPolicy do PowerShell.
- Certifique-se de que a API está acessível no `baseUrl` escolhido (ex.: `docker compose up -d --build`).
