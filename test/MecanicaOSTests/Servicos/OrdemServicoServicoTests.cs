using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using MediatR;
using Moq;
using Xunit;

namespace MecanicaOSTests.Servicos
{
    public class OrdemServicoServicoTests
    {
        private readonly Mock<IRepositorio<OrdemServico>> _repositorioMock;
        private readonly Mock<ILogServico<OrdemServicoServico>> _logMock;
        private readonly Mock<IUnidadeDeTrabalho> _udtMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRepositorio<Cliente>> _clienteRepositorioMock;
        private readonly Mock<IServicoServico> _servicoServicoMock;
        private readonly Mock<IUsuarioLogadoServico> _usuarioLogadoServicoMock;

        private readonly OrdemServicoServico _ordemServicoServico;

        public OrdemServicoServicoTests()
        {
            _repositorioMock = new Mock<IRepositorio<OrdemServico>>();
            _logMock = new Mock<ILogServico<OrdemServicoServico>>();
            _udtMock = new Mock<IUnidadeDeTrabalho>();
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _clienteRepositorioMock = new Mock<IRepositorio<Cliente>>();
            _servicoServicoMock = new Mock<IServicoServico>();
            _usuarioLogadoServicoMock = new Mock<IUsuarioLogadoServico>();

            _ordemServicoServico = new OrdemServicoServico(
                _repositorioMock.Object,
                _logMock.Object,
                _udtMock.Object,
                _mapperMock.Object,
                _mediatorMock.Object,
                _clienteRepositorioMock.Object,
                _servicoServicoMock.Object,
                _usuarioLogadoServicoMock.Object
            );
        }

        [Fact]
        public void DummyTest()
        {
            Assert.True(true);
        }
    }
}
