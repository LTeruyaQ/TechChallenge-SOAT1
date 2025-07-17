using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.DTOs.Responses.Veiculo;
using AutoMapper;
using Dominio.Entidades;

namespace Aplicacao.Mapeamentos
{
    public class VeiculoProfile : Profile
    {
        public VeiculoProfile()
        {
            CreateMap<CadastrarVeiculoRequest, Veiculo>();
            CreateMap<Veiculo, VeiculoResponse>();
        }
    }
}
