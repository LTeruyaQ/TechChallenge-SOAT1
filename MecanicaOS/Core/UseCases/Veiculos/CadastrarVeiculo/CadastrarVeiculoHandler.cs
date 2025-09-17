using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Veiculos.CadastrarVeiculo
{
    public class CadastrarVeiculoHandler : UseCasesAbstrato<CadastrarVeiculoHandler, Veiculo>
    {
        private readonly IVeiculoGateway _veiculoGateway;

        public CadastrarVeiculoHandler(
            IVeiculoGateway veiculoGateway,
            ILogServico<CadastrarVeiculoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _veiculoGateway = veiculoGateway ?? throw new ArgumentNullException(nameof(veiculoGateway));
        }

        public async Task<CadastrarVeiculoResponse> Handle(CadastrarVeiculoCommand command)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, command.Request);

                Veiculo veiculo = new()
                {
                    Ano = command.Request.Ano,
                    Cor = command.Request.Cor,
                    Marca = command.Request.Marca,
                    Modelo = command.Request.Modelo,
                    Placa = command.Request.Placa,
                    Anotacoes = command.Request.Anotacoes,
                    ClienteId = command.Request.ClienteId,
                };

                await _veiculoGateway.CadastrarAsync(veiculo);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar veículo");

                LogFim(metodo, veiculo);

                return new CadastrarVeiculoResponse { Veiculo = veiculo };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
