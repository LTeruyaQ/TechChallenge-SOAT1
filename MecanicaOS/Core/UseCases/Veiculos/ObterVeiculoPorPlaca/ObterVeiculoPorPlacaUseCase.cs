namespace Core.UseCases.Veiculos.ObterVeiculoPorPlaca
{
    public class ObterVeiculoPorPlacaUseCase
    {
        public string Placa { get; set; }

        public ObterVeiculoPorPlacaUseCase(string placa)
        {
            Placa = placa ?? throw new ArgumentNullException(nameof(placa));
        }
    }
}
