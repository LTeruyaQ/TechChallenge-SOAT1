using Dominio.Entidades;
using Dominio.Enumeradores;

namespace MecanicaOSTests.Fixtures
{
    public class OrdemServicoFixture
    {
        public OrdemServico CriarOrdemServicoValida()
        {
            var cliente = new ClienteFixture().GerarCliente();
            var veiculo = new VeiculoFixture().GerarVeiculo();
            var servico = new ServicoFixture().GerarServico();

            return new OrdemServico
            {
                ClienteId = cliente.Id,
                Cliente = cliente,
                VeiculoId = veiculo.Id,
                Veiculo = veiculo,
                ServicoId = servico.Id,
                Servico = servico,
                Status = StatusOrdemServico.Recebida,
                Descricao = "Ordem de serviço de teste"
            };
        }
    }
}
