using Dominio.Especificacoes.Base;
using System.Linq.Expressions;

namespace Dominio.Especificacoes.Veiculo
{
    public class ObterVeiculosResumidosEspecificacao : EspecificacaoBase<Entidades.Veiculo>
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

        public override Expression<Func<Entidades.Veiculo, bool>> Expressao
        {
            get
            {
                return v =>
                    (string.IsNullOrEmpty(_placa) || v.Placa.Contains(_placa)) &&
                    (string.IsNullOrEmpty(_modelo) || v.Modelo.Contains(_modelo));
            }
        }
    }

    public class VeiculoResumidoDto
    {
        public Guid Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Ano { get; set; } = string.Empty;
    }
}
