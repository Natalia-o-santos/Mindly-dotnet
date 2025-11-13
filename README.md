# Mindly – Gestão Inteligente de Foco e Bem-Estar

## Integrantes do Projeto

- **Natália Santos** – RM560306
- **Alex Ribeiro** – RM557356 
- **Felipe Damasceno** – RM559433 

## Visão Geral

O Mindly é uma API ASP.NET Core voltada para profissionais remotos, híbridos, estudantes e freelancers que precisam equilibrar produtividade e saúde mental. A plataforma organiza ciclos de foco e pausas usando princípios como Pomodoro, integrações IoT simuladas e monitoramento de hábitos. O projeto foi estruturado seguindo uma arquitetura limpa para facilitar evolução, testes e integração futura com front-ends ou automações.

## Arquitetura e Decisões

- **Camada de Domínio (`Mindly.Domain`)** – Entidades ricas (`UserProfile`, `FocusSession`, `BreakPeriod`, `DeviceSignal`) encapsulam invariantes de negócio (ex.: duração positiva, limites de pausas, regras de transição de status).
- **Camada de Aplicação (`Mindly.Application`)** – Serviços orquestram casos de uso, expõem DTOs, validam inputs e convertem erros em exceções específicas (`AppException`, `NotFoundException`, etc.).
- **Infraestrutura (`Mindly.Infrastructure`)** – EF Core + SQLite, repositórios concretos, migrations e `DataSeeder` com cenários de exemplo para IoT.
- **API (`Mindly.Api`)** – Controllers REST, ProblemDetails padronizado, links HATEOAS e endpoint `/search` com paginação, ordenação e filtros.
- **Injeção de dependências** – Extensões `AddApplication` e `AddInfrastructure` garantem acoplamento mínimo entre camadas.
- **Observabilidade** – Middleware único captura exceções de negócio, validações e falhas inesperadas gerando respostas `application/problem+json`.

### Estrutura do Repositório

```
Mindly-dotnet/
├─ src/
│  ├─ Mindly.Domain/           # Entidades, enums, value objects, contratos
│  ├─ Mindly.Application/      # Casos de uso, DTOs, mapeamentos, exceções
│  ├─ Mindly.Infrastructure/   # DbContext, migrations, repositórios, seed
│  └─ Mindly.Api/              # Program, controllers, HATEOAS, middleware
├─ Mindly.sln
└─ README.md
```

## Pré-Requisitos

- .NET SDK **8.0** ou superior (`dotnet --version`).
- (Opcional) CLI do EF Core já incluída no SDK (`dotnet ef`).

## Como Executar

1. **Restaurar dependências**
   ```bash
   dotnet restore
   ```

2. **Aplicar migrations (cria o SQLite `mindly.db` em `src/Mindly.Api/`)**
   ```bash
   dotnet ef database update --project src/Mindly.Infrastructure --startup-project src/Mindly.Api
   ```

3. **Executar a API**
   ```bash
   dotnet run --project src/Mindly.Api
   ```
   - Swagger disponível automaticamente em `https://localhost:7113/` (ou `http://localhost:5122/`).
   - Ao rodar pelo Rider/VS, o navegador abre na raiz e o Swagger é servido diretamente em `/`.
   - A inicialização executa o `DataSeeder`, inserindo perfis e sessões de exemplo.

### Variáveis e Configuração

- Connection string padrão (`appsettings*.json`):
  ```json
  "ConnectionStrings": {
    "Mindly": "Data Source=mindly.db"
  }
  ```
- Para customizar, use variável de ambiente `ConnectionStrings__Mindly`, apontando para outro arquivo SQLite ou provedor.

### Script SQL (Opcional)

- Se necessário, você pode gerar um script SQL a partir das migrations usando:
  ```bash
  dotnet ef migrations script --project src/Mindly.Infrastructure --startup-project src/Mindly.Api --output script.sql
  ```
  Isso gera um arquivo SQL com todo o schema da solução, útil para auditoria ou execução manual do banco.

### Arquivo HTTP (Opcional)

- O arquivo `src/Mindly.Api/Mindly.Api.http` pode ser usado com a extensão REST Client do VS Code/Rider para testar os endpoints diretamente do editor.

## Casos de Uso Principais

- Cadastro, atualização e remoção de perfis (`/api/users`).
- Agendamento, início, conclusão e cancelamento de sessões de foco (`/api/focus-sessions`).
- Registro de pausas, sinais IoT simulados e histórico de métricas de sessão.
- Busca avançada (`/api/focus-sessions/search`) com filtros por usuário, modo de foco, status, intervalos de data, ordenação e paginação.

## Endpoints & Exemplos

### Usuários (`/api/users`)

```bash
# Criar usuário
curl -X POST https://localhost:7113/api/users \
  -H "Content-Type: application/json" \
  -d '{
        "name": "Ana Souza",
        "email": "ana@mindly.dev",
        "workMode": 1,
        "dailyFocusGoalMinutes": 240
      }'

# Buscar usuário por ID
curl -X GET https://localhost:7113/api/users/{id}

# Atualizar usuário
curl -X PUT https://localhost:7113/api/users/{id} \
  -H "Content-Type: application/json" \
  -d '{
        "name": "Ana Silva",
        "email": "ana.silva@mindly.dev",
        "workMode": 2,
        "dailyFocusGoalMinutes": 300
      }'

# Deletar usuário
curl -X DELETE https://localhost:7113/api/users/{id}
```

### Sessões de Foco (`/api/focus-sessions`)

```bash
# Criar sessão
curl -X POST https://localhost:7113/api/focus-sessions \
  -H "Content-Type: application/json" \
  -d '{
        "userProfileId": "GUID_DO_USUARIO",
        "title": "Sprint Pomodoro",
        "description": "Execução de estórias críticas",
        "focusMode": 1,
        "plannedDurationMinutes": 50,
        "plannedStart": "2025-11-12T14:00:00Z"
      }'

# Buscar sessão por ID
curl -X GET https://localhost:7113/api/focus-sessions/{id}

# Iniciar sessão
curl -X PUT https://localhost:7113/api/focus-sessions/{id}/start \
  -H "Content-Type: application/json" \
  -d '{
        "startedAt": "2025-11-12T14:00:00Z"
      }'

# Completar sessão
curl -X PUT https://localhost:7113/api/focus-sessions/{id}/complete \
  -H "Content-Type: application/json" \
  -d '{
        "completedAt": "2025-11-12T14:50:00Z",
        "actualDurationMinutes": 50
      }'

# Cancelar sessão
curl -X PUT https://localhost:7113/api/focus-sessions/{id}/cancel \
  -H "Content-Type: application/json" \
  -d '{
        "reason": "Conflito de agenda"
      }'

# Adicionar pausa
curl -X POST https://localhost:7113/api/focus-sessions/{id}/breaks \
  -H "Content-Type: application/json" \
  -d '{
        "breakType": 1,
        "durationMinutes": 10,
        "startedAt": "2025-11-12T14:25:00Z",
        "notes": "Pausa para hidratação"
      }'

# Registrar sinal de dispositivo IoT
curl -X POST https://localhost:7113/api/focus-sessions/{id}/device-signals \
  -H "Content-Type: application/json" \
  -d '{
        "deviceName": "MindlyLamp",
        "signalType": "ambient-light",
        "payload": "{ \"level\": \"focus\" }",
        "recordedAt": "2025-11-12T14:00:00Z"
      }'

# Buscar sessões com filtros, ordenação e paginação
curl "https://localhost:7113/api/focus-sessions/search?userProfileId=GUID_DO_USUARIO&pageNumber=1&pageSize=5&focusMode=1&status=2&sortBy=plannedStart&descending=true"

# Deletar sessão
curl -X DELETE https://localhost:7113/api/focus-sessions/{id}
```

**Parâmetros de busca (`/api/focus-sessions/search`):**
- `userProfileId` (Guid?): Filtrar por usuário
- `startFrom` (DateTimeOffset?): Data inicial
- `startTo` (DateTimeOffset?): Data final
- `focusMode` (FocusMode?): Modo de foco (0=DeepWork, 1=Pomodoro, 2=TimeBlocking)
- `status` (SessionStatus?): Status da sessão (0=Planned, 1=InProgress, 2=Completed, 3=Cancelled)
- `keyword` (string?): Busca por título ou descrição
- `sortBy` (string?): Campo de ordenação (`plannedstart`, `createdat`, `title`, `status`)
- `descending` (bool): Ordem decrescente (padrão: true)
- `pageNumber` (int): Número da página (padrão: 1)
- `pageSize` (int): Tamanho da página (padrão: 10)

As respostas retornam objetos **Resource** com `links` (`self`, `start`, `complete`, `cancel`, `add-break`, `record-device-signal`, `delete`, etc.) permitindo navegação HATEOAS. As respostas paginadas incluem links `self`, `previous` e `next`.

## Regras de Negócio Implementadas

- Duração de sessões e pausas deve ser positiva; somatório de pausas não pode exceder a duração planejada.
- Transições de status controladas (`Planned → InProgress → Completed / Cancelled`).
- Impede duplicidade de perfis pelo e-mail.
- Sinais IoT não são registrados para sessões canceladas.
- Validações em DTOs + ProblemDetails para erros de modelo e domínios.

## Testes e Próximos Passos

- O projeto está pronto para incluir testes (unitários e integração) usando `xUnit`/`Moq` ou `WebApplicationFactory`.
- Possíveis evoluções:
  - Autenticação/JWT para perfis.
  - Dashboard com métricas agregadas.
  - Integração real com dispositivos via WebSockets.

## Logs e Monitoramento

- Middleware centralizado gera ProblemDetails consistentes.
- Logging configurado via `appsettings`; ajuste os níveis conforme necessidade.

## Referências Rápidas

- **Aplicar migrations:** `dotnet ef database update --project src/Mindly.Infrastructure --startup-project src/Mindly.Api`
- **Executar API:** `dotnet run --project src/Mindly.Api`
- **Gerar script SQL (opcional):** `dotnet ef migrations script --project src/Mindly.Infrastructure --startup-project src/Mindly.Api --output script.sql`

## Tecnologias Utilizadas

- **.NET 8.0** – Framework principal
- **ASP.NET Core Web API** – Camada de apresentação
- **Entity Framework Core** – ORM para acesso a dados
- **SQLite** – Banco de dados
- **Swagger/OpenAPI** – Documentação da API

---

Desenvolvido no contexto da trilha **Advanced Business Development with .NET**, priorizando boas práticas arquiteturais, validações robustas e experiência de uso focada em produtividade e bem-estar.

