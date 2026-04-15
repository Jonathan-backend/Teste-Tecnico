using Moq;
using MinhasFinancas.Application.DTOs;
using MinhasFinancas.Application.Services;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Interfaces;

namespace Backend.UnitTests.Transacoes;

public class TransacaoServiceTests
{
    [Fact]
    public async Task Nao_deve_permitir_receita_para_menor()
    {
        var pessoa = new Pessoa
        {
            Id = Guid.NewGuid(),
            Nome = "Joao",
            DataNascimento = DateTime.Today.AddYears(-10),
        };
        var categoria = new Categoria
        {
            Id = Guid.NewGuid(),
            Descricao = "Salario",
            Finalidade = Categoria.EFinalidade.Receita,
        };

        var unitOfWork = new Mock<IUnitOfWork>();
        var pessoas = new Mock<IPessoaRepository>();
        var categorias = new Mock<ICategoriaRepository>();
        var transacoes = new Mock<ITransacaoRepository>();

        unitOfWork.SetupGet(x => x.Pessoas).Returns(pessoas.Object);
        unitOfWork.SetupGet(x => x.Categorias).Returns(categorias.Object);
        unitOfWork.SetupGet(x => x.Transacoes).Returns(transacoes.Object);
        pessoas.Setup(x => x.GetByIdAsync(pessoa.Id)).ReturnsAsync(pessoa);
        categorias.Setup(x => x.GetByIdAsync(categoria.Id)).ReturnsAsync(categoria);

        var service = new TransacaoService(unitOfWork.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(new CreateTransacaoDto
        {
            Descricao = "Lancamento de teste",
            Valor = 100m,
            Data = DateTime.Today,
            Tipo = Transacao.ETipo.Receita,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id,
        }));
    }

    [Fact]
    public async Task Nao_deve_permitir_despesa_em_categoria_de_receita()
    {
        var pessoa = new Pessoa
        {
            Id = Guid.NewGuid(),
            Nome = "Carlos",
            DataNascimento = DateTime.Today.AddYears(-30),
        };
        var categoria = new Categoria
        {
            Id = Guid.NewGuid(),
            Descricao = "Salario",
            Finalidade = Categoria.EFinalidade.Receita,
        };

        var unitOfWork = new Mock<IUnitOfWork>();
        var pessoas = new Mock<IPessoaRepository>();
        var categorias = new Mock<ICategoriaRepository>();
        var transacoes = new Mock<ITransacaoRepository>();

        unitOfWork.SetupGet(x => x.Pessoas).Returns(pessoas.Object);
        unitOfWork.SetupGet(x => x.Categorias).Returns(categorias.Object);
        unitOfWork.SetupGet(x => x.Transacoes).Returns(transacoes.Object);
        pessoas.Setup(x => x.GetByIdAsync(pessoa.Id)).ReturnsAsync(pessoa);
        categorias.Setup(x => x.GetByIdAsync(categoria.Id)).ReturnsAsync(categoria);

        var service = new TransacaoService(unitOfWork.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(new CreateTransacaoDto
        {
            Descricao = "Lancamento de teste",
            Valor = 100m,
            Data = DateTime.Today,
            Tipo = Transacao.ETipo.Despesa,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id,
        }));
    }

    [Fact]
    public async Task Deve_salvar_transacao_valida()
    {
        var pessoa = new Pessoa
        {
            Id = Guid.NewGuid(),
            Nome = "Ana",
            DataNascimento = DateTime.Today.AddYears(-28),
        };
        var categoria = new Categoria
        {
            Id = Guid.NewGuid(),
            Descricao = "Freelance",
            Finalidade = Categoria.EFinalidade.Receita,
        };

        var unitOfWork = new Mock<IUnitOfWork>();
        var pessoas = new Mock<IPessoaRepository>();
        var categorias = new Mock<ICategoriaRepository>();
        var transacoes = new Mock<ITransacaoRepository>();

        unitOfWork.SetupGet(x => x.Pessoas).Returns(pessoas.Object);
        unitOfWork.SetupGet(x => x.Categorias).Returns(categorias.Object);
        unitOfWork.SetupGet(x => x.Transacoes).Returns(transacoes.Object);
        pessoas.Setup(x => x.GetByIdAsync(pessoa.Id)).ReturnsAsync(pessoa);
        categorias.Setup(x => x.GetByIdAsync(categoria.Id)).ReturnsAsync(categoria);
        transacoes.Setup(x => x.AddAsync(It.IsAny<Transacao>())).Returns(Task.CompletedTask);
        unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        var service = new TransacaoService(unitOfWork.Object);

        var result = await service.CreateAsync(new CreateTransacaoDto
        {
            Descricao = "Lancamento de teste",
            Valor = 100m,
            Data = DateTime.Today,
            Tipo = Transacao.ETipo.Receita,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id,
        });

        Assert.Equal(pessoa.Id, result.PessoaId);
        Assert.Equal(categoria.Id, result.CategoriaId);
        transacoes.Verify(x => x.AddAsync(It.IsAny<Transacao>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Nao_deve_salvar_quando_categoria_nao_existe()
    {
        var pessoa = new Pessoa
        {
            Id = Guid.NewGuid(),
            Nome = "Ana",
            DataNascimento = DateTime.Today.AddYears(-28),
        };

        var unitOfWork = new Mock<IUnitOfWork>();
        var pessoas = new Mock<IPessoaRepository>();
        var categorias = new Mock<ICategoriaRepository>();
        var transacoes = new Mock<ITransacaoRepository>();

        unitOfWork.SetupGet(x => x.Pessoas).Returns(pessoas.Object);
        unitOfWork.SetupGet(x => x.Categorias).Returns(categorias.Object);
        unitOfWork.SetupGet(x => x.Transacoes).Returns(transacoes.Object);
        pessoas.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(pessoa);
        categorias.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Categoria?)null);

        var service = new TransacaoService(unitOfWork.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(new CreateTransacaoDto
        {
            Descricao = "Lancamento de teste",
            Valor = 100m,
            Data = DateTime.Today,
            Tipo = Transacao.ETipo.Despesa,
            PessoaId = pessoa.Id,
            CategoriaId = Guid.NewGuid(),
        }));
    }
}
