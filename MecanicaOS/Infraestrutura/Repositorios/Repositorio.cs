﻿using Dominio.Entidades.Abstratos;
using Dominio.Especificacoes.Base.Extensoes;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Infraestrutura.Dados;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Repositorios
{
    public class Repositorio<T> : IRepositorio<T> where T : Entidade
    {
        protected readonly MecanicaContexto _dbContext;
        protected readonly DbSet<T> _dbSet;

        public Repositorio(MecanicaContexto dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public virtual async Task<T> CadastrarAsync(T entidade)
        {
            var resultado = await _dbSet.AddAsync(entidade);
            return resultado.Entity;
        }

        public virtual async Task<IEnumerable<T>> CadastrarVariosAsync(IEnumerable<T> entidades)
        {
            if (entidades == null || !entidades.Any())
                return Enumerable.Empty<T>();

            await _dbSet.AddRangeAsync(entidades);
            return entidades;
        }

        public virtual async Task DeletarAsync(T entidade)
        {
            await Task.Run(() => _dbSet.Remove(entidade));
        }

        public virtual async Task DeletarLogicamenteAsync(T entidade)
        {
            await Task.Run(() => entidade.Ativo = false);
        }

        public virtual async Task EditarAsync(T entidade)
        {
            await Task.Run(() => _dbContext.Entry(entidade).State = EntityState.Modified);
        }

        public virtual async Task<IEnumerable<T>> ObterPorFiltroAsync(IEspecificacao<T> especificacao)
        {
            var query = AvaliadorDeEspecificacao<T>.ObterConsultaSemRastreanemento(_dbSet.AsNoTracking(), especificacao);
            return await query.ToListAsync();
        }

        public virtual async Task<T?> ObterPorIdAsync(Guid id)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<IEnumerable<T>> ObterTodosAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<T?> ObterUmSemRastreamentoAsync(IEspecificacao<T> especificacao)
        {
            var query = AvaliadorDeEspecificacao<T>.ObterConsultaSemRastreanemento(_dbSet, especificacao);
            return await query
                .SingleOrDefaultAsync();
        }

        public virtual async Task<T?> ObterUmAsync(IEspecificacao<T> especificacao)
        {
            var query = AvaliadorDeEspecificacao<T>.ObterConsulta(_dbSet.AsNoTracking(), especificacao);
            return await query
                .SingleOrDefaultAsync();
        }
    }
}
