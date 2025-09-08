using Core.DTOs.Usuario;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases;

public class UsuarioUseCases : UseCasesAbstrato<UsuarioUseCases, Usuario>, IUsuarioUseCases
{
    private readonly IUsuarioGateway _usuarioGateway;
    private readonly IClienteUseCases _clienteUseCases;
    private readonly IServicoSenha _servicoSenha;

    public UsuarioUseCases(
        ILogServico<UsuarioUseCases> logServico,
        IUnidadeDeTrabalho udt,
        IClienteUseCases clienteUseCases,
        IServicoSenha servicoSenha,
        IUsuarioLogadoServico usuarioLogadoServico,
        IUsuarioGateway usuarioGateway) : base(logServico, udt, usuarioLogadoServico)
    {
        _usuarioGateway = usuarioGateway ?? throw new ArgumentNullException(nameof(usuarioGateway));
        _clienteUseCases = clienteUseCases ?? throw new ArgumentNullException(nameof(clienteUseCases));
        _servicoSenha = servicoSenha ?? throw new ArgumentNullException(nameof(servicoSenha));
    }

    public async Task<Usuario> AtualizarUseCaseAsync(Guid id, AtualizarUsuarioUseCaseDto request)
    {
        var metodo = nameof(AtualizarUseCaseAsync);

        try
        {
            LogInicio(metodo, new { id, request });

            Usuario usuario = await _usuarioGateway.ObterPorIdAsync(id) ?? throw new DadosNaoEncontradosException("Usuário não encontrado");

            string senhaCriptografada = !string.IsNullOrEmpty(request.Senha)
                ? _servicoSenha.CriptografarSenha(request.Senha)
                : usuario.Senha;

            usuario.Atualizar(
                request.Email,
                senhaCriptografada,
                request.DataUltimoAcesso,
                request.TipoUsuario,
                request.RecebeAlertaEstoque);

            await _usuarioGateway.EditarAsync(usuario);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao atualizar usuario");
            IsNotGetSenha(usuario);

            LogFim(metodo, usuario);

            return usuario;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<Usuario> CadastrarUseCaseAsync(CadastrarUsuarioUseCaseDto request)
    {
        var metodo = nameof(CadastrarUseCaseAsync);

        try
        {
            LogInicio(metodo, request);

            await VerificarUsuarioCadastradoAsync(request.Email);

            Usuario usuario = new ()
            {
                Email = request.Email,
                TipoUsuario = request.TipoUsuario,
                RecebeAlertaEstoque = request.RecebeAlertaEstoque.HasValue ? request.RecebeAlertaEstoque.Value : false,  
            };

            usuario.Senha = _servicoSenha.CriptografarSenha(request.Senha);

            if (request.TipoUsuario == TipoUsuario.Cliente)
                usuario.ClienteId = await GetClienteIdUseCaseAsync(request.Documento);

            var entidade = await _usuarioGateway.CadastrarAsync(usuario);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao cadastrar usuário");
            IsNotGetSenha(usuario);

            LogFim(metodo, usuario);

            return usuario;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    private async Task<Guid> GetClienteIdUseCaseAsync(string? documento)
    {
        string metodo = nameof(GetClienteIdUseCaseAsync);

        LogInicio(metodo, new { documento });

        try
        {
            _ = documento ?? throw new DadosInvalidosException("Usuários do tipo cliente devem informar o documento.");

            var cliente = await _clienteUseCases.ObterPorDocumentoUseCaseAsync(documento);

            LogFim(metodo);

            return cliente.Id;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    private async Task VerificarUsuarioCadastradoAsync(string email)
    {
        var metodo = nameof(VerificarUsuarioCadastradoAsync);

        try
        {
            LogInicio(metodo);

            var usuario = await ObterPorEmailUseCaseAsync(email);

            LogFim(metodo);

            if (usuario is not null)
                throw new DadosJaCadastradosException("Usuário já cadastrado");
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<bool> DeletarUseCaseAsync(Guid id)
    {
        var metodo = nameof(DeletarUseCaseAsync);

        try
        {
            LogInicio(metodo, id);

            var usuario = await _usuarioGateway.ObterPorIdAsync(id) ??
                throw new DadosNaoEncontradosException("Usuário não encontrado");

            await _usuarioGateway.DeletarAsync(usuario);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao deletar usuário");

            LogFim(metodo);

            return true;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    public async Task<Usuario?> ObterPorIdUseCaseAsync(Guid id)
    {
        var metodo = nameof(ObterPorIdUseCaseAsync);

        try
        {
            LogInicio(metodo);

            var usuario = await _usuarioGateway.ObterPorIdAsync(id);

            IsNotGetSenha(usuario);

            LogFim(metodo, usuario);

            return usuario;

        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    private static void IsNotGetSenha(Usuario? usuario)
    {
        usuario.Senha = null;
    }

    public async Task<IEnumerable<Usuario>> ObterTodosUseCaseAsync()
    {
        var metodo = nameof(ObterTodosUseCaseAsync);

        try
        {
            LogInicio(metodo);

            var usuarios = await _usuarioGateway.ObterTodosAsync();

            foreach (var usuario in usuarios)
            {
                IsNotGetSenha(usuario);
            }

            LogFim(metodo, usuarios);

            return usuarios;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<Usuario?> ObterPorEmailUseCaseAsync(string email)
    {
        var metodo = nameof(ObterPorEmailUseCaseAsync);
        LogInicio(metodo, new { email });
        try
        {
            //var especificacao = new ObterUsuarioPorEmailEspecificacao(email);
            //var usuario = await _repositorio.ObterUmSemRastreamentoAsync(especificacao);
            var usuario = await _usuarioGateway.ObterPorEmailAsync(email);

            LogFim(metodo, usuario);

            return usuario;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }
}
