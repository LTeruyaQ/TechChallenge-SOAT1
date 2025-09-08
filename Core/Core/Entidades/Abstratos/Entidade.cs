namespace Core.Entidades.Abstratos
{
    public abstract class Entidade
    {
        public Guid Id { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; } = true;

        protected Entidade()
        {
            Id = Guid.NewGuid();
            DataCadastro = DateTime.UtcNow;
        }

        public static bool operator ==(Entidade e1, Entidade e2)
        {
            return e1.Id.Equals(e2.Id);
        }

        public static bool operator !=(Entidade e1, Entidade e2)
        {
            return !(e1 == e2);
        }

        /// <summary>
        /// Determina se o objeto especificado é igual ao objeto atual.
        /// </summary>
        /// <param name="obj">O objeto a ser comparado com o objeto atual.</param>
        /// <returns>true se o objeto especificado for igual ao objeto atual; caso contrário, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not Entidade other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
