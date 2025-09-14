using Core.DTOs.UseCases.Cliente;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public static class ClienteUseCaseFixture
{
    public static CadastrarClienteUseCaseDto CriarCadastrarClienteUseCaseDtoValido()
    {
        return new CadastrarClienteUseCaseDto
        {
            Nome = "João Silva Santos",
            Sexo = "M",
            Documento = "12345678901",
            DataNascimento = "1985-03-15",
            TipoCliente = TipoCliente.PessoaFisica,
            Rua = "Rua das Flores",
            Bairro = "Centro",
            Cidade = "São Paulo",
            Numero = "123",
            CEP = "01234-567",
            Complemento = "Apto 45",
            Email = "joao.silva@email.com",
            Telefone = "(11) 99999-8888"
        };
    }

    public static CadastrarClienteUseCaseDto CriarCadastrarClienteUseCaseDtoPessoaJuridica()
    {
        return new CadastrarClienteUseCaseDto
        {
            Nome = "Empresa XYZ Ltda",
            Sexo = "O",
            Documento = "12345678000195",
            DataNascimento = "2010-01-01",
            TipoCliente = TipoCliente.PessoaJuridico,
            Rua = "Avenida Paulista",
            Bairro = "Bela Vista",
            Cidade = "São Paulo",
            Numero = "1000",
            CEP = "01310-100",
            Complemento = "Sala 501",
            Email = "contato@empresaxyz.com.br",
            Telefone = "(11) 3333-4444"
        };
    }

    public static CadastrarClienteUseCaseDto CriarCadastrarClienteUseCaseDtoSemComplemento()
    {
        return new CadastrarClienteUseCaseDto
        {
            Nome = "Maria Oliveira",
            Sexo = "F",
            Documento = "98765432100",
            DataNascimento = "1990-07-22",
            TipoCliente = TipoCliente.PessoaFisica,
            Rua = "Rua dos Jardins",
            Bairro = "Jardim América",
            Cidade = "São Paulo",
            Numero = "456",
            CEP = "05678-901",
            Complemento = null,
            Email = "maria.oliveira@email.com",
            Telefone = "(11) 88888-7777"
        };
    }

    public static AtualizarClienteUseCaseDto CriarAtualizarClienteUseCaseDtoValido()
    {
        return new AtualizarClienteUseCaseDto
        {
            Id = Guid.NewGuid(),
            Nome = "João Silva Santos Atualizado",
            Sexo = "M",
            Documento = "12345678901",
            DataNascimento = "1985-03-15",
            TipoCliente = TipoCliente.PessoaFisica,
            EnderecoId = Guid.NewGuid(),
            Rua = "Rua das Flores Atualizada",
            Bairro = "Centro",
            Cidade = "São Paulo",
            Numero = "123A",
            CEP = "01234-567",
            Complemento = "Apto 46",
            ContatoId = Guid.NewGuid(),
            Email = "joao.silva.novo@email.com",
            Telefone = "(11) 99999-9999"
        };
    }

    public static AtualizarClienteUseCaseDto CriarAtualizarClienteUseCaseDtoComCamposNulos()
    {
        return new AtualizarClienteUseCaseDto
        {
            Id = Guid.NewGuid(),
            Nome = null,
            Sexo = null,
            Documento = null,
            DataNascimento = null,
            TipoCliente = null,
            EnderecoId = Guid.NewGuid(),
            Rua = null,
            Bairro = null,
            Cidade = null,
            Numero = null,
            CEP = null,
            Complemento = null,
            ContatoId = Guid.NewGuid(),
            Email = null,
            Telefone = null
        };
    }

    public static AtualizarClienteUseCaseDto CriarAtualizarClienteUseCaseDtoApenasNome()
    {
        return new AtualizarClienteUseCaseDto
        {
            Id = Guid.NewGuid(),
            Nome = "Nome Atualizado",
            EnderecoId = Guid.NewGuid(),
            ContatoId = Guid.NewGuid()
        };
    }

    public static List<CadastrarClienteUseCaseDto> CriarListaCadastrarClienteUseCaseDto()
    {
        return new List<CadastrarClienteUseCaseDto>
        {
            CriarCadastrarClienteUseCaseDtoValido(),
            CriarCadastrarClienteUseCaseDtoPessoaJuridica(),
            CriarCadastrarClienteUseCaseDtoSemComplemento()
        };
    }

    public static List<AtualizarClienteUseCaseDto> CriarListaAtualizarClienteUseCaseDto()
    {
        return new List<AtualizarClienteUseCaseDto>
        {
            CriarAtualizarClienteUseCaseDtoValido(),
            CriarAtualizarClienteUseCaseDtoComCamposNulos(),
            CriarAtualizarClienteUseCaseDtoApenasNome()
        };
    }
}
