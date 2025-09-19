using Adapters.Controllers;
using Adapters.Gateways;
using Adapters.Presenters;
using API.Jobs;
using Core.DTOs.Config;
using Core.DTOs.Entidades.Autenticacao;
using Core.DTOs.Entidades.Cliente;
using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Entidades.OrdemServicos;
using Core.DTOs.Entidades.Servico;
using Core.DTOs.Entidades.Usuarios;
using Core.DTOs.Entidades.Veiculo;
using Core.Interfaces.Controllers;
using Core.Interfaces.Eventos;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Autenticacao;
using Core.Interfaces.Handlers.Clientes;
using Core.Interfaces.Handlers.Estoques;
using Core.Interfaces.Handlers.InsumosOS;
using Core.Interfaces.Handlers.Orcamentos;
using Core.Interfaces.Handlers.OrdensServico;
using Core.Interfaces.Handlers.Servicos;
using Core.Interfaces.Handlers.Usuarios;
using Core.Interfaces.Handlers.Veiculos;
using Core.Interfaces.Jobs;
using Core.Interfaces.Presenters;
using Core.Interfaces.Repositorios;
using Core.Interfaces.root;
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
using Infraestrutura.Logs;
using API.Notificacoes;
using Infraestrutura.Repositorios;
using Infraestrutura.Servicos;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;

namespace API
{
    public class CompositionRoot : ICompositionRoot
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
            IMediator mediator,
            IIdCorrelacionalService idCorrelacionalService,
            IHttpContextAccessor httpContext,
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
            _verificarEstoqueJob = new VerificarEstoqueJob(this);
        }

        public IRepositorio<T> CriarRepositorio<T>() where T : EntityDto
        {
            return new Repositorio<T>(_dbContext);
        }

        public ILogServico<T> CriarLogService<T>() where T : class
        {
            return new LogServico<T>(_idCorrelacionalService, NullLogger<T>.Instance, _usuarioLogadoServico);
        }

        public IServicoEmail CriarServicoEmail()
        {
            return _servicoEmail;
        }

        public IUnidadeDeTrabalho CriarUnidadeDeTrabalho()
        {
            return _unidadeDeTrabalho;
        }

        #region Criação de Gateways

        // Cliente e relacionados
        public IClienteGateway CriarClienteGateway()
        {
            return new ClienteGateway(_repositorioCliente);
        }

        public IEnderecoGateway CriarEnderecoGateway()
        {
            return new EnderecoGateway(_repositorioEndereco);
        }

        public IContatoGateway CriarContatoGateway()
        {
            return new ContatoGateway(_repositorioContato);
        }

        // Ordem de Serviço e relacionados
        public IOrdemServicoGateway CriarOrdemServicoGateway()
        {
            return new OrdemServicoGateway(_repositorioOrdemServico);
        }

        public IInsumosGateway CriarInsumosGateway()
        {
            return new InsumosGateway(_repositorioInsumoOS);
        }

        // Estoque
        public IEstoqueGateway CriarEstoqueGateway()
        {
            return new EstoqueGateway(_repositorioEstoque);
        }

        public IAlertaEstoqueGateway CriarAlertaEstoqueGateway()
        {
            return new AlertaEstoqueGateway(_repositorioAlertaEstoque);
        }

        public IVerificarEstoqueJobGateway CriarVerificarEstoqueJobGateway()
        {
            return new VerificarEstoqueJobGateway(_verificarEstoqueJob);
        }

        // Serviço
        public IServicoGateway CriarServicoGateway()
        {
            return new ServicoGateway(_repositorioServico);
        }

        // Usuário
        public IUsuarioGateway CriarUsuarioGateway()
        {
            return new UsuarioGateway(_repositorioUsuario);
        }

        // Veículo
        public IVeiculoGateway CriarVeiculoGateway()
        {
            return new VeiculoGateway(_repositorioVeiculo);
        }

        // Eventos
        public IEventosPublisher CriarEventosPublisher()
        {
            return new EventoPublisher(_mediator);
        }

        public IEventosGateway CriarEventosGateway()
        {
            var eventosPublisher = CriarEventosPublisher();
            return new EventosGateway(eventosPublisher);
        }

        #endregion

        #region Criação de Serviços de Log

        #endregion

        #region Criação de Handlers Individuais - Cliente

        public ICadastrarClienteHandler CriarCadastrarClienteHandler()
        {
            var clienteGateway = CriarClienteGateway();
            var enderecoGateway = CriarEnderecoGateway();
            var contatoGateway = CriarContatoGateway();
            var logServico = new LogServico<CadastrarClienteHandler>(_idCorrelacionalService, NullLogger<CadastrarClienteHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarClienteHandler(
                clienteGateway,
                enderecoGateway,
                contatoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IAtualizarClienteHandler CriarAtualizarClienteHandler()
        {
            var clienteGateway = CriarClienteGateway();
            var enderecoGateway = CriarEnderecoGateway();
            var contatoGateway = CriarContatoGateway();
            var logServico = new LogServico<AtualizarClienteHandler>(_idCorrelacionalService, NullLogger<AtualizarClienteHandler>.Instance, _usuarioLogadoServico);

            return new AtualizarClienteHandler(
                clienteGateway,
                enderecoGateway,
                contatoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterClienteHandler CriarObterClienteHandler()
        {
            var clienteGateway = CriarClienteGateway();
            var logServico = new LogServico<ObterClienteHandler>(_idCorrelacionalService, NullLogger<ObterClienteHandler>.Instance, _usuarioLogadoServico);

            return new ObterClienteHandler(
                clienteGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterTodosClientesHandler CriarObterTodosClientesHandler()
        {
            var clienteGateway = CriarClienteGateway();
            var logServico = new LogServico<ObterTodosClientesHandler>(_idCorrelacionalService, NullLogger<ObterTodosClientesHandler>.Instance, _usuarioLogadoServico);

            return new ObterTodosClientesHandler(
                clienteGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IRemoverClienteHandler CriarRemoverClienteHandler()
        {
            var clienteGateway = CriarClienteGateway();
            var logServico = new LogServico<RemoverClienteHandler>(_idCorrelacionalService, NullLogger<RemoverClienteHandler>.Instance, _usuarioLogadoServico);

            return new RemoverClienteHandler(
                clienteGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterClientePorDocumentoHandler CriarObterClientePorDocumentoHandler()
        {
            var clienteGateway = CriarClienteGateway();
            var logServico = new LogServico<ObterClientePorDocumentoHandler>(_idCorrelacionalService, NullLogger<ObterClientePorDocumentoHandler>.Instance, _usuarioLogadoServico);

            return new ObterClientePorDocumentoHandler(
                clienteGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - Veículo

        public ICadastrarVeiculoHandler CriarCadastrarVeiculoHandler()
        {
            var veiculoGateway = CriarVeiculoGateway();
            var logServico = new LogServico<CadastrarVeiculoHandler>(_idCorrelacionalService, NullLogger<CadastrarVeiculoHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarVeiculoHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IAtualizarVeiculoHandler CriarAtualizarVeiculoHandler()
        {
            var veiculoGateway = CriarVeiculoGateway();
            var logServico = new LogServico<AtualizarVeiculoHandler>(_idCorrelacionalService, NullLogger<AtualizarVeiculoHandler>.Instance, _usuarioLogadoServico);

            return new AtualizarVeiculoHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterVeiculoHandler CriarObterVeiculoHandler()
        {
            var veiculoGateway = CriarVeiculoGateway();
            var logServico = new LogServico<ObterVeiculoHandler>(_idCorrelacionalService, NullLogger<ObterVeiculoHandler>.Instance, _usuarioLogadoServico);

            return new ObterVeiculoHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterTodosVeiculosHandler CriarObterTodosVeiculosHandler()
        {
            var veiculoGateway = CriarVeiculoGateway();
            var logServico = new LogServico<ObterTodosVeiculosHandler>(_idCorrelacionalService, NullLogger<ObterTodosVeiculosHandler>.Instance, _usuarioLogadoServico);

            return new ObterTodosVeiculosHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterVeiculoPorClienteHandler CriarObterVeiculoPorClienteHandler()
        {
            var veiculoGateway = CriarVeiculoGateway();
            var logServico = new LogServico<ObterVeiculoPorClienteHandler>(_idCorrelacionalService, NullLogger<ObterVeiculoPorClienteHandler>.Instance, _usuarioLogadoServico);

            return new ObterVeiculoPorClienteHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterVeiculoPorPlacaHandler CriarObterVeiculoPorPlacaHandler()
        {
            var veiculoGateway = CriarVeiculoGateway();
            var logServico = new LogServico<ObterVeiculoPorPlacaHandler>(_idCorrelacionalService, NullLogger<ObterVeiculoPorPlacaHandler>.Instance, _usuarioLogadoServico);

            return new ObterVeiculoPorPlacaHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IDeletarVeiculoHandler CriarDeletarVeiculoHandler()
        {
            var veiculoGateway = CriarVeiculoGateway();
            var logServico = new LogServico<DeletarVeiculoHandler>(_idCorrelacionalService, NullLogger<DeletarVeiculoHandler>.Instance, _usuarioLogadoServico);

            return new DeletarVeiculoHandler(
                veiculoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - Usuário

        public ICadastrarUsuarioHandler CriarCadastrarUsuarioHandler()
        {
            var usuarioGateway = CriarUsuarioGateway();
            var clienteUseCases = CriarClienteUseCases();
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

        public IAtualizarUsuarioHandler CriarAtualizarUsuarioHandler()
        {
            var usuarioGateway = CriarUsuarioGateway();
            var servicoSenha = new ServicoSenha();
            var logServico = new LogServico<AtualizarUsuarioHandler>(_idCorrelacionalService, NullLogger<AtualizarUsuarioHandler>.Instance, _usuarioLogadoServico);

            return new AtualizarUsuarioHandler(
                usuarioGateway,
                servicoSenha,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterUsuarioHandler CriarObterUsuarioHandler()
        {
            var usuarioGateway = CriarUsuarioGateway();
            var logServico = new LogServico<ObterUsuarioHandler>(_idCorrelacionalService, NullLogger<ObterUsuarioHandler>.Instance, _usuarioLogadoServico);

            return new ObterUsuarioHandler(
                usuarioGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterTodosUsuariosHandler CriarObterTodosUsuariosHandler()
        {
            var usuarioGateway = CriarUsuarioGateway();
            var logServico = new LogServico<ObterTodosUsuariosHandler>(_idCorrelacionalService, NullLogger<ObterTodosUsuariosHandler>.Instance, _usuarioLogadoServico);

            return new ObterTodosUsuariosHandler(
                usuarioGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IDeletarUsuarioHandler CriarDeletarUsuarioHandler()
        {
            var usuarioGateway = CriarUsuarioGateway();
            var logServico = new LogServico<DeletarUsuarioHandler>(_idCorrelacionalService, NullLogger<DeletarUsuarioHandler>.Instance, _usuarioLogadoServico);

            return new DeletarUsuarioHandler(
                usuarioGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterUsuarioPorEmailHandler CriarObterUsuarioPorEmailHandler()
        {
            var usuarioGateway = CriarUsuarioGateway();
            var logServico = new LogServico<ObterUsuarioPorEmailHandler>(_idCorrelacionalService, NullLogger<ObterUsuarioPorEmailHandler>.Instance, _usuarioLogadoServico);

            return new ObterUsuarioPorEmailHandler(
                usuarioGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - Serviço

        public ICadastrarServicoHandler CriarCadastrarServicoHandler()
        {
            var servicoGateway = CriarServicoGateway();
            var logServico = new LogServico<CadastrarServicoHandler>(_idCorrelacionalService, NullLogger<CadastrarServicoHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarServicoHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IEditarServicoHandler CriarEditarServicoHandler()
        {
            var servicoGateway = CriarServicoGateway();
            var logServico = new LogServico<EditarServicoHandler>(_idCorrelacionalService, NullLogger<EditarServicoHandler>.Instance, _usuarioLogadoServico);

            return new EditarServicoHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IDeletarServicoHandler CriarDeletarServicoHandler()
        {
            var servicoGateway = CriarServicoGateway();
            var logServico = new LogServico<DeletarServicoHandler>(_idCorrelacionalService, NullLogger<DeletarServicoHandler>.Instance, _usuarioLogadoServico);

            return new DeletarServicoHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterServicoHandler CriarObterServicoHandler()
        {
            var servicoGateway = CriarServicoGateway();
            var logServico = new LogServico<ObterServicoHandler>(_idCorrelacionalService, NullLogger<ObterServicoHandler>.Instance, _usuarioLogadoServico);

            return new ObterServicoHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterTodosServicosHandler CriarObterTodosServicosHandler()
        {
            var servicoGateway = CriarServicoGateway();
            var logServico = new LogServico<ObterTodosServicosHandler>(_idCorrelacionalService, NullLogger<ObterTodosServicosHandler>.Instance, _usuarioLogadoServico);

            return new ObterTodosServicosHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterServicoPorNomeHandler CriarObterServicoPorNomeHandler()
        {
            var servicoGateway = CriarServicoGateway();
            var logServico = new LogServico<ObterServicoPorNomeHandler>(_idCorrelacionalService, NullLogger<ObterServicoPorNomeHandler>.Instance, _usuarioLogadoServico);

            return new ObterServicoPorNomeHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterServicosDisponiveisHandler CriarObterServicosDisponiveisHandler()
        {
            var servicoGateway = CriarServicoGateway();
            var logServico = new LogServico<ObterServicosDisponiveisHandler>(_idCorrelacionalService, NullLogger<ObterServicosDisponiveisHandler>.Instance, _usuarioLogadoServico);

            return new ObterServicosDisponiveisHandler(
                servicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - Orçamento

        public IGerarOrcamentoHandler CriarGerarOrcamentoHandler()
        {
            var logServico = new LogServico<GerarOrcamentoHandler>(_idCorrelacionalService, NullLogger<GerarOrcamentoHandler>.Instance, _usuarioLogadoServico);

            return new GerarOrcamentoHandler(
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - Estoque

        public ICadastrarEstoqueHandler CriarCadastrarEstoqueHandler()
        {
            var estoqueGateway = CriarEstoqueGateway();
            var logServico = new LogServico<CadastrarEstoqueHandler>(_idCorrelacionalService, NullLogger<CadastrarEstoqueHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarEstoqueHandler(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IAtualizarEstoqueHandler CriarAtualizarEstoqueHandler()
        {
            var estoqueGateway = CriarEstoqueGateway();
            var logServico = new LogServico<AtualizarEstoqueHandler>(_idCorrelacionalService, NullLogger<AtualizarEstoqueHandler>.Instance, _usuarioLogadoServico);

            return new AtualizarEstoqueHandler(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IDeletarEstoqueHandler CriarDeletarEstoqueHandler()
        {
            var estoqueGateway = CriarEstoqueGateway();
            var logServico = new LogServico<DeletarEstoqueHandler>(_idCorrelacionalService, NullLogger<DeletarEstoqueHandler>.Instance, _usuarioLogadoServico);

            return new DeletarEstoqueHandler(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterEstoqueHandler CriarObterEstoqueHandler()
        {
            var estoqueGateway = CriarEstoqueGateway();
            var logServico = new LogServico<ObterEstoqueHandler>(_idCorrelacionalService, NullLogger<ObterEstoqueHandler>.Instance, _usuarioLogadoServico);

            return new ObterEstoqueHandler(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterTodosEstoquesHandler CriarObterTodosEstoquesHandler()
        {
            var estoqueGateway = CriarEstoqueGateway();
            var logServico = new LogServico<ObterTodosEstoquesHandler>(_idCorrelacionalService, NullLogger<ObterTodosEstoquesHandler>.Instance, _usuarioLogadoServico);

            return new ObterTodosEstoquesHandler(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterEstoqueCriticoHandler CriarObterEstoqueCriticoHandler()
        {
            var estoqueGateway = CriarEstoqueGateway();
            var logServico = new LogServico<ObterEstoqueCriticoHandler>(_idCorrelacionalService, NullLogger<ObterEstoqueCriticoHandler>.Instance, _usuarioLogadoServico);

            return new ObterEstoqueCriticoHandler(
                estoqueGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - Autenticação

        public IAutenticarUsuarioHandler CriarAutenticarUsuarioHandler()
        {
            var usuarioUseCases = CriarUsuarioUseCases();
            var clienteUseCases = CriarClienteUseCases();
            var servicoSenha = new ServicoSenha();
            var servicoJwt = CriarServicoJwt();
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

        public ICadastrarInsumosHandler CriarCadastrarInsumosHandler()
        {
            var ordemServicoUseCases = CriarOrdemServicoUseCases();
            var estoqueUseCases = CriarEstoqueUseCases();
            var logServico = new LogServico<CadastrarInsumosHandler>(_idCorrelacionalService, NullLogger<CadastrarInsumosHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarInsumosHandler(
                ordemServicoUseCases,
                estoqueUseCases,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IDevolverInsumosHandler CriarDevolverInsumosHandler()
        {
            var obterEstoqueHandler = CriarObterEstoqueHandler();
            var atualizarEstoqueHandler = CriarAtualizarEstoqueHandler();

            var logServico = new LogServico<DevolverInsumosHandler>(_idCorrelacionalService, NullLogger<DevolverInsumosHandler>.Instance, _usuarioLogadoServico);

            return new DevolverInsumosHandler(
                obterEstoqueHandler,
                atualizarEstoqueHandler,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        #endregion

        #region Criação de Handlers Individuais - OrdemServico

        public ICadastrarOrdemServicoHandler CriarCadastrarOrdemServicoHandler()
        {
            var ordemServicoGateway = CriarOrdemServicoGateway();
            var clienteUseCases = CriarClienteUseCases();
            var servicoUseCases = CriarServicoUseCases();
            var logServico = new LogServico<CadastrarOrdemServicoHandler>(_idCorrelacionalService, NullLogger<CadastrarOrdemServicoHandler>.Instance, _usuarioLogadoServico);

            return new CadastrarOrdemServicoHandler(
                ordemServicoGateway,
                clienteUseCases,
                servicoUseCases,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IAtualizarOrdemServicoHandler CriarAtualizarOrdemServicoHandler()
        {
            var ordemServicoGateway = CriarOrdemServicoGateway();
            var eventosGateway = CriarEventosGateway();
            var logServico = new LogServico<AtualizarOrdemServicoHandler>(_idCorrelacionalService, NullLogger<AtualizarOrdemServicoHandler>.Instance, _usuarioLogadoServico);

            return new AtualizarOrdemServicoHandler(
                ordemServicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterOrdemServicoHandler CriarObterOrdemServicoHandler()
        {
            var ordemServicoGateway = CriarOrdemServicoGateway();
            var logServico = new LogServico<ObterOrdemServicoHandler>(_idCorrelacionalService, NullLogger<ObterOrdemServicoHandler>.Instance, _usuarioLogadoServico);

            return new ObterOrdemServicoHandler(
                ordemServicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterTodosOrdensServicoHandler CriarObterTodosOrdensServicoHandler()
        {
            var ordemServicoGateway = CriarOrdemServicoGateway();
            var logServico = new LogServico<ObterTodosOrdensServicoHandler>(_idCorrelacionalService, NullLogger<ObterTodosOrdensServicoHandler>.Instance, _usuarioLogadoServico);

            return new ObterTodosOrdensServicoHandler(
                ordemServicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IObterOrdemServicoPorStatusHandler CriarObterOrdemServicoPorStatusHandler()
        {
            var ordemServicoGateway = CriarOrdemServicoGateway();
            var logServico = new LogServico<ObterOrdemServicoPorStatusHandler>(_idCorrelacionalService, NullLogger<ObterOrdemServicoPorStatusHandler>.Instance, _usuarioLogadoServico);

            return new ObterOrdemServicoPorStatusHandler(
                ordemServicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IAceitarOrcamentoHandler CriarAceitarOrcamentoHandler()
        {
            var ordemServicoGateway = CriarOrdemServicoGateway();
            var logServico = new LogServico<AceitarOrcamentoHandler>(_idCorrelacionalService, NullLogger<AceitarOrcamentoHandler>.Instance, _usuarioLogadoServico);

            return new AceitarOrcamentoHandler(
                ordemServicoGateway,
                logServico,
                _unidadeDeTrabalho,
                _usuarioLogadoServico);
        }

        public IRecusarOrcamentoHandler CriarRecusarOrcamentoHandler()
        {
            var ordemServicoGateway = CriarOrdemServicoGateway();
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
        public IClienteUseCases CriarClienteUseCases()
        {
            var cadastrarClienteHandler = CriarCadastrarClienteHandler();
            var atualizarClienteHandler = CriarAtualizarClienteHandler();
            var obterClienteHandler = CriarObterClienteHandler();
            var obterTodosClientesHandler = CriarObterTodosClientesHandler();
            var removerClienteHandler = CriarRemoverClienteHandler();
            var obterClientePorDocumentoHandler = CriarObterClientePorDocumentoHandler();

            return new ClienteUseCasesFacade(
                cadastrarClienteHandler,
                atualizarClienteHandler,
                obterClienteHandler,
                obterTodosClientesHandler,
                removerClienteHandler,
                obterClientePorDocumentoHandler);
        }

        // Serviço - Usando novo padrão com facade
        public IServicoUseCases CriarServicoUseCases()
        {
            var cadastrarServicoHandler = CriarCadastrarServicoHandler();
            var editarServicoHandler = CriarEditarServicoHandler();
            var deletarServicoHandler = CriarDeletarServicoHandler();
            var obterServicoHandler = CriarObterServicoHandler();
            var obterTodosServicosHandler = CriarObterTodosServicosHandler();
            var obterServicoPorNomeHandler = CriarObterServicoPorNomeHandler();
            var obterServicosDisponiveisHandler = CriarObterServicosDisponiveisHandler();

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
        public IEstoqueUseCases CriarEstoqueUseCases()
        {
            var cadastrarEstoqueHandler = CriarCadastrarEstoqueHandler();
            var atualizarEstoqueHandler = CriarAtualizarEstoqueHandler();
            var deletarEstoqueHandler = CriarDeletarEstoqueHandler();
            var obterEstoqueHandler = CriarObterEstoqueHandler();
            var obterTodosEstoquesHandler = CriarObterTodosEstoquesHandler();
            var obterEstoqueCriticoHandler = CriarObterEstoqueCriticoHandler();

            return new EstoqueUseCasesFacade(
                cadastrarEstoqueHandler,
                atualizarEstoqueHandler,
                deletarEstoqueHandler,
                obterEstoqueHandler,
                obterTodosEstoquesHandler,
                obterEstoqueCriticoHandler);
        }

        // Ordem de Serviço - Usando novo padrão com facade
        public IOrdemServicoUseCases CriarOrdemServicoUseCases()
        {
            var cadastrarOrdemServicoHandler = CriarCadastrarOrdemServicoHandler();
            var atualizarOrdemServicoHandler = CriarAtualizarOrdemServicoHandler();
            var obterOrdemServicoHandler = CriarObterOrdemServicoHandler();
            var obterTodosOrdensServicoHandler = CriarObterTodosOrdensServicoHandler();
            var obterOrdemServicoPorStatusHandler = CriarObterOrdemServicoPorStatusHandler();
            var aceitarOrcamentoHandler = CriarAceitarOrcamentoHandler();
            var recusarOrcamentoHandler = CriarRecusarOrcamentoHandler();

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
        public IInsumoOSUseCases CriarInsumoOSUseCases()
        {
            var cadastrarInsumosHandler = CriarCadastrarInsumosHandler();
            var devolverInsumosHandler = CriarDevolverInsumosHandler();

            return new InsumoOSUseCasesFacade(
                cadastrarInsumosHandler,
                devolverInsumosHandler);
        }

        // Usuário - Usando novo padrão com facade
        public IUsuarioUseCases CriarUsuarioUseCases()
        {
            var cadastrarUsuarioHandler = CriarCadastrarUsuarioHandler();
            var atualizarUsuarioHandler = CriarAtualizarUsuarioHandler();
            var obterUsuarioHandler = CriarObterUsuarioHandler();
            var obterTodosUsuariosHandler = CriarObterTodosUsuariosHandler();
            var deletarUsuarioHandler = CriarDeletarUsuarioHandler();
            var obterUsuarioPorEmailHandler = CriarObterUsuarioPorEmailHandler();

            return new UsuarioUseCasesFacade(
                cadastrarUsuarioHandler,
                atualizarUsuarioHandler,
                obterUsuarioHandler,
                obterTodosUsuariosHandler,
                deletarUsuarioHandler,
                obterUsuarioPorEmailHandler);
        }

        // Veículo - Usando novo padrão com facade
        public IVeiculoUseCases CriarVeiculoUseCases()
        {
            var cadastrarVeiculoHandler = CriarCadastrarVeiculoHandler();
            var atualizarVeiculoHandler = CriarAtualizarVeiculoHandler();
            var obterVeiculoHandler = CriarObterVeiculoHandler();
            var obterTodosVeiculosHandler = CriarObterTodosVeiculosHandler();
            var obterVeiculoPorClienteHandler = CriarObterVeiculoPorClienteHandler();
            var obterVeiculoPorPlacaHandler = CriarObterVeiculoPorPlacaHandler();
            var deletarVeiculoHandler = CriarDeletarVeiculoHandler();

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
        public IOrcamentoUseCases CriarOrcamentoUseCases()
        {
            var gerarOrcamentoHandler = CriarGerarOrcamentoHandler();
            return new OrcamentoUseCasesFacade(gerarOrcamentoHandler);
        }

        // Serviços de autenticação
        public ConfiguracaoJwt CriarConfiguracaoJwt()
        {
            // Criar uma configuração JWT com valores padrão
            var configuracao = new ConfiguracaoJwt
            {
                SecretKey = "chave-secreta-mecanica-os-para-testes-locais",
                Issuer = "MecanicaOS",
                Audience = "MecanicaOS.Users",
                ExpiryInMinutes = 60
            };

            return configuracao;
        }

        public IServicoJwt CriarServicoJwt()
        {
            var configuracaoJwt = CriarConfiguracaoJwt();
            return new ServicoJwt(configuracaoJwt);
        }

        // Autenticação - Usando novo padrão com facade
        public IAutenticacaoUseCases CriarAutenticacaoUseCases()
        {
            var autenticarUsuarioHandler = CriarAutenticarUsuarioHandler();
            return new AutenticacaoUseCasesFacade(autenticarUsuarioHandler);
        }

        #endregion

        #region Criação de Presenters

        public IClientePresenter CriarClientePresenter()
        {
            return new ClientePresenter();
        }

        public IOrdemServicoPresenter CriarOrdemServicoPresenter()
        {
            return new OrdemServicoPresenter();
        }

        public IServicoPresenter CriarServicoPresenter()
        {
            return new ServicoPresenter();
        }

        public IEstoquePresenter CriarEstoquePresenter()
        {
            return new EstoquePresenter();
        }

        public IUsuarioPresenter CriarUsuarioPresenter()
        {
            return new UsuarioPresenter();
        }

        public IVeiculoPresenter CriarVeiculoPresenter()
        {
            return new VeiculoPresenter();
        }

        public IAutenticacaoPresenter CriarAutenticacaoPresenter()
        {
            return new AutenticacaoPresenter();
        }

        #endregion

        #region Criação de Serviços

        public IUsuarioLogadoServico CriarUsuarioLogadoServico()
        {
            return _usuarioLogadoServico;
        }

        #endregion

        #region Criação de Controllers

        public IClienteController CriarClienteController()
        {
            return new ClienteController(this);
        }

        public IOrdemServicoController CriarOrdemServicoController()
        {
            return new OrdemServicoController(this);
        }

        public IInsumoOSController CriarInsumoOSController()
        {
            return new InsumoOSController(this);
        }

        public IServicoController CriarServicoController()
        {
            return new ServicoController(this);
        }

        public IEstoqueController CriarEstoqueController()
        {
            return new EstoqueController(this);
        }

        public IUsuarioController CriarUsuarioController()
        {
            return new UsuarioController(this);
        }

        public IVeiculoController CriarVeiculoController()
        {
            return new VeiculoController(this);
        }

        public IAutenticacaoController CriarAutenticacaoController()
        {
            return new AutenticacaoController(this);
        }

        #endregion
    }
}
