# Mindly – Gestão Inteligente de Foco e Bem-Estar

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
├─ script.sql                  # Script gerado pelas migrations
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

### Script SQL

- O arquivo `script.sql` é gerado via `dotnet ef migrations script ...` e contém todo o schema da solução. Útil para auditoria ou execução manual do banco.

## Casos de Uso Principais

- Cadastro, atualização e remoção de perfis (`/api/users`).
- Agendamento, início, conclusão e cancelamento de sessões de foco (`/api/focus-sessions`).
- Registro de pausas, sinais IoT simulados e histórico de métricas de sessão.
- Busca avançada (`/api/focus-sessions/search`) com filtros por usuário, modo de foco, status, intervalos de data, ordenação e paginação.

## Endpoints & Exemplos

### Usuários

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
```

### Sessões

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

# Buscar com filtros
curl "https://localhost:7113/api/focus-sessions/search?userProfileId=GUID_DO_USUARIO&pageNumber=1&pageSize=5&focusMode=1&sortBy=plannedStart&descending=true"
```

As respostas retornam objetos **Resource** com `links` (`self`, `start`, `complete`, `add-break`, etc.) permitindo navegação HATEOAS.

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
- **Gerar script SQL:** `dotnet ef migrations script --project src/Mindly.Infrastructure --startup-project src/Mindly.Api --output script.sql`
- **Executar API:** `dotnet run --project src/Mindly.Api`

---

Desenvolvido no contexto da trilha **Advanced Business Development with .NET**, priorizando boas práticas arquiteturais, validações robustas e experiência de uso focada em produtividade e bem-estar.

