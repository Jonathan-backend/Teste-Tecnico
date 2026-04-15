using Moq;
using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Application.Services;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Interfaces;

namespace Backend.UnitTests.Categorias;

public class CategoriaServiceAndDomainTests
{
    [Fact]
    public async Task Deve_criar_categoria()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var categorias = new Mock<ICategoriaRepository>();

        unitOfWork.SetupGet(x => x.Categorias).Returns(categorias.Object);
        categorias.Setup(x => x.AddAsync(It.IsAny<Categoria>())).Returns(Task.CompletedTask);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var service = new CategoriaService(unitOfWork.Object);
        var dto = new CreateCategoriaDto
        {
            Descricao = "Alimentacao",
            Finalidade = Categoria.EFinalidade.Despesa,
        };

        
        var result = await service.CreateAsync(dto);

        
        Assert.Equal(dto.Descricao, result.Descricao);
        Assert.Equal(dto.Finalidade, result.Finalidade);
    }

    [Theory]
    [InlineData(Categoria.EFinalidade.Receita, Transacao.ETipo.Receita, true)]
    [InlineData(Categoria.EFinalidade.Receita, Transacao.ETipo.Despesa, false)]
    [InlineData(Categoria.EFinalidade.Despesa, Transacao.ETipo.Despesa, true)]
    [InlineData(Categoria.EFinalidade.Ambas, Transacao.ETipo.Receita, true)]
    [InlineData(Categoria.EFinalidade.Ambas, Transacao.ETipo.Despesa, true)]
    public void Deve_validar_finalidade_da_categoria(Categoria.EFinalidade finalidade, Transacao.ETipo tipo, bool esperado)
    {
        var categoria = new Categoria
        {
            Descricao = "Categoria",
            Finalidade = finalidade,
        };

        
        var permite = categoria.PermiteTipo(tipo);

        
        Assert.Equal(esperado, permite);
    }
}
