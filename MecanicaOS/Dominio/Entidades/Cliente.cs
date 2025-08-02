using Dominio.Entidades.Abstratos;
using Dominio.Enumeradores;
using Dominio.Exceptions;
using System.Text.RegularExpressions;

namespace Dominio.Entidades;

public class Cliente : Entidade
{
    public string Nome { get; set; } = default!;
    public string? NomeFantasia { get; set; }
    public string? Sexo { get; set; }
    public string Documento { get; set; } = default!;
    public string DataNascimento { get; set; } = default!;
    public TipoCliente TipoCliente { get; set; }
    public Endereco Endereco { get; set; } = null!;
    public Contato Contato { get; set; } = null!;
    public ICollection<Veiculo> Veiculos { get; set; } = [];

    public void Atualizar(string? nome, string? sexo, TipoCliente? tipoCliente, string? dtNascimento)
    {
        if (!string.IsNullOrEmpty(nome)) Nome = nome;
        if (!string.IsNullOrEmpty(sexo)) Sexo = sexo;
        if (tipoCliente.HasValue) TipoCliente = tipoCliente.Value;
        if (!string.IsNullOrEmpty(dtNascimento)) DataNascimento = dtNascimento;
    }
    
    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Nome))
            throw new DadosInvalidosException("O nome é obrigatório");
            
        var nome = Nome.Trim();
        if (nome.Length < 3 || nome.Length > 100)
            throw new DadosInvalidosException("O nome deve ter entre 3 e 100 caracteres");
            
        if (string.IsNullOrWhiteSpace(Documento))
            throw new DadosInvalidosException("Documento inválido");
            
        if (TipoCliente == TipoCliente.PessoaFisica)
        {
            var cpf = new string(Documento.Where(char.IsDigit).ToArray());
            if (cpf.Length != 11 || !ValidarCPF(cpf))
                throw new DadosInvalidosException("Documento inválido");
        }
        else if (TipoCliente == TipoCliente.PessoaJuridico)
        {
            if (string.IsNullOrWhiteSpace(NomeFantasia) || string.IsNullOrWhiteSpace(NomeFantasia.Trim()))
                throw new DadosInvalidosException("O nome fantasia é obrigatório para pessoa jurídica");
                
            var nomeFantasia = NomeFantasia.Trim();
            if (nomeFantasia.Length < 3 || nomeFantasia.Length > 100)
                throw new DadosInvalidosException("O nome fantasia deve ter entre 3 e 100 caracteres");
            
            var cnpj = new string(Documento.Where(char.IsDigit).ToArray());
            
            if (cnpj.Length != 14 || !ValidarCNPJ(cnpj))
                throw new DadosInvalidosException("Documento inválido");
        }
        
        if (TipoCliente == TipoCliente.PessoaFisica)
        {
            if (!DateTime.TryParse(DataNascimento, out DateTime dataNascimento))
                throw new DadosInvalidosException("Data de nascimento inválida");
                
            var idade = DateTime.Today.Year - dataNascimento.Year;
            if (dataNascimento.Date > DateTime.Today.AddYears(-idade)) idade--;
            
            if (idade < 18)
                throw new DadosInvalidosException("O cliente deve ser maior de idade");
        }
    }
    
    private static bool ValidarCPF(string cpf)
    {
        if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
            return false;
            
        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        
        string tempCpf = cpf[..9];
        int soma = 0;
        
        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            
        int resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
            
        string digito = resto.ToString();
        tempCpf += digito;
        soma = 0;
        
        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            
        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
            
        digito += resto.ToString();
        
        return cpf.EndsWith(digito);
    }
    
    public bool ValidarCNPJ(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;
            
        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());
        
        if (cnpj.Length != 14 || cnpj.Distinct().Count() == 1)
            return false;

        int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma = 0;
        
        for (int i = 0; i < 12; i++)
        {
            int digito = int.Parse(cnpj[i].ToString());
            int produto = digito * multiplicadores1[i];
            soma += produto;
        }
        
        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;
        int digito1Esperado = int.Parse(cnpj[12].ToString());
        
        if (digito1 != digito1Esperado)
            return false;

        int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        soma = 0;
        
        for (int i = 0; i < 13; i++)
        {
            int digito = int.Parse(cnpj[i].ToString());
            int produto = digito * multiplicadores2[i];
            soma += produto;
        }
        
        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;
        int digito2Esperado = int.Parse(cnpj[13].ToString());
        
        return digito2 == digito2Esperado;
    }
}