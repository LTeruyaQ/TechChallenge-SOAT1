using Aplicacao.DTOs.Requests.OrdemServico;
using Dominio.Entidades;
using Dominio.Enumeradores;

namespace MecanicaOSTests.Fixtures
{
    public static class OrdemServicoFixture
    {
        public static CadastrarOrdemServicoRequest CriarCadastrarOrdemServicoRequestValido()
        {
            return new CadastrarOrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Descricao = "Problema no motor",
            };
        }

        public static AtualizarOrdemServicoRequest CriarAtualizarOrdemServicoRequestValido()
        {
            return new AtualizarOrdemServicoRequest
            {
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Descricao = "Atualização do problema no motor",
                Status = StatusOrdemServico.EmExecucao
            };
        }

        public static OrdemServico CriarOrdemServicoValida()
        {
            return new OrdemServico
            {
                Id = Guid.NewGuid(),
                ClienteId = Guid.NewGuid(),
                VeiculoId = Guid.NewGuid(),
                ServicoId = Guid.NewGuid(),
                Descricao = "Problema na suspensão",
                Status = StatusOrdemServico.Recebida
            };
        }

        public static OrdemServico CriarOrdemServicoAguardandoAprovacao()
        {
            var os = CriarOrdemServicoValida();
            os.Status = StatusOrdemServico.AguardandoAprovação;
            os.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-1);
            return os;
        }

        public static OrdemServico CriarOrdemServicoComOrcamentoExpirado()
        {
            var os = CriarOrdemServicoValida();
            os.Status = StatusOrdemServico.AguardandoAprovação;
            os.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-4);
            return os;
        }
    }
}
