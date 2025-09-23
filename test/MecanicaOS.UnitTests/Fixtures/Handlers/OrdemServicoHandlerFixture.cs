using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.OrdensServico;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.OrdensServico.AceitarOrcamento;
using Core.UseCases.OrdensServico.AtualizarOrdemServico;
using Core.UseCases.OrdensServico.CadastrarOrdemServico;
using Core.UseCases.OrdensServico.ObterOrdemServico;
using Core.UseCases.OrdensServico.ObterOrdemServicoPorStatus;
using Core.UseCases.OrdensServico.ObterTodosOrdensServico;
using Core.UseCases.OrdensServico.RecusarOrcamento;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MecanicaOS.UnitTests.Fixtures.Handlers
{
    public class OrdemServicoHandlerFixture
    {
        // Gateways
        public IOrdemServicoGateway OrdemServicoGateway { get; }
        
        // UseCases
        public IClienteUseCases ClienteUseCases { get; }
        public IServicoUseCases ServicoUseCases { get; }
        public IVeiculoUseCases VeiculoUseCases { get; }
        
        // LogServices
        public ILogServico<CadastrarOrdemServicoHandler> LogServicoCadastrar { get; }
        public ILogServico<AtualizarOrdemServicoHandler> LogServicoAtualizar { get; }
        public ILogServico<ObterOrdemServicoHandler> LogServicoObter { get; }
        public ILogServico<ObterTodosOrdensServicoHandler> LogServicoObterTodos { get; }
        public ILogServico<ObterOrdemServicoPorStatusHandler> LogServicoObterPorStatus { get; }
        public ILogServico<AceitarOrcamentoHandler> LogServicoAceitarOrcamento { get; }
        public ILogServico<RecusarOrcamentoHandler> LogServicoRecusarOrcamento { get; }
        
        // Outros serviços
        public IUnidadeDeTrabalho UnidadeDeTrabalho { get; }
        public IUsuarioLogadoServico UsuarioLogadoServico { get; }
        public IEventosGateway EventosGateway { get; }

        public OrdemServicoHandlerFixture()
        {
            // Inicializar gateways
            OrdemServicoGateway = Substitute.For<IOrdemServicoGateway>();
            EventosGateway = Substitute.For<IEventosGateway>();
            
            // Inicializar use cases
            ClienteUseCases = Substitute.For<IClienteUseCases>();
            ServicoUseCases = Substitute.For<IServicoUseCases>();
            VeiculoUseCases = Substitute.For<IVeiculoUseCases>();
            
            // Inicializar log services
            LogServicoCadastrar = Substitute.For<ILogServico<CadastrarOrdemServicoHandler>>();
            LogServicoAtualizar = Substitute.For<ILogServico<AtualizarOrdemServicoHandler>>();
            LogServicoObter = Substitute.For<ILogServico<ObterOrdemServicoHandler>>();
            LogServicoObterTodos = Substitute.For<ILogServico<ObterTodosOrdensServicoHandler>>();
            LogServicoObterPorStatus = Substitute.For<ILogServico<ObterOrdemServicoPorStatusHandler>>();
            LogServicoAceitarOrcamento = Substitute.For<ILogServico<AceitarOrcamentoHandler>>();
            LogServicoRecusarOrcamento = Substitute.For<ILogServico<RecusarOrcamentoHandler>>();
            
            // Inicializar outros serviços
            UnidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalho>();
            UsuarioLogadoServico = Substitute.For<IUsuarioLogadoServico>();
        }

        // Métodos para criar handlers
        public ICadastrarOrdemServicoHandler CriarCadastrarOrdemServicoHandler()
        {
            return new CadastrarOrdemServicoHandler(
                OrdemServicoGateway,
                ClienteUseCases,
                ServicoUseCases,
                LogServicoCadastrar,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public IAtualizarOrdemServicoHandler CriarAtualizarOrdemServicoHandler()
        {
            return new AtualizarOrdemServicoHandler(
                OrdemServicoGateway,
                LogServicoAtualizar,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public IObterOrdemServicoHandler CriarObterOrdemServicoHandler()
        {
            return new ObterOrdemServicoHandler(
                OrdemServicoGateway,
                LogServicoObter,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public IObterTodosOrdensServicoHandler CriarObterTodosOrdensServicoHandler()
        {
            return new ObterTodosOrdensServicoHandler(
                OrdemServicoGateway,
                LogServicoObterTodos,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public IObterOrdemServicoPorStatusHandler CriarObterOrdemServicoPorStatusHandler()
        {
            return new ObterOrdemServicoPorStatusHandler(
                OrdemServicoGateway,
                LogServicoObterPorStatus,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public IAceitarOrcamentoHandler CriarAceitarOrcamentoHandler()
        {
            return new AceitarOrcamentoHandler(
                OrdemServicoGateway,
                LogServicoAceitarOrcamento,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public IRecusarOrcamentoHandler CriarRecusarOrcamentoHandler()
        {
            return new RecusarOrcamentoHandler(
                OrdemServicoGateway,
                EventosGateway,
                LogServicoRecusarOrcamento,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        // Métodos de configuração para mocks
        public void ConfigurarMockUdtParaCommitSucesso()
        {
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(true));
        }

        public void ConfigurarMockUdtParaCommitFalha()
        {
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(false));
        }

        public void ConfigurarMockOrdemServicoGatewayParaObterPorId(Guid id, OrdemServico ordemServico)
        {
            OrdemServicoGateway.ObterPorIdAsync(id).Returns(ordemServico);
        }

        public void ConfigurarMockOrdemServicoGatewayParaObterPorIdNull(Guid id)
        {
            OrdemServicoGateway.ObterPorIdAsync(id).Returns((OrdemServico)null);
        }

        public void ConfigurarMockOrdemServicoGatewayParaObterTodos(List<OrdemServico> ordensServico)
        {
            OrdemServicoGateway.ObterTodosAsync().Returns(ordensServico);
        }

        public void ConfigurarMockOrdemServicoGatewayParaObterPorStatus(StatusOrdemServico status, List<OrdemServico> ordensServico)
        {
            OrdemServicoGateway.ObterOrdemServicoPorStatusAsync(status).Returns(ordensServico);
        }

        public void ConfigurarMockClienteUseCasesParaObterPorId(Guid id, Cliente cliente)
        {
            ClienteUseCases.ObterPorIdUseCaseAsync(id).Returns(cliente);
        }

        public void ConfigurarMockClienteUseCasesParaObterPorIdNull(Guid id)
        {
            ClienteUseCases.ObterPorIdUseCaseAsync(id).Returns((Cliente)null);
        }

        public void ConfigurarMockServicoUseCasesParaObterPorId(Guid id, Servico servico)
        {
            ServicoUseCases.ObterServicoPorIdUseCaseAsync(id).Returns(servico);
        }

        public void ConfigurarMockServicoUseCasesParaObterPorIdNull(Guid id)
        {
            ServicoUseCases.ObterServicoPorIdUseCaseAsync(id).Returns((Servico)null);
        }

        public void ConfigurarMockVeiculoUseCasesParaObterPorId(Guid id, Veiculo veiculo)
        {
            VeiculoUseCases.ObterPorIdUseCaseAsync(id).Returns(veiculo);
        }

        // Métodos para criar entidades de teste
        public static OrdemServico CriarOrdemServicoValida(StatusOrdemServico status = StatusOrdemServico.Recebida)
        {
            var cliente = CriarClienteValido();
            var veiculo = CriarVeiculoValido(cliente.Id);
            var servico = CriarServicoValido();

            return new OrdemServico
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                Cliente = cliente,
                VeiculoId = veiculo.Id,
                Veiculo = veiculo,
                ServicoId = servico.Id,
                Servico = servico,
                Descricao = "Troca de óleo completa",
                Status = status,
                DataCadastro = DateTime.UtcNow.AddDays(-5),
                DataAtualizacao = DateTime.UtcNow.AddDays(-3),
                InsumosOS = new List<InsumoOS>(),
                DataEnvioOrcamento = status == StatusOrdemServico.AguardandoAprovação ? 
                    DateTime.UtcNow.AddDays(-2) : null
            };
        }

        public static Cliente CriarClienteValido()
        {
            var contato = new Contato
            {
                Id = Guid.NewGuid(),
                Email = "cliente@teste.com",
                Telefone = "(11) 99999-9999"
            };

            var endereco = new Endereco
            {
                Id = Guid.NewGuid(),
                Rua = "Rua Teste",
                Numero = "123",
                Complemento = "Apto 45",
                Bairro = "Centro",
                Cidade = "São Paulo",
                CEP = "01234-567"
            };

            return new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Teste",
                Documento = "123.456.789-00",
                TipoCliente = TipoCliente.PessoaFisica,
                Ativo = true,
                Contato = contato,
                Endereco = endereco
            };
        }

        public static Veiculo CriarVeiculoValido(Guid clienteId)
        {
            return new Veiculo
            {
                Id = Guid.NewGuid(),
                Placa = "ABC-1234",
                Modelo = "Gol",
                Marca = "Volkswagen",
                Ano = "2020",
                ClienteId = clienteId
            };
        }

        public static Servico CriarServicoValido()
        {
            return new Servico
            {
                Id = Guid.NewGuid(),
                Nome = "Troca de Óleo",
                Descricao = "Troca de óleo do motor",
                Valor = 100.00m,
                Disponivel = true
            };
        }

        public static CadastrarOrdemServicoUseCaseDto CriarCadastrarOrdemServicoDto(
            Guid clienteId, Guid veiculoId, Guid servicoId)
        {
            return new CadastrarOrdemServicoUseCaseDto
            {
                ClienteId = clienteId,
                VeiculoId = veiculoId,
                ServicoId = servicoId,
                Descricao = "Troca de óleo completa"
            };
        }

        public static AtualizarOrdemServicoUseCaseDto CriarAtualizarOrdemServicoDto(
            StatusOrdemServico? status = null, string? descricao = null)
        {
            return new AtualizarOrdemServicoUseCaseDto
            {
                Status = status,
                Descricao = descricao
            };
        }
    }
}
