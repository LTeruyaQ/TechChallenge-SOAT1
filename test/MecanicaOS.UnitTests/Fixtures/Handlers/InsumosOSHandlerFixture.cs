using Adapters.Gateways;
using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Entidades.OrdemServicos;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Enumeradores;
using Core.Especificacoes.Base.Interfaces;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Estoques;
using Core.Interfaces.Handlers.InsumosOS;
using Core.Interfaces.Handlers.OrdensServico;
using Core.Interfaces.Repositorios;
using Core.UseCases.Estoques.AtualizarEstoque;
using Core.UseCases.Estoques.ObterEstoque;
using Core.UseCases.InsumosOS.CadastrarInsumos;
using Core.UseCases.OrdensServico.ObterOrdemServico;

namespace MecanicaOS.UnitTests.Fixtures.Handlers
{
    public class InsumosOSHandlerFixture
    {
        // Handlers
        public IObterOrdemServicoHandler ObterOrdemServicoHandler { get; private set; }
        public IObterEstoqueHandler ObterEstoqueHandler { get; private set; }
        public IAtualizarEstoqueHandler AtualizarEstoqueHandler { get; private set; }

        // Serviços
        public IVerificarEstoqueJobGateway VerificarEstoqueJobGateway { get; }
        public ILogGateway<CadastrarInsumosHandler> LogServicoCadastrar { get; }
        public IUnidadeDeTrabalhoGateway UnidadeDeTrabalho { get; }
        public IUsuarioLogadoServicoGateway UsuarioLogadoServico { get; }

        // Repositórios (mockados)
        public IRepositorio<OrdemServicoEntityDto> RepositorioOrdemServico { get; }
        public IRepositorio<EstoqueEntityDto> RepositorioEstoque { get; }
        public IRepositorio<InsumoOSEntityDto> RepositorioInsumoOS { get; }

        // Gateways reais
        public IOrdemServicoGateway OrdemServicoGateway { get; }
        public IEstoqueGateway EstoqueGateway { get; }
        public IInsumosGateway InsumosGateway { get; }

        public InsumosOSHandlerFixture()
        {
            // Inicializar serviços mockados
            VerificarEstoqueJobGateway = Substitute.For<IVerificarEstoqueJobGateway>();
            LogServicoCadastrar = Substitute.For<ILogGateway<CadastrarInsumosHandler>>();
            UnidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalhoGateway>();
            UsuarioLogadoServico = Substitute.For<IUsuarioLogadoServicoGateway>();

            // Configurar UnidadeDeTrabalho para sucesso por padrão
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(true));

            // Inicializar repositórios mockados
            RepositorioOrdemServico = Substitute.For<IRepositorio<OrdemServicoEntityDto>>();
            RepositorioEstoque = Substitute.For<IRepositorio<EstoqueEntityDto>>();
            RepositorioInsumoOS = Substitute.For<IRepositorio<InsumoOSEntityDto>>();

            // Criar gateways reais com repositórios mockados
            OrdemServicoGateway = new OrdemServicoGateway(RepositorioOrdemServico);
            EstoqueGateway = new EstoqueGateway(RepositorioEstoque);
            InsumosGateway = new InsumosGateway(RepositorioInsumoOS);

            // Criar log services para handlers
            var logServicoObterOS = Substitute.For<ILogGateway<ObterOrdemServicoHandler>>();
            var logServicoObterEstoque = Substitute.For<ILogGateway<ObterEstoqueHandler>>();
            var logServicoAtualizarEstoque = Substitute.For<ILogGateway<AtualizarEstoqueHandler>>();

            // Criar handlers reais
            ObterOrdemServicoHandler = new ObterOrdemServicoHandler(
                OrdemServicoGateway,
                logServicoObterOS,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);

            ObterEstoqueHandler = new ObterEstoqueHandler(
                EstoqueGateway,
                logServicoObterEstoque,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);

            AtualizarEstoqueHandler = new AtualizarEstoqueHandler(
                EstoqueGateway,
                logServicoAtualizarEstoque,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public ICadastrarInsumosHandler CriarCadastrarInsumosHandler()
        {
            return new CadastrarInsumosHandler(
                LogServicoCadastrar,
                UnidadeDeTrabalho,
                UsuarioLogadoServico,
                VerificarEstoqueJobGateway,
                InsumosGateway);
        }


        public void ConfigurarMockOrdemServicoRepositorioParaObterPorId(Guid ordemServicoId, OrdemServico ordemServico)
        {
            // Criar DTO correspondente à entidade
            var ordemServicoDto = new OrdemServicoEntityDto
            {
                Id = ordemServico.Id,
                Descricao = ordemServico.Descricao,
                Status = ordemServico.Status,
                DataCadastro = ordemServico.DataCadastro,
                DataAtualizacao = ordemServico.DataAtualizacao
            };

            // Configurar o repositório para retornar o DTO quando consultado por ID
            RepositorioOrdemServico
                .ObterPorIdAsync(ordemServicoId)
                .Returns(ordemServicoDto);

            // Configurar o repositório para retornar a entidade quando projetada
            RepositorioOrdemServico
                .ObterUmProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>())
                .Returns(Task.FromResult(ordemServico));
        }

        public void ConfigurarMockEstoqueRepositorioParaObterPorId(Guid estoqueId, Estoque estoque)
        {
            var estoqueDto = new EstoqueEntityDto
            {
                Id = estoque.Id,
                Insumo = estoque.Insumo,
                Descricao = estoque.Descricao,
                Preco = estoque.Preco,
                QuantidadeDisponivel = estoque.QuantidadeDisponivel,
                QuantidadeMinima = estoque.QuantidadeMinima,
                Ativo = estoque.Ativo,
                DataCadastro = estoque.DataCadastro,
                DataAtualizacao = estoque.DataAtualizacao
            };

            // Configurar o repositório para retornar o DTO quando consultado por ID
            RepositorioEstoque
                .ObterPorIdAsync(estoqueId)
                .Returns(estoqueDto);

            // Configurar também o método ObterPorIdSemRastreamentoAsync que é usado pelo gateway
            RepositorioEstoque
                .ObterPorIdSemRastreamentoAsync(estoqueId)
                .Returns(estoqueDto);

            // Configurar o repositório para retornar a entidade quando projetada
            RepositorioEstoque
                .ObterUmProjetadoAsync<Estoque>(Arg.Any<IEspecificacao<EstoqueEntityDto>>())
                .Returns(Task.FromResult(estoque));
        }

        public List<EstoqueEntityDto> ConfigurarMockEstoqueRepositorioParaAtualizar(Guid estoqueId)
        {
            // Lista para capturar os DTOs enviados ao repositório
            var dtosCapturados = new List<EstoqueEntityDto>();

            // Configurar o repositório para capturar o DTO e retornar sucesso
            RepositorioEstoque
                .When(x => x.EditarAsync(Arg.Any<EstoqueEntityDto>()))
                .Do(callInfo =>
                {
                    var dto = callInfo.Arg<EstoqueEntityDto>();
                    if (dto.Id == estoqueId)
                    {
                        dtosCapturados.Add(dto);
                    }
                });

            RepositorioEstoque
                .EditarAsync(Arg.Any<EstoqueEntityDto>())
                .Returns(Task.CompletedTask);

            return dtosCapturados;
        }

        public void ConfigurarMockUdtParaCommitSucesso()
        {
            UnidadeDeTrabalho.Commit().Returns(Task.FromResult(true));
        }

        public void ConfigurarMockUdtParaCommitFalha()
        {
            UnidadeDeTrabalho
                .When(x => x.Commit())
                .Do(_ => { throw new PersistirDadosException("Erro ao atualizar estoque"); });
        }

        public List<InsumoOSEntityDto> ConfigurarMockInsumoOSRepositorioParaInserir()
        {
            // Lista para capturar os DTOs enviados ao repositório
            var dtosCapturados = new List<InsumoOSEntityDto>();

            // Configurar o repositório para capturar o DTO e retornar sucesso
            RepositorioInsumoOS
                .When(x => x.CadastrarAsync(Arg.Any<InsumoOSEntityDto>()))
                .Do(callInfo =>
                {
                    var dto = callInfo.Arg<InsumoOSEntityDto>();
                    dtosCapturados.Add(dto);
                });

            RepositorioInsumoOS
                .CadastrarAsync(Arg.Any<InsumoOSEntityDto>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<InsumoOSEntityDto>()));

            return dtosCapturados;
        }


        public static OrdemServico CriarOrdemServicoValida()
        {
            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Teste",
                Documento = "123.456.789-00",
                TipoCliente = TipoCliente.PessoaFisica,
                Ativo = true
            };

            var veiculo = new Veiculo
            {
                Id = Guid.NewGuid(),
                Placa = "ABC-1234",
                Modelo = "Gol",
                Marca = "Volkswagen",
                Ano = "2020",
                ClienteId = cliente.Id,
                Cliente = cliente
            };

            var servico = new Servico
            {
                Id = Guid.NewGuid(),
                Nome = "Troca de Óleo",
                Descricao = "Troca de óleo do motor",
                Valor = 100.00m,
                Disponivel = true
            };

            var ordemServico = new OrdemServico
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                Cliente = cliente,
                VeiculoId = veiculo.Id,
                Veiculo = veiculo,
                ServicoId = servico.Id,
                Servico = servico,
                Descricao = "Troca de óleo completa",
                Status = StatusOrdemServico.Recebida,
                InsumosOS = new List<InsumoOS>()
            };

            return ordemServico;
        }

        public static Estoque CriarEstoqueValido(int quantidadeDisponivel = 10, int quantidadeMinima = 2)
        {
            return new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Óleo de Motor",
                Descricao = "Óleo sintético 5W30",
                Preco = 50.00m,
                QuantidadeDisponivel = quantidadeDisponivel,
                QuantidadeMinima = quantidadeMinima
            };
        }

        public static List<CadastrarInsumoOSUseCaseDto> CriarListaInsumosValida(Guid estoqueId, int quantidade = 2)
        {
            return new List<CadastrarInsumoOSUseCaseDto>
            {
                new CadastrarInsumoOSUseCaseDto
                {
                    EstoqueId = estoqueId,
                    Quantidade = quantidade
                }
            };
        }

        public static List<CadastrarInsumoOSUseCaseDto> CriarListaInsumosMultiplosValida(List<Guid> estoqueIds, List<int> quantidades)
        {
            if (estoqueIds.Count != quantidades.Count)
                throw new ArgumentException("A quantidade de IDs de estoque deve ser igual à quantidade de quantidades");

            var insumos = new List<CadastrarInsumoOSUseCaseDto>();

            for (int i = 0; i < estoqueIds.Count; i++)
            {
                insumos.Add(new CadastrarInsumoOSUseCaseDto
                {
                    EstoqueId = estoqueIds[i],
                    Quantidade = quantidades[i]
                });
            }

            return insumos;
        }
    }
}
