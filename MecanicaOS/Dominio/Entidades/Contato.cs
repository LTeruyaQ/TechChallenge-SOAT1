using Dominio.Entidades.Abstratos;
using System.Text.RegularExpressions;

namespace Dominio.Entidades;

public class Contato : Entidade
{
    public Guid IdCliente { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefone { get; set; } = null!;

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Email))
            throw new ArgumentException("O e-mail é obrigatório");

        // Verifica se há espaços em branco no início ou no final
        if (Email != Email.Trim())
        {
            Console.WriteLine("Falha: E-mail contém espaços em branco no início ou no final");
            throw new ArgumentException("E-mail inválido");
        }

        try
        {
            Console.WriteLine($"Validando e-mail: {Email}");
            var addr = new System.Net.Mail.MailAddress(Email);
            Console.WriteLine($"Endereço normalizado: {addr.Address}");

            if (addr.Address != Email.Trim())
            {
                Console.WriteLine("Falha: Endereço não normalizado");
                throw new ArgumentException("E-mail inválido");
            }

            // Validação adicional para o domínio
            var dominio = Email.Split('@')[1];
            Console.WriteLine($"Domínio: {dominio}");

            var tld = dominio.Split('.').Last();
            Console.WriteLine($"TLD: {tld}, Tamanho: {tld.Length}");

            // Validação do TLD
            if (tld.Length < 2 || tld.Any(char.IsDigit))
            {
                Console.WriteLine($"Falha: TLD inválido - {tld}");
                throw new ArgumentException("E-mail inválido");
            }

            // Verifica caracteres inválidos no domínio
            var caracteresInvalidos = new[] { '/', '\\', ' ', ',', ';', ':', '!', '#', '$', '%', '&', '*', '(', ')', '=', '?', '^', '`', '{', '}', '|', '"', '<', '>', '[', ']', '_' };
            if (dominio.IndexOfAny(caracteresInvalidos) >= 0)
            {
                Console.WriteLine($"Falha: Domínio contém caracteres inválidos - {dominio}");
                throw new ArgumentException("E-mail inválido");
            }

            // Verifica caracteres acentuados no endereço de e-mail
            var nomeUsuario = Email.Split('@')[0];
            if (nomeUsuario.Any(c => c > 127)) // Caracteres acentuados têm código > 127
            {
                Console.WriteLine($"Falha: Nome de usuário contém caracteres acentuados - {nomeUsuario}");
                throw new ArgumentException("E-mail inválido");
            }

            // Verifica se o domínio contém pelo menos um ponto
            if (!dominio.Contains('.'))
            {
                Console.WriteLine("Falha: Domínio deve conter pelo menos um ponto");
                throw new ArgumentException("E-mail inválido");
            }

            // Verifica pontos consecutivos no domínio
            if (dominio.Contains(".."))
            {
                Console.WriteLine("Falha: Domínio contém pontos consecutivos");
                throw new ArgumentException("E-mail inválido");
            }

            // Verifica hífens no início ou final de cada parte do domínio
            var partesDominio = dominio.Split('.');
            foreach (var parte in partesDominio)
            {
                if (parte.StartsWith('-') || parte.EndsWith('-'))
                {
                    Console.WriteLine($"Falha: Parte do domínio não pode começar ou terminar com hífen - {parte}");
                    throw new ArgumentException("E-mail inválido");
                }

                // Verifica se a parte do domínio está vazia (pode acontecer com pontos consecutivos)
                if (string.IsNullOrEmpty(parte))
                {
                    Console.WriteLine("Falha: Parte do domínio está vazia");
                    throw new ArgumentException("E-mail inválido");
                }

                // Verifica se é o TLD (última parte do domínio)
                if (parte == partesDominio.Last())
                {
                    // TLD deve ter pelo menos 2 caracteres
                    if (parte.Length < 2)
                    {
                        Console.WriteLine($"Falha: TLD deve ter pelo menos 2 caracteres - {parte}");
                        throw new ArgumentException("E-mail inválido");
                    }

                    // Se for um IDN (Internationalized Domain Name) em Punycode
                    if (parte.StartsWith("xn--"))
                    {
                        // Verifica se o resto do TLD após xn-- é válido
                        var punycode = parte[4..];
                        if (punycode.Length == 0 || punycode.Any(c => !IsValidPunycodeChar(c)))
                        {
                            Console.WriteLine($"Falha: TLD com IDN inválido - {parte}");
                            throw new ArgumentException("E-mail inválido");
                        }
                    }
                    // Se o TLD contém hífen, deve ser um domínio de país (ex: co-uk, com-br)
                    else if (parte.Contains('-'))
                    {
                        // Verifica se tem pelo menos 2 partes separadas por hífen
                        var partesTld = parte.Split('-');
                        if (partesTld.Length != 2 || partesTld.Any(p => p.Length < 2))
                        {
                            Console.WriteLine($"Falha: TLD com hífen deve ter duas partes de pelo menos 2 caracteres cada - {parte}");
                            throw new ArgumentException("E-mail inválido");
                        }

                        // Verifica se as duas partes contêm apenas letras
                        if (partesTld.Any(p => p.Any(c => !char.IsLetter(c))))
                        {
                            Console.WriteLine($"Falha: Partes do TLD devem conter apenas letras - {parte}");
                            throw new ArgumentException("E-mail inválido");
                        }
                    }
                    // Se não tem hífen e não é IDN, deve conter apenas letras
                    else if (parte.Any(c => !char.IsLetter(c)))
                    {
                        Console.WriteLine($"Falha: TLD sem hífen deve conter apenas letras - {parte}");
                        throw new ArgumentException("E-mail inválido");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exceção ao validar e-mail: {ex.Message}");
            throw new ArgumentException("E-mail inválido");
        }

        // Valida o telefone
        ValidarTelefone();
    }

    private static bool IsValidPunycodeChar(char c)
    {
        // Caracteres permitidos em Punycode: a-z, 0-9, hífen
        return (c >= 'a' && c <= 'z') ||
               (c >= '0' && c <= '9') ||
               c == '-';
    }

    private void ValidarTelefone()
    {
        if (string.IsNullOrWhiteSpace(Telefone))
            throw new ArgumentException("O telefone é obrigatório");

        var telefoneLimpo = FormatarTelefone(Telefone);
        
        // Validação básica de telefone (pode ser ajustada conforme necessidade)
        if (telefoneLimpo.Length < 10 || telefoneLimpo.Length > 11)
            throw new ArgumentException("Telefone inválido");
    }

    public string FormatarTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return string.Empty;

        // Remove tudo que não for dígito
        var numeros = new string(telefone.Where(char.IsDigit).ToArray());

        // Se o número já estiver no formato esperado (11 dígitos para celular ou 10 para fixo), retorna sem alterações
        if (numeros.Length == 11 || numeros.Length == 10)
        {
            return numeros;
        }

        // Verifica se o número tem código de país (começa com +55, 0055 ou 55 seguido de não-dígito)
        bool temCodigoPais = telefone.StartsWith("+") ||
                           telefone.StartsWith("00") ||
                           (telefone.StartsWith("55") && (telefone.Length > 2 && !char.IsDigit(telefone[2])));

        if (temCodigoPais)
        {
            // Remove o prefixo + ou 00 se existir
            if (numeros.StartsWith("00"))
            {
                numeros = numeros[2..];
            }

            // Se começar com 55 (código do Brasil), remove zeros iniciais após o código do país
            if (numeros.StartsWith("55") && numeros.Length > 2)
            {
                var parteLocal = numeros[2..];
                if (parteLocal.StartsWith("0"))
                {
                    parteLocal = parteLocal[1..];
                }
                numeros = "55" + parteLocal;
            }
        }
        else
        {
            // Para números nacionais sem formatação
            bool temParenteses = telefone.Contains('(') && telefone.Contains(')');

            // Se não tiver parênteses e começar com 0, remove o zero
            if (!temParenteses && numeros.StartsWith("0") && numeros.Length > 1)
            {
                numeros = numeros[1..];
            }

            // Garante que o número tenha o tamanho correto
            // Se for um número de celular (começa com 9), mantém 11 dígitos
            // Se for fixo (começa com 2-5), mantém 10 dígitos
            if (numeros.Length >= 11 && numeros[2] == '9')
            {
                numeros = numeros[..11];
            }
            else if (numeros.Length >= 10)
            {
                numeros = numeros[..10];
            }
        }

        return numeros;
    }
}