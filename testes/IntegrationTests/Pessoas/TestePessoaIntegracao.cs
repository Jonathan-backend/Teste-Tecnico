using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Domain.Entities;
using Backend.IntegrationTests.Support;

namespace Backend.IntegrationTests.Pessoas;

public class PessoaCascadeDeleteIntegrationTests : SqliteIntegrationTestBase
{
    [Fact]
    public async Task Deve_apagar_as_transacoes_quando_excluir_pessoa()
    {
        
        var pessoa = await PessoaService.CreateAsync(new CreatePessoaDto
        {
            Nome = "Pessoa cascata",
            DataNascimento = DateTime.Today.AddYears(-32),
        });

        var categoria = await CategoriaService.CreateAsync(new CreateCategoriaDto
        {
            Descricao = "Moradia",
            Finalidade = Categoria.EFinalidade.Despesa,
        });

        await TransacaoService.CreateAsync(new CreateTransacaoDto
        {
            Descricao = "Aluguel",
            Valor = 1200m,
            Data = DateTime.Today,
            Tipo = Transacao.ETipo.Despesa,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id,
        });

        Assert.Single(DbContext.Transacoes);

        
        await PessoaService.DeleteAsync(pessoa.Id);

        
        Assert.Empty(DbContext.Transacoes);
        Assert.Null(await DbContext.Pessoas.FindAsync(pessoa.Id));
    }
}
