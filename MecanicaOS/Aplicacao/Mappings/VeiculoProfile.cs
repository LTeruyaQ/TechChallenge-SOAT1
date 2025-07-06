using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.DTOs.Responses.Veiculo;
using AutoMapper;
using Dominio.Entidades;

namespace Aplicacao.Mappings
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
