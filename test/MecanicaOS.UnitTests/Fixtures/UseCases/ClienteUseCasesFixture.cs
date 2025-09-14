using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public class ClienteUseCasesFixture : UseCasesFixtureBase
{
    public ClienteUseCases CriarClienteUseCases(
        IClienteGateway? mockClienteGateway = null,
        IEnderecoGateway? mockEnderecoGateway = null,
        IContatoGateway? mockContatoGateway = null,
        ILogServico<ClienteUseCases>? mockLogServico = null,
        IUnidadeDeTrabalho? mockUdt = null,
        IUsuarioLogadoServico? mockUsuarioLogado = null)
    {
        if (mockClienteGateway == null)
            throw new ArgumentNullException(nameof(mockClienteGateway));
        if (mockEnderecoGateway == null)
            throw new ArgumentNullException(nameof(mockEnderecoGateway));
        if (mockContatoGateway == null)
            throw new ArgumentNullException(nameof(mockContatoGateway));
        if (mockLogServico == null)
            throw new ArgumentNullException(nameof(mockLogServico));
        if (mockUdt == null)
            throw new ArgumentNullException(nameof(mockUdt));
        if (mockUsuarioLogado == null)
            throw new ArgumentNullException(nameof(mockUsuarioLogado));

        ConfigurarMocksBasicos(mockUdt, mockUsuarioLogado);

        return new ClienteUseCases(
            mockClienteGateway,
            mockEnderecoGateway,
            mockContatoGateway,
            mockLogServico,
            mockUdt,
            mockUsuarioLogado);
    }

    public static Cliente CriarClienteValido()
    {
        return new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "João Silva Santos",
            Documento = "12345678901",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = DateTime.UtcNow.AddYears(-30).ToString("yyyy-MM-dd"),
            DataCadastro = DateTime.UtcNow.AddDays(-5),
            Contato = ContatoFixture.CriarContatoValido(),
            Endereco = EnderecoFixture.CriarEnderecoValido(),
            Veiculos = new List<Veiculo> { VeiculoFixture.CriarVeiculoValido() }
        };
    }

    public static Cliente CriarClientePessoaJuridica()
    {
        return new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "Empresa ABC Ltda",
            Documento = "12345678000195",
            TipoCliente = TipoCliente.PessoaJuridico,
            DataNascimento = DateTime.UtcNow.AddYears(-35).ToString("yyyy-MM-dd"),
            DataCadastro = DateTime.UtcNow.AddDays(-10),
            Contato = ContatoFixture.CriarContatoValido(),
            Endereco = EnderecoFixture.CriarEnderecoValido()
        };
    }

    public static Cliente CriarClienteInativo()
    {
        return new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "Maria Oliveira",
            Documento = "98765432100",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = DateTime.UtcNow.AddYears(-25).ToString("yyyy-MM-dd"),
            Ativo = false,
            DataCadastro = DateTime.UtcNow.AddDays(-90),
            DataAtualizacao = DateTime.UtcNow.AddDays(-30)
        };
    }

    public static CadastrarClienteUseCaseDto CriarCadastrarClienteUseCaseDtoValido()
    {
        return new CadastrarClienteUseCaseDto
        {
            Nome = "Carlos Eduardo Silva",
            Documento = "11144477735",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = new DateTime(1988, 3, 10).ToString("yyyy-MM-dd"),
            Rua = "Rua das Flores, 123",
            Numero = "123",
            Complemento = "Casa",
            Bairro = "Jardim das Rosas",
            Cidade = "São Paulo",
            CEP = "12345678",
            Email = "carlos.eduardo@email.com",
            Telefone = "11999888777"
        };
    }

    public static CadastrarClienteUseCaseDto CriarCadastrarClienteUseCaseDtoPessoaJuridica()
    {
        return new CadastrarClienteUseCaseDto
        {
            Nome = "Tech Solutions Ltda",
            Documento = "11222333000181",
            TipoCliente = TipoCliente.PessoaJuridico,
            DataNascimento = null,
            Rua = "Av. Paulista, 1000",
            Numero = "1000",
            Complemento = "Sala 1501",
            Bairro = "Bela Vista",
            Cidade = "São Paulo",
            CEP = "01310100",
            Email = "contato@techsolutions.com.br",
            Telefone = "1144556677"
        };
    }

    public static AtualizarClienteUseCaseDto CriarAtualizarClienteUseCaseDtoValido()
    {
        return new AtualizarClienteUseCaseDto
        {
            Nome = "João Silva Santos Atualizado",
            TipoCliente = TipoCliente.PessoaFisica,
            DataNascimento = new DateTime(1985, 5, 15).ToString("yyyy-MM-dd"),
            EnderecoId = Guid.NewGuid(),
            ContatoId = Guid.NewGuid(),
            Email = "joao.atualizado@email.com"
        };
    }


    public void ConfigurarMockClienteGatewayParaCadastro(
        IClienteGateway mockClienteGateway,
        Cliente? clienteRetorno = null)
    {
        clienteRetorno ??= CriarClienteValido();
        mockClienteGateway.CadastrarAsync(Arg.Any<Cliente>()).Returns(clienteRetorno);
        mockClienteGateway.ObterClientePorDocumentoAsync(Arg.Any<string>()).Returns(Task.FromResult<Cliente?>(null));
    }

    public void ConfigurarMockClienteGatewayParaAtualizacao(
        IClienteGateway mockClienteGateway,
        Cliente clienteExistente)
    {
        mockClienteGateway.ObterPorIdAsync(clienteExistente.Id).Returns(clienteExistente);
        mockClienteGateway.EditarAsync(Arg.Any<Cliente>()).Returns(Task.CompletedTask);
    }

    public void ConfigurarMockClienteGatewayParaClienteNaoEncontrado(
        IClienteGateway mockClienteGateway,
        Guid clienteId)
    {
        mockClienteGateway.ObterPorIdAsync(clienteId).Returns((Cliente?)null);
    }

    public void ConfigurarMockClienteGatewayParaDocumentoJaCadastrado(
        IClienteGateway mockClienteGateway,
        string documento)
    {
        var clienteExistente = CriarClienteValido();
        clienteExistente.Documento = documento;
        mockClienteGateway.ObterClientePorDocumentoAsync(documento).Returns(clienteExistente);
    }

    public void ConfigurarMockEnderecoGateway(
        IEnderecoGateway mockEnderecoGateway,
        Endereco? enderecoRetorno = null)
    {
        enderecoRetorno ??= EnderecoFixture.CriarEnderecoValido();
        mockEnderecoGateway.CadastrarAsync(Arg.Any<Endereco>()).Returns(Task.FromResult(enderecoRetorno));
        mockEnderecoGateway.ObterPorIdAsync(Arg.Any<Guid>()).Returns(enderecoRetorno);
        mockEnderecoGateway.EditarAsync(Arg.Any<Endereco>()).Returns(Task.CompletedTask);
    }

    public void ConfigurarMockContatoGateway(
        IContatoGateway mockContatoGateway,
        Contato? contatoRetorno = null)
    {
        contatoRetorno ??= ContatoFixture.CriarContatoValido();
        mockContatoGateway.CadastrarAsync(Arg.Any<Contato>()).Returns(Task.FromResult(contatoRetorno));
        mockContatoGateway.ObterPorIdAsync(Arg.Any<Guid>()).Returns(contatoRetorno);
        mockContatoGateway.EditarAsync(Arg.Any<Contato>()).Returns(Task.CompletedTask);
    }
}
