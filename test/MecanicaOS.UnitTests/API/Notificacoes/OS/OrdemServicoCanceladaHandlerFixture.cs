using API.Notificacoes.OS;
using Core.DTOs.Responses.Estoque;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.Enumeradores;
using Core.Interfaces.Controllers;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoCanceladaHandlerFixture
    {
        public IOrdemServicoController OrdemServicoController { get; }
        public IInsumoOSController InsumoOSController { get; }
        public ILogServico<OrdemServicoCanceladaHandler> LogServico { get; }
        public ICompositionRoot CompositionRoot { get; }
        public OrdemServicoCanceladaHandler Handler { get; }

        public OrdemServicoCanceladaHandlerFixture()
        {
            // Configurar mocks
            OrdemServicoController = Substitute.For<IOrdemServicoController>();
            InsumoOSController = Substitute.For<IInsumoOSController>();
            LogServico = Substitute.For<ILogServico<OrdemServicoCanceladaHandler>>();
            CompositionRoot = Substitute.For<ICompositionRoot>();

            // Configurar o CompositionRoot para retornar os mocks
            CompositionRoot.CriarOrdemServicoController().Returns(OrdemServicoController);
            CompositionRoot.CriarInsumoOSController().Returns(InsumoOSController);
            CompositionRoot.CriarLogService<OrdemServicoCanceladaHandler>().Returns(LogServico);

            // Criar o handler com o CompositionRoot mockado
            Handler = new OrdemServicoCanceladaHandler(CompositionRoot);
        }

        public OrdemServicoResponse CriarOrdemServicoSemInsumos(Guid ordemServicoId)
        {
            return new OrdemServicoResponse
            {
                Id = ordemServicoId,
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Status = StatusOrdemServico.Cancelada,
                Descricao = "Ordem de serviço sem insumos",
                Insumos = new List<InsumoOSResponse>()
            };
        }

        public OrdemServicoResponse CriarOrdemServicoComInsumos(Guid ordemServicoId, int quantidadeInsumos = 2)
        {
            var insumos = new List<InsumoOSResponse>();

            for (int i = 0; i < quantidadeInsumos; i++)
            {
                insumos.Add(new InsumoOSResponse
                {
                    OrdemServicoId = ordemServicoId,
                    EstoqueId = Guid.NewGuid(),
                    Quantidade = (i + 1) * 2,
                    Estoque = new EstoqueResponse
                    {
                        Id = Guid.NewGuid(),
                        Insumo = $"Insumo {i + 1}",
                        Preco = 10.0 * (i + 1),
                        QuantidadeDisponivel = 50
                    }
                });
            }

            return new OrdemServicoResponse
            {
                Id = ordemServicoId,
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Status = StatusOrdemServico.Cancelada,
                Descricao = "Ordem de serviço com insumos",
                Insumos = insumos
            };
        }

        public OrdemServicoResponse CriarOrdemServicoComInsumosEspecificos(Guid ordemServicoId, List<(Guid EstoqueId, int Quantidade)> insumosInfo)
        {
            var insumos = new List<InsumoOSResponse>();

            foreach (var (estoqueId, quantidade) in insumosInfo)
            {
                insumos.Add(new InsumoOSResponse
                {
                    OrdemServicoId = ordemServicoId,
                    EstoqueId = estoqueId,
                    Quantidade = quantidade,
                    Estoque = new EstoqueResponse
                    {
                        Id = estoqueId,
                        Insumo = $"Insumo {estoqueId.ToString().Substring(0, 4)}",
                        Preco = 25.0,
                        QuantidadeDisponivel = 30
                    }
                });
            }

            return new OrdemServicoResponse
            {
                Id = ordemServicoId,
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Status = StatusOrdemServico.Cancelada,
                Descricao = "Ordem de serviço com insumos específicos",
                Insumos = insumos
            };
        }

        public OrdemServicoCanceladaEvent CriarEvento(Guid ordemServicoId)
        {
            return new OrdemServicoCanceladaEvent(ordemServicoId);
        }
    }
}
