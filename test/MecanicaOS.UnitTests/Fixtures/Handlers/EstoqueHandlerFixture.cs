using Core.DTOs.Entidades.Estoque;
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
using Adapters.Gateways;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MecanicaOS.UnitTests.Fixtures.Handlers
{
    public class EstoqueHandlerFixture
    {
        // Repositório mockado
        public IRepositorio<EstoqueEntityDto> RepositorioEstoque { get; }
        
        // Gateway real
        public IEstoqueGateway EstoqueGateway { get; }
        
        // Serviços mockados
        public ILogServicoGateway<CadastrarEstoqueHandler> LogServicoCadastrar { get; }
        public ILogServicoGateway<ObterEstoqueHandler> LogServicoObter { get; }
        public ILogServicoGateway<ObterTodosEstoquesHandler> LogServicoObterTodos { get; }
        public ILogServicoGateway<ObterEstoqueCriticoHandler> LogServicoObterCritico { get; }
        public ILogServicoGateway<AtualizarEstoqueHandler> LogServicoAtualizar { get; }
        public ILogServicoGateway<DeletarEstoqueHandler> LogServicoDeletar { get; }
        public IUnidadeDeTrabalhoGateway UnidadeDeTrabalho { get; }
        public IUsuarioLogadoServicoGateway UsuarioLogadoServico { get; }

        public EstoqueHandlerFixture()
        {
            // Inicializar repositório mockado
            RepositorioEstoque = Substitute.For<IRepositorio<EstoqueEntityDto>>();
            
            // Inicializar gateway real usando o repositório mockado
            EstoqueGateway = new EstoqueGateway(RepositorioEstoque);
            
            // Inicializar serviços mockados
            LogServicoCadastrar = Substitute.For<ILogServicoGateway<CadastrarEstoqueHandler>>();
            LogServicoObter = Substitute.For<ILogServicoGateway<ObterEstoqueHandler>>();
            LogServicoObterTodos = Substitute.For<ILogServicoGateway<ObterTodosEstoquesHandler>>();
            LogServicoObterCritico = Substitute.For<ILogServicoGateway<ObterEstoqueCriticoHandler>>();
            LogServicoAtualizar = Substitute.For<ILogServicoGateway<AtualizarEstoqueHandler>>();
            LogServicoDeletar = Substitute.For<ILogServicoGateway<DeletarEstoqueHandler>>();
            UnidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalhoGateway>();
            UsuarioLogadoServico = Substitute.For<IUsuarioLogadoServicoGateway>();
            
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

        #region Configuração de Mocks dos Repositórios

        public void ConfigurarMockRepositorioEstoqueParaObterPorId(Guid id, Estoque estoque)
        {
            var dto = estoque != null ? ToEstoqueDto(estoque) : null;
            RepositorioEstoque.ObterPorIdAsync(id).Returns(dto);
        }

        public void ConfigurarMockRepositorioEstoqueParaObterTodos(List<Estoque> estoques)
        {
            var dtos = estoques.Select(ToEstoqueDto).ToList();
            RepositorioEstoque.ObterTodosAsync().Returns(dtos);
        }

        public void ConfigurarMockRepositorioEstoqueParaObterEstoqueCritico(List<Estoque> estoquesCriticos)
        {
            // Para consultas com especificação (ObterEstoqueCriticoAsync)
            RepositorioEstoque.ListarProjetadoAsync<Estoque>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<EstoqueEntityDto>>())
                .Returns(estoquesCriticos);
        }

        public void ConfigurarMockRepositorioEstoqueParaCadastrar(Estoque estoque)
        {
            var dto = ToEstoqueDto(estoque);
            RepositorioEstoque.CadastrarAsync(Arg.Any<EstoqueEntityDto>()).Returns(dto);
        }

        public void ConfigurarMockRepositorioEstoqueParaEditar()
        {
            RepositorioEstoque.EditarAsync(Arg.Any<EstoqueEntityDto>()).Returns(Task.CompletedTask);
        }

        public void ConfigurarMockRepositorioEstoqueParaDeletar()
        {
            RepositorioEstoque.DeletarAsync(Arg.Any<EstoqueEntityDto>()).Returns(Task.CompletedTask);
        }

        public void ConfigurarMockUdtParaCommitFalha()
        {
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(false));
        }

        #endregion

        #region Métodos de Compatibilidade (para facilitar migração dos testes)

        public void ConfigurarMockEstoqueGatewayParaObterPorId(Guid id, Estoque estoque)
        {
            ConfigurarMockRepositorioEstoqueParaObterPorId(id, estoque);
        }

        public void ConfigurarMockEstoqueGatewayParaObterTodos(List<Estoque> estoques)
        {
            ConfigurarMockRepositorioEstoqueParaObterTodos(estoques);
        }

        public void ConfigurarMockEstoqueGatewayParaObterEstoqueCritico(List<Estoque> estoquesCriticos)
        {
            ConfigurarMockRepositorioEstoqueParaObterEstoqueCritico(estoquesCriticos);
        }

        public void ConfigurarMockEstoqueGatewayParaCadastrar(Estoque estoque)
        {
            ConfigurarMockRepositorioEstoqueParaCadastrar(estoque);
        }

        public void ConfigurarMockEstoqueGatewayParaAtualizar(Estoque estoque)
        {
            ConfigurarMockRepositorioEstoqueParaEditar();
        }

        public void ConfigurarMockEstoqueGatewayParaDeletar(Estoque estoque, bool sucesso)
        {
            ConfigurarMockRepositorioEstoqueParaDeletar();
        }

        #endregion

        #region Métodos de Conversão para DTOs

        private static EstoqueEntityDto ToEstoqueDto(Estoque estoque)
        {
            return new EstoqueEntityDto
            {
                Id = estoque.Id,
                Ativo = estoque.Ativo,
                DataCadastro = estoque.DataCadastro,
                DataAtualizacao = estoque.DataAtualizacao,
                QuantidadeMinima = estoque.QuantidadeMinima,
                QuantidadeDisponivel = estoque.QuantidadeDisponivel,
                Descricao = estoque.Descricao,
                Insumo = estoque.Insumo,
                Preco = estoque.Preco
            };
        }

        private static Estoque FromEstoqueDto(EstoqueEntityDto dto)
        {
            return new Estoque
            {
                Id = dto.Id,
                Ativo = dto.Ativo,
                DataCadastro = dto.DataCadastro,
                DataAtualizacao = dto.DataAtualizacao,
                QuantidadeMinima = dto.QuantidadeMinima,
                QuantidadeDisponivel = dto.QuantidadeDisponivel,
                Descricao = dto.Descricao,
                Insumo = dto.Insumo,
                Preco = dto.Preco
            };
        }

        #endregion
    }
}
