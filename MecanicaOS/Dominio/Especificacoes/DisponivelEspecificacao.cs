using Dominio.Entidades;
using Dominio.Especificacoes.Base.Interfaces;

namespace Dominio.Especificacoes
{
    public class ServicoDisponivelEspecificacao : IEspecificacao<Servico>
    {
        public bool EhSatisfeitoPor(Servico servico)
        {
            return servico.Disponivel;
        }
    }
}
