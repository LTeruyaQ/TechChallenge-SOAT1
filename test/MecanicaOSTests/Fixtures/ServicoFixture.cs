namespace MecanicaOSTests.Fixtures
{
    public static class ServicoFixture
    {
        public static CadastrarServicoRequest CriarCadastrarServicoRequestValido()
        {
            return new CadastrarServicoRequest
            {
                Nome = "Troca de Óleo",
                Descricao = "Troca de óleo e filtro",
                Valor = 150.50m,
                Disponivel = true
            };
        }

        public static EditarServicoRequest CriarEditarServicoRequestValido()
        {
            return new EditarServicoRequest
            {
                Nome = "Troca de Óleo Premium",
                Descricao = "Troca de óleo sintético e filtro",
                Valor = 200.00m,
                Disponivel = true
            };
        }

        public static Servico CriarServicoValido()
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
