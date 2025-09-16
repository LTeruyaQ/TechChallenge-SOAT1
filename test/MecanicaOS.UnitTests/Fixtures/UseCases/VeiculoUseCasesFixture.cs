using Core.DTOs.UseCases.Veiculo;
using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.UseCases;

namespace MecanicaOS.UnitTests.Fixtures.UseCases;

public class VeiculoUseCasesFixture : UseCasesFixtureBase
{

    public VeiculoUseCases CriarVeiculoUseCases(
        IVeiculoGateway? mockVeiculoGateway = null,
        IUnidadeDeTrabalho? mockUdt = null)
    {
        return new VeiculoUseCases(
            CriarMockLogServico<VeiculoUseCases>(),
            mockUdt ?? CriarMockUnidadeDeTrabalho(),
            CriarMockUsuarioLogadoServico(),
            mockVeiculoGateway ?? CriarMockVeiculoGateway());
    }

    public static Veiculo CriarVeiculoValido()
    {
        return new Veiculo
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            Marca = "Toyota",
            Modelo = "Corolla",
            Ano = "2020",
            Placa = "ABC1234",
            Cor = "Branco",
            Anotacoes = "Veículo em bom estado",
            DataCadastro = DateTime.UtcNow.AddDays(-30),
            DataAtualizacao = DateTime.UtcNow.AddDays(-1),
            Ativo = true
        };
    }

    public static Veiculo CriarVeiculoSemAnotacoes()
    {
        return new Veiculo
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            Marca = "Honda",
            Modelo = "Civic",
            Ano = "2019",
            Placa = "XYZ5678",
            Cor = "Preto",
            Anotacoes = null,
            DataCadastro = DateTime.UtcNow.AddDays(-15),
            DataAtualizacao = DateTime.UtcNow.AddDays(-15),
            Ativo = true
        };
    }

    public static Veiculo CriarVeiculoAntigo()
    {
        return new Veiculo
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            Marca = "Volkswagen",
            Modelo = "Gol",
            Ano = "2018",
            Placa = "OLD9999",
            Cor = "Azul",
            Anotacoes = "Veículo antigo, necessita manutenção frequente",
            DataCadastro = DateTime.UtcNow.AddDays(-365),
            DataAtualizacao = DateTime.UtcNow.AddDays(-30),
            Ativo = true
        };
    }

    public static Veiculo CriarVeiculoInativo()
    {
        return new Veiculo
        {
            Id = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            Marca = "Ford",
            Modelo = "Ka",
            Ano = "2015",
            Placa = "INA0000",
            Cor = "Vermelho",
            Anotacoes = "Veículo vendido",
            DataCadastro = DateTime.UtcNow.AddDays(-100),
            DataAtualizacao = DateTime.UtcNow.AddDays(-50),
            Ativo = false
        };
    }

    public static CadastrarVeiculoUseCaseDto CriarCadastrarVeiculoUseCaseDtoValido()
    {
        return new CadastrarVeiculoUseCaseDto
        {
            ClienteId = Guid.NewGuid(),
            Marca = "Chevrolet",
            Modelo = "Onix",
            Ano = "2022",
            Placa = "NEW1234",
            Cor = "Prata",
            Anotacoes = "Veículo novo, primeira revisão"
        };
    }

    public static CadastrarVeiculoUseCaseDto CriarCadastrarVeiculoSemAnotacoes()
    {
        return new CadastrarVeiculoUseCaseDto
        {
            ClienteId = Guid.NewGuid(),
            Marca = "Nissan",
            Modelo = "March",
            Ano = "2021",
            Placa = "SEM1111",
            Cor = "Branco",
            Anotacoes = null
        };
    }

    public static AtualizarVeiculoUseCaseDto CriarAtualizarVeiculoUseCaseDtoValido()
    {
        return new AtualizarVeiculoUseCaseDto
        {
            ClienteId = Guid.NewGuid(),
            Marca = "Toyota",
            Modelo = "Corolla Cross",
            Ano = "2023",
            Placa = "UPD4321",
            Cor = "Cinza",
            Anotacoes = "Veículo atualizado com novas informações"
        };
    }

    public static AtualizarVeiculoUseCaseDto CriarAtualizarVeiculoParcial()
    {
        return new AtualizarVeiculoUseCaseDto
        {
            ClienteId = null,
            Marca = null,
            Modelo = "Modelo Atualizado",
            Ano = null,
            Placa = "PAR5555",
            Cor = null,
            Anotacoes = "Apenas modelo e placa atualizados"
        };
    }

    public static List<Veiculo> CriarListaVeiculosVariados()
    {
        return new List<Veiculo>
        {
            CriarVeiculoValido(),
            CriarVeiculoSemAnotacoes(),
            CriarVeiculoAntigo(),
            CriarVeiculoInativo()
        };
    }

    public static List<Veiculo> CriarListaVeiculosPorCliente(Guid clienteId)
    {
        var veiculos = CriarListaVeiculosVariados();
        foreach (var veiculo in veiculos.Take(2)) // Apenas os 2 primeiros pertencem ao cliente
        {
            veiculo.ClienteId = clienteId;
        }
        return veiculos.Take(2).ToList();
    }

    public void ConfigurarMockVeiculoGatewayParaCadastro(
        IVeiculoGateway mockGateway,
        Veiculo? veiculoEsperado = null)
    {
        var veiculo = veiculoEsperado ?? CriarVeiculoValido();
        mockGateway.CadastrarAsync(Arg.Any<Veiculo>()).Returns(Task.FromResult(veiculo));
    }

    public void ConfigurarMockVeiculoGatewayParaAtualizacao(
        IVeiculoGateway mockGateway,
        Veiculo veiculoExistente)
    {
        mockGateway.ObterPorIdAsync(veiculoExistente.Id).Returns(Task.FromResult(veiculoExistente));
        mockGateway.EditarAsync(Arg.Any<Veiculo>()).Returns(Task.CompletedTask);
    }

    public void ConfigurarMockVeiculoGatewayParaVeiculoNaoEncontrado(
        IVeiculoGateway mockGateway,
        Guid veiculoId)
    {
        mockGateway.ObterPorIdAsync(veiculoId).Returns((Veiculo?)null);
    }

    public void ConfigurarMockVeiculoGatewayParaListagem(
        IVeiculoGateway mockGateway,
        List<Veiculo> veiculos)
    {
        mockGateway.ObterTodosAsync().Returns(veiculos);
    }

    public void ConfigurarMockVeiculoGatewayParaBuscaPorCliente(
        IVeiculoGateway mockGateway,
        Guid clienteId,
        List<Veiculo> veiculos)
    {
        mockGateway.ObterVeiculoPorClienteAsync(clienteId).Returns(veiculos);
    }

    public void ConfigurarMockVeiculoGatewayParaBuscaPorPlaca(
        IVeiculoGateway mockGateway,
        string placa,
        List<Veiculo> veiculos)
    {
        mockGateway.ObterVeiculoPorPlacaAsync(placa).Returns(veiculos);
    }

    public void ConfigurarMockVeiculoGatewayParaDelecao(
        IVeiculoGateway mockGateway,
        Veiculo veiculo)
    {
        mockGateway.ObterPorIdAsync(veiculo.Id).Returns(Task.FromResult(veiculo));
        mockGateway.DeletarAsync(veiculo).Returns(Task.FromResult(true));
    }
}
