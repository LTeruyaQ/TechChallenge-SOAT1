using Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;
using Dominio.Entidades;
using System.Collections.Generic;

namespace MecanicaOSTests.Fixtures
{
    public static class InsumoOSFixture
    {
        public static List<CadastrarInsumoOSRequest> CriarListaCadastrarInsumoOSRequestValida()
        {
            return new List<CadastrarInsumoOSRequest>
            {
                new CadastrarInsumoOSRequest { EstoqueId = Guid.NewGuid(), Quantidade = 1 },
                new CadastrarInsumoOSRequest { EstoqueId = Guid.NewGuid(), Quantidade = 2 }
            };
        }

        public static InsumoOS CriarInsumoOSValido()
        {
            return new InsumoOS
            {
                Id = Guid.NewGuid(),
                OrdemServicoId = Guid.NewGuid(),
                EstoqueId = Guid.NewGuid(),
                Quantidade = 1,
                Estoque = EstoqueFixture.CriarEstoque(50)
            };
        }

        public static InsumoOS CriarInsumoOS(int quantidade, decimal precoEstoque)
        {
            return new InsumoOS
            {
                Id = Guid.NewGuid(),
                OrdemServicoId = Guid.NewGuid(),
                EstoqueId = Guid.NewGuid(),
                Quantidade = quantidade,
                Estoque = EstoqueFixture.CriarEstoque(precoEstoque)
            };
        }

        public static List<InsumoOS> CriarListaInsumoOSValida()
        {
            return new List<InsumoOS>
            {
                CriarInsumoOSValido(),
                CriarInsumoOSValido()
            };
        }
    }
}
