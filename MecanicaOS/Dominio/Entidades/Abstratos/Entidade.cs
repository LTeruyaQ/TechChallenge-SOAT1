
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
        
        public static bool operator ==(Entidade e1, Entidade e2){
            return e1.Id.Equals(e2.Id);
        }

        public static bool operator !=(Entidade e1, Entidade e2){
            return !(e1 == e2);
        }
    }
}
