using Moq;
using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Application.Services;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Interfaces;

namespace Backend.UnitTests.Pessoas;

public class PessoaServiceTests
{
    [Fact]
    public async Task Deve_criar_uma_pessoa()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var pessoas = new Mock<IPessoaRepository>();

        unitOfWork.SetupGet(x => x.Pessoas).Returns(pessoas.Object);
        pessoas.Setup(x => x.AddAsync(It.IsAny<Pessoa>())).Returns(Task.CompletedTask);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var service = new PessoaService(unitOfWork.Object);
        var dto = new CreatePessoaDto
        {
            Nome = "Carlos",
            DataNascimento = DateTime.Today.AddYears(-30),
        };

        // act
        var result = await service.CreateAsync(dto);

        // assert
        Assert.Equal("Carlos", result.Nome);
        Assert.True(result.Idade >= 18);
        pessoas.Verify(x => x.AddAsync(It.IsAny<Pessoa>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Deve_buscar_pessoa_por_id()
    {
        var pessoa = new Pessoa
        {
            Id = Guid.NewGuid(),
            Nome = "Maria",
            DataNascimento = DateTime.Today.AddYears(-25),
        };

        var unitOfWork = new Mock<IUnitOfWork>();
        var pessoas = new Mock<IPessoaRepository>();

        unitOfWork.SetupGet(x => x.Pessoas).Returns(pessoas.Object);
        pessoas.Setup(x => x.GetByIdAsync(pessoa.Id)).ReturnsAsync(pessoa);

        var service = new PessoaService(unitOfWork.Object);

        // act
        var result = await service.GetByIdAsync(pessoa.Id);

        // assert
        Assert.NotNull(result);
        Assert.Equal(pessoa.Nome, result!.Nome);
        Assert.Equal(pessoa.Id, result.Id);
    }

    [Fact]
    public async Task Nao_deve_atualizar_quando_a_pessoa_nao_existe()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var pessoas = new Mock<IPessoaRepository>();

        unitOfWork.SetupGet(x => x.Pessoas).Returns(pessoas.Object);
        pessoas.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Pessoa?)null);

        var service = new PessoaService(unitOfWork.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(Guid.NewGuid(), new UpdatePessoaDto
        {
            Nome = "Pessoa inexistente",
            DataNascimento = DateTime.Today.AddYears(-20),
        }));
    }

    [Fact]
    public async Task Deve_deletar_pessoa()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var pessoas = new Mock<IPessoaRepository>();

        unitOfWork.SetupGet(x => x.Pessoas).Returns(pessoas.Object);
        pessoas.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var service = new PessoaService(unitOfWork.Object);
        var id = Guid.NewGuid();

        // act
        await service.DeleteAsync(id);

        // assert
        pessoas.Verify(x => x.DeleteAsync(id), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
