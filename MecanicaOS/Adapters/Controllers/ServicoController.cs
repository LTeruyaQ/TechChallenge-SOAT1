using Core.Interfaces.UseCases;

namespace Adapters.Controllers
{
    public class ServicoController
    {
        private readonly IServicoUseCases _servicoUseCases;

        public ServicoController(IServicoUseCases servicoUseCases)
        {
            _servicoUseCases = servicoUseCases;
        }

    }
}
