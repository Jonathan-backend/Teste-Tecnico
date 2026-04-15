# [BUG] Exclusao de pessoa inexistente retorna 204

## Regra de negócio esperada
Ao solicitar a exclusão de uma pessoa inexistente, a API deve informar que o recurso não foi encontrado, retornando `404 Not Found`.

## Comportamento atual
O controller espera capturar `KeyNotFoundException`, mas o serviço e o repositório ignoram silenciosamente a ausência da entidade. Como consequência, a API responde `204 No Content` mesmo quando o ID informado não existe.

## Passos para reproduzir
1. Enviar `DELETE` para `/api/v1.0/pessoas/{id}` com um GUID inexistente.
2. Observar o status retornado.

## Resultado esperado
A API deveria retornar `404 Not Found`, sinalizando corretamente que a pessoa não existe.

## Resultado obtido
A API retorna `204 No Content`, indicando sucesso em uma exclusão que não ocorreu.

## Evidências
- Controller: [api/MinhasFinancas.API/Controllers/PessoasController.cs](/abs/path/c:/Users/lordg/Downloads/ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/api/MinhasFinancas.API/Controllers/PessoasController.cs:104)
- Serviço: [api/MinhasFinancas.Application/Services/PessoaService.cs](/abs/path/c:/Users/lordg/Downloads/ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/api/MinhasFinancas.Application/Services/PessoaService.cs:115)
- Repositório base: [api/MinhasFinancas.Infrastructure/Repositories/RepositoryBase.cs](/abs/path/c:/Users/lordg/Downloads/ExameDesenvolvedorDeTestes/ExameDesenvolvedorDeTestes/api/MinhasFinancas.Infrastructure/Repositories/RepositoryBase.cs:116)

## Impacto
- Severidade: media
- Área afetada: pessoas / API
