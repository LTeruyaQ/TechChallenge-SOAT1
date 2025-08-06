using Aplicacao.DTOs.Requests.Servico;
using Dominio.Entidades;

namespace MecanicaOSTests.Fixtures
{
    public class ServicoFixture
    {
        public CadastrarServicoRequest CriarCadastrarServicoRequestValido()
        {
            return new CadastrarServicoRequest
            {
                Nome = "Troca de Óleo",
                Descricao = "Troca de óleo e filtro",
                Valor = 150.50m,
                Disponivel = true
            };
        }

        public EditarServicoRequest CriarEditarServicoRequestValido()
        {
            return new EditarServicoRequest
            {
                Nome = "Troca de Óleo Premium",
                Descricao = "Troca de óleo sintético e filtro",
                Valor = 200.00m,
                Disponivel = true
            };
        }

        public Servico CriarServicoValido()
        {
            return new Servico
            {
                Id = Guid.NewGuid(),
                Nome = "Troca de Óleo",
                Descricao = "Troca de óleo e filtro",
                Valor = 150.50m,
                Disponivel = true,
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };
        }
    }
}
