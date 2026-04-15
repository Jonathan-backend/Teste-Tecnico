# Piramide de Testes

Esta pasta concentra a estrategia de testes implementada para a aplicacao `Minhas Financas`.

## Cobertura criada

- Unitarios em `testes/UnitTests`
- Integracao de backend com `Application + Infrastructure + SQLite em memoria` em `testes/IntegrationTests`
- Frontend unitario e de integracao com `Vitest` em `web/src`
- End-to-End com `Playwright` em `web/tests/e2e`
- CI em `.github/workflows/tests.yml`

## Organizacao dos arquivos

- Os nomes dos arquivos foram deixados simples, como `TestePessoa.cs`, `TesteTransacaoIntegracao.cs` e `cadastro.spec.ts`
- A ideia foi manter uma estrutura facil de entender, com menos abstrações e mais foco na leitura direta

## Regras cobertas

1. Menor de idade nao pode registrar receita.
2. Categoria precisa respeitar a finalidade da transacao.
3. Exclusao de pessoa deve remover transacoes relacionadas.
4. Totais por pessoa precisam bater com as transacoes persistidas.

## Como rodar

### Backend

```bash
dotnet test testes/UnitTests/Backend.UnitTests.csproj
dotnet test testes/IntegrationTests/Backend.IntegrationTests.csproj
```

### Frontend

```bash
cd web
npm install
npm run test:unit
npm run test:integration
npx playwright install
npm run test:e2e
```

## Observacoes

- Os testes de integracao usam SQLite em memoria para evitar acoplamento com banco externo.
- Os E2E assumem a API rodando em `http://localhost:5000` com Swagger em `http://localhost:5000/swagger/index.html` e o frontend em `http://localhost:5173`.
- O workflow do GitHub Actions sobe a API, roda a aplicacao web e executa toda a piramide automaticamente.
