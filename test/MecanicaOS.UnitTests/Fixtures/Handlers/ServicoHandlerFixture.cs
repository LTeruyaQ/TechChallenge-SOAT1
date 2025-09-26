using Adapters.Gateways;
using Core.DTOs.Entidades.Servico;
using Core.DTOs.UseCases.Servico;
using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Servicos;
using Core.Interfaces.Repositorios;
using Core.UseCases.Servicos.CadastrarServico;
using Core.UseCases.Servicos.DeletarServico;
using Core.UseCases.Servicos.EditarServico;
using Core.UseCases.Servicos.ObterServico;
using Core.UseCases.Servicos.ObterServicoPorNome;
using Core.UseCases.Servicos.ObterServicosDisponiveis;
using Core.UseCases.Servicos.ObterTodosServicos;

namespace MecanicaOS.UnitTests.Fixtures.Handlers
{
    public class ServicoHandlerFixture
    {
        // Repositórios
        public IRepositorio<ServicoEntityDto> RepositorioServico { get; }

        // Gateways
        public IServicoGateway ServicoGateway { get; }

        // LogServices
        public ILogGateway<CadastrarServicoHandler> LogServicoCadastrar { get; }
        public ILogGateway<EditarServicoHandler> LogServicoEditar { get; }
        public ILogGateway<DeletarServicoHandler> LogServicoDeletar { get; }
        public ILogGateway<ObterServicoHandler> LogServicoObter { get; }
        public ILogGateway<ObterTodosServicosHandler> LogServicoObterTodos { get; }
        public ILogGateway<ObterServicoPorNomeHandler> LogServicoObterPorNome { get; }
        public ILogGateway<ObterServicosDisponiveisHandler> LogServicoObterDisponiveis { get; }

        // Outros serviços
        public IUnidadeDeTrabalhoGateway UnidadeDeTrabalho { get; }
        public IUsuarioLogadoServicoGateway UsuarioLogadoServico { get; }

        public ServicoHandlerFixture()
        {
            // Inicializar repositórios mockados
            RepositorioServico = Substitute.For<IRepositorio<ServicoEntityDto>>();

            // Inicializar gateway real usando o repositório mockado
            ServicoGateway = new ServicoGateway(RepositorioServico);

            // Inicializar log services
            LogServicoCadastrar = Substitute.For<ILogGateway<CadastrarServicoHandler>>();
            LogServicoEditar = Substitute.For<ILogGateway<EditarServicoHandler>>();
            LogServicoDeletar = Substitute.For<ILogGateway<DeletarServicoHandler>>();
            LogServicoObter = Substitute.For<ILogGateway<ObterServicoHandler>>();
            LogServicoObterTodos = Substitute.For<ILogGateway<ObterTodosServicosHandler>>();
            LogServicoObterPorNome = Substitute.For<ILogGateway<ObterServicoPorNomeHandler>>();
            LogServicoObterDisponiveis = Substitute.For<ILogGateway<ObterServicosDisponiveisHandler>>();

            // Inicializar outros serviços
            UnidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalhoGateway>();
            UsuarioLogadoServico = Substitute.For<IUsuarioLogadoServicoGateway>();
        }

        // Métodos para criar handlers
        public ICadastrarServicoHandler CriarCadastrarServicoHandler()
        {
            return new CadastrarServicoHandler(
                ServicoGateway,
                LogServicoCadastrar,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public IEditarServicoHandler CriarEditarServicoHandler()
        {
            return new EditarServicoHandler(
                ServicoGateway,
                LogServicoEditar,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public IDeletarServicoHandler CriarDeletarServicoHandler()
        {
            return new DeletarServicoHandler(
                ServicoGateway,
                LogServicoDeletar,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public IObterServicoHandler CriarObterServicoHandler()
        {
            return new ObterServicoHandler(
                ServicoGateway,
                LogServicoObter,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public IObterTodosServicosHandler CriarObterTodosServicosHandler()
        {
            return new ObterTodosServicosHandler(
                ServicoGateway,
                LogServicoObterTodos,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public IObterServicoPorNomeHandler CriarObterServicoPorNomeHandler()
        {
            return new ObterServicoPorNomeHandler(
                ServicoGateway,
                LogServicoObterPorNome,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public IObterServicosDisponiveisHandler CriarObterServicosDisponiveisHandler()
        {
            return new ObterServicosDisponiveisHandler(
                ServicoGateway,
                LogServicoObterDisponiveis,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        // Métodos de configuração para mocks
        public void ConfigurarMockUdtParaCommitSucesso()
        {
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(true));
        }

        public void ConfigurarMockUdtParaCommitFalha()
        {
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(false));
        }

        // Métodos de configuração para o repositório
        public void ConfigurarMockRepositorioParaObterPorId(Guid id, Servico servico)
        {
            var dto = servico != null ? ToDto(servico) : null;
            RepositorioServico.ObterPorIdAsync(id).Returns(dto);
        }

        public void ConfigurarMockRepositorioParaObterPorIdNull(Guid id)
        {
            RepositorioServico.ObterPorIdAsync(id).Returns((ServicoEntityDto)null);
        }

        public void ConfigurarMockRepositorioParaObterTodos(List<Servico> servicos)
        {
            var dtos = servicos.Select(ToDto).ToList();
            RepositorioServico.ObterTodosAsync().Returns(dtos);
        }

        public void ConfigurarMockRepositorioParaObterPorNome(string nome, Servico servico)
        {
            // Configurar o repositório para responder à especificação
            RepositorioServico
                .ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ServicoEntityDto>>())
                .Returns(servico);
        }

        public void ConfigurarMockRepositorioParaObterPorNomeNull(string nome)
        {
            // Configurar o repositório para responder à especificação com null
            RepositorioServico
                .ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ServicoEntityDto>>())
                .Returns((Servico)null);
        }

        public void ConfigurarMockRepositorioParaObterDisponiveis(List<Servico> servicos)
        {
            // Configurar o repositório para responder à especificação
            RepositorioServico
                .ListarProjetadoAsync<Servico>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ServicoEntityDto>>())
                .Returns(servicos);
        }

        public void ConfigurarMockRepositorioParaCadastrar(Servico servico)
        {
            var dto = ToDto(servico);
            RepositorioServico.CadastrarAsync(Arg.Any<ServicoEntityDto>()).Returns(dto);
        }

        public void ConfigurarMockRepositorioParaEditar()
        {
            RepositorioServico.EditarAsync(Arg.Any<ServicoEntityDto>()).Returns(Task.CompletedTask);
        }

        public void ConfigurarMockRepositorioParaDeletar()
        {
            RepositorioServico.DeletarAsync(Arg.Any<ServicoEntityDto>()).Returns(Task.CompletedTask);
        }

        // Métodos de compatibilidade com o gateway (para facilitar migração dos testes)
        public void ConfigurarMockServicoGatewayParaObterPorId(Guid id, Servico servico)
        {
            ConfigurarMockRepositorioParaObterPorId(id, servico);
        }

        public void ConfigurarMockServicoGatewayParaObterPorIdNull(Guid id)
        {
            ConfigurarMockRepositorioParaObterPorIdNull(id);
        }

        public void ConfigurarMockServicoGatewayParaObterTodos(List<Servico> servicos)
        {
            ConfigurarMockRepositorioParaObterTodos(servicos);
        }

        public void ConfigurarMockServicoGatewayParaObterPorNome(string nome, Servico servico)
        {
            ConfigurarMockRepositorioParaObterPorNome(nome, servico);
        }

        public void ConfigurarMockServicoGatewayParaObterPorNomeNull(string nome)
        {
            ConfigurarMockRepositorioParaObterPorNomeNull(nome);
        }

        public void ConfigurarMockServicoGatewayParaObterDisponiveis(List<Servico> servicos)
        {
            ConfigurarMockRepositorioParaObterDisponiveis(servicos);
        }

        // Métodos para criar entidades de teste
        public static Servico CriarServicoValido(
            Guid? id = null,
            string nome = "Troca de Óleo",
            string descricao = "Troca de óleo completa",
            decimal valor = 150.00m,
            bool disponivel = true)
        {
            return new Servico
            {
                Id = id ?? Guid.NewGuid(),
                Nome = nome,
                Descricao = descricao,
                Valor = valor,
                Disponivel = disponivel,
                DataCadastro = DateTime.UtcNow.AddDays(-10),
                DataAtualizacao = DateTime.UtcNow.AddDays(-5),
                Ativo = true
            };
        }

        public static CadastrarServicoUseCaseDto CriarCadastrarServicoDto(
            string nome = "Troca de Óleo",
            string descricao = "Troca de óleo completa",
            decimal valor = 150.00m,
            bool disponivel = true)
        {
            return new CadastrarServicoUseCaseDto
            {
                Nome = nome,
                Descricao = descricao,
                Valor = valor,
                Disponivel = disponivel
            };
        }

        // Métodos de conversão entre Servico e ServicoEntityDto
        public static ServicoEntityDto ToDto(Servico servico)
        {
            return new ServicoEntityDto
            {
                Id = servico.Id,
                Ativo = servico.Ativo,
                DataCadastro = servico.DataCadastro,
                DataAtualizacao = servico.DataAtualizacao,
                Nome = servico.Nome,
                Descricao = servico.Descricao,
                Valor = servico.Valor,
                Disponivel = servico.Disponivel
            };
        }

        public static Servico FromDto(ServicoEntityDto dto)
        {
            return new Servico
            {
                Id = dto.Id,
                Ativo = dto.Ativo,
                DataCadastro = dto.DataCadastro,
                DataAtualizacao = dto.DataAtualizacao,
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Valor = dto.Valor,
                Disponivel = dto.Disponivel
            };
        }
    }
}
