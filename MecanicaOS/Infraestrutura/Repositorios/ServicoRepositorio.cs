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

        public async Task<Servico> CadastrarAsync(Servico servico)
        {
            var entidade =_dbContext.Servicos.Add(servico);
            await _dbContext.SaveChangesAsync();
            return entidade.Entity;
        }

        public async Task DeletarAsync(Servico servico)
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

        public async Task<Servico> ObterPorIdAsync(Guid id)
        {
            return await _dbContext.Servicos.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Servico>> ObterTodos()
        {
            return await _dbContext.Servicos.AsNoTracking().ToListAsync();
        }
    }
}
