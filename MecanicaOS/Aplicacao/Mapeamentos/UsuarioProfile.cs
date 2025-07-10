using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.DTOs.Responses.Usuario;
using AutoMapper;
using Dominio.Entidades;

namespace Aplicacao.Mapeamentos;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<Usuario, UsuarioResponse>();

        CreateMap<CadastrarUsuarioRequest, Usuario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Ativo, opt => opt.MapFrom(_ => true));

        CreateMap<AtualizarUsuarioRequest, Usuario>()
              .ForMember(dest => dest.Id, opt => opt.Ignore())
              .ForMember(dest => dest.DataAtualizacao, opt => opt.MapFrom(_ => DateTime.UtcNow))
              .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
