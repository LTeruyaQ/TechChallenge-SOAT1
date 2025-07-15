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

        CreateMap<OrdemServico, AtualizarOrdemServicoRequest>()
            .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.ClienteId))
            .ForMember(dest => dest.VeiculoId, opt => opt.MapFrom(src => src.VeiculoId))
            .ForMember(dest => dest.ServicoId, opt => opt.MapFrom(src => src.ServicoId))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<CadastrarOrdemServicoRequest, OrdemServico>()
              .ForMember(dest => dest.Id, opt => opt.Ignore())
              .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(_ => DateTime.UtcNow))
              .ForMember(dest => dest.Ativo, opt => opt.MapFrom(_ => true))
              .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => StatusOrdemServico.Recebida));
    }
}
