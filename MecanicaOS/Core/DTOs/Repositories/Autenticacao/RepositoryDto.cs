namespace Core.DTOs.Repositories.Autenticacao
{
    public abstract class RepositoryDto
    {
        public Guid Id { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}
