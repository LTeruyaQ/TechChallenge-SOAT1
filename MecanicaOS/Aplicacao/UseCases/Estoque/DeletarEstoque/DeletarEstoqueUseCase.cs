using Aplicacao.Ports;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;

namespace Aplicacao.UseCases.Estoque.DeletarEstoque
{
    public class DeletarEstoqueUseCase(IEstoqueRepository repositorio, IUnidadeDeTrabalho udt) : IDeletarEstoqueUseCase
    {
        private readonly IEstoqueRepository repositorio = repositorio;
        private readonly IUnidadeDeTrabalho udt = udt;

        public async Task<bool> ExecuteAsync(Guid id)
        {
            var estoque = await repositorio.ObterPorIdAsync(id) ?? throw new DadosNaoEncontradosException("Estoque não encontrado.");

            await repositorio.DeletarAsync(estoque);

            return await udt.Commit();
        }
    }
}
