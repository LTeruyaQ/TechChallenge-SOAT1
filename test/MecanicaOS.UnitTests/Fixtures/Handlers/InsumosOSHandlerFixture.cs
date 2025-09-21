using Core.DTOs.UseCases.Estoque;
using Core.DTOs.UseCases.OrdemServico;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.InsumosOS;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.InsumosOS.CadastrarInsumos;
using Core.UseCases.InsumosOS.DevolverInsumos;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MecanicaOS.UnitTests.Fixtures.Handlers
{
    public class InsumosOSHandlerFixture
    {
        public IOrdemServicoUseCases OrdemServicoUseCases { get; }
        public IEstoqueUseCases EstoqueUseCases { get; }
        public IVerificarEstoqueJobGateway VerificarEstoqueJobGateway { get; }
        public ILogServico<CadastrarInsumosHandler> LogServicoCadastrar { get; }
        public ILogServico<DevolverInsumosHandler> LogServicoDevolverInsumos { get; }
        public IUnidadeDeTrabalho UnidadeDeTrabalho { get; }
        public IUsuarioLogadoServico UsuarioLogadoServico { get; }

        public InsumosOSHandlerFixture()
        {
            OrdemServicoUseCases = Substitute.For<IOrdemServicoUseCases>();
            EstoqueUseCases = Substitute.For<IEstoqueUseCases>();
            VerificarEstoqueJobGateway = Substitute.For<IVerificarEstoqueJobGateway>();
            LogServicoCadastrar = Substitute.For<ILogServico<CadastrarInsumosHandler>>();
            LogServicoDevolverInsumos = Substitute.For<ILogServico<DevolverInsumosHandler>>();
            UnidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalho>();
            UsuarioLogadoServico = Substitute.For<IUsuarioLogadoServico>();
        }

        public ICadastrarInsumosHandler CriarCadastrarInsumosHandler()
        {
            return new CadastrarInsumosHandler(
                OrdemServicoUseCases,
                EstoqueUseCases,
                LogServicoCadastrar,
                UnidadeDeTrabalho,
                UsuarioLogadoServico,
                VerificarEstoqueJobGateway);
        }

        public void ConfigurarMockOrdemServicoUseCasesParaObterPorId(Guid ordemServicoId, OrdemServico ordemServico)
        {
            OrdemServicoUseCases.ObterPorIdUseCaseAsync(ordemServicoId).Returns(ordemServico);
        }

        public void ConfigurarMockEstoqueUseCasesParaObterPorId(Guid estoqueId, Estoque estoque)
        {
            EstoqueUseCases.ObterPorIdUseCaseAsync(estoqueId).Returns(estoque);
        }

        public void ConfigurarMockEstoqueUseCasesParaAtualizar(Guid estoqueId, AtualizarEstoqueUseCaseDto dto)
        {
            EstoqueUseCases.AtualizarUseCaseAsync(estoqueId, Arg.Any<AtualizarEstoqueUseCaseDto>())
                .Returns(Task.FromResult(new Estoque
                {
                    Id = estoqueId,
                    Insumo = dto.Insumo ?? string.Empty,
                    Descricao = dto.Descricao,
                    Preco = dto.Preco ?? 0m,
                    QuantidadeDisponivel = dto.QuantidadeDisponivel ?? 0,
                    QuantidadeMinima = dto.QuantidadeMinima ?? 0
                }));
        }

        public void ConfigurarMockUdtParaCommitSucesso()
        {
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(true));
        }

        public void ConfigurarMockUdtParaCommitFalha()
        {
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(false));
        }

        public static OrdemServico CriarOrdemServicoValida()
        {
            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Teste",
                Documento = "123.456.789-00",
                TipoCliente = TipoCliente.PessoaFisica,
                Ativo = true
            };

            var veiculo = new Veiculo
            {
                Id = Guid.NewGuid(),
                Placa = "ABC-1234",
                Modelo = "Gol",
                Marca = "Volkswagen",
                Ano = "2020",
                ClienteId = cliente.Id,
                Cliente = cliente
            };

            var servico = new Servico
            {
                Id = Guid.NewGuid(),
                Nome = "Troca de Óleo",
                Descricao = "Troca de óleo do motor",
                Valor = 100.00m,
                Disponivel = true
            };

            var ordemServico = new OrdemServico
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                Cliente = cliente,
                VeiculoId = veiculo.Id,
                Veiculo = veiculo,
                ServicoId = servico.Id,
                Servico = servico,
                Descricao = "Troca de óleo completa",
                Status = StatusOrdemServico.Recebida,
                InsumosOS = new List<InsumoOS>()
            };

            return ordemServico;
        }

        public static Estoque CriarEstoqueValido(int quantidadeDisponivel = 10, int quantidadeMinima = 2)
        {
            return new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Óleo de Motor",
                Descricao = "Óleo sintético 5W30",
                Preco = 50.00m,
                QuantidadeDisponivel = quantidadeDisponivel,
                QuantidadeMinima = quantidadeMinima
            };
        }

        public static List<CadastrarInsumoOSUseCaseDto> CriarListaInsumosValida(Guid estoqueId, int quantidade = 2)
        {
            return new List<CadastrarInsumoOSUseCaseDto>
            {
                new CadastrarInsumoOSUseCaseDto
                {
                    EstoqueId = estoqueId,
                    Quantidade = quantidade
                }
            };
        }

        public static List<CadastrarInsumoOSUseCaseDto> CriarListaInsumosMultiplosValida(List<Guid> estoqueIds, List<int> quantidades)
        {
            if (estoqueIds.Count != quantidades.Count)
                throw new ArgumentException("A quantidade de IDs de estoque deve ser igual à quantidade de quantidades");

            var insumos = new List<CadastrarInsumoOSUseCaseDto>();

            for (int i = 0; i < estoqueIds.Count; i++)
            {
                insumos.Add(new CadastrarInsumoOSUseCaseDto
                {
                    EstoqueId = estoqueIds[i],
                    Quantidade = quantidades[i]
                });
            }

            return insumos;
        }
    }
}
