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
using Core.Interfaces.Jobs;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases;
using Infraestrutura.Autenticacao;
using Infraestrutura.Dados;
using Infraestrutura.Dados.UdT;
using Infraestrutura.Jobs;
using Infraestrutura.Logs;
using Infraestrutura.Notificacoes;
using Infraestrutura.Repositorios;
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
        private readonly IMediator _mediator;

        // Loggers
        private readonly ILogger<ClienteUseCases> _loggerClienteUseCases;
        private readonly ILogger<OrdemServicoUseCases> _loggerOrdemServicoUseCases;
        private readonly ILogger<EstoqueUseCases> _loggerEstoqueUseCases;
        private readonly ILogger<InsumoOSUseCases> _loggerInsumoOSUseCases;
        private readonly ILogger<ServicoUseCases> _loggerServicoUseCases;
        private readonly ILogger<UsuarioUseCases> _loggerUsuarioUseCases;
        private readonly ILogger<VeiculoUseCases> _loggerVeiculoUseCases;
        private readonly ILogger<AutenticacaoUseCases> _loggerAutenticacaoUseCases;
        private readonly ILogger<VerificarEstoqueJob> _loggerVerificarEstoqueJob;

        // Construtor
        public CompositionRoot(MecanicaContexto contexto,
            Mediator mediator,
            IServicoEmail servicoEmail,
            IIdCorrelacionalService idCorrelacionalService,
            HttpContextAccessor httpContext)
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
            _loggerClienteUseCases = NullLogger<ClienteUseCases>.Instance;
            _loggerOrdemServicoUseCases = NullLogger<OrdemServicoUseCases>.Instance;
            _loggerEstoqueUseCases = NullLogger<EstoqueUseCases>.Instance;
            _loggerInsumoOSUseCases = NullLogger<InsumoOSUseCases>.Instance;
            _loggerServicoUseCases = NullLogger<ServicoUseCases>.Instance;
            _loggerUsuarioUseCases = NullLogger<UsuarioUseCases>.Instance;
            _loggerVeiculoUseCases = NullLogger<VeiculoUseCases>.Instance;
            _loggerAutenticacaoUseCases = NullLogger<AutenticacaoUseCases>.Instance;
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

            // Criando o job real
            _verificarEstoqueJob = new VerificarEstoqueJob(
                alertaEstoqueGateway,
                servicoEmail,
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

        public ILogServico<ClienteUseCases> CreateClienteLogServico()
        {
            return new LogServico<ClienteUseCases>(_idCorrelacionalService, _loggerClienteUseCases, _usuarioLogadoServico);
        }

        public ILogServico<OrdemServicoUseCases> CreateOrdemServicoLogServico()
        {
            return new LogServico<OrdemServicoUseCases>(_idCorrelacionalService, _loggerOrdemServicoUseCases, _usuarioLogadoServico);
        }

        public ILogServico<EstoqueUseCases> CreateEstoqueLogServico()
        {
            return new LogServico<EstoqueUseCases>(_idCorrelacionalService, _loggerEstoqueUseCases, _usuarioLogadoServico);
        }

        public ILogServico<InsumoOSUseCases> CreateInsumoOSLogServico()
        {
            return new LogServico<InsumoOSUseCases>(_idCorrelacionalService, _loggerInsumoOSUseCases, _usuarioLogadoServico);
        }

        public ILogServico<ServicoUseCases> CreateServicoLogServico()
        {
            return new LogServico<ServicoUseCases>(_idCorrelacionalService, _loggerServicoUseCases, _usuarioLogadoServico);
        }

        public ILogServico<UsuarioUseCases> CreateUsuarioLogServico()
        {
            return new LogServico<UsuarioUseCases>(_idCorrelacionalService, _loggerUsuarioUseCases, _usuarioLogadoServico);
        }

        public ILogServico<VeiculoUseCases> CreateVeiculoLogServico()
        {
            return new LogServico<VeiculoUseCases>(_idCorrelacionalService, _loggerVeiculoUseCases, _usuarioLogadoServico);
        }

        public ILogServico<AutenticacaoUseCases> CreateAutenticacaoLogServico()
        {
            return new LogServico<AutenticacaoUseCases>(_idCorrelacionalService, _loggerAutenticacaoUseCases, _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Use Cases

        // Cliente
        public IClienteUseCases CreateClienteUseCases()
        {
            var clienteGateway = CreateClienteGateway();
            var enderecoGateway = CreateEnderecoGateway();
            var contatoGateway = CreateContatoGateway();
            var logServico = CreateClienteLogServico();

            return new ClienteUseCases(
                clienteGateway,
                enderecoGateway,
                contatoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        // Serviço
        public IServicoUseCases CreateServicoUseCases()
        {
            var servicoGateway = CreateServicoGateway();
            var logServico = CreateServicoLogServico();

            return new ServicoUseCases(
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico,
                servicoGateway);
        }

        // Estoque
        public IEstoqueUseCases CreateEstoqueUseCases()
        {
            var estoqueGateway = CreateEstoqueGateway();
            var logServico = CreateEstoqueLogServico();

            return new EstoqueUseCases(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        // Ordem de Serviço
        public IOrdemServicoUseCases CreateOrdemServicoUseCases()
        {
            var ordemServicoGateway = CreateOrdemServicoGateway();
            var logServico = CreateOrdemServicoLogServico();
            var clienteGateway = CreateClienteGateway();
            var servicoUseCases = CreateServicoUseCases();
            var eventosGateway = CreateEventosGateway();

            return new OrdemServicoUseCases(
                logServico,
                _unidadeDeTrabalho,
                clienteGateway,
                servicoUseCases,
                _usuarioLogadoServico,
                ordemServicoGateway,
                eventosGateway);
        }

        // Insumo OS
        public IInsumoOSUseCases CreateInsumoOSUseCases()
        {
            var ordemServicoUseCases = CreateOrdemServicoUseCases();
            var estoqueUseCases = CreateEstoqueUseCases();
            var insumosGateway = CreateInsumosGateway();
            var logServico = CreateInsumoOSLogServico();
            var verificarEstoqueJobGateway = CreateVerificarEstoqueJobGateway();

            return new InsumoOSUseCases(
                ordemServicoUseCases,
                estoqueUseCases,
                insumosGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico,
                verificarEstoqueJobGateway);
        }

        // Usuário
        public IUsuarioUseCases CreateUsuarioUseCases()
        {
            var usuarioGateway = CreateUsuarioGateway();
            var logServico = CreateUsuarioLogServico();
            var clienteUseCases = CreateClienteUseCases();
            var servicoSenha = new ServicoSenha();

            return new UsuarioUseCases(
                logServico,
                _unidadeDeTrabalho,
                clienteUseCases,
                servicoSenha,
                _usuarioLogadoServico,
                usuarioGateway);
        }

        // Veículo
        public IVeiculoUseCases CreateVeiculoUseCases()
        {
            var veiculoGateway = CreateVeiculoGateway();
            var logServico = CreateVeiculoLogServico();

            return new VeiculoUseCases(
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico,
                veiculoGateway);
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

        // Autenticação
        public IAutenticacaoUseCases CreateAutenticacaoUseCases()
        {
            var usuarioUseCases = CreateUsuarioUseCases();
            var servicoSenha = new ServicoSenha();
            var servicoJwt = CreateServicoJwt();
            var logServico = CreateAutenticacaoLogServico();
            var clienteUseCases = CreateClienteUseCases();

            return new AutenticacaoUseCases(
                usuarioUseCases,
                servicoSenha,
                servicoJwt,
                logServico,
                clienteUseCases);
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
