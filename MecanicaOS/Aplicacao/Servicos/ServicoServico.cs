using Aplicacao.DTOs.Requests.Servico;
using Aplicacao.DTOs.Responses.Servico;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos.Abstrato;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Especificacoes.Base.Extensoes;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Especificacoes.Servico;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos
{
    public class ServicoServico : ServicoAbstrato<ServicoServico, Servico>, IServicoServico
    {
        public ServicoServico(
            IRepositorio<Servico> repositorio,
            ILogServico<ServicoServico> logServico,
            IUnidadeDeTrabalho udt,
            IMapper mapper,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(repositorio, logServico, udt, mapper, usuarioLogadoServico)
        { }

        public async Task<ServicoResponse> CadastrarServicoAsync(CadastrarServicoRequest request)
        {
            var metodo = nameof(CadastrarServicoAsync);
            try
            {
                LogInicio(metodo, request);

                if (await ObterServicoPorNomeAsync(request.Nome) != null)
                    throw new DadosJaCadastradosException("Serviço já cadastrado");

                var servico = _mapper.Map<Servico>(request);

                var entidade = await _repositorio.CadastrarAsync(servico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar serviço");

                var response = _mapper.Map<ServicoResponse>(entidade);
                LogFim(metodo, response);
                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<ServicoResponse?> ObterServicoPorNomeAsync(string nome)
        {
            var metodo = nameof(ObterServicosDisponiveisAsync);
            try
            {
                LogInicio(metodo);

                var especificacao = new ObterServicoPorNomeEspecificacao(nome)
                    .E(new ObterServicoDisponivelEspecificacao());

                var servico = await _repositorio.ObterUmSemRastreamentoAsync(especificacao);

                var response = _mapper.Map<ServicoResponse>(servico);

                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task DeletarServicoAsync(Guid id)
        {
            var metodo = nameof(DeletarServicoAsync);
            try
            {
                LogInicio(metodo, id);

                var servico = await _repositorio.ObterPorIdAsync(id) ?? throw new DadosNaoEncontradosException("Serviço não encontrado");
                await _repositorio.DeletarAsync(servico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao deletar serviço");

                LogFim(metodo);
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<ServicoResponse> EditarServicoAsync(Guid id, EditarServicoRequest request)
        {
            var metodo = nameof(EditarServicoAsync);

            try
            {
                LogInicio(metodo, new { id, request });

                var servico = await _repositorio.ObterPorIdAsync(id);

                servico.Atualizar(request.Nome, request.Descricao, request.Valor, request.Disponivel);

                await _repositorio.EditarAsync(servico);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar serviço");

                var response = _mapper.Map<ServicoResponse>(servico);

                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<ServicoResponse> ObterServicoPorIdAsync(Guid id)
        {
            var metodo = nameof(ObterServicoPorIdAsync);
            try
            {
                LogInicio(metodo);

                var servico = await _repositorio.ObterPorIdAsync(id) ?? throw new DadosNaoEncontradosException("Serviço não encontrado");

                var response = _mapper.Map<ServicoResponse>(servico);
                
                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<ServicoResponse>> ObterServicosDisponiveisAsync()
        {
            var metodo = nameof(ObterServicosDisponiveisAsync);
            try
            {
                LogInicio(metodo);

                IEspecificacao<Servico> filtro = new ObterServicoDisponivelEspecificacao();

                var servicos = await _repositorio.ListarAsync(filtro);

                var response = _mapper.Map<IEnumerable<ServicoResponse>>(servicos);
                
                LogFim(metodo, servicos);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }

        public async Task<IEnumerable<ServicoResponse>> ObterTodosAsync()
        {
            var metodo = nameof(ObterTodosAsync);
            try
            {
                LogInicio(metodo);

                var servicos = await _repositorio.ObterTodosAsync();

                var response = _mapper.Map<IEnumerable<ServicoResponse>>(servicos);
                
                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
