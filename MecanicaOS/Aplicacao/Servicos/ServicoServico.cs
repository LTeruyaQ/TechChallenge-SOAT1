using Dominio.DTOs;
using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Especificacoes.Base.Interfaces;
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
            await _repositorio.CadastrarAsync(servico);
        }

        public async Task DeletarServico(Guid id)
        {
            var servico = await ObterServicoPorId(id);
            await _repositorio.DeletarAsync(servico);
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

            novoServico.DataAtualizacao = DateTime.Now;

            await _repositorio.Editar(servico);
        }

        public async Task<Servico> ObterServicoPorId(Guid id)
        {
            var servico = await _repositorio.ObterPorIdAsync(id);
            
            if (servico is null) throw new Exception($"Não foi encontrado o serviço de id: {id}");

            return servico;
        }

        public async Task<IEnumerable<Servico>> ObterServicosPorFiltro(FiltrarServicoDto filtroDto)
        {
            IEspecificacao<Servico> filtro = new ServicoDisponivelEspecificacao();
            
            return await _repositorio.ObterPorFiltro(filtro);
        }

        public async Task<IEnumerable<Servico>> ObterTodos()
        {
            return await _repositorio.ObterTodos();
        }
    }
}
