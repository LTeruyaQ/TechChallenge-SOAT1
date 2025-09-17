using Core.DTOs.UseCases.Servico;

namespace Core.UseCases.Servicos.EditarServico
{
    public class EditarServicoCommand
    {
        public Guid Id { get; set; }
        public EditarServicoUseCaseDto Request { get; set; }

        public EditarServicoCommand(Guid id, EditarServicoUseCaseDto request)
        {
            Id = id;
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
