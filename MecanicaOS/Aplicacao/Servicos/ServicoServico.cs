using Dominio.Entidades;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Services;

namespace Aplicacao.Servicos
{
    public class ServicoServico : IServicoServico
    {
        private readonly ICrudRepositorio<Servico> _repositorio;

        public ServicoServico(ICrudRepositorio<Servico> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task CadastrarServico(Servico servico)
        {
            await _repositorio.Cadastrar(servico);
        }

        public async Task DeletarServico(Guid id)
        {
            var servico = await ObterServicoPorId(id);
            await _repositorio.Deletar(servico);
        }

        public async Task EditarServico(Guid id, Servico novoServico)
        {
            var servico = await ObterServicoPorId(id);

            if (servico.Nome != novoServico.Nome)
                servico.Nome = novoServico.Nome;

            if (servico.Descricao != novoServico.Descricao)
                servico.Descricao = novoServico.Descricao;

            if (servico.Valor != novoServico.Valor)
                servico.Valor = novoServico.Valor;

            if (servico.Disponivel != novoServico.Disponivel)
                servico.Disponivel = novoServico.Disponivel;

            await _repositorio.Editar(servico);
        }

        public async Task<Servico> ObterServicoPorId(Guid id)
        {
            var servico = await _repositorio.ObterPorId(id);
            
            if (servico is null) throw new Exception($"Não foi encontrado o serviço de id: {id}");

            return servico;
        }

        public async Task<IEnumerable<Servico>> ObterServicosPorFiltro(Servico filtro)
        {
            throw new NotImplementedException();
        }
    }
}
