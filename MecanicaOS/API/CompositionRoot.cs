using Adapters.Controllers;
using Adapters.Gateways;
using Adapters.Presenters;
using Adapters.Presenters.Interfaces;
using Core.DTOs.Entidades.Cliente;
using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Entidades.OrdemServicos;
using Core.DTOs.Entidades.Servico;
using Core.DTOs.Entidades.Usuarios;
using Core.DTOs.Entidades.Veiculo;
using Core.Interfaces.Eventos;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Veiculos;
using Core.Interfaces.Jobs;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Autenticacao;
using Core.UseCases.Autenticacao.AutenticarUsuario;
using Core.UseCases.Clientes;
using Core.UseCases.Clientes.AtualizarCliente;
using Core.UseCases.Clientes.CadastrarCliente;
using Core.UseCases.Clientes.ObterCliente;
using Core.UseCases.Clientes.ObterClientePorDocumento;
using Core.UseCases.Clientes.ObterTodosClientes;
using Core.UseCases.Clientes.RemoverCliente;
using Core.UseCases.Estoques;
using Core.UseCases.Estoques.AtualizarEstoque;
using Core.UseCases.Estoques.CadastrarEstoque;
using Core.UseCases.Estoques.DeletarEstoque;
using Core.UseCases.Estoques.ObterEstoque;
using Core.UseCases.Estoques.ObterEstoqueCritico;
using Core.UseCases.Estoques.ObterTodosEstoques;
using Core.UseCases.InsumosOS;
using Core.UseCases.InsumosOS.CadastrarInsumos;
using Core.UseCases.InsumosOS.DevolverInsumos;
using Core.UseCases.Orcamentos;
using Core.UseCases.Orcamentos.GerarOrcamento;
using Core.UseCases.OrdensServico;
using Core.UseCases.OrdensServico.AceitarOrcamento;
using Core.UseCases.OrdensServico.AtualizarOrdemServico;
using Core.UseCases.OrdensServico.CadastrarOrdemServico;
using Core.UseCases.OrdensServico.ObterOrdemServico;
using Core.UseCases.OrdensServico.ObterOrdemServicoPorStatus;
using Core.UseCases.OrdensServico.ObterTodosOrdensServico;
using Core.UseCases.OrdensServico.RecusarOrcamento;
using Core.UseCases.Servicos;
using Core.UseCases.Servicos.CadastrarServico;
using Core.UseCases.Servicos.DeletarServico;
using Core.UseCases.Servicos.EditarServico;
using Core.UseCases.Servicos.ObterServico;
using Core.UseCases.Servicos.ObterServicoPorNome;
using Core.UseCases.Servicos.ObterServicosDisponiveis;
using Core.UseCases.Servicos.ObterTodosServicos;
using Core.UseCases.Usuarios;
using Core.UseCases.Usuarios.AtualizarUsuario;
using Core.UseCases.Usuarios.CadastrarUsuario;
using Core.UseCases.Usuarios.DeletarUsuario;
using Core.UseCases.Usuarios.ObterTodosUsuarios;
using Core.UseCases.Usuarios.ObterUsuario;
using Core.UseCases.Usuarios.ObterUsuarioPorEmail;
using Core.UseCases.Veiculos;
using Core.UseCases.Veiculos.AtualizarVeiculo;
using Core.UseCases.Veiculos.CadastrarVeiculo;
using Core.UseCases.Veiculos.DeletarVeiculo;
using Core.UseCases.Veiculos.ObterTodosVeiculos;
using Core.UseCases.Veiculos.ObterVeiculo;
using Core.UseCases.Veiculos.ObterVeiculoPorCliente;
using Core.UseCases.Veiculos.ObterVeiculoPorPlaca;
using Infraestrutura.Autenticacao;
using Infraestrutura.Dados;
using Infraestrutura.Dados.UdT;
using Infraestrutura.Jobs;
using Infraestrutura.Logs;
using Infraestrutura.Notificacoes;
using Infraestrutura.Repositorios;
using Infraestrutura.Servicos;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace API
{
    public class CompositionRoot
    {
        // Contexto de banco de dados
        private readonly MecanicaContexto _dbContext;

        // Repositórios
        private readonly IRepositorio<ClienteEntityDto> _repositorioCliente;
        private readonly IRepositorio<EnderecoEntityDto> _repositorioEndereco;
        private readonly IRepositorio<ContatoEntityDto> _repositorioContato;
        private readonly IRepositorio<OrdemServicoEntityDto> _repositorioOrdemServico;
        private readonly IRepositorio<InsumoOSEntityDto> _repositorioInsumoOS;
        private readonly IRepositorio<EstoqueEntityDto> _repositorioEstoque;
        private readonly IRepositorio<ServicoEntityDto> _repositorioServico;
        private readonly IRepositorio<UsuarioEntityDto> _repositorioUsuario;
        private readonly IRepositorio<VeiculoEntityDto> _repositorioVeiculo;
        private readonly IRepositorio<AlertaEstoqueEntityDto> _repositorioAlertaEstoque;

        // Serviços
        private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;
        private readonly IUsuarioLogadoServico _usuarioLogadoServico;
        private readonly IIdCorrelacionalService _idCorrelacionalService;
        private readonly IVerificarEstoqueJob _verificarEstoqueJob;
        private readonly IServicoEmail _servicoEmail;
        private readonly IMediator _mediator;

        // Loggers
        private readonly ILogger<VerificarEstoqueJob> _loggerVerificarEstoqueJob;

        // Construtor
        public CompositionRoot(MecanicaContexto contexto,
            Mediator mediator,
            IIdCorrelacionalService idCorrelacionalService,
            HttpContextAccessor httpContext,
            IConfiguration configuration)
        {
            _dbContext = contexto;
            _mediator = mediator;

            // Criando unidade de trabalho
            _unidadeDeTrabalho = new UnidadeDeTrabalho(_dbContext);

            // Criando repositórios
            _repositorioCliente = new Repositorio<ClienteEntityDto>(_dbContext);
            _repositorioEndereco = new Repositorio<EnderecoEntityDto>(_dbContext);
            _repositorioContato = new Repositorio<ContatoEntityDto>(_dbContext);
            _repositorioOrdemServico = new Repositorio<OrdemServicoEntityDto>(_dbContext);
            _repositorioInsumoOS = new Repositorio<InsumoOSEntityDto>(_dbContext);
            _repositorioEstoque = new Repositorio<EstoqueEntityDto>(_dbContext);
            _repositorioServico = new Repositorio<ServicoEntityDto>(_dbContext);
            _repositorioUsuario = new Repositorio<UsuarioEntityDto>(_dbContext);
            _repositorioVeiculo = new Repositorio<VeiculoEntityDto>(_dbContext);
            _repositorioAlertaEstoque = new Repositorio<AlertaEstoqueEntityDto>(_dbContext);

            // Criando loggers (usando NullLogger para simplificar)
            _loggerVerificarEstoqueJob = NullLogger<VerificarEstoqueJob>.Instance;


            _idCorrelacionalService = idCorrelacionalService;

            _usuarioLogadoServico = new UsuarioLogadoServico(httpContext, _repositorioUsuario);

            // Criando gateways para o job
            var alertaEstoqueGateway = new AlertaEstoqueGateway(_repositorioAlertaEstoque);
            var estoqueGateway = new EstoqueGateway(_repositorioEstoque);
            var usuarioGateway = new UsuarioGateway(_repositorioUsuario);

            // Criando logger para o job
            var logServicoVerificarEstoqueJob = new LogServico<VerificarEstoqueJob>(
                _idCorrelacionalService,
                _loggerVerificarEstoqueJob,
                _usuarioLogadoServico);

            _servicoEmail = new ServicoEmail(configuration);

            // Criando o job real
            _verificarEstoqueJob = new VerificarEstoqueJob(
                alertaEstoqueGateway,
                _servicoEmail,
                logServicoVerificarEstoqueJob,
                _unidadeDeTrabalho,
                estoqueGateway,
                usuarioGateway);
        }

        #region Criação de Gateways

        // Cliente e relacionados
        public IClienteGateway CreateClienteGateway()
        {
            return new ClienteGateway(_repositorioCliente);
        }

        public IEnderecoGateway CreateEnderecoGateway()
        {
            return new EnderecoGateway(_repositorioEndereco);
        }

        public IContatoGateway CreateContatoGateway()
        {
            return new ContatoGateway(_repositorioContato);
        }

        // Ordem de Serviço e relacionados
        public IOrdemServicoGateway CreateOrdemServicoGateway()
        {
            return new OrdemServicoGateway(_repositorioOrdemServico);
        }

        public IInsumosGateway CreateInsumosGateway()
        {
            return new InsumosGateway(_repositorioInsumoOS);
        }

        // Estoque
        public IEstoqueGateway CreateEstoqueGateway()
        {
            return new EstoqueGateway(_repositorioEstoque);
        }

        public IAlertaEstoqueGateway CreateAlertaEstoqueGateway()
        {
            return new AlertaEstoqueGateway(_repositorioAlertaEstoque);
        }

        public IVerificarEstoqueJobGateway CreateVerificarEstoqueJobGateway()
        {
            return new VerificarEstoqueJobGateway(_verificarEstoqueJob);
        }

        // Serviço
        public IServicoGateway CreateServicoGateway()
        {
            return new ServicoGateway(_repositorioServico);
        }

        // Usuário
        public IUsuarioGateway CreateUsuarioGateway()
        {
            return new UsuarioGateway(_repositorioUsuario);
        }

        // Veículo
        public IVeiculoGateway CreateVeiculoGateway()
        {
            return new VeiculoGateway(_repositorioVeiculo);
        }

        // Eventos
        public IEventosPublisher CreateEventosPublisher()
        {
            return new EventoPublisher(_mediator);
        }

        public IEventosGateway CreateEventosGateway()
        {
            var eventosPublisher = CreateEventosPublisher();
            return new EventosGateway(eventosPublisher);
        }

        #endregion

        #region Criação de Serviços de Log

        #endregion

        #region Criação de Handlers Individuais - Cliente

        public CadastrarClienteHandler CreateCadastrarClienteHandler()
        {
            var clienteGateway = CreateClienteGateway();
            var enderecoGateway = CreateEnderecoGateway();
            var contatoGateway = CreateContatoGateway();
            var logServico = new LogServico<CadastrarClienteHandler>(_idCorrelacionalService, NullLogger<CadastrarClienteHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarClienteHandler(
                clienteGateway,
                enderecoGateway,
                contatoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public AtualizarClienteHandler CreateAtualizarClienteHandler()
        {
            var clienteGateway = CreateClienteGateway();
            var enderecoGateway = CreateEnderecoGateway();
            var contatoGateway = CreateContatoGateway();
            var logServico = new LogServico<AtualizarClienteHandler>(_idCorrelacionalService, NullLogger<AtualizarClienteHandler>.Instance, _usuarioLogadoServico);

            return new AtualizarClienteHandler(
                clienteGateway,
                enderecoGateway,
                contatoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterClienteHandler CreateObterClienteHandler()
        {
            var clienteGateway = CreateClienteGateway();
            var logServico = new LogServico<ObterClienteHandler>(_idCorrelacionalService, NullLogger<ObterClienteHandler>.Instance, _usuarioLogadoServico);

            return new ObterClienteHandler(
                clienteGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterTodosClientesHandler CreateObterTodosClientesHandler()
        {
            var clienteGateway = CreateClienteGateway();
            var logServico = new LogServico<ObterTodosClientesHandler>(_idCorrelacionalService, NullLogger<ObterTodosClientesHandler>.Instance, _usuarioLogadoServico);

            return new ObterTodosClientesHandler(
                clienteGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public RemoverClienteHandler CreateRemoverClienteHandler()
        {
            var clienteGateway = CreateClienteGateway();
            var logServico = new LogServico<RemoverClienteHandler>(_idCorrelacionalService, NullLogger<RemoverClienteHandler>.Instance, _usuarioLogadoServico);

            return new RemoverClienteHandler(
                clienteGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterClientePorDocumentoHandler CreateObterClientePorDocumentoHandler()
        {
            var clienteGateway = CreateClienteGateway();
            var logServico = new LogServico<ObterClientePorDocumentoHandler>(_idCorrelacionalService, NullLogger<ObterClientePorDocumentoHandler>.Instance, _usuarioLogadoServico);

            return new ObterClientePorDocumentoHandler(
                clienteGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - Veículo

        public ICadastrarVeiculoHandler CreateCadastrarVeiculoHandler()
        {
            var veiculoGateway = CreateVeiculoGateway();
            var logServico = new LogServico<CadastrarVeiculoHandler>(_idCorrelacionalService, NullLogger<CadastrarVeiculoHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarVeiculoHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IAtualizarVeiculoHandler CreateAtualizarVeiculoHandler()
        {
            var veiculoGateway = CreateVeiculoGateway();
            var logServico = new LogServico<AtualizarVeiculoHandler>(_idCorrelacionalService, NullLogger<AtualizarVeiculoHandler>.Instance, _usuarioLogadoServico);

            return new AtualizarVeiculoHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterVeiculoHandler CreateObterVeiculoHandler()
        {
            var veiculoGateway = CreateVeiculoGateway();
            var logServico = new LogServico<ObterVeiculoHandler>(_idCorrelacionalService, NullLogger<ObterVeiculoHandler>.Instance, _usuarioLogadoServico);

            return new ObterVeiculoHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterTodosVeiculosHandler CreateObterTodosVeiculosHandler()
        {
            var veiculoGateway = CreateVeiculoGateway();
            var logServico = new LogServico<ObterTodosVeiculosHandler>(_idCorrelacionalService, NullLogger<ObterTodosVeiculosHandler>.Instance, _usuarioLogadoServico);

            return new ObterTodosVeiculosHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterVeiculoPorClienteHandler CreateObterVeiculoPorClienteHandler()
        {
            var veiculoGateway = CreateVeiculoGateway();
            var logServico = new LogServico<ObterVeiculoPorClienteHandler>(_idCorrelacionalService, NullLogger<ObterVeiculoPorClienteHandler>.Instance, _usuarioLogadoServico);

            return new ObterVeiculoPorClienteHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterVeiculoPorPlacaHandler CreateObterVeiculoPorPlacaHandler()
        {
            var veiculoGateway = CreateVeiculoGateway();
            var logServico = new LogServico<ObterVeiculoPorPlacaHandler>(_idCorrelacionalService, NullLogger<ObterVeiculoPorPlacaHandler>.Instance, _usuarioLogadoServico);

            return new ObterVeiculoPorPlacaHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IDeletarVeiculoHandler CreateDeletarVeiculoHandler()
        {
            var veiculoGateway = CreateVeiculoGateway();
            var logServico = new LogServico<DeletarVeiculoHandler>(_idCorrelacionalService, NullLogger<DeletarVeiculoHandler>.Instance, _usuarioLogadoServico);

            return new DeletarVeiculoHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - Usuário

        public CadastrarUsuarioHandler CreateCadastrarUsuarioHandler()
        {
            var usuarioGateway = CreateUsuarioGateway();
            var clienteUseCases = CreateClienteUseCases();
            var servicoSenha = new ServicoSenha();
            var logServico = new LogServico<CadastrarUsuarioHandler>(_idCorrelacionalService, NullLogger<CadastrarUsuarioHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarUsuarioHandler(
                usuarioGateway,
                clienteUseCases,
                servicoSenha,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public AtualizarUsuarioHandler CreateAtualizarUsuarioHandler()
        {
            var usuarioGateway = CreateUsuarioGateway();
            var servicoSenha = new ServicoSenha();
            var logServico = new LogServico<AtualizarUsuarioHandler>(_idCorrelacionalService, NullLogger<AtualizarUsuarioHandler>.Instance, _usuarioLogadoServico);

            return new AtualizarUsuarioHandler(
                usuarioGateway,
                servicoSenha,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterUsuarioHandler CreateObterUsuarioHandler()
        {
            var usuarioGateway = CreateUsuarioGateway();
            var logServico = new LogServico<ObterUsuarioHandler>(_idCorrelacionalService, NullLogger<ObterUsuarioHandler>.Instance, _usuarioLogadoServico);

            return new ObterUsuarioHandler(
                usuarioGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterTodosUsuariosHandler CreateObterTodosUsuariosHandler()
        {
            var usuarioGateway = CreateUsuarioGateway();
            var logServico = new LogServico<ObterTodosUsuariosHandler>(_idCorrelacionalService, NullLogger<ObterTodosUsuariosHandler>.Instance, _usuarioLogadoServico);

            return new ObterTodosUsuariosHandler(
                usuarioGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public DeletarUsuarioHandler CreateDeletarUsuarioHandler()
        {
            var usuarioGateway = CreateUsuarioGateway();
            var logServico = new LogServico<DeletarUsuarioHandler>(_idCorrelacionalService, NullLogger<DeletarUsuarioHandler>.Instance, _usuarioLogadoServico);

            return new DeletarUsuarioHandler(
                usuarioGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterUsuarioPorEmailHandler CreateObterUsuarioPorEmailHandler()
        {
            var usuarioGateway = CreateUsuarioGateway();
            var logServico = new LogServico<ObterUsuarioPorEmailHandler>(_idCorrelacionalService, NullLogger<ObterUsuarioPorEmailHandler>.Instance, _usuarioLogadoServico);

            return new ObterUsuarioPorEmailHandler(
                usuarioGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - Serviço

        public CadastrarServicoHandler CreateCadastrarServicoHandler()
        {
            var servicoGateway = CreateServicoGateway();
            var logServico = new LogServico<CadastrarServicoHandler>(_idCorrelacionalService, NullLogger<CadastrarServicoHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarServicoHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public EditarServicoHandler CreateEditarServicoHandler()
        {
            var servicoGateway = CreateServicoGateway();
            var logServico = new LogServico<EditarServicoHandler>(_idCorrelacionalService, NullLogger<EditarServicoHandler>.Instance, _usuarioLogadoServico);

            return new EditarServicoHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public DeletarServicoHandler CreateDeletarServicoHandler()
        {
            var servicoGateway = CreateServicoGateway();
            var logServico = new LogServico<DeletarServicoHandler>(_idCorrelacionalService, NullLogger<DeletarServicoHandler>.Instance, _usuarioLogadoServico);

            return new DeletarServicoHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterServicoHandler CreateObterServicoHandler()
        {
            var servicoGateway = CreateServicoGateway();
            var logServico = new LogServico<ObterServicoHandler>(_idCorrelacionalService, NullLogger<ObterServicoHandler>.Instance, _usuarioLogadoServico);

            return new ObterServicoHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterTodosServicosHandler CreateObterTodosServicosHandler()
        {
            var servicoGateway = CreateServicoGateway();
            var logServico = new LogServico<ObterTodosServicosHandler>(_idCorrelacionalService, NullLogger<ObterTodosServicosHandler>.Instance, _usuarioLogadoServico);

            return new ObterTodosServicosHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterServicoPorNomeHandler CreateObterServicoPorNomeHandler()
        {
            var servicoGateway = CreateServicoGateway();
            var logServico = new LogServico<ObterServicoPorNomeHandler>(_idCorrelacionalService, NullLogger<ObterServicoPorNomeHandler>.Instance, _usuarioLogadoServico);

            return new ObterServicoPorNomeHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterServicosDisponiveisHandler CreateObterServicosDisponiveisHandler()
        {
            var servicoGateway = CreateServicoGateway();
            var logServico = new LogServico<ObterServicosDisponiveisHandler>(_idCorrelacionalService, NullLogger<ObterServicosDisponiveisHandler>.Instance, _usuarioLogadoServico);

            return new ObterServicosDisponiveisHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - Orçamento

        public GerarOrcamentoHandler CreateGerarOrcamentoHandler()
        {
            var logServico = new LogServico<GerarOrcamentoHandler>(_idCorrelacionalService, NullLogger<GerarOrcamentoHandler>.Instance, _usuarioLogadoServico);

            return new GerarOrcamentoHandler(
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - Estoque

        public CadastrarEstoqueHandler CreateCadastrarEstoqueHandler()
        {
            var estoqueGateway = CreateEstoqueGateway();
            var logServico = new LogServico<CadastrarEstoqueHandler>(_idCorrelacionalService, NullLogger<CadastrarEstoqueHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarEstoqueHandler(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public AtualizarEstoqueHandler CreateAtualizarEstoqueHandler()
        {
            var estoqueGateway = CreateEstoqueGateway();
            var logServico = new LogServico<AtualizarEstoqueHandler>(_idCorrelacionalService, NullLogger<AtualizarEstoqueHandler>.Instance, _usuarioLogadoServico);

            return new AtualizarEstoqueHandler(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public DeletarEstoqueHandler CreateDeletarEstoqueHandler()
        {
            var estoqueGateway = CreateEstoqueGateway();
            var logServico = new LogServico<DeletarEstoqueHandler>(_idCorrelacionalService, NullLogger<DeletarEstoqueHandler>.Instance, _usuarioLogadoServico);

            return new DeletarEstoqueHandler(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterEstoqueHandler CreateObterEstoqueHandler()
        {
            var estoqueGateway = CreateEstoqueGateway();
            var logServico = new LogServico<ObterEstoqueHandler>(_idCorrelacionalService, NullLogger<ObterEstoqueHandler>.Instance, _usuarioLogadoServico);

            return new ObterEstoqueHandler(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterTodosEstoquesHandler CreateObterTodosEstoquesHandler()
        {
            var estoqueGateway = CreateEstoqueGateway();
            var logServico = new LogServico<ObterTodosEstoquesHandler>(_idCorrelacionalService, NullLogger<ObterTodosEstoquesHandler>.Instance, _usuarioLogadoServico);

            return new ObterTodosEstoquesHandler(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterEstoqueCriticoHandler CreateObterEstoqueCriticoHandler()
        {
            var estoqueGateway = CreateEstoqueGateway();
            var logServico = new LogServico<ObterEstoqueCriticoHandler>(_idCorrelacionalService, NullLogger<ObterEstoqueCriticoHandler>.Instance, _usuarioLogadoServico);

            return new ObterEstoqueCriticoHandler(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - Autenticação

        public AutenticarUsuarioHandler CreateAutenticarUsuarioHandler()
        {
            var usuarioUseCases = CreateUsuarioUseCases();
            var clienteUseCases = CreateClienteUseCases();
            var servicoSenha = new ServicoSenha();
            var servicoJwt = CreateServicoJwt();
            var logServico = new LogServico<AutenticarUsuarioHandler>(_idCorrelacionalService, NullLogger<AutenticarUsuarioHandler>.Instance, _usuarioLogadoServico);

            return new AutenticarUsuarioHandler(
                usuarioUseCases,
                servicoSenha,
                servicoJwt,
                logServico,
                clienteUseCases);
        }

        #endregion

        #region Criação de Handlers Individuais - InsumoOS

        public CadastrarInsumosHandler CreateCadastrarInsumosHandler()
        {
            var ordemServicoUseCases = CreateOrdemServicoUseCases();
            var estoqueUseCases = CreateEstoqueUseCases();
            var logServico = new LogServico<CadastrarInsumosHandler>(_idCorrelacionalService, NullLogger<CadastrarInsumosHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarInsumosHandler(
                ordemServicoUseCases,
                estoqueUseCases,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public DevolverInsumosHandler CreateDevolverInsumosHandler()
        {
            var estoqueUseCases = CreateEstoqueUseCases();
            var logServico = new LogServico<DevolverInsumosHandler>(_idCorrelacionalService, NullLogger<DevolverInsumosHandler>.Instance, _usuarioLogadoServico);

            return new DevolverInsumosHandler(
                estoqueUseCases,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - OrdemServico

        public CadastrarOrdemServicoHandler CreateCadastrarOrdemServicoHandler()
        {
            var ordemServicoGateway = CreateOrdemServicoGateway();
            var clienteUseCases = CreateClienteUseCases();
            var servicoUseCases = CreateServicoUseCases();
            var logServico = new LogServico<CadastrarOrdemServicoHandler>(_idCorrelacionalService, NullLogger<CadastrarOrdemServicoHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarOrdemServicoHandler(
                ordemServicoGateway,
                clienteUseCases,
                servicoUseCases,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public AtualizarOrdemServicoHandler CreateAtualizarOrdemServicoHandler()
        {
            var ordemServicoGateway = CreateOrdemServicoGateway();
            var eventosGateway = CreateEventosGateway();
            var logServico = new LogServico<AtualizarOrdemServicoHandler>(_idCorrelacionalService, NullLogger<AtualizarOrdemServicoHandler>.Instance, _usuarioLogadoServico);

            return new AtualizarOrdemServicoHandler(
                ordemServicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterOrdemServicoHandler CreateObterOrdemServicoHandler()
        {
            var ordemServicoGateway = CreateOrdemServicoGateway();
            var logServico = new LogServico<ObterOrdemServicoHandler>(_idCorrelacionalService, NullLogger<ObterOrdemServicoHandler>.Instance, _usuarioLogadoServico);

            return new ObterOrdemServicoHandler(
                ordemServicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterTodosOrdensServicoHandler CreateObterTodosOrdensServicoHandler()
        {
            var ordemServicoGateway = CreateOrdemServicoGateway();
            var logServico = new LogServico<ObterTodosOrdensServicoHandler>(_idCorrelacionalService, NullLogger<ObterTodosOrdensServicoHandler>.Instance, _usuarioLogadoServico);

            return new ObterTodosOrdensServicoHandler(
                ordemServicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public ObterOrdemServicoPorStatusHandler CreateObterOrdemServicoPorStatusHandler()
        {
            var ordemServicoGateway = CreateOrdemServicoGateway();
            var logServico = new LogServico<ObterOrdemServicoPorStatusHandler>(_idCorrelacionalService, NullLogger<ObterOrdemServicoPorStatusHandler>.Instance, _usuarioLogadoServico);

            return new ObterOrdemServicoPorStatusHandler(
                ordemServicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public AceitarOrcamentoHandler CreateAceitarOrcamentoHandler()
        {
            var ordemServicoGateway = CreateOrdemServicoGateway();
            var logServico = new LogServico<AceitarOrcamentoHandler>(_idCorrelacionalService, NullLogger<AceitarOrcamentoHandler>.Instance, _usuarioLogadoServico);

            return new AceitarOrcamentoHandler(
                ordemServicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public RecusarOrcamentoHandler CreateRecusarOrcamentoHandler()
        {
            var ordemServicoGateway = CreateOrdemServicoGateway();
            var logServico = new LogServico<RecusarOrcamentoHandler>(_idCorrelacionalService, NullLogger<RecusarOrcamentoHandler>.Instance, _usuarioLogadoServico);

            return new RecusarOrcamentoHandler(
                ordemServicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Use Cases

        // Cliente - Usando novo padrão com facade
        public IClienteUseCases CreateClienteUseCases()
        {
            var cadastrarClienteHandler = CreateCadastrarClienteHandler();
            var atualizarClienteHandler = CreateAtualizarClienteHandler();
            var obterClienteHandler = CreateObterClienteHandler();
            var obterTodosClientesHandler = CreateObterTodosClientesHandler();
            var removerClienteHandler = CreateRemoverClienteHandler();
            var obterClientePorDocumentoHandler = CreateObterClientePorDocumentoHandler();

            return new ClienteUseCasesFacade(
                cadastrarClienteHandler,
                atualizarClienteHandler,
                obterClienteHandler,
                obterTodosClientesHandler,
                removerClienteHandler,
                obterClientePorDocumentoHandler);
        }

        // Serviço - Usando novo padrão com facade
        public IServicoUseCases CreateServicoUseCases()
        {
            var cadastrarServicoHandler = CreateCadastrarServicoHandler();
            var editarServicoHandler = CreateEditarServicoHandler();
            var deletarServicoHandler = CreateDeletarServicoHandler();
            var obterServicoHandler = CreateObterServicoHandler();
            var obterTodosServicosHandler = CreateObterTodosServicosHandler();
            var obterServicoPorNomeHandler = CreateObterServicoPorNomeHandler();
            var obterServicosDisponiveisHandler = CreateObterServicosDisponiveisHandler();

            return new ServicoUseCasesFacade(
                cadastrarServicoHandler,
                editarServicoHandler,
                deletarServicoHandler,
                obterServicoHandler,
                obterTodosServicosHandler,
                obterServicoPorNomeHandler,
                obterServicosDisponiveisHandler);
        }

        // Estoque - Usando novo padrão com facade completo
        public IEstoqueUseCases CreateEstoqueUseCases()
        {
            var cadastrarEstoqueHandler = CreateCadastrarEstoqueHandler();
            var atualizarEstoqueHandler = CreateAtualizarEstoqueHandler();
            var deletarEstoqueHandler = CreateDeletarEstoqueHandler();
            var obterEstoqueHandler = CreateObterEstoqueHandler();
            var obterTodosEstoquesHandler = CreateObterTodosEstoquesHandler();
            var obterEstoqueCriticoHandler = CreateObterEstoqueCriticoHandler();

            return new EstoqueUseCasesFacade(
                cadastrarEstoqueHandler,
                atualizarEstoqueHandler,
                deletarEstoqueHandler,
                obterEstoqueHandler,
                obterTodosEstoquesHandler,
                obterEstoqueCriticoHandler);
        }

        // Ordem de Serviço - Usando novo padrão com facade
        public IOrdemServicoUseCases CreateOrdemServicoUseCases()
        {
            var cadastrarOrdemServicoHandler = CreateCadastrarOrdemServicoHandler();
            var atualizarOrdemServicoHandler = CreateAtualizarOrdemServicoHandler();
            var obterOrdemServicoHandler = CreateObterOrdemServicoHandler();
            var obterTodosOrdensServicoHandler = CreateObterTodosOrdensServicoHandler();
            var obterOrdemServicoPorStatusHandler = CreateObterOrdemServicoPorStatusHandler();
            var aceitarOrcamentoHandler = CreateAceitarOrcamentoHandler();
            var recusarOrcamentoHandler = CreateRecusarOrcamentoHandler();

            return new OrdemServicoUseCasesFacade(
                cadastrarOrdemServicoHandler,
                atualizarOrdemServicoHandler,
                obterOrdemServicoHandler,
                obterTodosOrdensServicoHandler,
                obterOrdemServicoPorStatusHandler,
                aceitarOrcamentoHandler,
                recusarOrcamentoHandler);
        }

        // Insumo OS
        public IInsumoOSUseCases CreateInsumoOSUseCases()
        {
            var cadastrarInsumosHandler = CreateCadastrarInsumosHandler();
            var devolverInsumosHandler = CreateDevolverInsumosHandler();

            return new InsumoOSUseCasesFacade(
                cadastrarInsumosHandler,
                devolverInsumosHandler);
        }

        // Usuário - Usando novo padrão com facade
        public IUsuarioUseCases CreateUsuarioUseCases()
        {
            var cadastrarUsuarioHandler = CreateCadastrarUsuarioHandler();
            var atualizarUsuarioHandler = CreateAtualizarUsuarioHandler();
            var obterUsuarioHandler = CreateObterUsuarioHandler();
            var obterTodosUsuariosHandler = CreateObterTodosUsuariosHandler();
            var deletarUsuarioHandler = CreateDeletarUsuarioHandler();
            var obterUsuarioPorEmailHandler = CreateObterUsuarioPorEmailHandler();

            return new UsuarioUseCasesFacade(
                cadastrarUsuarioHandler,
                atualizarUsuarioHandler,
                obterUsuarioHandler,
                obterTodosUsuariosHandler,
                deletarUsuarioHandler,
                obterUsuarioPorEmailHandler);
        }

        // Veículo - Usando novo padrão com facade
        public IVeiculoUseCases CreateVeiculoUseCases()
        {
            var cadastrarVeiculoHandler = CreateCadastrarVeiculoHandler();
            var atualizarVeiculoHandler = CreateAtualizarVeiculoHandler();
            var obterVeiculoHandler = CreateObterVeiculoHandler();
            var obterTodosVeiculosHandler = CreateObterTodosVeiculosHandler();
            var obterVeiculoPorClienteHandler = CreateObterVeiculoPorClienteHandler();
            var obterVeiculoPorPlacaHandler = CreateObterVeiculoPorPlacaHandler();
            var deletarVeiculoHandler = CreateDeletarVeiculoHandler();

            return new VeiculoUseCasesFacade(
                cadastrarVeiculoHandler,
                atualizarVeiculoHandler,
                obterVeiculoHandler,
                obterTodosVeiculosHandler,
                obterVeiculoPorClienteHandler,
                obterVeiculoPorPlacaHandler,
                deletarVeiculoHandler);
        }

        // Orçamento - Usando novo padrão com facade
        public IOrcamentoUseCases CreateOrcamentoUseCases()
        {
            var gerarOrcamentoHandler = CreateGerarOrcamentoHandler();
            return new OrcamentoUseCasesFacade(gerarOrcamentoHandler);
        }

        // Serviços de autenticação
        public IOptions<ConfiguracaoJwt> CreateConfiguracaoJwt()
        {
            // Criar uma configuração JWT com valores padrão
            var configuracao = new ConfiguracaoJwt
            {
                SecretKey = "chave-secreta-mecanica-os-para-testes-locais",
                Issuer = "MecanicaOS",
                Audience = "MecanicaOS.Users",
                ExpiryInMinutes = 60
            };

            return Options.Create(configuracao);
        }

        public IServicoJwt CreateServicoJwt()
        {
            var configuracaoJwt = CreateConfiguracaoJwt();
            return new ServicoJwt(configuracaoJwt);
        }

        // Autenticação - Usando novo padrão com facade
        public IAutenticacaoUseCases CreateAutenticacaoUseCases()
        {
            var autenticarUsuarioHandler = CreateAutenticarUsuarioHandler();
            return new AutenticacaoUseCasesFacade(autenticarUsuarioHandler);
        }

        #endregion

        #region Criação de Presenters

        public IClientePresenter CreateClientePresenter()
        {
            return new ClientePresenter();
        }

        public IOrdemServicoPresenter CreateOrdemServicoPresenter()
        {
            return new OrdemServicoPresenter();
        }

        public IServicoPresenter CreateServicoPresenter()
        {
            return new ServicoPresenter();
        }

        public IEstoquePresenter CreateEstoquePresenter()
        {
            return new EstoquePresenter();
        }

        public IUsuarioPresenter CreateUsuarioPresenter()
        {
            return new UsuarioPresenter();
        }

        public IVeiculoPresenter CreateVeiculoPresenter()
        {
            return new VeiculoPresenter();
        }

        public IAutenticacaoPresenter CreateAutenticacaoPresenter()
        {
            return new AutenticacaoPresenter();
        }

        #endregion

        #region Criação de Serviços

        public IUsuarioLogadoServico CreateUsuarioLogadoServico()
        {
            return _usuarioLogadoServico;
        }

        #endregion

        #region Criação de Controllers

        public Adapters.Controllers.ClienteController CreateClienteController()
        {
            var clienteUseCases = CreateClienteUseCases();
            var clientePresenter = CreateClientePresenter();

            return new Adapters.Controllers.ClienteController(clienteUseCases, clientePresenter);
        }

        public Adapters.Controllers.OrdemServicoController CreateOrdemServicoController()
        {
            var ordemServicoUseCases = CreateOrdemServicoUseCases();
            var ordemServicoPresenter = CreateOrdemServicoPresenter();

            return new Adapters.Controllers.OrdemServicoController(ordemServicoUseCases, ordemServicoPresenter);
        }

        public InsumoOSController CreateInsumoOSController()
        {
            var insumoOSUseCases = CreateInsumoOSUseCases();
            var ordemServicoPresenter = CreateOrdemServicoPresenter();

            return new InsumoOSController(insumoOSUseCases, ordemServicoPresenter);
        }

        public Adapters.Controllers.ServicoController CreateServicoController()
        {
            var servicoUseCases = CreateServicoUseCases();
            var servicoPresenter = CreateServicoPresenter();

            return new Adapters.Controllers.ServicoController(servicoUseCases, servicoPresenter);
        }

        public Adapters.Controllers.EstoqueController CreateEstoqueController()
        {
            var estoqueUseCases = CreateEstoqueUseCases();
            var estoquePresenter = CreateEstoquePresenter();

            return new Adapters.Controllers.EstoqueController(estoqueUseCases, estoquePresenter);
        }

        public Adapters.Controllers.UsuarioController CreateUsuarioController()
        {
            var usuarioUseCases = CreateUsuarioUseCases();
            var usuarioPresenter = CreateUsuarioPresenter();

            return new Adapters.Controllers.UsuarioController(usuarioUseCases, usuarioPresenter);
        }

        public Adapters.Controllers.VeiculoController CreateVeiculoController()
        {
            var veiculoUseCases = CreateVeiculoUseCases();
            var veiculoPresenter = CreateVeiculoPresenter();

            return new Adapters.Controllers.VeiculoController(veiculoUseCases, veiculoPresenter);
        }

        public Adapters.Controllers.AutenticacaoController CreateAutenticacaoController()
        {
            var autenticacaoUseCases = CreateAutenticacaoUseCases();
            var autenticacaoPresenter = CreateAutenticacaoPresenter();

            return new Adapters.Controllers.AutenticacaoController(autenticacaoUseCases, autenticacaoPresenter);
        }

        #endregion
    }
}
