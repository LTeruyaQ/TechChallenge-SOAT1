namespace Dominio.Exceptions.Estoque
{
    public class EstoqueQuantidadeMinimaException : DomainException
    {
        public EstoqueQuantidadeMinimaException()
            : base("Quantidade mínima não pode ser maior que a quantidade disponível.") { }
    }
}