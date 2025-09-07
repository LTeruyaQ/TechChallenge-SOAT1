using Aplicacao.UseCases.Estoque;
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
}