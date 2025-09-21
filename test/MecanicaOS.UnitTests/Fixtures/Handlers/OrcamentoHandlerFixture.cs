using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Handlers.Orcamentos;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Orcamentos.GerarOrcamento;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace MecanicaOS.UnitTests.Fixtures.Handlers
{
    public class OrcamentoHandlerFixture
    {
        public ILogServico<GerarOrcamentoHandler> LogServico { get; }
        public IUnidadeDeTrabalho UnidadeDeTrabalho { get; }
        public IUsuarioLogadoServico UsuarioLogadoServico { get; }

        public OrcamentoHandlerFixture()
        {
            LogServico = Substitute.For<ILogServico<GerarOrcamentoHandler>>();
            UnidadeDeTrabalho = Substitute.For<IUnidadeDeTrabalho>();
            UsuarioLogadoServico = Substitute.For<IUsuarioLogadoServico>();
        }

        public IGerarOrcamentoHandler CriarGerarOrcamentoHandler()
        {
            return new GerarOrcamentoHandler(
                LogServico,
                UnidadeDeTrabalho,
                UsuarioLogadoServico);
        }

        public static OrdemServico CriarOrdemServicoComInsumosValida()
        {
            var servico = new Servico
            {
                Id = Guid.NewGuid(),
                Nome = "Troca de Óleo",
                Descricao = "Troca de óleo do motor",
                Valor = 100.00m,
                Disponivel = true
            };

            var estoque1 = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Óleo de Motor",
                Descricao = "Óleo sintético 5W30",
                Preco = 50.00m,
                QuantidadeDisponivel = 10,
                QuantidadeMinima = 2
            };

            var estoque2 = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Filtro de Óleo",
                Descricao = "Filtro de óleo compatível",
                Preco = 25.00m,
                QuantidadeDisponivel = 5,
                QuantidadeMinima = 1
            };

            var insumoOS1 = new InsumoOS
            {
                Id = Guid.NewGuid(),
                EstoqueId = estoque1.Id,
                Estoque = estoque1,
                Quantidade = 2
            };

            var insumoOS2 = new InsumoOS
            {
                Id = Guid.NewGuid(),
                EstoqueId = estoque2.Id,
                Estoque = estoque2,
                Quantidade = 1
            };

            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Teste",
                Documento = "123.456.789-00",
                TipoCliente = TipoCliente.PessoaFisica,
                Ativo = true
            };

            var veiculo = new Veiculo
            {
                Id = Guid.NewGuid(),
                Placa = "ABC-1234",
                Modelo = "Gol",
                Marca = "Volkswagen",
                Ano = "2020",
                ClienteId = cliente.Id,
                Cliente = cliente
            };

            var ordemServico = new OrdemServico
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                Cliente = cliente,
                VeiculoId = veiculo.Id,
                Veiculo = veiculo,
                ServicoId = servico.Id,
                Servico = servico,
                Descricao = "Troca de óleo completa",
                Status = StatusOrdemServico.Recebida,
                InsumosOS = new List<InsumoOS> { insumoOS1, insumoOS2 }
            };

            return ordemServico;
        }

        public static OrdemServico CriarOrdemServicoSemInsumosValida()
        {
            var servico = new Servico
            {
                Id = Guid.NewGuid(),
                Nome = "Alinhamento",
                Descricao = "Alinhamento das rodas",
                Valor = 80.00m,
                Disponivel = true
            };

            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Teste",
                Documento = "123.456.789-00",
                TipoCliente = TipoCliente.PessoaFisica,
                Ativo = true
            };

            var veiculo = new Veiculo
            {
                Id = Guid.NewGuid(),
                Placa = "ABC-1234",
                Modelo = "Gol",
                Marca = "Volkswagen",
                Ano = "2020",
                ClienteId = cliente.Id,
                Cliente = cliente
            };

            var ordemServico = new OrdemServico
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                Cliente = cliente,
                VeiculoId = veiculo.Id,
                Veiculo = veiculo,
                ServicoId = servico.Id,
                Servico = servico,
                Descricao = "Alinhamento das quatro rodas",
                Status = StatusOrdemServico.Recebida,
                InsumosOS = new List<InsumoOS>()
            };

            return ordemServico;
        }
    }
}
