using FluentAssertions;
using FluentAssertions.Execution;

namespace MecanicaOS.UnitTests.Helpers;

public static class FluentAssertionsExtensions
{
    public static class MensagensErro
    {
        public const string EntidadeNaoDeveSerNula = "a entidade não deve ser nula";
        public const string DtoNaoDeveSerNulo = "o DTO não deve ser nulo";
        public const string IdDeveSerValido = "o ID deve ser um GUID válido";
        public const string IdNaoDeveSerVazio = "o ID não deve ser vazio";
        public const string NomeNaoDeveSerVazio = "o nome não deve ser vazio ou nulo";
        public const string DocumentoNaoDeveSerVazio = "o documento não deve ser vazio ou nulo";
        public const string EmailDeveSerValido = "o email deve ter um formato válido";
        public const string TelefoneNaoDeveSerVazio = "o telefone não deve ser vazio ou nulo";
        public const string PlacaNaoDeveSerVazia = "a placa não deve ser vazia ou nula";
        public const string MarcaNaoDeveSerVazia = "a marca não deve ser vazia ou nula";
        public const string ModeloNaoDeveSerVazio = "o modelo não deve ser vazio ou nulo";
        public const string AnoDeveSerValido = "o ano deve ser válido (entre 1900 e ano atual + 1)";
        public const string DataCadastroDeveSerDefinida = "a data de cadastro deve ser definida";
        public const string DataAtualizacaoDeveSerDefinida = "a data de atualização deve ser definida";
        public const string AtivoDeveSerDefinido = "o campo Ativo deve ser definido";
        public const string ListaNaoDeveSerVazia = "a lista não deve ser vazia";
        public const string ListaNaoDeveSerNula = "a lista não deve ser nula";
        public const string QuantidadeEsperada = "a quantidade de itens deve ser a esperada";
        public const string ValorDeveSerPositivo = "o valor deve ser positivo";
        public const string ValorNaoDeveSerNegativo = "o valor não deve ser negativo";
        public const string EnderecoNaoDeveSerNulo = "o endereço não deve ser nulo";
        public const string ContatoNaoDeveSerNulo = "o contato não deve ser nulo";
        public const string LogradouroNaoDeveSerVazio = "o logradouro não deve ser vazio ou nulo";
        public const string BairroNaoDeveSerVazio = "o bairro não deve ser vazio ou nulo";
        public const string CidadeNaoDeveSerVazia = "a cidade não deve ser vazia ou nula";
        public const string EstadoNaoDeveSerVazio = "o estado não deve ser vazio ou nulo";
        public const string CepDeveSerValido = "o CEP deve ter um formato válido";
        public const string ClienteIdDeveSerValido = "o ClienteId deve ser um GUID válido";
        public const string TipoClienteDeveSerDefinido = "o tipo de cliente deve ser definido";
        public const string DataNascimentoNaoDeveSerVazia = "a data de nascimento não deve ser vazia ou nula";
    }
    
    public static void ComMensagem<T>(this T valor, string mensagem)
    {
        // Extensão simplificada para mensagens customizadas
        // Utilizada em conjunto com Should() do FluentAssertions
    }
}
