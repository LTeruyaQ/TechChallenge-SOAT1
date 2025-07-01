using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades.Abstratos
{
    public abstract class Entidade
    {
        public Guid Id { get; }
        public DateTime DataCadastro { get; }
        public DateTime DataAtualizacao { get; set; }

        protected Entidade()
        {
            Id = Guid.NewGuid();
            DataCadastro = DateTime.Now;
        }
    }
}
