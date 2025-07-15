using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Responses.Estoque;
using AutoMapper;
using Dominio.Entidades;

namespace Aplicacao.Mapeamentos
{
    public class EstoqueProfile : Profile
    {
        public EstoqueProfile()
        {
            CreateMap<Estoque, EstoqueResponse>();

            CreateMap<CadastrarEstoqueRequest, Estoque>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Ativo, opt => opt.MapFrom(_ => true));
        }
    }
}


