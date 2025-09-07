namespace Dominio.Exceptions.Estoque
{
    public class EstoquePrecoInvalidoException : DomainException
    {
        public EstoquePrecoInvalidoException()
            : base("O preço do estoque deve ser maior que zero.") { }
    }
}