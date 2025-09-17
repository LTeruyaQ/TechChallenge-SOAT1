using Core.DTOs.UseCases.Estoque;

namespace Core.UseCases.Estoques.AtualizarEstoque
{
    public class AtualizarEstoqueCommand
    {
        public Guid Id { get; set; }
        public AtualizarEstoqueUseCaseDto Request { get; set; }

        public AtualizarEstoqueCommand(Guid id, AtualizarEstoqueUseCaseDto request)
        {
            Id = id;
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
