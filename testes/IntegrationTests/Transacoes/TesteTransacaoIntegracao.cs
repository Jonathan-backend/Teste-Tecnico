using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Domain.Entities;
using Backend.IntegrationTests.Support;

namespace Backend.IntegrationTests.Transacoes;

public class TransacaoRulesIntegrationTests : SqliteIntegrationTestBase
{
    [Fact]
    public async Task Deve_salvar_transacao_valida()
    {
        
        var pessoa = await PessoaService.CreateAsync(new CreatePessoaDto
        {
            Nome = "Ana",
            DataNascimento = DateTime.Today.AddYears(-27),
        });

        var categoria = await CategoriaService.CreateAsync(new CreateCategoriaDto
        {
            Descricao = "Freelance",
            Finalidade = Categoria.EFinalidade.Receita,
        });

        var transacao = await TransacaoService.CreateAsync(new CreateTransacaoDto
        {
            Descricao = "Projeto",
            Valor = 450m,
            Data = DateTime.Today,
            Tipo = Transacao.ETipo.Receita,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id,
        });

        var persisted = await DbContext.Transacoes.FindAsync(transacao.Id);

        
        Assert.NotNull(persisted);
        Assert.Equal(450m, persisted!.Valor);
    }

    [Fact]
    public async Task Nao_deve_deixar_menor_cadastrar_receita()
    {
        
        var pessoa = await PessoaService.CreateAsync(new CreatePessoaDto
        {
            Nome = "Menor",
            DataNascimento = DateTime.Today.AddYears(-15),
        });

        var categoria = await CategoriaService.CreateAsync(new CreateCategoriaDto
        {
            Descricao = "Bolsa",
            Finalidade = Categoria.EFinalidade.Receita,
        });

        
        await Assert.ThrowsAsync<InvalidOperationException>(() => TransacaoService.CreateAsync(new CreateTransacaoDto
        {
            Descricao = "Receita indevida",
            Valor = 99m,
            Data = DateTime.Today,
            Tipo = Transacao.ETipo.Receita,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id,
        }));
    }
}
