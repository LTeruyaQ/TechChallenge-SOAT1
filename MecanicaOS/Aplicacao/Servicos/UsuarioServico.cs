using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.DTOs.Responses.Usuario;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Usuario;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos;

public class UsuarioServico : ServicoAbstrato<UsuarioServico, Usuario>, IUsuarioServico
{
    private readonly IClienteServico _clienteServico;
    private readonly IServicoSenha _servicoSenha;

    public UsuarioServico(
        IRepositorio<Usuario> repositorio,
        ILogServico<UsuarioServico> logServico,
        IUnidadeDeTrabalho udt,
        IMapper mapper,
        IClienteServico clienteServico,
        IServicoSenha servicoSenha,
        IUsuarioLogadoServico usuarioLogadoServico) : base(repositorio, logServico, udt, mapper, usuarioLogadoServico)
    {
        _clienteServico = clienteServico ?? throw new ArgumentNullException(nameof(clienteServico));
        _servicoSenha = servicoSenha ?? throw new ArgumentNullException(nameof(servicoSenha));
    }

    public async Task<UsuarioResponse> AtualizarAsync(Guid id, AtualizarUsuarioRequest request)
    {
        var metodo = nameof(AtualizarAsync);

        try
        {
            LogInicio(metodo, new { id, request });

            Usuario usuario = await _repositorio.ObterPorIdAsync(id) ?? throw new DadosNaoEncontradosException("Usuário não encontrado");

            string senhaCriptografada = !string.IsNullOrEmpty(request.Senha)
                ? _servicoSenha.CriptografarSenha(request.Senha)
                : usuario.Senha;

            usuario.Atualizar(
                request.Email,
                senhaCriptografada,
                request.DataUltimoAcesso,
                request.TipoUsuario,
                request.RecebeAlertaEstoque);

            await _repositorio.EditarAsync(usuario);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao atualizar usuario");

            LogFim(metodo, usuario);

            return _mapper.Map<UsuarioResponse>(usuario);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<UsuarioResponse> CadastrarAsync(CadastrarUsuarioRequest request)
    {
        var metodo = nameof(CadastrarAsync);

        try
        {
            LogInicio(metodo, request);

            await VerificarUsuarioCadastradoAsync(request.Email);

            var usuario = _mapper.Map<Usuario>(request);

            usuario.Senha = _servicoSenha.CriptografarSenha(request.Senha);

            if (request.TipoUsuario == TipoUsuario.Cliente)
                usuario.ClienteId = await GetClienteIdAsync(request.Documento);

            var entidade = await _repositorio.CadastrarAsync(usuario);

            if (!await Commit())
                throw new PersistirDadosException("Erro ao cadastrar usuário");

            LogFim(metodo, entidade);

            return _mapper.Map<UsuarioResponse>(entidade);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    private async Task<Guid> GetClienteIdAsync(string? documento)
    {
        string metodo = nameof(GetClienteIdAsync);

        LogInicio(metodo, new { documento });

        try
        {
            _ = documento ?? throw new DadosInvalidosException("Usuários do tipo cliente devem informar o documento.");

            var cliente = await _clienteServico.ObterPorDocumento(documento);

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

            var usuario = await ObterPorEmailAsync(email);

            LogFim(metodo, usuario);

            if (usuario is not null)
            {
                throw new DadosJaCadastradosException("Usuário já cadastrado");
            }
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<bool> DeletarAsync(Guid id)
    {
        var metodo = nameof(DeletarAsync);

        try
        {
            LogInicio(metodo, id);

            var usuario = await _repositorio.ObterPorIdAsync(id) ??
                throw new DadosNaoEncontradosException("Usuário não encontrado");

            await _repositorio.DeletarAsync(usuario);

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

    public async Task<UsuarioResponse?> ObterPorIdAsync(Guid id)
    {
        var metodo = nameof(ObterPorIdAsync);

        try
        {
            LogInicio(metodo);

            var usuario = await _repositorio.ObterPorIdAsync(id);

            LogFim(metodo, usuario);

            return _mapper.Map<UsuarioResponse>(usuario);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);

            throw;
        }
    }

    public async Task<IEnumerable<UsuarioResponse>> ObterTodosAsync()
    {
        var metodo = nameof(ObterTodosAsync);

        try
        {
            LogInicio(metodo);

            var usuarios = await _repositorio.ObterTodosAsync();

            LogFim(metodo, usuarios);

            return _mapper.Map<IEnumerable<UsuarioResponse>>(usuarios);
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        var metodo = nameof(ObterPorEmailAsync);
        LogInicio(metodo, new { email });
        try
        {
            var especificacao = new ObterUsuarioPorEmailEspecificacao(email);

            var usuario = await _repositorio.ObterUmSemRastreamentoAsync(especificacao);

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
