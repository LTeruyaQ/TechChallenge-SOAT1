using Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;
using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using AutoMapper;
using Dominio.Entidades;

namespace Aplicacao.Mapeamentos;

public class InsumoOSProfile : Profile
{
    public InsumoOSProfile()
    {
        CreateMap<InsumoOS, InsumoOSResponse>().ReverseMap();

        CreateMap<CadastrarInsumoOSRequest, InsumoOS>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Ativo, opt => opt.MapFrom(_ => true));
    }
}