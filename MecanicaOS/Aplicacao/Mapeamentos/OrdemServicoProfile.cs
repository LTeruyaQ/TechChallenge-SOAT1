using Aplicacao.DTOs.Requests.OrdermServico;
using Aplicacao.DTOs.Responses.OrdemServico;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;

namespace Aplicacao.Mapeamentos;

public class OrdemServicoProfile : Profile
{
    public OrdemServicoProfile()
    {
        CreateMap<OrdemServico, OrdemServicoResponse>()
               .ReverseMap();

        CreateMap<CadastrarOrdemServicoRequest, OrdemServico>()
              .ForMember(dest => dest.Id, opt => opt.Ignore())
              .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(_ => DateTime.UtcNow))
              .ForMember(dest => dest.Ativo, opt => opt.MapFrom(_ => true))
              .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => StatusOrdemServico.Recebida));

        CreateMap<AtualizarOrdemServicoRequest, OrdemServico>()
             .ForMember(dest => dest.Id, opt => opt.Ignore())
             .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(_ => DateTime.UtcNow))
             .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
