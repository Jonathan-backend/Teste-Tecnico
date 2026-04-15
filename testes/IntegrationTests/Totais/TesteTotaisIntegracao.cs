using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Domain.Entities;
using Backend.IntegrationTests.Support;

namespace Backend.IntegrationTests.Totais;

public class TotaisPorPessoaIntegrationTests : SqliteIntegrationTestBase
{
    [Fact]
    public async Task GetTotaisPorPessoaAsync_DeveBaterComAsTransacoes()
    {
        var pessoa = await PessoaService.CreateAsync(new CreatePessoaDto
        {
            Nome = "Fechamento",
            DataNascimento = DateTime.Today.AddYears(-40),
        });

        var categoriaReceita = await CategoriaService.CreateAsync(new CreateCategoriaDto
        {
            Descricao = "Salario",
            Finalidade = Categoria.EFinalidade.Receita,
        });

        var categoriaDespesa = await CategoriaService.CreateAsync(new CreateCategoriaDto
        {
            Descricao = "Mercado",
            Finalidade = Categoria.EFinalidade.Despesa,
        });

        await TransacaoService.CreateAsync(new CreateTransacaoDto
        {
            Descricao = "Pagamento",
            Valor = 3000m,
            Data = DateTime.Today,
            Tipo = Transacao.ETipo.Receita,
            PessoaId = pessoa.Id,
            CategoriaId = categoriaReceita.Id,
        });

        await TransacaoService.CreateAsync(new CreateTransacaoDto
        {
            Descricao = "Compras",
            Valor = 500m,
            Data = DateTime.Today,
            Tipo = Transacao.ETipo.Despesa,
            PessoaId = pessoa.Id,
            CategoriaId = categoriaDespesa.Id,
        });

        var totais = await TotalService.GetTotaisPorPessoaAsync();
        var totalPessoa = Assert.Single(totais.Items.Where(x => x.PessoaId == pessoa.Id));

        Assert.Equal(3000m, totalPessoa.TotalReceitas);
        Assert.Equal(500m, totalPessoa.TotalDespesas);
        Assert.Equal(2500m, totalPessoa.Saldo);
    }
}
