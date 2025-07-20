using Dominio.Especificacoes.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Base
{
    public abstract class EspecificacaoBase<T> : IEspecificacao<T>
    {
        private readonly List<string> _inclusoes = new();
        
        public List<string> Inclusoes => _inclusoes;
        
        public virtual Expression<Func<T, bool>>? Expressao { get; } = null;

        protected void AdicionarInclusao<TProp>(Expression<Func<T, TProp>> navegacao)
        {
            if (navegacao == null) throw new ArgumentNullException(nameof(navegacao));
            _inclusoes.Add(AuxiliarExpressao.ObterCaminho(navegacao));
        }

        protected void AdicionarInclusao<TColecao, TProp>(
            Expression<Func<T, IEnumerable<TColecao>>> colecao,
            Expression<Func<TColecao, TProp>> navegacao)
        {
            if (colecao == null) throw new ArgumentNullException(nameof(colecao));
            if (navegacao == null) throw new ArgumentNullException(nameof(navegacao));
            
            var caminhoColecao = AuxiliarExpressao.ObterCaminho(colecao);
            var caminhoNavegacao = AuxiliarExpressao.ObterCaminho(navegacao);
            _inclusoes.Add($"{caminhoColecao}.{caminhoNavegacao}");
        }
    }
}