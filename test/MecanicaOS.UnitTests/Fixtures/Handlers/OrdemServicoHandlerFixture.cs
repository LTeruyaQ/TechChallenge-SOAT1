using Core.DTOs.Entidades.OrdemServicos;
using Core.DTOs.UseCases.Eventos;
using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Especificacoes.Base.Interfaces;
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
using Adapters.Gateways;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MecanicaOS.UnitTests.Fixtures.Handlers
{
    public class OrdemServicoHandlerFixture
    {
        // Repositórios mockados
        public IRepositorio<OrdemServicoEntityDto> RepositorioOrdemServico { get; }
        
        // Gateways reais
        public IOrdemServicoGateway OrdemServicoGateway { get; }
        public IEventosGateway EventosGateway { get; }
        
        // UseCases
        public IClienteUseCases ClienteUseCases { get; }
        public IServicoUseCases ServicoUseCases { get; }
        public IVeiculoUseCases VeiculoUseCases { get; }
        
        // LogServices
        public ILogServicoGateway<CadastrarOrdemServicoHandler> LogServicoCadastrar { get; }
        public ILogServicoGateway<AtualizarOrdemServicoHandler> LogServicoAtualizar { get; }
        public ILogServicoGateway<ObterOrdemServicoHandler> LogServicoObter { get; }
        public ILogServicoGateway<ObterTodosOrdensServicoHandler> LogServicoObterTodos { get; }
        public ILogServicoGateway<ObterOrdemServicoPorStatusHandler> LogServicoObterPorStatus { get; }
        public ILogServicoGateway<AceitarOrcamentoHandler> LogServicoAceitarOrcamento { get; }
        public ILogServicoGateway<RecusarOrcamentoHandler> LogServicoRecusarOrcamento { get; }
        
        // Outros serviços
        public IUnidadeDeTrabalhoGateway UnidadeDeTrabalho { get; }
        public IUsuarioLogadoServicoGateway UsuarioLogadoServico { get; }

        public OrdemServicoHandlerFixture()
        {
            // Inicializar repositórios mockados
            RepositorioOrdemServico = Substitute.For<IRepositorio<OrdemServicoEntityDto>>();
            
            // Inicializar gateways reais usando os repositórios mockados
            OrdemServicoGateway = new OrdemServicoGateway(RepositorioOrdemServico);
            EventosGateway = Substitute.For<IEventosGateway>();
            
            // Inicializar use cases
            ClienteUseCases = Substitute.For<IClienteUseCases>();
            ServicoUseCases = Substitute.For<IServicoUseCases>();
            VeiculoUseCases = Substitute.For<IVeiculoUseCases>();
            
            // Inicializar log services
            LogServicoCadastrar = Substitute.For<ILogServicoGateway<CadastrarOrdemServicoHandler>>();
            LogServicoAtualizar = Substitute.For<ILogServicoGateway<AtualizarOrdemServicoHandler>>();
            LogServicoObter = Substitute.For<ILogServicoGateway<ObterOrdemServicoHandler>>();
            LogServicoObterTodos = Substitute.For<ILogServicoGateway<ObterTodosOrdensServicoHandler>>();
            LogServicoObterPorStatus = Substitute.For<ILogServicoGateway<ObterOrdemServicoPorStatusHandler>>();
            LogServicoAceitarOrcamento = Substitute.For<ILogServicoGateway<AceitarOrcamentoHandler>>();
            LogServicoRecusarOrcamento = Substitute.For<ILogServicoGateway<RecusarOrcamentoHandler>>();
            
            // Inicializar outros serviços
            UnidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalhoGateway>();
            UsuarioLogadoServico = Substitute.For<IUsuarioLogadoServicoGateway>();
            
            // Configuração padrão para o UDT
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(true));
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

        public void ConfigurarMockRepositorioOrdemServicoParaObterPorId(Guid id, OrdemServico ordemServico)
        {
            var dto = ordemServico != null ? ToOrdemServicoDto(ordemServico) : null;
            RepositorioOrdemServico.ObterPorIdAsync(id).Returns(dto);
            RepositorioOrdemServico.ObterUmProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>()).Returns(ordemServico);
        }

        public void ConfigurarMockRepositorioOrdemServicoParaObterTodos(List<OrdemServico> ordensServico)
        {
            var dtos = ordensServico.Select(os => ToOrdemServicoDto(os)).ToList();
            RepositorioOrdemServico.ObterTodosAsync().Returns(dtos);
            RepositorioOrdemServico.ListarAsync(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>()).Returns(dtos);
        }

        public void ConfigurarMockRepositorioOrdemServicoParaObterPorStatus(StatusOrdemServico status, List<OrdemServico> ordensServico)
        {
            var dtos = ordensServico.Select(os => ToOrdemServicoDto(os)).ToList();
            RepositorioOrdemServico.ListarProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>())
                .Returns(ordensServico);
        }
        
        public void ConfigurarMockRepositorioOrdemServicoParaCadastrar(OrdemServico ordemServico)
        {
            var dto = ToOrdemServicoDto(ordemServico);
            RepositorioOrdemServico.CadastrarAsync(Arg.Any<OrdemServicoEntityDto>()).Returns(dto);
        }
        
        public void ConfigurarMockRepositorioOrdemServicoParaEditar()
        {
            RepositorioOrdemServico.EditarAsync(Arg.Any<OrdemServicoEntityDto>()).Returns(Task.CompletedTask);
        }
        
        public void ConfigurarMockRepositorioOrdemServicoParaLancarExcecaoAoEditar(Guid id, Exception excecao)
        {
            // Configura o mock para obter a ordem de serviço
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida(StatusOrdemServico.AguardandoAprovação);
            ordemServico.Id = id;
            ordemServico.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-1);
            
            ConfigurarMockRepositorioOrdemServicoParaObterPorId(id, ordemServico);
            
            // Configura o mock para lançar exceção ao editar
            RepositorioOrdemServico.EditarAsync(Arg.Any<OrdemServicoEntityDto>())
                .Returns(Task.FromException(excecao));
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
        
        #region Métodos de Conversão para DTOs
        
        private static OrdemServicoEntityDto ToOrdemServicoDto(OrdemServico ordemServico)
        {
            return new OrdemServicoEntityDto
            {
                Id = ordemServico.Id,
                Ativo = ordemServico.Ativo,
                DataCadastro = ordemServico.DataCadastro,
                DataAtualizacao = ordemServico.DataAtualizacao,
                ClienteId = ordemServico.ClienteId,
                VeiculoId = ordemServico.VeiculoId,
                ServicoId = ordemServico.ServicoId,
                Descricao = ordemServico.Descricao,
                Status = ordemServico.Status,
                DataEnvioOrcamento = ordemServico.DataEnvioOrcamento,
                Orcamento = ordemServico.Orcamento
            };
        }
        
        #endregion
    }
}
