# Documentacao de testes e bugs

Este documento foi escrito de forma simples, pensando em quem esta comecando na area de qualidade.

## Objetivo desta documentacao

Aqui explico:

- como rodar cada tipo de teste
- como a piramide de testes foi organizada
- quais bugs foram encontrados
- por que cada tipo de teste foi escolhido

## Como rodar cada tipo de teste

### 1. Testes unitarios

Os testes unitarios verificam partes pequenas do sistema, normalmente funcoes isoladas.

Comando:

```powershell
cd web
npm run test:unit
```

Quando usar:

- para validar regras pequenas
- para pegar erro rapido
- para ter feedback curto durante desenvolvimento

### 2. Testes de integracao

Os testes de integracao verificam se mais de uma parte funciona junto, por exemplo uma pagina com hooks, componentes e estados.

Comando:

```powershell
cd web
npm run test:integration
```

Quando usar:

- para validar fluxo de tela sem precisar abrir navegador real
- para testar integracao entre formulario, componente e estado

### 3. Testes E2E

Os testes E2E simulam o comportamento do usuario na aplicacao rodando de ponta a ponta.

Comando:

```powershell
cd web
npm run test:e2e
```

Esses testes foram configurados para:

- abrir o navegador
- mostrar a execucao na tela
- gerar video da execucao

Os videos e artefatos ficam em:

- `web/test-results`

### 4. Rodar apenas a suite de regras de negocio

Foi criada uma suite especifica para validar regras mais sensiveis do sistema.

Comando:

```powershell
cd web
npx playwright test tests/e2e/regras-negocio.spec.ts
```

Essa suite cobre:

- menor de idade nao pode registrar receita
- categoria so pode ser usada conforme sua finalidade
- exclusao em cascata de transacoes ao excluir pessoa

## Como a piramide de testes foi estruturada

A ideia da piramide de testes e esta:

### Base da piramide: testes unitarios

Ficam na base porque:

- sao mais rapidos
- custam menos para manter
- ajudam a encontrar erro pequeno cedo

### Meio da piramide: testes de integracao

Ficam no meio porque:

- validam combinacoes de componentes
- dao mais confianca que o unitario sozinho
- ainda sao mais baratos do que E2E

### Topo da piramide: testes E2E

Ficam no topo porque:

- sao mais lentos
- sao mais caros de manter
- mas mostram o sistema como o usuario usa de verdade

### Como isso ficou neste projeto

Estrutura escolhida:

- poucos testes E2E, focados em fluxo importante
- testes de integracao para comportamento de paginas/componentes
- testes unitarios para regras pequenas e utilitarios

Essa escolha foi feita para equilibrar:

- velocidade
- confianca
- custo de manutencao

## Bugs encontrados

### Bug 1. Regra de negocio invalida retorna erro 500

Regra que falhou:

- quando uma transacao viola uma regra de negocio esperada, a API nao deveria responder como erro interno

Exemplos:

- menor tentando registrar receita
- categoria de despesa sendo usada como receita

Problema observado:

- a API responde `500 Internal Server Error`

O ideal seria:

- responder `400` ou `422`
- devolver mensagem de validacao clara

Arquivo com detalhes:

- [BUG-001-regra-negocio-retorna-500.md](./bugs/BUG-001-regra-negocio-retorna-500.md)

### Bug 2. Excluir pessoa inexistente retorna sucesso

Regra que falhou:

- se a pessoa nao existe, a API deveria informar que nao encontrou o recurso

Problema observado:

- a API retorna `204 No Content`

O ideal seria:

- retornar `404 Not Found`

Arquivo com detalhes:

- [BUG-002-exclusao-pessoa-inexistente-retorna-204.md](./bugs/BUG-002-exclusao-pessoa-inexistente-retorna-204.md)

### Bug 3. Lista de transacoes nao mostra categoria e pessoa

Regra que falhou:

- a tela deveria mostrar os nomes da pessoa e da categoria da transacao

Problema observado:

- as colunas existem, mas podem aparecer vazias

Arquivo com detalhes:

- [BUG-003-lista-transacoes-sem-categoria-e-pessoa.md](./bugs/BUG-003-lista-transacoes-sem-categoria-e-pessoa.md)

### Bug 4. Filtro de categoria por tipo nao funciona

Regra que falhou:

- ao escolher `receita` ou `despesa`, o formulario deveria ajudar o usuario mostrando categorias compativeis

Problema observado:

- o frontend parece preparado para isso, mas a integracao nao funciona de verdade

Arquivo com detalhes:

- [BUG-004-filtro-categoria-por-tipo-nao-funciona.md](./bugs/BUG-004-filtro-categoria-por-tipo-nao-funciona.md)

## Justificativa das escolhas de testes

### Por que nao testar tudo so com E2E?

Porque E2E:

- demora mais
- quebra com mais facilidade por detalhe visual
- custa mais para manter

Entao ele foi usado so no que realmente importa para o negocio.

### Por que criar uma suite separada para regras de negocio?

Porque essas regras sao muito importantes e merecem um foco proprio.

Exemplos:

- menor de idade e receita
- categoria compativel com tipo
- exclusao em cascata

Separar isso ajuda porque:

- facilita validar regra critica sem rodar tudo
- melhora a leitura do objetivo do teste
- ajuda o time de QA a saber exatamente o que esta protegido

### Por que usar UI e API juntos em alguns testes?

Em alguns cenarios, validar somente pela tela pode ser fraco por causa de:

- paginacao
- ordenacao
- listas que mudam de pagina

Entao foi usada a API do proprio Playwright em alguns casos para confirmar persistencia e regra de negocio de forma mais confiavel.

Isso nao enfraquece o teste. Na verdade, deixa o teste mais estavel quando o objetivo e validar regra, e nao layout.

### Quando eu escolheria cada tipo de teste como QA junior?

Uma forma simples de pensar:

- teste unitario: quando quero validar uma regra pequena
- teste de integracao: quando quero validar uma tela ou componente com dependencias
- teste E2E: quando quero validar o fluxo do usuario do comeco ao fim

## Resumo final

O objetivo nao foi criar a maior quantidade de testes.

O objetivo foi criar testes que deem confianca real:

- cobrindo fluxo principal
- cobrindo regras importantes
- registrando bugs encontrados com clareza

Se voce estiver estudando QA, pense assim:

- primeiro entenda o risco
- depois escolha o menor teste que consegue provar esse risco
- use E2E quando for realmente necessario
