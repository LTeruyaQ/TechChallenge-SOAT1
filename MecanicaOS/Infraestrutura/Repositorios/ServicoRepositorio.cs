using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Infraestrutura.Dados;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Repositorios
{
    public class ServicoRepositorio : ICrudRepositorio<Servico>
    {
        private readonly MecanicaContexto _dbContext;

        public ServicoRepositorio(MecanicaContexto dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Cadastrar(Servico entidade)
        {
            _dbContext.Servicos.Add(entidade);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Deletar(Servico servico)
        {
            _dbContext.Servicos.Remove(servico);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Editar(Servico novoServico)
        {
            _dbContext.Servicos.Update(novoServico);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Servico>> ObterPorFiltro(IEspecificacao<Servico> filtro)
        {
            return await _dbContext.Servicos.AsNoTracking().Where(x => filtro.EhSatisfeitoPor(x)).ToListAsync();
        }

        public async Task<Servico> ObterPorId(Guid id)
        {
            return await _dbContext.Servicos.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Servico>> ObterTodos()
        {
            return await _dbContext.Servicos.AsNoTracking().ToListAsync();
        }
    }
}
