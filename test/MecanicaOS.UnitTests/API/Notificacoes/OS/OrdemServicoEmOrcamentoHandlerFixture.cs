using API.Notificacoes.OS;
using Core.DTOs.Responses.Cliente;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.DTOs.Responses.Servico;
using Core.DTOs.Responses.Estoque;
using Core.Enumeradores;
using Core.Interfaces.Controllers;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoEmOrcamentoHandlerFixture
    {
        public IOrdemServicoController OrdemServicoController { get; }
        public IServicoEmail ServicoEmail { get; }
        public ILogServico<OrdemServicoEmOrcamentoHandler> LogServico { get; }
        public ICompositionRoot CompositionRoot { get; }
        public OrdemServicoEmOrcamentoHandlerMock Handler { get; }
        public ILogServico<OrdemServicoEmOrcamentoHandlerMock> LogServicoMock { get; }

        public OrdemServicoEmOrcamentoHandlerFixture()
        {
            OrdemServicoController = Substitute.For<IOrdemServicoController>();
            ServicoEmail = Substitute.For<IServicoEmail>();
            LogServico = Substitute.For<ILogServico<OrdemServicoEmOrcamentoHandler>>();
            LogServicoMock = Substitute.For<ILogServico<OrdemServicoEmOrcamentoHandlerMock>>();
            
            CompositionRoot = Substitute.For<ICompositionRoot>();
            CompositionRoot.CriarOrdemServicoController().Returns(OrdemServicoController);
            CompositionRoot.CriarServicoEmail().Returns(ServicoEmail);
            CompositionRoot.CriarLogService<OrdemServicoEmOrcamentoHandler>().Returns(LogServico);
            CompositionRoot.CriarLogService<OrdemServicoEmOrcamentoHandlerMock>().Returns(LogServicoMock);
            
            Handler = new OrdemServicoEmOrcamentoHandlerMock(CompositionRoot);
        }

        public OrdemServicoEmOrcamentoEvent CriarEvento(Guid ordemServicoId)
        {
            return new OrdemServicoEmOrcamentoEvent(ordemServicoId);
        }

        public OrdemServicoResponse CriarOrdemServicoComOrcamento(Guid ordemServicoId, decimal valorServico = 100M, decimal valorOrcamento = 150M)
        {
            return new OrdemServicoResponse
            {
                Id = ordemServicoId,
                Status = StatusOrdemServico.AguardandoAprovação,
                Cliente = new ClienteResponse
                {
                    Id = Guid.NewGuid(),
                    Nome = "Cliente Teste",
                    Contato = new ContatoResponse
                    {
                        Email = "cliente@teste.com",
                        Telefone = "(11) 99999-9999"
                    }
                },
                Servico = new ServicoResponse
                {
                    Id = Guid.NewGuid(),
                    Nome = "Troca de Óleo",
                    Valor = valorServico
                },
                Orcamento = (double)valorOrcamento,
                InsumosOS = CriarInsumos(ordemServicoId)
            };
        }

        public List<InsumoOSResponse> CriarInsumos(Guid ordemServicoId, int quantidade = 2)
        {
            var insumos = new List<InsumoOSResponse>();
            
            for (int i = 0; i < quantidade; i++)
            {
                insumos.Add(new InsumoOSResponse
                {
                    OrdemServicoId = ordemServicoId,
                    EstoqueId = Guid.NewGuid(),
                    Quantidade = i + 1,
                    Estoque = new EstoqueResponse
                    {
                        Id = Guid.NewGuid(),
                        Insumo = $"Insumo {i+1}",
                        Preco = 25.0 * (i + 1)
                    }
                });
            }
            
            return insumos;
        }

        public OrdemServicoResponse CriarOrdemServicoSemOrcamento(Guid ordemServicoId)
        {
            var os = CriarOrdemServicoComOrcamento(ordemServicoId);
            os.Orcamento = null;
            return os;
        }

        public OrdemServicoResponse CriarOrdemServicoSemInsumos(Guid ordemServicoId)
        {
            var os = CriarOrdemServicoComOrcamento(ordemServicoId);
            os.InsumosOS = new List<InsumoOSResponse>();
            return os;
        }

        public OrdemServicoResponse CriarOrdemServicoSemCliente(Guid ordemServicoId)
        {
            var os = CriarOrdemServicoComOrcamento(ordemServicoId);
            os.Cliente = null;
            return os;
        }

        public OrdemServicoResponse CriarOrdemServicoSemEmail(Guid ordemServicoId)
        {
            var os = CriarOrdemServicoComOrcamento(ordemServicoId);
            os.Cliente.Contato.Email = null;
            return os;
        }
    }
}
