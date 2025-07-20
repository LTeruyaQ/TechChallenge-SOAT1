using Aplicacao.Mapeamentos;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;

namespace MecanicaOSTests.Fixtures
{
    public abstract class BaseTestFixture<T> where T : class
    {
        protected readonly Mock<ILogger<T>> LoggerMock;
        protected readonly IMapper Mapper;

        protected BaseTestFixture()
        {
            LoggerMock = new Mock<ILogger<T>>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(new[] {
                    typeof(ClienteProfile),
                    typeof(EstoqueProfile),
                    typeof(ServicoProfile),
                    typeof(UsuarioProfile),
                    typeof(VeiculoProfile)
                });
            });

            Mapper = configuration.CreateMapper();
        }

        protected Mock<T> CreateServiceMock<T>() where T : class
        {
            return new Mock<T>();
        }

        protected void SetupLogger<T>(Mock<ILogger<T>> loggerMock, LogLevel expectedLogLevel, string expectedMessage)
        {
            loggerMock.Verify(
                x => x.Log(
                    expectedLogLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
