using Core.DTOs.Entidades.Cliente;
using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Clientes
{
    public class CadastrarClienteHandlerTests
    {
        private readonly ClienteHandlerFixture _fixture;

        public CadastrarClienteHandlerTests()
        {
            _fixture = new ClienteHandlerFixture();
        }

        [Fact]
        public async Task Handle_DevePreservarCamposTecnicosERelacionamentosAoCadastrar()
        {
            // Arrange
            var request = new CadastrarClienteUseCaseDto
            {
                Nome = "Cliente Específico para Teste",
                Documento = "12345678900",
                TipoCliente = TipoCliente.PessoaFisica,
                DataNascimento = "01/01/1990",
                Sexo = "M",
                Email = "teste@exemplo.com",
                Telefone = "(11) 98765-4321",
                Rua = "Rua de Teste",
                Numero = "123",
                Complemento = "Apto 45",
                Bairro = "Bairro Teste",
                Cidade = "Cidade Teste",
                CEP = "01234-567"
            };

            // Capturar o DTO que será passado para o repositório
            ClienteEntityDto dtoCadastrado = null;
            var dataCadastro = DateTime.UtcNow;

            // Configurar o repositório para retornar null ao verificar se o cliente já existe
            _fixture.RepositorioCliente.ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>())
                .Returns((Cliente)null);
            
            _fixture.RepositorioCliente.CadastrarAsync(Arg.Any<ClienteEntityDto>())
                .Returns(callInfo =>
                {
                    dtoCadastrado = callInfo.Arg<ClienteEntityDto>();
                    
                    // Verificar campos técnicos
                    if (dtoCadastrado.DataCadastro == default)
                        throw new Exception("DataCadastro não foi preenchido");
                    // DataAtualizacao não é preenchido durante o cadastro, apenas em atualizações
                    if (dtoCadastrado.Ativo != true)
                        throw new Exception("Ativo não foi definido como true");
                        
                    dtoCadastrado.Id = Guid.NewGuid();
                    return dtoCadastrado;
                });

            EnderecoEntityDto enderecoDtoPassado = null;
            _fixture.RepositorioEndereco.CadastrarAsync(Arg.Any<EnderecoEntityDto>())
                .Returns(callInfo =>
                {
                    enderecoDtoPassado = callInfo.Arg<EnderecoEntityDto>();
                    enderecoDtoPassado.Id = Guid.NewGuid();
                    return enderecoDtoPassado;
                });

            ContatoEntityDto contatoDtoPassado = null;
            _fixture.RepositorioContato.CadastrarAsync(Arg.Any<ContatoEntityDto>())
                .Returns(callInfo =>
                {
                    contatoDtoPassado = callInfo.Arg<ContatoEntityDto>();
                    contatoDtoPassado.Id = Guid.NewGuid();
                    return contatoDtoPassado;
                });

            var handler = _fixture.CriarCadastrarClienteHandler();

            // Act
            var resultado = await handler.Handle(request);

            // Assert
            await _fixture.RepositorioCliente.Received(1).CadastrarAsync(Arg.Any<ClienteEntityDto>());

            // Verificar dados básicos
            dtoCadastrado.Should().NotBeNull();
            dtoCadastrado.Nome.Should().Be("Cliente Específico para Teste");
            dtoCadastrado.Documento.Should().Be("12345678900");
            dtoCadastrado.TipoCliente.Should().Be(TipoCliente.PessoaFisica);
            dtoCadastrado.DataNascimento.Should().Be("01/01/1990");
            dtoCadastrado.Sexo.Should().Be("M");
            
            // Verificar campos técnicos
            dtoCadastrado.Id.Should().NotBeEmpty();
            dtoCadastrado.Ativo.Should().BeTrue();
            dtoCadastrado.DataCadastro.Should().BeCloseTo(dataCadastro, TimeSpan.FromSeconds(5));
            // DataAtualizacao não é preenchido durante o cadastro, apenas em atualizações

            // Verificar relacionamentos
            enderecoDtoPassado.Should().NotBeNull();
            enderecoDtoPassado.Rua.Should().Be("Rua de Teste");
            enderecoDtoPassado.Numero.Should().Be("123");
            enderecoDtoPassado.Complemento.Should().Be("Apto 45");
            enderecoDtoPassado.Bairro.Should().Be("Bairro Teste");
            enderecoDtoPassado.Cidade.Should().Be("Cidade Teste");
            enderecoDtoPassado.CEP.Should().Be("01234-567");
            enderecoDtoPassado.IdCliente.Should().Be(dtoCadastrado.Id);
            
            contatoDtoPassado.Should().NotBeNull();
            contatoDtoPassado.Email.Should().Be("teste@exemplo.com");
            contatoDtoPassado.Telefone.Should().Be("(11) 98765-4321");
            contatoDtoPassado.IdCliente.Should().Be(dtoCadastrado.Id);

            // Verificar resultado
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(dtoCadastrado.Id);
            resultado.Nome.Should().Be("Cliente Específico para Teste");
            resultado.Ativo.Should().BeTrue();
            resultado.DataCadastro.Should().BeCloseTo(dataCadastro, TimeSpan.FromSeconds(5));
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), request);
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<Cliente>());
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveCadastrarCliente()
        {
            // Arrange
            var dto = ClienteHandlerFixture.CriarCadastrarClientePessoaFisicaDtoValido();
            var clienteId = Guid.NewGuid();

            // Configurar o repositório para retornar null ao verificar se o cliente já existe
            _fixture.RepositorioCliente.ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>())
                .Returns((Cliente)null);

            // Configurar o repositório para simular o cadastro
            var clienteCadastrado = new Cliente
            {
                Id = clienteId,
                Nome = dto.Nome,
                Documento = dto.Documento,
                DataNascimento = dto.DataNascimento,
                Sexo = dto.Sexo,
                TipoCliente = dto.TipoCliente,
                Ativo = true,
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };

            _fixture.ConfigurarMockRepositorioClienteParaCadastrar(clienteCadastrado);

            var handler = _fixture.CriarCadastrarClienteHandler();

            // Act
            var resultado = await handler.Handle(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().NotBeEmpty();
            resultado.Nome.Should().Be(dto.Nome);
            resultado.Documento.Should().Be(dto.Documento);
            resultado.TipoCliente.Should().Be(dto.TipoCliente);

            // Verificar que o repositório foi chamado para verificar se o cliente já existe
            await _fixture.RepositorioCliente.Received(1).ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>());

            // Verificar que o repositório foi chamado para cadastrar o cliente
            await _fixture.RepositorioCliente.Received(1).CadastrarAsync(Arg.Any<ClienteEntityDto>());

            // Verificar que os repositórios de endereço e contato foram chamados
            await _fixture.RepositorioEndereco.Received(1).CadastrarAsync(Arg.Any<EnderecoEntityDto>());
            await _fixture.RepositorioContato.Received(1).CadastrarAsync(Arg.Any<ContatoEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), dto);
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<Cliente>());
        }

        [Fact]
        public async Task Handle_ComDocumentoJaCadastrado_DeveLancarDadosJaCadastradosException()
        {
            // Arrange
            var dto = ClienteHandlerFixture.CriarCadastrarClientePessoaFisicaDtoValido();
            var clienteExistente = ClienteHandlerFixture.CriarClientePessoaFisicaValido();
            clienteExistente.Documento = dto.Documento;

            // Configurar o repositório para retornar um cliente existente ao verificar se o cliente já existe
            _fixture.RepositorioCliente.ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>())
                .Returns(clienteExistente);

            var handler = _fixture.CriarCadastrarClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dto);

            await act.Should().ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Cliente já cadastrado");

            // Verificar que o repositório foi chamado para verificar se o cliente já existe
            await _fixture.RepositorioCliente.Received(1).ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>());

            // Verificar que o repositório NÃO foi chamado para cadastrar o cliente
            await _fixture.RepositorioCliente.DidNotReceive().CadastrarAsync(Arg.Any<ClienteEntityDto>());

            // Verificar que os repositórios de endereço e contato NÃO foram chamados
            await _fixture.RepositorioEndereco.DidNotReceive().CadastrarAsync(Arg.Any<EnderecoEntityDto>());
            await _fixture.RepositorioContato.DidNotReceive().CadastrarAsync(Arg.Any<ContatoEntityDto>());

            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), dto);
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosJaCadastradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var dto = ClienteHandlerFixture.CriarCadastrarClientePessoaFisicaDtoValido();
            var clienteId = Guid.NewGuid();

            // Configurar o repositório para retornar null ao verificar se o cliente já existe
            _fixture.RepositorioCliente.ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>())
                .Returns((Cliente)null);

            // Criar um cliente para retornar do repositório
            var clienteCadastrado = new Cliente
            {
                Id = clienteId,
                Nome = dto.Nome,
                Documento = dto.Documento,
                DataNascimento = dto.DataNascimento,
                Sexo = dto.Sexo,
                TipoCliente = dto.TipoCliente,
                Ativo = true,
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };

            // Configurar o repositório para retornar o cliente cadastrado
            _fixture.ConfigurarMockRepositorioClienteParaCadastrar(clienteCadastrado);

            // Configurar o UDT para falhar no commit
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarCadastrarClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dto);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao cadastrar cliente");

            // Verificar que o repositório foi chamado para verificar se o cliente já existe
            await _fixture.RepositorioCliente.Received(1).ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>());

            // Verificar que o repositório foi chamado para cadastrar o cliente
            await _fixture.RepositorioCliente.Received(1).CadastrarAsync(Arg.Any<ClienteEntityDto>());

            // Verificar que os repositórios de endereço e contato foram chamados
            await _fixture.RepositorioEndereco.Received(1).CadastrarAsync(Arg.Any<EnderecoEntityDto>());
            await _fixture.RepositorioContato.Received(1).CadastrarAsync(Arg.Any<ContatoEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), dto);
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_QuandoRepositorioLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var dto = ClienteHandlerFixture.CriarCadastrarClientePessoaFisicaDtoValido();
            var excecaoEsperada = new Exception("Erro no banco de dados");

            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioCliente.ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>())
                .Returns(Task.FromException<Cliente>(excecaoEsperada));

            var handler = _fixture.CriarCadastrarClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dto);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro no banco de dados");

            // Verificar que o repositório foi chamado para verificar se o cliente já existe
            await _fixture.RepositorioCliente.Received(1).ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>());

            // Verificar que o repositório NÃO foi chamado para cadastrar o cliente
            await _fixture.RepositorioCliente.DidNotReceive().CadastrarAsync(Arg.Any<ClienteEntityDto>());

            // Verificar que os repositórios de endereço e contato NÃO foram chamados
            await _fixture.RepositorioEndereco.DidNotReceive().CadastrarAsync(Arg.Any<EnderecoEntityDto>());
            await _fixture.RepositorioContato.DidNotReceive().CadastrarAsync(Arg.Any<ContatoEntityDto>());

            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), dto);
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }

        [Theory]
        [InlineData(TipoCliente.PessoaFisica)]
        [InlineData(TipoCliente.PessoaJuridico)]
        public async Task Handle_ComDiferentesTiposCliente_DeveCadastrarCorretamente(TipoCliente tipoCliente)
        {
            // Arrange
            var dto = tipoCliente == TipoCliente.PessoaFisica
                ? ClienteHandlerFixture.CriarCadastrarClientePessoaFisicaDtoValido()
                : ClienteHandlerFixture.CriarCadastrarClientePessoaJuridicaDtoValido();

            dto.TipoCliente = tipoCliente;
            var clienteId = Guid.NewGuid();

            // Configurar o repositório para retornar null ao verificar se o cliente já existe
            _fixture.RepositorioCliente.ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>())
                .Returns((Cliente)null);

            // Capturar o DTO que está sendo passado para o repositório
            ClienteEntityDto clienteDtoPassado = null;
            _fixture.RepositorioCliente.CadastrarAsync(Arg.Any<ClienteEntityDto>())
                .Returns(callInfo =>
                {
                    clienteDtoPassado = callInfo.Arg<ClienteEntityDto>();
                    clienteDtoPassado.Id = clienteId;
                    return clienteDtoPassado;
                });

            var handler = _fixture.CriarCadastrarClienteHandler();

            // Act
            var resultado = await handler.Handle(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TipoCliente.Should().Be(tipoCliente);

            // Verificar que o cliente passado tem o tipo correto
            clienteDtoPassado.Should().NotBeNull();
            clienteDtoPassado.TipoCliente.Should().Be(tipoCliente);

            // Verificar que o repositório foi chamado para verificar se o cliente já existe
            await _fixture.RepositorioCliente.Received(1).ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>());

            // Verificar que o repositório foi chamado para cadastrar o cliente
            await _fixture.RepositorioCliente.Received(1).CadastrarAsync(Arg.Is<ClienteEntityDto>(c => c.TipoCliente == tipoCliente));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
        }
    }
}
