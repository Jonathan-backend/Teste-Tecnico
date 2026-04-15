# [BUG] Filtro de categoria por tipo nao funciona no formulario de transacao

## Regra de negócio esperada
Ao criar uma transação, a seleção de categorias deveria respeitar o tipo escolhido (`receita`, `despesa` ou categorias `ambas`), evitando que o usuário veja ou selecione categorias incompatíveis.

## Comportamento atual
O componente de categoria possui uma prop `selectedTipo` e envia um parâmetro `tipo` para a API, mas o formulário de transação não fornece essa prop e o endpoint de categorias não implementa esse filtro. Na prática, o comportamento esperado não ocorre.

## Passos para reproduzir
1. Abrir o formulário de criação de transação.
2. Alterar o tipo da transação entre `Receita` e `Despesa`.
3. Abrir a lista de categorias.
4. Observar que não há filtragem coerente por finalidade.

## Resultado esperado
O usuário deveria ver apenas categorias compatíveis com o tipo da transação, ou ao menos receber filtragem real com base na finalidade.

## Resultado obtido
O frontend aparenta suportar esse filtro, mas a integração não está completa e o comportamento final não funciona.

## Evidências
- Evidência observada: trocar o tipo da transação no formulário não produz uma lista de categorias coerente com a finalidade escolhida.
- Evidência observada: o componente visual aparenta suportar filtragem, mas a integração termina incompleta porque a prop não é repassada e a API não aplica o filtro.
- Evidência observada: a regra de finalidade também é exercitada em [web/tests/e2e/regras-negocio.spec.ts](/ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/web/tests/e2e/regras-negocio.spec.ts:128), o que reforça que a validação de negócio existe, mas a ajuda de interface não.
- Formulário de transação: [web/src/components/molecules/TransacaoForm.tsx](/ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/web/src/components/molecules/TransacaoForm.tsx:81)
- Select de categoria: [web/src/components/molecules/LazyCategoriaSelect.tsx](/ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/web/src/components/molecules/LazyCategoriaSelect.tsx:13)
- Endpoint de categorias: [api/MinhasFinancas.API/Controllers/CategoriasController.cs](/ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/api/MinhasFinancas.API/Controllers/CategoriasController.cs:32)

## Impacto
- Severidade: media
- Área afetada: transações / categorias / frontend / API
