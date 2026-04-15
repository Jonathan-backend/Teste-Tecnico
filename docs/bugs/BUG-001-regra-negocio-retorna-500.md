# [BUG] Regra de negocio de transacao invalida retorna 500

## Regra de negócio esperada
Violações de regra de negócio previsíveis, como menor de idade com receita ou categoria incompatível com o tipo da transação, devem retornar erro de validação para o cliente, com status `400` ou `422`, e mensagem clara.

## Comportamento atual
Quando a entidade `Transacao` detecta uma violação de regra de negócio, ela lança `InvalidOperationException`. O controller de transações não trata essa exceção e o middleware global converte o caso em `500 Internal Server Error`.

## Passos para reproduzir
1. Criar uma pessoa menor de 18 anos.
2. Criar ou reutilizar uma categoria de receita.
3. Tentar criar uma transação de receita para essa pessoa.
4. Observar a resposta da API.

## Resultado esperado
A API deveria rejeitar a operação com erro de negócio controlado, preservando a mensagem da regra violada e retornando um status de cliente inválido.

## Resultado obtido
A requisição falha com `500 Internal Server Error`, embora o caso seja uma violação funcional esperada e não uma falha interna do servidor.

## Evidências
- Código de domínio: [api/MinhasFinancas.Domain/Entities/Transacao.cs](/abs/path/c:/Users/lordg/Downloads/ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/api/MinhasFinancas.Domain/Entities/Transacao.cs:69)
- Controller: [api/MinhasFinancas.API/Controllers/TransacoesController.cs](/abs/path/c:/Users/lordg/Downloads/ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/api/MinhasFinancas.API/Controllers/TransacoesController.cs:57)
- Middleware global: [api/MinhasFinancas.API/Middlewares/ExceptionMiddleware.cs](/abs/path/c:/Users/lordg/Downloads/ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/api/MinhasFinancas.API/Middlewares/ExceptionMiddleware.cs:39)
- Teste automatizado relacionado: `web/tests/e2e/regras-negocio.spec.ts`

## Impacto
- Severidade: alta
- Área afetada: transações / API / regras de negócio
