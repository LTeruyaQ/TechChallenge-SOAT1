namespace MecanicaOSTests.Fixtures
{
    public static class ClienteFixture
    {
        public static CadastrarClienteRequest CriarCadastrarClienteRequestValido()
        {
            return new CadastrarClienteRequest
            {
                Nome = "João da Silva",
                Email = "joao@teste.com",
                Telefone = "11999998888",
                Documento = "12345678901"
            };
        }

        public static AtualizarClienteRequest CriarAtualizarClienteRequestValido()
        {
            return new AtualizarClienteRequest
            {
                Nome = "João da Silva Atualizado",
                Email = "joao.atualizado@teste.com",
                Telefone = "11999997777",
                Documento = "12345678901"
            };
        }

        public static Cliente CriarClienteValido()
        {
            return new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "João da Silva",
                Contato = new Contato
                {
                    Email = "joao@teste.com",
                    Telefone = "11999998888",
                },
                Documento = "12345678901",
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };
        }
    }
}
