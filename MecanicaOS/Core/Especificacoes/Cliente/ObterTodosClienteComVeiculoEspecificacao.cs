using Core.DTOs.Repositories.Cliente;
using Core.Entidades;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Cliente;

public class ObterTodosClienteComVeiculoEspecificacao : EspecificacaoBase<ClienteRepositoryDTO>
{
    public ObterTodosClienteComVeiculoEspecificacao()
    {
        AdicionarInclusao(c => c.Veiculos);
        DefinirProjecao(c => new Entidades.Cliente()
        {
            Id = c.Id,
            Ativo = c.Ativo,
            DataCadastro = c.DataCadastro,
            DataAtualizacao = c.DataAtualizacao,
            Nome = c.Nome,
            Documento = c.Documento,
            Sexo = c.Sexo,
            DataNascimento = c.DataNascimento,
            TipoCliente = c.TipoCliente,
            Veiculos = c.Veiculos.Select(v => new Entidades.Veiculo()
            {
                Id = v.Id,
                Ativo = v.Ativo,
                DataCadastro = v.DataCadastro,
                DataAtualizacao = v.DataAtualizacao,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Ano = v.Ano,
                Placa = v.Placa,
                ClienteId = v.ClienteId
            })
        });
    }

    public override Expression<Func<ClienteRepositoryDTO, bool>> Expressao => c => c.Ativo;
}