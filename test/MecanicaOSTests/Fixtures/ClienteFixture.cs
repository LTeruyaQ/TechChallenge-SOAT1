using System.Collections.Generic;
using Aplicacao.DTOs.Requests;
using Aplicacao.DTOs.Requests.Cliente;
using Aplicacao.DTOs.Responses;
using Aplicacao.DTOs.Responses.Cliente;
using Bogus;
using Dominio.Entidades;
using Xunit;
using Dominio.Enumeradores;

namespace MecanicaOSTests.Fixtures
{
    public class ClienteFixture
    {
        public Cliente GerarCliente()
        {
            return new Faker<Cliente>("pt_BR")
                .RuleFor(c => c.Nome, f => f.Person.FullName)
                .RuleFor(c => c.Sexo, f => f.Person.Gender.ToString())
                .RuleFor(c => c.Documento, f => f.Random.Replace("###.###.###-##"))
                .RuleFor(c => c.DataNascimento, f => f.Person.DateOfBirth.ToString())
                .RuleFor(c => c.TipoCliente, f => f.PickRandom<TipoCliente>())
                .RuleFor(c => c.Endereco, f => new Faker<Endereco>("pt_BR")
                    .RuleFor(e => e.Rua, fk => fk.Address.StreetName())
                    .RuleFor(e => e.Bairro, fk => fk.Address.StreetAddress())
                    .RuleFor(e => e.Cidade, fk => fk.Address.City())
                    .RuleFor(e => e.Numero, fk => fk.Address.State())
                    .RuleFor(e => e.CEP, fk => fk.Address.ZipCode())
                    .RuleFor(e => e.Complemento, fk => fk.Address.SecondaryAddress())
                    .Generate())
                .RuleFor(c => c.Contato, f => new Faker<Contato>("pt_BR")
                    .RuleFor(ct => ct.Email, fk => fk.Person.Email)
                    .RuleFor(ct => ct.Telefone, fk => fk.Phone.PhoneNumber())
                    .Generate())
                .Generate();
        }

        public List<Cliente> GerarClientes(int quantidade)
        {
            return new Faker<Cliente>("pt_BR")
                 .RuleFor(c => c.Nome, f => f.Person.FullName)
                .RuleFor(c => c.Sexo, f => f.Person.Gender.ToString())
                .RuleFor(c => c.Documento, f => f.Random.Replace("###.###.###-##"))
                .RuleFor(c => c.DataNascimento, f => f.Person.DateOfBirth.ToString())
                .RuleFor(c => c.TipoCliente, f => f.PickRandom<TipoCliente>())
                .RuleFor(c => c.Endereco, f => new Faker<Endereco>("pt_BR")
                    .RuleFor(e => e.Rua, fk => fk.Address.StreetName())
                    .RuleFor(e => e.Bairro, fk => fk.Address.StreetAddress())
                    .RuleFor(e => e.Cidade, fk => fk.Address.City())
                    .RuleFor(e => e.Numero, fk => fk.Address.State())
                    .RuleFor(e => e.CEP, fk => fk.Address.ZipCode())
                    .RuleFor(e => e.Complemento, fk => fk.Address.SecondaryAddress())
                    .Generate())
                .RuleFor(c => c.Contato, f => new Faker<Contato>("pt_BR")
                    .RuleFor(ct => ct.Email, fk => fk.Person.Email)
                    .RuleFor(ct => ct.Telefone, fk => fk.Phone.PhoneNumber())
                    .Generate())
                .Generate(quantidade);
        }

        public CadastrarClienteRequest GerarCadastroClienteRequest()
        {
            return new Faker<CadastrarClienteRequest>("pt_BR")
                .CustomInstantiator(f => new CadastrarClienteRequest
                {
                    Nome = f.Person.FullName,
                    Sexo = f.Person.Gender.ToString(),
                    Documento = f.Random.Replace("###.###.###-##"),
                    DataNascimento = f.Person.DateOfBirth.ToString(),
                    TipoCliente = f.PickRandom<TipoCliente>(),
                    Rua = f.Address.StreetName(),
                    Bairro = f.Address.StreetAddress(),
                    Cidade = f.Address.City(),
                    Numero = f.Address.State(),
                    CEP = f.Address.ZipCode(),
                    Complemento = f.Address.SecondaryAddress(),
                    Email = f.Person.Email,
                    Telefone = f.Phone.PhoneNumber()
                }).Generate();
        }

        public AtualizarClienteRequest GerarAtualizarClienteRequest()
        {
            return new Faker<AtualizarClienteRequest>("pt_BR")
                .CustomInstantiator(f => new AtualizarClienteRequest
                {
                    Nome = f.Person.FullName,
                    Sexo = f.Person.Gender.ToString(),
                    Documento = f.Random.Replace("###.###.###-##"),
                    DataNascimento = f.Person.DateOfBirth.ToString(),
                    TipoCliente = f.PickRandom<TipoCliente>(),
                    Rua = f.Address.StreetName(),
                    Bairro = f.Address.StreetAddress(),
                    Cidade = f.Address.City(),
                    Numero = f.Address.State(),
                    CEP = f.Address.ZipCode(),
                    Complemento = f.Address.SecondaryAddress(),
                    Email = f.Person.Email,
                    Telefone = f.Phone.PhoneNumber()
                }).Generate();
        }
    }
}
