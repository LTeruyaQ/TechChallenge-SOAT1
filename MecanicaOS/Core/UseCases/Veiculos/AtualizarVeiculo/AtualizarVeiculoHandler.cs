using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Veiculos.AtualizarVeiculo
{
    public class AtualizarVeiculoHandler : UseCasesAbstrato<AtualizarVeiculoHandler, Veiculo>
    {
        private readonly IVeiculoGateway _veiculoGateway;

        public AtualizarVeiculoHandler(
            IVeiculoGateway veiculoGateway,
            ILogServico<AtualizarVeiculoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _veiculoGateway = veiculoGateway ?? throw new ArgumentNullException(nameof(veiculoGateway));
        }

        public async Task<AtualizarVeiculoResponse> Handle(AtualizarVeiculoCommand command)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { command.Id, command.Request });

                var veiculo = await _veiculoGateway.ObterPorIdAsync(command.Id)
                    ?? throw new DadosNaoEncontradosException("Veículo não encontrado");

                if (command.Request.Placa != null) veiculo.Placa = command.Request.Placa;
                if (command.Request.Marca != null) veiculo.Marca = command.Request.Marca;
                if (command.Request.Modelo != null) veiculo.Modelo = command.Request.Modelo;
                if (command.Request.Cor != null) veiculo.Cor = command.Request.Cor;
                if (command.Request.Ano != null) veiculo.Ano = command.Request.Ano;
                if (command.Request.Anotacoes != null) veiculo.Anotacoes = command.Request.Anotacoes;
                if (command.Request.ClienteId.HasValue) veiculo.ClienteId = command.Request.ClienteId.Value;

                veiculo.DataAtualizacao = DateTime.UtcNow;

                await _veiculoGateway.EditarAsync(veiculo);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar veículo");

                LogFim(metodo, veiculo);

                return new AtualizarVeiculoResponse { Veiculo = veiculo };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
