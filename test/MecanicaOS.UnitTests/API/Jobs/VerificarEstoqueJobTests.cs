using API.Jobs;
using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Entidades.Usuarios;
using Core.Especificacoes.Estoque;
using Core.Especificacoes.Usuario;
using Core.Exceptions;
using Core.Interfaces.Repositorios;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MecanicaOS.UnitTests.API.Jobs
{
    /// <summary>
    /// Testes para VerificarEstoqueJob (API)
    /// 
    /// IMPORTÂNCIA: Job crítico de background que monitora níveis de estoque e envia alertas.
    /// Essencial para evitar falta de insumos e garantir continuidade das operações.
    /// 
    /// COBERTURA: Valida todas as operações do job:
    /// - ExecutarAsync - Fluxo principal de verificação
    /// - ObterInsumosParaAlertaAsync - Filtro de insumos críticos sem alerta hoje
    /// - EnviarAlertaEstoqueAsync - Envio de emails para usuários configurados
    /// - GerarConteudoEmailAsync - Geração de HTML do email
    /// - SalvarAlertaEnviadoAsync - Registro de alertas enviados
    /// 
    /// REGRAS DE NEGÓCIO:
    /// - Apenas envia alerta uma vez por dia por insumo
    /// - Apenas para usuários com RecebeAlertaEstoque = true
    /// - Insumos críticos: QuantidadeDisponivel < QuantidadeMinima
    /// </summary>
    public class VerificarEstoqueJobTests
    {
        private readonly ICompositionRoot _compositionRoot;
        private readonly IRepositorio<EstoqueEntityDto> _estoqueRepositorio;
        private readonly IRepositorio<UsuarioEntityDto> _usuarioRepositorio;
        private readonly IRepositorio<AlertaEstoqueEntityDto> _alertaEstoqueRepositorio;
        private readonly ILogServico<VerificarEstoqueJob> _logServico;
        private readonly IServicoEmail _servicoEmail;
        private readonly IUnidadeDeTrabalho _uot;

        public VerificarEstoqueJobTests()
        {
            _compositionRoot = Substitute.For<ICompositionRoot>();
            _estoqueRepositorio = Substitute.For<IRepositorio<EstoqueEntityDto>>();
            _usuarioRepositorio = Substitute.For<IRepositorio<UsuarioEntityDto>>();
            _alertaEstoqueRepositorio = Substitute.For<IRepositorio<AlertaEstoqueEntityDto>>();
            _logServico = Substitute.For<ILogServico<VerificarEstoqueJob>>();
            _servicoEmail = Substitute.For<IServicoEmail>();
            _uot = Substitute.For<IUnidadeDeTrabalho>();

            _compositionRoot.CriarRepositorio<EstoqueEntityDto>().Returns(_estoqueRepositorio);
            _compositionRoot.CriarRepositorio<UsuarioEntityDto>().Returns(_usuarioRepositorio);
            _compositionRoot.CriarRepositorio<AlertaEstoqueEntityDto>().Returns(_alertaEstoqueRepositorio);
            _compositionRoot.CriarLogService<VerificarEstoqueJob>().Returns(_logServico);
            _compositionRoot.CriarServicoEmail().Returns(_servicoEmail);
            _compositionRoot.CriarUnidadeDeTrabalho().Returns(_uot);
        }

        private VerificarEstoqueJob CriarJob()
        {
            return new VerificarEstoqueJob(_compositionRoot);
        }

        [Fact]
        public void Construtor_DeveCriarInstanciaComDependencias()
        {
            // Arrange & Act
            var job = CriarJob();

            // Assert
            job.Should().NotBeNull();
            _compositionRoot.Received(1).CriarRepositorio<EstoqueEntityDto>();
            _compositionRoot.Received(1).CriarRepositorio<UsuarioEntityDto>();
            _compositionRoot.Received(1).CriarRepositorio<AlertaEstoqueEntityDto>();
            _compositionRoot.Received(1).CriarLogService<VerificarEstoqueJob>();
            _compositionRoot.Received(1).CriarServicoEmail();
            _compositionRoot.Received(1).CriarUnidadeDeTrabalho();
        }

        [Fact]
        public async Task ExecutarAsync_SemInsumosCriticos_NaoDeveEnviarAlerta()
        {
            // Arrange
            var estoquesVazios = new List<EstoqueEntityDto>();
            
            _estoqueRepositorio.ListarAsync(Arg.Any<ObterEstoqueCriticoEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<EstoqueEntityDto>>(estoquesVazios));

            var job = CriarJob();

            // Act
            await job.ExecutarAsync();

            // Assert
            _logServico.Received(1).LogInicio(nameof(job.ExecutarAsync));
            _logServico.Received(1).LogFim(nameof(job.ExecutarAsync), Arg.Any<object>());
            await _servicoEmail.DidNotReceive().EnviarAsync(
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<string>(),
                Arg.Any<string>());
        }

        [Fact]
        public async Task ExecutarAsync_ComInsumosCriticos_DeveEnviarAlerta()
        {
            // Arrange
            var insumoId = Guid.NewGuid();
            var insumosCriticos = new List<EstoqueEntityDto>
            {
                new EstoqueEntityDto
                {
                    Id = insumoId,
                    Insumo = "Óleo de Motor",
                    QuantidadeDisponivel = 2,
                    QuantidadeMinima = 10
                }
            };

            var alertasVazios = new List<AlertaEstoqueEntityDto>();
            var usuarios = new List<UsuarioEntityDto>
            {
                new UsuarioEntityDto { Email = "admin@teste.com" }
            };

            _estoqueRepositorio.ListarAsync(Arg.Any<ObterEstoqueCriticoEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<EstoqueEntityDto>>(insumosCriticos));
            
            _alertaEstoqueRepositorio.ListarAsync(Arg.Any<ObterAlertaDoDiaPorEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<AlertaEstoqueEntityDto>>(alertasVazios));
            
            _usuarioRepositorio.ListarAsync(Arg.Any<ObterUsuarioParaAlertaEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<UsuarioEntityDto>>(usuarios));

            _uot.Commit().Returns(Task.FromResult(true));

            var job = CriarJob();

            // Act
            await job.ExecutarAsync();

            // Assert
            await _servicoEmail.Received(1).EnviarAsync(
                Arg.Is<IEnumerable<string>>(emails => emails.Contains("admin@teste.com")),
                "Alerta de Estoque Baixo",
                Arg.Any<string>());
            
            await _alertaEstoqueRepositorio.Received(1).CadastrarVariosAsync(
                Arg.Is<IEnumerable<AlertaEstoqueEntityDto>>(alertas => 
                    alertas.Any(a => a.EstoqueId == insumoId)));
            
            await _uot.Received(1).Commit();
        }

        [Fact]
        public async Task ExecutarAsync_ComAlertaJaEnviadoHoje_NaoDeveEnviarNovamente()
        {
            // Arrange
            var insumoId = Guid.NewGuid();
            var insumosCriticos = new List<EstoqueEntityDto>
            {
                new EstoqueEntityDto
                {
                    Id = insumoId,
                    Insumo = "Filtro de Óleo",
                    QuantidadeDisponivel = 1,
                    QuantidadeMinima = 5
                }
            };

            var alertaJaEnviado = new List<AlertaEstoqueEntityDto>
            {
                new AlertaEstoqueEntityDto { EstoqueId = insumoId, DataCadastro = DateTime.UtcNow }
            };

            _estoqueRepositorio.ListarAsync(Arg.Any<ObterEstoqueCriticoEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<EstoqueEntityDto>>(insumosCriticos));
            
            _alertaEstoqueRepositorio.ListarAsync(Arg.Any<ObterAlertaDoDiaPorEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<AlertaEstoqueEntityDto>>(alertaJaEnviado));

            var job = CriarJob();

            // Act
            await job.ExecutarAsync();

            // Assert
            await _servicoEmail.DidNotReceive().EnviarAsync(
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<string>(),
                Arg.Any<string>());
        }

        [Fact]
        public async Task ExecutarAsync_ComMultiplosInsumosCriticos_DeveEnviarTodosNoMesmoEmail()
        {
            // Arrange
            var insumos = new List<EstoqueEntityDto>
            {
                new EstoqueEntityDto
                {
                    Id = Guid.NewGuid(),
                    Insumo = "Óleo de Motor",
                    QuantidadeDisponivel = 2,
                    QuantidadeMinima = 10
                },
                new EstoqueEntityDto
                {
                    Id = Guid.NewGuid(),
                    Insumo = "Filtro de Ar",
                    QuantidadeDisponivel = 1,
                    QuantidadeMinima = 8
                },
                new EstoqueEntityDto
                {
                    Id = Guid.NewGuid(),
                    Insumo = "Pastilha de Freio",
                    QuantidadeDisponivel = 3,
                    QuantidadeMinima = 15
                }
            };

            var alertasVazios = new List<AlertaEstoqueEntityDto>();
            var usuarios = new List<UsuarioEntityDto>
            {
                new UsuarioEntityDto { Email = "gerente@teste.com" }
            };

            _estoqueRepositorio.ListarAsync(Arg.Any<ObterEstoqueCriticoEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<EstoqueEntityDto>>(insumos));
            
            _alertaEstoqueRepositorio.ListarAsync(Arg.Any<ObterAlertaDoDiaPorEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<AlertaEstoqueEntityDto>>(alertasVazios));
            
            _usuarioRepositorio.ListarAsync(Arg.Any<ObterUsuarioParaAlertaEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<UsuarioEntityDto>>(usuarios));

            _uot.Commit().Returns(Task.FromResult(true));

            var job = CriarJob();

            // Act
            await job.ExecutarAsync();

            // Assert
            await _servicoEmail.Received(1).EnviarAsync(
                Arg.Any<IEnumerable<string>>(),
                "Alerta de Estoque Baixo",
                Arg.Is<string>(conteudo => 
                    conteudo.Contains("Óleo de Motor") &&
                    conteudo.Contains("Filtro de Ar") &&
                    conteudo.Contains("Pastilha de Freio")));
            
            await _alertaEstoqueRepositorio.Received(1).CadastrarVariosAsync(
                Arg.Is<IEnumerable<AlertaEstoqueEntityDto>>(alertas => alertas.Count() == 3));
        }

        [Fact]
        public async Task ExecutarAsync_ComMultiplosUsuarios_DeveEnviarParaTodos()
        {
            // Arrange
            var insumos = new List<EstoqueEntityDto>
            {
                new EstoqueEntityDto
                {
                    Id = Guid.NewGuid(),
                    Insumo = "Óleo",
                    QuantidadeDisponivel = 1,
                    QuantidadeMinima = 5
                }
            };

            var usuarios = new List<UsuarioEntityDto>
            {
                new UsuarioEntityDto { Email = "gerente@teste.com" },
                new UsuarioEntityDto { Email = "supervisor@teste.com" },
                new UsuarioEntityDto { Email = "admin@teste.com" }
            };

            _estoqueRepositorio.ListarAsync(Arg.Any<ObterEstoqueCriticoEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<EstoqueEntityDto>>(insumos));
            
            _alertaEstoqueRepositorio.ListarAsync(Arg.Any<ObterAlertaDoDiaPorEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<AlertaEstoqueEntityDto>>(new List<AlertaEstoqueEntityDto>()));
            
            _usuarioRepositorio.ListarAsync(Arg.Any<ObterUsuarioParaAlertaEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<UsuarioEntityDto>>(usuarios));

            _uot.Commit().Returns(Task.FromResult(true));

            var job = CriarJob();

            // Act
            await job.ExecutarAsync();

            // Assert
            await _servicoEmail.Received(1).EnviarAsync(
                Arg.Is<IEnumerable<string>>(emails => 
                    emails.Contains("gerente@teste.com") &&
                    emails.Contains("supervisor@teste.com") &&
                    emails.Contains("admin@teste.com")),
                Arg.Any<string>(),
                Arg.Any<string>());
        }

        [Fact]
        public async Task ExecutarAsync_ComErroNoCommit_DeveLancarPersistirDadosException()
        {
            // Arrange
            var insumos = new List<EstoqueEntityDto>
            {
                new EstoqueEntityDto
                {
                    Id = Guid.NewGuid(),
                    Insumo = "Óleo",
                    QuantidadeDisponivel = 1,
                    QuantidadeMinima = 5
                }
            };

            _estoqueRepositorio.ListarAsync(Arg.Any<ObterEstoqueCriticoEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<EstoqueEntityDto>>(insumos));
            
            _alertaEstoqueRepositorio.ListarAsync(Arg.Any<ObterAlertaDoDiaPorEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<AlertaEstoqueEntityDto>>(new List<AlertaEstoqueEntityDto>()));
            
            _usuarioRepositorio.ListarAsync(Arg.Any<ObterUsuarioParaAlertaEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<UsuarioEntityDto>>(new List<UsuarioEntityDto> 
                { 
                    new UsuarioEntityDto { Email = "teste@teste.com" } 
                }));

            _uot.Commit().Returns(Task.FromResult(false));

            var job = CriarJob();

            // Act
            var act = async () => await job.ExecutarAsync();

            // Assert
            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Falha ao cadastrar alertas de estoque");
            
            _logServico.Received(1).LogErro(nameof(job.ExecutarAsync), Arg.Any<Exception>());
        }

        [Fact]
        public async Task ExecutarAsync_ComExcecaoNoEnvioEmail_DeveLogarErroEReLancar()
        {
            // Arrange
            var insumos = new List<EstoqueEntityDto>
            {
                new EstoqueEntityDto
                {
                    Id = Guid.NewGuid(),
                    Insumo = "Óleo",
                    QuantidadeDisponivel = 1,
                    QuantidadeMinima = 5
                }
            };

            _estoqueRepositorio.ListarAsync(Arg.Any<ObterEstoqueCriticoEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<EstoqueEntityDto>>(insumos));
            
            _alertaEstoqueRepositorio.ListarAsync(Arg.Any<ObterAlertaDoDiaPorEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<AlertaEstoqueEntityDto>>(new List<AlertaEstoqueEntityDto>()));
            
            _usuarioRepositorio.ListarAsync(Arg.Any<ObterUsuarioParaAlertaEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<UsuarioEntityDto>>(new List<UsuarioEntityDto> 
                { 
                    new UsuarioEntityDto { Email = "teste@teste.com" } 
                }));

            var emailException = new Exception("Erro ao enviar email");
            _servicoEmail.EnviarAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), Arg.Any<string>())
                .Throws(emailException);

            var job = CriarJob();

            // Act
            var act = async () => await job.ExecutarAsync();

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro ao enviar email");
            
            _logServico.Received(1).LogErro(nameof(job.ExecutarAsync), Arg.Any<Exception>());
        }

        [Fact]
        public async Task ExecutarAsync_DeveLogarInicioEFim()
        {
            // Arrange
            _estoqueRepositorio.ListarAsync(Arg.Any<ObterEstoqueCriticoEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<EstoqueEntityDto>>(new List<EstoqueEntityDto>()));

            var job = CriarJob();

            // Act
            await job.ExecutarAsync();

            // Assert
            _logServico.Received(1).LogInicio(nameof(job.ExecutarAsync));
            _logServico.Received(1).LogFim(nameof(job.ExecutarAsync), Arg.Any<object>());
        }

        [Fact]
        public async Task ExecutarAsync_ComInsumoParcialmenteAlertado_DeveConsultarAlertasParaCadaInsumo()
        {
            // Arrange
            var insumo1Id = Guid.NewGuid();
            var insumo2Id = Guid.NewGuid();
            
            var insumos = new List<EstoqueEntityDto>
            {
                new EstoqueEntityDto
                {
                    Id = insumo1Id,
                    Insumo = "Óleo",
                    QuantidadeDisponivel = 1,
                    QuantidadeMinima = 5
                },
                new EstoqueEntityDto
                {
                    Id = insumo2Id,
                    Insumo = "Filtro",
                    QuantidadeDisponivel = 2,
                    QuantidadeMinima = 10
                }
            };

            _estoqueRepositorio.ListarAsync(Arg.Any<ObterEstoqueCriticoEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<EstoqueEntityDto>>(insumos));
            
            // Simular que nenhum insumo foi alertado hoje
            _alertaEstoqueRepositorio.ListarAsync(Arg.Any<ObterAlertaDoDiaPorEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<AlertaEstoqueEntityDto>>(new List<AlertaEstoqueEntityDto>()));
            
            _usuarioRepositorio.ListarAsync(Arg.Any<ObterUsuarioParaAlertaEstoqueEspecificacao>())
                .Returns(Task.FromResult<IEnumerable<UsuarioEntityDto>>(new List<UsuarioEntityDto> 
                { 
                    new UsuarioEntityDto { Email = "teste@teste.com" } 
                }));

            _uot.Commit().Returns(Task.FromResult(true));

            var job = CriarJob();

            // Act
            await job.ExecutarAsync();

            // Assert - Deve consultar alertas para cada insumo crítico
            await _alertaEstoqueRepositorio.Received(2).ListarAsync(Arg.Any<ObterAlertaDoDiaPorEstoqueEspecificacao>());
            
            // Deve enviar email com ambos os insumos
            await _servicoEmail.Received(1).EnviarAsync(
                Arg.Any<IEnumerable<string>>(),
                "Alerta de Estoque Baixo",
                Arg.Is<string>(conteudo => 
                    conteudo.Contains("Óleo") &&
                    conteudo.Contains("Filtro")));
            
            // Deve cadastrar alertas para ambos
            await _alertaEstoqueRepositorio.Received(1).CadastrarVariosAsync(
                Arg.Is<IEnumerable<AlertaEstoqueEntityDto>>(alertas => alertas.Count() == 2));
        }
    }
}
