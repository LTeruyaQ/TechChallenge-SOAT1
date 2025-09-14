using Adapters.Controllers;
using Adapters.DTOs.Requests.Autenticacao;
using Adapters.DTOs.Requests.Usuario;
using Adapters.DTOs.Responses.Autenticacao;
using Adapters.Gateways;
using Adapters.Presenters;
using Adapters.Presenters.Interfaces;
using API.Models;
using Core.Entidades;
using Core.DTOs.Repositories.Usuarios;
using Core.DTOs.Repositories.Cliente;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases;
using Infraestrutura.Autenticacao;
using Infraestrutura.Logs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    public class AutenticacaoController : BaseApiController
    {
        private readonly IAutenticacaoController _autenticacaoController;
        private readonly IUsuarioController _usuarioController;

        public AutenticacaoController(
            IRepositorio<UsuarioRepositoryDto> repositorioUsuario,
            IRepositorio<ClienteRepositoryDTO> repositorioCliente,
            IRepositorio<EnderecoRepositoryDto> repositorioEndereco,
            IRepositorio<ContatoRepositoryDTO> repositorioContato,
            IOptions<ConfiguracaoJwt> configuracaoJwt,
            IUnidadeDeTrabalho unidadeDeTrabalho,
            IUsuarioLogadoServico usuarioLogadoServico,
            IIdCorrelacionalService idCorrelacionalService,
            ILogger<ClienteUseCases> loggerClienteUseCases,
            ILogger<UsuarioUseCases> loggerUsuarioUseCases,
            ILogger<AutenticacaoUseCases> loggerAutenticacaoUseCases)
        {
            // Criando gateways
            IUsuarioGateway usuarioGateway = new UsuarioGateway(repositorioUsuario);
            IClienteGateway clienteGateway = new ClienteGateway(repositorioCliente);
            IEnderecoGateway enderecoGateway = new EnderecoGateway(repositorioEndereco);
            IContatoGateway contatoGateway = new ContatoGateway(repositorioContato);

            // Criando serviços
            IServicoSenha servicoSenha = new ServicoSenha();
            IServicoJwt servicoJwt = new ServicoJwt(configuracaoJwt);

            // Criando logs
            ILogServico<ClienteUseCases> logClienteUseCases = new LogServico<ClienteUseCases>(idCorrelacionalService, loggerClienteUseCases, usuarioLogadoServico);
            ILogServico<UsuarioUseCases> logUsuarioUseCases = new LogServico<UsuarioUseCases>(idCorrelacionalService, loggerUsuarioUseCases, usuarioLogadoServico);
            ILogServico<AutenticacaoUseCases> logAutenticacaoUseCases = new LogServico<AutenticacaoUseCases>(idCorrelacionalService, loggerAutenticacaoUseCases, usuarioLogadoServico);

            // Criando use cases
            IClienteUseCases clienteUseCases = new ClienteUseCases(
                clienteGateway,
                enderecoGateway,
                contatoGateway,
                logClienteUseCases,
                unidadeDeTrabalho,
                usuarioLogadoServico);

            IUsuarioUseCases usuarioUseCases = new UsuarioUseCases(
                logUsuarioUseCases,
                unidadeDeTrabalho,
                clienteUseCases,
                servicoSenha,
                usuarioLogadoServico,
                usuarioGateway);

            IAutenticacaoUseCases autenticacaoUseCases = new AutenticacaoUseCases(
                usuarioUseCases,
                servicoSenha,
                servicoJwt,
                logAutenticacaoUseCases,
                clienteUseCases);

            // Criando presenters
            IAutenticacaoPresenter autenticacaoPresenter = new AutenticacaoPresenter();
            IUsuarioPresenter usuarioPresenter = new UsuarioPresenter();

            // Criando controllers
            _autenticacaoController = new Adapters.Controllers.AutenticacaoController(autenticacaoUseCases, autenticacaoPresenter);
            _usuarioController = new Adapters.Controllers.UsuarioController(usuarioUseCases, usuarioPresenter);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AutenticacaoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] AutenticacaoRequest request)
        {
            return ValidarModelState() ?? Ok(await _autenticacaoController.AutenticarAsync(request));
        }

        [HttpPost("Registrar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registrar(CadastrarUsuarioRequest request)
        {
            var usuario = await _usuarioController.CadastrarAsync(request);
            return CreatedAtAction(nameof(Login), new AutenticacaoRequest { Email = usuario.Email, Senha = usuario.Senha }, usuario);
        }

        [HttpGet("Validar-Token")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ValidarToken()
        {
            return Ok(new { mensagem = "Token válido" });
        }
    }
}
