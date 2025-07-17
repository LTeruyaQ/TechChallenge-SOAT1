using Aplicacao.DTOs.Requests.Usuario;
using Bogus;
using Bogus.DataSets;
using Dominio.Entidades;
using Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MecanicaOSTests.Fixtures
{
    public static class UsuarioBogusFixture
    {
        private static readonly Faker _faker = new Faker("pt_BR");
        private static readonly string[] _dominios = { "gmail.com", "outlook.com", "hotmail.com", "yahoo.com.br" };
        private static readonly string[] _nomes = { "João", "Maria", "Pedro", "Ana", "Carlos", "Juliana" };
        private static readonly string[] _sobrenomes = { "Silva", "Santos", "Oliveira", "Souza", "Pereira" };

        public static CadastrarUsuarioRequest CriarCadastrarUsuarioRequestValido()
        {
            return new Faker<CadastrarUsuarioRequest>("pt_BR")
                .RuleFor(u => u.Email, f => f.Internet.Email(
                    f.PickRandom(_nomes), 
                    f.PickRandom(_sobrenomes), 
                    f.PickRandom(_dominios)))
                .RuleFor(u => u.Senha, f => f.Internet.Password(
                    length: 10, 
                    prefix: "P@ss"))
                .RuleFor(u => u.TipoUsuario, f => f.PickRandom<TipoUsuario>())
                .RuleFor(u => u.Documento, f => f.Random.Replace("###.###.###-##"))
                .RuleFor(u => u.RecebeAlertaEstoque, f => f.Random.Bool())
                .Generate();
        }

        public static AtualizarUsuarioRequest CriarAtualizarUsuarioRequestValido()
        {
            return new Faker<AtualizarUsuarioRequest>("pt_BR")
                .RuleFor(u => u.Email, f => f.Internet.Email(
                    f.PickRandom(_nomes), 
                    f.PickRandom(_sobrenomes), 
                    f.PickRandom(_dominios)))
                .RuleFor(u => u.TipoUsuario, f => f.PickRandom<TipoUsuario>())
                .RuleFor(u => u.Documento, f => f.Random.Replace("###.###.###-##"))
                .RuleFor(u => u.RecebeAlertaEstoque, f => f.Random.Bool())
                .Generate();
        }

        public static Usuario CriarUsuarioValido()
        {
            return new Faker<Usuario>("pt_BR")
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.Email, f => f.Internet.Email(
                    f.PickRandom(_nomes), 
                    f.PickRandom(_sobrenomes), 
                    f.PickRandom(_dominios)))
                .RuleFor(u => u.Senha, f => f.Internet.Password(10, prefix: "Hash"))
                .RuleFor(u => u.Ativo, true)
                .RuleFor(u => u.TipoUsuario, f => f.PickRandom<TipoUsuario>())
                .RuleFor(u => u.ClienteId, f => f.Random.Guid())
                .RuleFor(u => u.RecebeAlertaEstoque, f => f.Random.Bool())
                .RuleFor(u => u.DataCadastro, f => f.Date.Past(2))
                .RuleFor(u => u.DataAtualizacao, (f, u) => u.DataCadastro.AddDays(f.Random.Int(1, 30)))
                .Generate();
        }

        public static Usuario CriarUsuarioAdmin()
        {
            return new Faker<Usuario>("pt_BR")
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.Email, f => f.Internet.Email("admin", "sistema", "mecanicaos.com"))
                .RuleFor(u => u.Senha, f => f.Internet.Password(10, prefix: "Admin@"))
                .RuleFor(u => u.Ativo, true)
                .RuleFor(u => u.TipoUsuario, TipoUsuario.Admin)
                .RuleFor(u => u.RecebeAlertaEstoque, true)
                .RuleFor(u => u.DataCadastro, f => f.Date.Past(1))
                .Generate();
        }

        public static IEnumerable<Usuario> CriarUsuarios(int quantidade = 5)
        {
            return Enumerable.Range(1, quantidade)
                .Select(_ => CriarUsuarioValido())
                .ToList();
        }

        public static IEnumerable<Usuario> CriarUsuariosComTipo(TipoUsuario tipo, int quantidade = 3)
        {
            return Enumerable.Range(1, quantidade)
                .Select(_ =>
                {
                    var usuario = CriarUsuarioValido();
                    usuario.TipoUsuario = tipo;
                    return usuario;
                })
                .ToList();
        }
    }
}
