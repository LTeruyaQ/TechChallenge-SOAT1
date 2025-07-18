using Aplicacao.DTOs.Requests.Servico;
using Aplicacao.DTOs.Responses.Servico;
using AutoMapper;
using Dominio.Entidades;

namespace Aplicacao.Mapeamentos
{
    public class ServicoProfile : Profile
    {
        public ServicoProfile()
        {
            CreateMap<Servico, ServicoResponse>()
                .ReverseMap();

            CreateMap<CadastrarServicoRequest, Servico>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Ativo, opt => opt.MapFrom(_ => true));

            CreateMap<EditarServicoRequest, Servico>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
