
namespace Dominio.Entidades.Abstratos
{
    public abstract class Entidade
    {
        public Guid Id { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public Boolean? Ativo { get; set; }

        protected Entidade()
        {
            Id = Guid.NewGuid();
        }

        public static bool operator ==(Entidade e1, Entidade e2)
        {
            return e1.Id.Equals(e2.Id);
        }

        public static bool operator !=(Entidade e1, Entidade e2)
        {
            return !(e1 == e2);
        }
    }
}
