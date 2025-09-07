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
            CreateMap<Estoque, EstoqueResponse>()
                .ReverseMap();

            CreateMap<Estoque, AtualizarEstoqueRequest>();
        }
    }
}