using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Infraestrutura.Dados;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            var entidade = await Task.Run(() => _dbContext.Servicos.Add(servico));
            return entidade.Entity;
        }

        public async Task DeletarAsync(Servico servico)
        {
            await Task.Run(() => _dbContext.Servicos.Remove(servico));
        }

        public async Task EditarAsync(Servico novoServico)
        {
            await Task.Run(() => _dbContext.Servicos.Update(novoServico));
        }

        public async Task<IEnumerable<Servico>> ObterPorFiltroAsync(IEspecificacao<Servico> especificacao)
        {
            return await _dbContext.Servicos.AsNoTracking().Where(especificacao.Expressao).ToListAsync();
        }

        public async Task<Servico?> ObterPorIdAsync(Guid id)
        {
            return await _dbContext.Servicos.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Servico>> ObterTodosAsync()
        {
            return await _dbContext.Servicos.AsNoTracking().ToListAsync();
        }

        public async Task<Servico?> ObterUmAsync(IEspecificacao<Servico> especificacao)
        {
            return await _dbContext.Servicos.AsNoTracking().Where(especificacao.Expressao).SingleOrDefaultAsync();
        }
    }
}
