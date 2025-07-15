using Aplicacao.DTOs.Requests.Cliente;
using AutoMapper;
using Dominio.Entidades;

namespace Aplicacao.Mapeamentos
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<CadastrarClienteRequest, Cliente>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src => src.Sexo))
                .ForMember(dest => dest.Documento, opt => opt.MapFrom(src => src.Documento))
                .ForMember(dest => dest.DataNascimento, opt => opt.MapFrom(src => src.DataNascimento))
                .ForMember(dest => dest.TipoCliente, opt => opt.MapFrom(src => src.TipoCliente))
                .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => new Endereco
                {
                    Rua = src.Rua,
                    Bairro = src.Bairro,
                    Cidade = src.Cidade,
                    Numero = src.Numero,
                    CEP = src.CEP,
                    Complemento = src.Complemento
                }))
                .ForMember(dest => dest.Contato, opt => opt.MapFrom(src => new Contato
                {
                    Email = src.Email,
                    Telefone = src.Telefone
                }))
                .ForMember(dest => dest.Veiculos, opt => opt.Ignore());
        }
    }
}


