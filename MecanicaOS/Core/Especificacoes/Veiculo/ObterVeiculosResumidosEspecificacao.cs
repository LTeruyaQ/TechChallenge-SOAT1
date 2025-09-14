using Core.DTOs.Repositories.Veiculo;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Veiculo
{
    public class ObterVeiculosResumidosEspecificacao : EspecificacaoBase<VeiculoRepositoryDto>
    {
        private readonly string? _placa;
        private readonly string? _modelo;

        public ObterVeiculosResumidosEspecificacao(string? placa = null, string? modelo = null)
        {
            _placa = placa;
            _modelo = modelo;

            DefinirProjecao(v => new VeiculoResumidoDto
            {
                Id = v.Id,
                Placa = v.Placa,
                Modelo = v.Modelo,
                Ano = v.Ano
            });
        }

        public override Expression<Func<VeiculoRepositoryDto, bool>> Expressao
        {
            get
            {
                return v =>
                    (string.IsNullOrEmpty(_placa) || v.Placa.Contains(_placa)) &&
                    (string.IsNullOrEmpty(_modelo) || v.Modelo.Contains(_modelo));
            }
        }
    }
}
