namespace MecanicaOSTests.Dominio.Especificacoes.Cliente
{
    public class ClienteEspecificacaoTests
    {
        private List<global::Dominio.Entidades.Cliente> GetClientesDeTeste()
        {
            var clienteComVeiculo = new global::Dominio.Entidades.Cliente { Id = Guid.NewGuid(), Nome = "Cliente Com Veiculo", Documento = "11111111111" };
            clienteComVeiculo.Veiculos.Add(new global::Dominio.Entidades.Veiculo { Id = Guid.NewGuid(), Placa = "ABC-1234", Cliente = clienteComVeiculo });

            var clienteSemVeiculo = new global::Dominio.Entidades.Cliente { Id = Guid.NewGuid(), Nome = "Cliente Sem Veiculo", Documento = "22222222222" };

            return new List<global::Dominio.Entidades.Cliente> { clienteComVeiculo, clienteSemVeiculo };
        }

        [Fact]
        public void ObterClienteComVeiculoPorIdEspecificacao_DeveRetornarClienteCorreto()
        {
            // Arrange
            var clientes = GetClientesDeTeste();
            var clienteComVeiculo = clientes.First(c => c.Veiculos.Any());
            var especificacao = new ObterClienteComVeiculoPorIdEspecificacao(clienteComVeiculo.Id);

            // Act
            var resultado = clientes.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Single(resultado);
            Assert.Equal(clienteComVeiculo.Id, resultado.First().Id);
        }

        [Fact]
        public void ObterClientePorDocumento_DeveRetornarClienteCorreto()
        {
            // Arrange
            var documento = "11111111111";
            var especificacao = new ObterClientePorDocumento(documento);
            var clientes = GetClientesDeTeste();

            // Act
            var resultado = clientes.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Single(resultado);
            Assert.Equal(documento, resultado.First().Documento);
        }
    }
}
