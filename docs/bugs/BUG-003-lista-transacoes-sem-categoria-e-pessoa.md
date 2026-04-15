# [BUG] Lista de transacoes nao exibe categoria e pessoa

## Regra de negócio esperada
A tela de transações deve exibir os nomes da categoria e da pessoa associados a cada item, conforme as colunas da grade.

## Comportamento atual
O backend devolve `CategoriaDescricao` e `PessoaNome`, mas o hook do frontend descarta esses campos ao mapear a resposta. A página tenta renderizar colunas que não existem no objeto final usado pela tabela.

## Passos para reproduzir
1. Criar uma transação vinculada a uma pessoa e uma categoria.
2. Abrir a tela `/transacoes`.
3. Observar as colunas `Categoria` e `Pessoa`.

## Resultado esperado
As colunas deveriam exibir os nomes legíveis da categoria e da pessoa.

## Resultado obtido
As colunas tendem a aparecer vazias, mesmo com a API já trazendo esses dados.

## Evidências
- Hook de mapeamento: [web/src/hooks/useTransacoes.ts](/abs/path/c:/Users/lordg/Downloads/ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/web/src/hooks/useTransacoes.ts:19)
- Página de transações: [web/src/pages/TransacoesList.tsx](/abs/path/c:/Users/lordg/Downloads/ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/web/src/pages/TransacoesList.tsx:43)

## Impacto
- Severidade: media
- Área afetada: transações / frontend
