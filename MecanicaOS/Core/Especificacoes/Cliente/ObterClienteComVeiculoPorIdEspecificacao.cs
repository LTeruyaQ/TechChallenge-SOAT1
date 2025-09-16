using Core.DTOs.Entidades.Cliente;
using Core.Entidades;
using Core.Especificacoes.Base;
using System.Linq.Expressions;

namespace Core.Especificacoes.Cliente;

public class ObterClienteComVeiculoPorIdEspecificacao : EspecificacaoBase<ClienteEntityDto>
{
    private readonly Guid _clienteId;

    public ObterClienteComVeiculoPorIdEspecificacao(Guid clienteId)
    {
        _clienteId = clienteId;
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
            Contato = new Contato
            {
                Id = c.Contato.Id,
                Email = c.Contato.Email,
                Telefone = c.Contato.Telefone,
                IdCliente = c.Id
            },
            Endereco = new Endereco
            {
                Id = c.Endereco.Id,
                Numero = c.Endereco.Numero,
                Complemento = c.Endereco.Complemento,
                Bairro = c.Endereco.Bairro,
                Cidade = c.Endereco.Cidade,
                CEP = c.Endereco.CEP,
                IdCliente = c.Id
            },
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

    public override Expression<Func<ClienteEntityDto, bool>> Expressao =>
       i => i.Id == _clienteId;
}