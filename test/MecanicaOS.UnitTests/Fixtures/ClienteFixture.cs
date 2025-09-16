using Core.DTOs.Entidades.Cliente;
using Core.Entidades;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Fixtures;

public class ClienteFixture : BaseFixture
{
    public static Cliente CriarClienteValido()
    {
        var cliente = CriarEntidadeComCamposObrigatorios<Cliente>();
        cliente.Nome = "João Silva";
        cliente.Documento = "12345678901";
        cliente.DataNascimento = "1990-01-01";
        cliente.Sexo = "M";
        cliente.TipoCliente = TipoCliente.PessoaFisica;
        cliente.Endereco = EnderecoFixture.CriarEnderecoValido();
        cliente.Contato = ContatoFixture.CriarContatoValido();
        cliente.Veiculos = new List<Veiculo>();

        return cliente;
    }

    public static Cliente CriarClienteComDadosInvalidos()
    {
        var cliente = CriarEntidadeComCamposObrigatorios<Cliente>();
        cliente.Nome = ""; // Nome vazio - inválido
        cliente.Documento = ""; // Documento vazio - inválido
        cliente.DataNascimento = ""; // Data vazia - inválida
        cliente.TipoCliente = TipoCliente.PessoaFisica;
        cliente.Endereco = EnderecoFixture.CriarEnderecoValido();
        cliente.Contato = ContatoFixture.CriarContatoValido();
        cliente.Veiculos = new List<Veiculo>();

        return cliente;
    }

    public static ClienteEntityDto CriarClienteEntityDtoValido()
    {
        var dto = CriarRepositoryDtoComCamposObrigatorios<ClienteEntityDto>();
        dto.Nome = "João Silva";
        dto.Documento = "12345678901";
        dto.DataNascimento = "1990-01-01";
        dto.Sexo = "M";
        dto.TipoCliente = TipoCliente.PessoaFisica;
        dto.Endereco = EnderecoFixture.CriarEnderecoEntityDtoValido();
        dto.Contato = ContatoFixture.CriarContatoEntityDtoValido();

        return dto;
    }

    public static List<Cliente> CriarListaClientesValidos(int quantidade = 3)
    {
        var clientes = new List<Cliente>();

        for (int i = 0; i < quantidade; i++)
        {
            var cliente = CriarClienteValido();
            cliente.Id = Guid.NewGuid();
            cliente.Nome = $"Cliente {i + 1}";
            cliente.Documento = $"1234567890{i}";
            clientes.Add(cliente);
        }

        return clientes;
    }

    public static List<ClienteEntityDto> CriarListaClienteEntityDtosValidos(int quantidade = 3)
    {
        var dtos = new List<ClienteEntityDto>();

        for (int i = 0; i < quantidade; i++)
        {
            var dto = CriarClienteEntityDtoValido();
            dto.Id = Guid.NewGuid();
            dto.Nome = $"Cliente {i + 1}";
            dto.Documento = $"1234567890{i}";
            dtos.Add(dto);
        }

        return dtos;
    }
}
