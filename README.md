# Entrega de Testes - Minhas Financas

Este repositório contem apenas os testes automatizados, a documentacao de bugs encontrados e o workflow de CI.

O codigo da aplicacao nao foi alterado. Para executar os testes, use o codigo-fonte original fornecido no enunciado e apliquei a esta estrutura de arquivos sobre ele, preservando os mesmos caminhos de pasta.

## Objetivo do teste

Esta entrega foi preparada para atender ao que o enunciado solicita:

1. Entender regras de negocio a partir do codigo existente.
2. Projetar e implementar uma piramide de testes adequada.
3. Identificar e documentar falhas de implementacao por meio de testes.
4. Aplicar boas praticas de testes automatizados em .NET e React/TypeScript.

## Escopo funcional validado

O sistema avaliado contem:

- CRUD de Pessoas
- Cadastro de Categorias
- Cadastro de Transacoes
- Consultas de totais por pessoa

As regras de negocio priorizadas nos testes foram:

- menor de idade nao pode ter receitas
- categoria so pode ser usada conforme sua finalidade
- exclusao em cascata de transacoes ao excluir pessoa
- consistencia das consultas de totais por pessoa

## Tecnologias utilizadas

- Back-end: C#, .NET e xUnit
- Front-end: React, TypeScript, Vitest e Playwright

## O que foi construido

### 1. Testes Unitarios

Back-end em `testes/UnitTests`:

- `Categorias/TesteCategoria.cs`
- `Pessoas/TestePessoa.cs`
- `Transacoes/TesteTransacao.cs`

Front-end unitario em `web/src/lib`:

- `formatters.test.ts`

### 2. Testes de Integracao

Back-end em `testes/IntegrationTests`:

- `Pessoas/TestePessoaIntegracao.cs`
- `Totais/TesteTotaisIntegracao.cs`
- `Transacoes/TesteTransacaoIntegracao.cs`
- `Support/SqliteIntegrationTestBase.cs`

Front-end de integracao em `web/src/pages`:

- `PessoasList.integration.test.tsx`

### 3. Testes End-to-End

Em `web/tests/e2e`:

- `cadastro.spec.ts`
- `navegacao.spec.ts`
- `regras-negocio.spec.ts`
- `transacao.spec.ts`

### 4. CI GitHub Actions

Workflow em `.github/workflows/tests.yml`

## Como rodar cada tipo de teste

Antes de executar:

1. Obtenha o codigo-fonte original da aplicacao a partir do link do enunciado.
2. Copie os arquivos deste repositório para dentro da estrutura original, mantendo os mesmos caminhos.
3. Garanta os pre-requisitos:
   - .NET 9 SDK
   - Node.js 22+
   - npm

### Testes unitarios de back-end

```bash
dotnet test testes/UnitTests/Backend.UnitTests.csproj
```

### Testes de integracao de back-end

```bash
dotnet test testes/IntegrationTests/Backend.IntegrationTests.csproj
```

### Testes unitarios de front-end

```bash
cd web
npm install
npm run test:unit
```

### Testes de integracao de front-end

```bash
cd web
npm install
npm run test:integration
```

### Testes E2E

Os E2E assumem:

- API em `http://localhost:5000`
- Front-end em `http://localhost:5173`

```bash
cd web
npm install
npx playwright install
npm run test:e2e
```

## Como a piramide de testes foi estruturada

### Base

Os testes unitarios foram usados para validar regras pequenas e comportamentos isolados, com feedback rapido e manutencao mais simples.

### Meio

Os testes de integracao validam a conversa entre servicos, persistencia, SQLite em memoria, pagina, hooks e componentes mais proximos do uso real.

### Topo

Os testes E2E foram reservados para fluxos mais importantes do usuario e para regras de negocio com maior risco funcional.

## Bugs encontrados

Os bugs documentados estao em `docs/bugs`:

- `BUG-001-regra-negocio-retorna-500.md`
- `BUG-002-exclusao-pessoa-inexistente-retorna-204.md`
- `BUG-003-lista-transacoes-sem-categoria-e-pessoa.md`
- `BUG-004-filtro-categoria-por-tipo-nao-funciona.md`

Resumo:

- regra de negocio previsivel retornando `500 Internal Server Error`
- exclusao de pessoa inexistente retornando `204 No Content`
- lista de transacoes sem exibir pessoa e categoria corretamente
- filtro de categoria por tipo nao concluido de ponta a ponta

## Justificativa das escolhas de testes

- Os testes priorizam regra de negocio em vez de cobertura total, conforme o enunciado.
- A separacao entre unitario, integracao e E2E foi feita para equilibrar velocidade, confianca e custo de manutencao.
- Bugs foram documentados em Markdown com reproducao, esperado, obtido e evidencias.
- O codigo da aplicacao foi preservado, respeitando a restricao explicita do teste.

## Estrutura da entrega

```text
.github/workflows/tests.yml
docs/README.md
docs/bugs/*.md
testes/UnitTests/*
testes/IntegrationTests/*
web/src/**/*.test.ts
web/src/**/*.integration.test.tsx
web/tests/e2e/*.spec.ts
web/package.json
web/package-lock.json
web/playwright.config.ts
web/vitest.config.ts
web/tsconfig*.json
```

## Observacao final

Esta entrega foi organizada para que o repositorio publicado contenha somente testes, documentacao e arquivos de suporte a execucao, em conformidade com o enunciado.
