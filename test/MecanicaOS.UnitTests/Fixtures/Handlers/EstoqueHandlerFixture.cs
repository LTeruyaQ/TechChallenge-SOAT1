using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Estoques.AtualizarEstoque;
using Core.UseCases.Estoques.CadastrarEstoque;
using Core.UseCases.Estoques.DeletarEstoque;
using Core.UseCases.Estoques.ObterEstoque;
using Core.UseCases.Estoques.ObterEstoqueCritico;
using Core.UseCases.Estoques.ObterTodosEstoques;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MecanicaOS.UnitTests.Fixtures.Handlers
{
    public class EstoqueHandlerFixture
    {
        public IEstoqueGateway EstoqueGateway { get; }
        public ILogServico<CadastrarEstoqueHandler> LogServicoCadastrar { get; }
        public ILogServico<ObterEstoqueHandler> LogServicoObter { get; }
        public ILogServico<ObterTodosEstoquesHandler> LogServicoObterTodos { get; }
        public ILogServico<ObterEstoqueCriticoHandler> LogServicoObterCritico { get; }
        public ILogServico<AtualizarEstoqueHandler> LogServicoAtualizar { get; }
        public ILogServico<DeletarEstoqueHandler> LogServicoDeletar { get; }
        public IUnidadeDeTrabalho UnidadeDeTrabalho { get; }
        public IUsuarioLogadoServico UsuarioLogadoServico { get; }

        public EstoqueHandlerFixture()
        {
            EstoqueGateway = Substitute.For<IEstoqueGateway>();
            LogServicoCadastrar = Substitute.For<ILogServico<CadastrarEstoqueHandler>>();
            LogServicoObter = Substitute.For<ILogServico<ObterEstoqueHandler>>();
            LogServicoObterTodos = Substitute.For<ILogServico<ObterTodosEstoquesHandler>>();
            LogServicoObterCritico = Substitute.For<ILogServico<ObterEstoqueCriticoHandler>>();
            LogServicoAtualizar = Substitute.For<ILogServico<AtualizarEstoqueHandler>>();
            LogServicoDeletar = Substitute.For<ILogServico<DeletarEstoqueHandler>>();
            UnidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalho>();
            UsuarioLogadoServico = Substitute.For<IUsuarioLogadoServico>();
            
            // Configuração padrão para o UDT
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(true));
        }

        #region Criação de Handlers

        public CadastrarEstoqueHandler CriarCadastrarEstoqueHandler()
        {
            return new CadastrarEstoqueHandler(
                EstoqueGateway,
                LogServicoCadastrar,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public ObterEstoqueHandler CriarObterEstoqueHandler()
        {
            return new ObterEstoqueHandler(
                EstoqueGateway,
                LogServicoObter,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public ObterTodosEstoquesHandler CriarObterTodosEstoquesHandler()
        {
            return new ObterTodosEstoquesHandler(
                EstoqueGateway,
                LogServicoObterTodos,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public ObterEstoqueCriticoHandler CriarObterEstoqueCriticoHandler()
        {
            return new ObterEstoqueCriticoHandler(
                EstoqueGateway,
                LogServicoObterCritico,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public AtualizarEstoqueHandler CriarAtualizarEstoqueHandler()
        {
            return new AtualizarEstoqueHandler(
                EstoqueGateway,
                LogServicoAtualizar,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public DeletarEstoqueHandler CriarDeletarEstoqueHandler()
        {
            return new DeletarEstoqueHandler(
                EstoqueGateway,
                LogServicoDeletar,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        #endregion

        #region Dados de Teste

        public static Estoque CriarEstoqueValido()
        {
            return new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Óleo de Motor",
                Descricao = "Óleo sintético 5W30",
                QuantidadeDisponivel = 10,
                QuantidadeMinima = 5,
                Preco = 25.99m,
                Ativo = true,
                DataCadastro = DateTime.UtcNow.AddDays(-30),
                DataAtualizacao = DateTime.UtcNow.AddDays(-5)
            };
        }

        public static Estoque CriarEstoqueCritico()
        {
            return new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Filtro de Ar",
                Descricao = "Filtro de ar para motor",
                QuantidadeDisponivel = 8,
                QuantidadeMinima = 10,
                Preco = 45.00m,
                Ativo = true,
                DataCadastro = DateTime.UtcNow.AddDays(-15),
                DataAtualizacao = DateTime.UtcNow.AddDays(-2)
            };
        }

        public static CadastrarEstoqueUseCaseDto CriarCadastrarEstoqueUseCaseDtoValido()
        {
            return new CadastrarEstoqueUseCaseDto
            {
                Insumo = "Vela de Ignição",
                Descricao = "Vela de ignição NGK padrão",
                QuantidadeDisponivel = 25,
                QuantidadeMinima = 12,
                Preco = 18.90m
            };
        }

        public static AtualizarEstoqueUseCaseDto CriarAtualizarEstoqueUseCaseDtoValido()
        {
            return new AtualizarEstoqueUseCaseDto
            {
                Insumo = "Óleo Motor 5W30 Atualizado",
                Descricao = "Óleo lubrificante sintético premium",
                QuantidadeDisponivel = 75,
                QuantidadeMinima = 15,
                Preco = 52.90m
            };
        }

        public static List<Estoque> CriarListaEstoquesVariados()
        {
            return new List<Estoque>
            {
                CriarEstoqueValido(),
                CriarEstoqueCritico(),
                new Estoque
                {
                    Id = Guid.NewGuid(),
                    Insumo = "Pastilha de Freio",
                    Descricao = "Pastilha de freio dianteira",
                    QuantidadeDisponivel = 0,
                    QuantidadeMinima = 6,
                    Preco = 89.90m,
                    Ativo = true,
                    DataCadastro = DateTime.UtcNow.AddDays(-10),
                    DataAtualizacao = DateTime.UtcNow.AddHours(-6)
                }
            };
        }

        #endregion

        #region Configuração de Mocks

        public void ConfigurarMockEstoqueGatewayParaObterPorId(Guid id, Estoque estoque)
        {
            EstoqueGateway.ObterPorIdAsync(id).Returns(estoque);
        }

        public void ConfigurarMockEstoqueGatewayParaObterTodos(List<Estoque> estoques)
        {
            EstoqueGateway.ObterTodosAsync().Returns(estoques);
        }

        public void ConfigurarMockEstoqueGatewayParaObterEstoqueCritico(List<Estoque> estoquesCriticos)
        {
            EstoqueGateway.ObterEstoqueCriticoAsync().Returns(estoquesCriticos);
        }

        public void ConfigurarMockEstoqueGatewayParaCadastrar(Estoque estoque)
        {
            EstoqueGateway.CadastrarAsync(Arg.Any<Estoque>()).Returns(Task.CompletedTask);
            EstoqueGateway.When(x => x.CadastrarAsync(Arg.Any<Estoque>()))
                .Do(callInfo => 
                {
                    var estoqueArg = callInfo.Arg<Estoque>();
                    estoqueArg.Id = estoque.Id;
                    estoqueArg.DataCadastro = estoque.DataCadastro;
                    estoqueArg.DataAtualizacao = estoque.DataAtualizacao;
                    estoqueArg.Ativo = estoque.Ativo;
                });
        }

        public void ConfigurarMockEstoqueGatewayParaAtualizar(Estoque estoque)
        {
            EstoqueGateway.EditarAsync(Arg.Any<Estoque>()).Returns(Task.CompletedTask);
            EstoqueGateway.When(x => x.EditarAsync(Arg.Any<Estoque>()))
                .Do(callInfo => 
                {
                    var estoqueArg = callInfo.Arg<Estoque>();
                    estoqueArg.DataAtualizacao = DateTime.UtcNow;
                });
        }

        public void ConfigurarMockEstoqueGatewayParaDeletar(Estoque estoque, bool sucesso)
        {
            EstoqueGateway.DeletarAsync(estoque).Returns(Task.CompletedTask);
        }

        public void ConfigurarMockUdtParaCommitFalha()
        {
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(false));
        }

        #endregion
    }
}
