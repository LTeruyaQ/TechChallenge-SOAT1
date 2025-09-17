using Core.DTOs.UseCases.Estoque;

namespace Core.UseCases.Estoques.CadastrarEstoque
{
    public class CadastrarEstoqueCommand
    {
        public CadastrarEstoqueUseCaseDto Request { get; set; }

        public CadastrarEstoqueCommand(CadastrarEstoqueUseCaseDto request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
