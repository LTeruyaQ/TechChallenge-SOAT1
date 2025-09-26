using Core.DTOs.Entidades.Cliente;
using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.UseCases.Clientes.CadastrarCliente;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

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

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var dto = new CadastrarClienteUseCaseDto
            {
                Nome = "Cliente Específico de Teste",
                Documento = "111.222.333-44",
                DataNascimento = "01/01/1990",
                Sexo = "M",
                TipoCliente = TipoCliente.PessoaFisica,
                Email = "teste.especifico@example.com",
                Telefone = "(11) 91234-5678",
                Rua = "Rua de Teste",
                Numero = "123",
                Complemento = "Apto 42",
                Bairro = "Bairro Teste",
                Cidade = "Cidade Teste",
                CEP = "12345-678"
            };
            
            var clienteId = Guid.NewGuid();
            
            // Configurar o repositório para retornar null ao verificar se o cliente já existe
            _fixture.RepositorioCliente.ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>())
                .Returns((Cliente)null);
            
            // Capturar os DTOs que estão sendo passados para os repositórios
            ClienteEntityDto clienteDtoPassado = null;
            _fixture.RepositorioCliente.CadastrarAsync(Arg.Any<ClienteEntityDto>())
                .Returns(callInfo => 
                {
                    clienteDtoPassado = callInfo.Arg<ClienteEntityDto>();
                    clienteDtoPassado.Id = clienteId;
                    return clienteDtoPassado;
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
            var resultado = await handler.Handle(dto);

            // Assert
            // Verificar que o DTO do cliente foi passado com os dados corretos (testando o trânsito de dados)
            clienteDtoPassado.Should().NotBeNull();
            clienteDtoPassado.Nome.Should().Be(dto.Nome);
            clienteDtoPassado.Documento.Should().Be(dto.Documento);
            clienteDtoPassado.DataNascimento.Should().Be(dto.DataNascimento);
            clienteDtoPassado.Sexo.Should().Be(dto.Sexo);
            clienteDtoPassado.TipoCliente.Should().Be(dto.TipoCliente);
            
            // Verificar que o DTO do endereço foi cadastrado com os dados corretos
            enderecoDtoPassado.Should().NotBeNull();
            enderecoDtoPassado.Rua.Should().Be(dto.Rua);
            enderecoDtoPassado.Numero.Should().Be(dto.Numero);
            enderecoDtoPassado.Complemento.Should().Be(dto.Complemento);
            enderecoDtoPassado.Bairro.Should().Be(dto.Bairro);
            enderecoDtoPassado.Cidade.Should().Be(dto.Cidade);
            enderecoDtoPassado.CEP.Should().Be(dto.CEP);
            enderecoDtoPassado.IdCliente.Should().Be(clienteId);
            
            // Verificar que o DTO do contato foi cadastrado com os dados corretos
            contatoDtoPassado.Should().NotBeNull();
            contatoDtoPassado.Email.Should().Be(dto.Email);
            contatoDtoPassado.Telefone.Should().Be(dto.Telefone);
            contatoDtoPassado.IdCliente.Should().Be(clienteId);
            
            // Verificar que os repositórios foram chamados corretamente
            await _fixture.RepositorioCliente.Received(1).ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ClienteEntityDto>>());
            await _fixture.RepositorioCliente.Received(1).CadastrarAsync(Arg.Any<ClienteEntityDto>());
            await _fixture.RepositorioEndereco.Received(1).CadastrarAsync(Arg.Any<EnderecoEntityDto>());
            await _fixture.RepositorioContato.Received(1).CadastrarAsync(Arg.Any<ContatoEntityDto>());
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar o resultado
            resultado.Should().NotBeNull();
            resultado.Id.Should().NotBeEmpty();
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
