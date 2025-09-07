using Aplicacao.DTOs.Responses.Estoque;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos;

public class EstoqueServico : ServicoAbstrato<EstoqueServico, Estoque>, IEstoqueServico
{
    public EstoqueServico(
        IRepositorio<Estoque> repositorio,
        ILogServico<EstoqueServico> logServico,
        IUnidadeDeTrabalho udt,
        IMapper mapper,
        IUsuarioLogadoServico usuarioLogadoServico)
        : base(repositorio, logServico, udt, mapper, usuarioLogadoServico)
    {
    }

    public async Task<IEnumerable<EstoqueResponse>> ObterTodosAsync()
    {
        string metodo = nameof(ObterTodosAsync);

        try
        {
            LogInicio(metodo);

            var estoques = await _repositorio.ObterTodosAsync();
            var response = _mapper.Map<IEnumerable<EstoqueResponse>>(estoques);

            LogFim(metodo, response);

            return response;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<EstoqueResponse> ObterPorIdAsync(Guid id)
    {
        string metodo = nameof(ObterPorIdAsync);

        try
        {
            LogInicio(metodo);

            var estoque = await _repositorio.ObterPorIdAsync(id)
                ?? throw new DadosNaoEncontradosException("Estoque não encontrado");

            var response = _mapper.Map<EstoqueResponse>(estoque);
            LogFim(metodo, response);

            return response;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }

    public async Task<bool> DeletarAsync(Guid id)
    {
        string metodo = nameof(DeletarAsync);

        try
        {
            LogInicio(metodo);

            var estoque = await _repositorio.ObterPorIdAsync(id)
                ?? throw new DadosNaoEncontradosException("Estoque não encontrado");

            await _repositorio.DeletarAsync(estoque);
            var sucesso = await Commit();

            LogFim(metodo, sucesso);

            return sucesso;
        }
        catch (Exception e)
        {
            LogErro(metodo, e);
            throw;
        }
    }
}