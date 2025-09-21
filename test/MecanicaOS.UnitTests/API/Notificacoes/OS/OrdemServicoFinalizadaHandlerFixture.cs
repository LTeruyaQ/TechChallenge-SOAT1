using API.Notificacoes.OS;
using Core.DTOs.Responses.Cliente;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.Responses.Servico;
using Core.DTOs.Responses.Veiculo;
using Core.Enumeradores;
using Core.Interfaces.Controllers;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using NSubstitute;
using System;
using System.Threading.Tasks;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoFinalizadaHandlerFixture
    {
        public IOrdemServicoController OrdemServicoController { get; }
        public IServicoEmail ServicoEmail { get; }
        public ILogServico<OrdemServicoFinalizadaHandler> LogServico { get; }
        public ICompositionRoot CompositionRoot { get; }
        public OrdemServicoFinalizadaHandlerMock Handler { get; }
        public ILogServico<OrdemServicoFinalizadaHandlerMock> LogServicoMock { get; }

        public OrdemServicoFinalizadaHandlerFixture()
        {
            OrdemServicoController = Substitute.For<IOrdemServicoController>();
            ServicoEmail = Substitute.For<IServicoEmail>();
            LogServico = Substitute.For<ILogServico<OrdemServicoFinalizadaHandler>>();
            LogServicoMock = Substitute.For<ILogServico<OrdemServicoFinalizadaHandlerMock>>();
            
            CompositionRoot = Substitute.For<ICompositionRoot>();
            CompositionRoot.CriarOrdemServicoController().Returns(OrdemServicoController);
            CompositionRoot.CriarServicoEmail().Returns(ServicoEmail);
            CompositionRoot.CriarLogService<OrdemServicoFinalizadaHandler>().Returns(LogServico);
            CompositionRoot.CriarLogService<OrdemServicoFinalizadaHandlerMock>().Returns(LogServicoMock);
            
            Handler = new OrdemServicoFinalizadaHandlerMock(CompositionRoot);
        }

        public OrdemServicoFinalizadaEvent CriarEvento(Guid ordemServicoId)
        {
            return new OrdemServicoFinalizadaEvent(ordemServicoId);
        }

        public OrdemServicoResponse CriarOrdemServicoFinalizada(Guid ordemServicoId)
        {
            return new OrdemServicoResponse
            {
                Id = ordemServicoId,
                Status = StatusOrdemServico.Finalizada,
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
                    Valor = 100
                },
                Veiculo = new VeiculoResponse
                {
                    Id = Guid.NewGuid(),
                    Marca = "Toyota",
                    Modelo = "Corolla",
                    Placa = "ABC-1234",
                    Ano = "2020"
                }
            };
        }

        public OrdemServicoResponse CriarOrdemServicoNula()
        {
            return null;
        }

        public OrdemServicoResponse CriarOrdemServicoSemCliente(Guid ordemServicoId)
        {
            var os = CriarOrdemServicoFinalizada(ordemServicoId);
            os.Cliente = null;
            return os;
        }

        public OrdemServicoResponse CriarOrdemServicoSemEmail(Guid ordemServicoId)
        {
            var os = CriarOrdemServicoFinalizada(ordemServicoId);
            os.Cliente.Contato.Email = null;
            return os;
        }

        public OrdemServicoResponse CriarOrdemServicoSemVeiculo(Guid ordemServicoId)
        {
            var os = CriarOrdemServicoFinalizada(ordemServicoId);
            os.Veiculo = null;
            return os;
        }
    }
}
