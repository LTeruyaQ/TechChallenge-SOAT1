﻿using System.ComponentModel.DataAnnotations;

namespace Dominio.DTOs.Veiculo
{
    public class CadastrarVeiculoDto
    {
        [Required(ErrorMessage = "A placa é obrigatória.")]
        [StringLength(8, MinimumLength = 7, ErrorMessage = "A placa deve ter entre 7 e 8 caracteres.")]
        [RegularExpression(@"^[A-Za-z]{3}\d{1}[A-Za-z]{1}\d{2}$|^[A-Za-z]{3}\d{4}$",
            ErrorMessage = "Formato de placa inválido. Use o formato AAA1A11 ou AAA1111.")]
        public required string Placa { get; set; }

        [Required(ErrorMessage = "A marca é obrigatória.")]
        [StringLength(50, ErrorMessage = "A marca deve ter no máximo 50 caracteres.")]
        public required string Marca { get; set; }

        [Required(ErrorMessage = "O modelo é obrigatório.")]
        [StringLength(50, ErrorMessage = "O modelo deve ter no máximo 50 caracteres.")]
        public required string Modelo { get; set; }

        [Required(ErrorMessage = "A cor é obrigatória.")]
        [StringLength(30, ErrorMessage = "A cor deve ter no máximo 30 caracteres.")]
        public required string Cor { get; set; }

        [Required(ErrorMessage = "O ano é obrigatório.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "O ano deve ter 4 dígitos.")]
        [Range(1900, 2100, ErrorMessage = "O ano deve estar entre 1900 e 2100.")]
        public required string Ano { get; set; }

        [StringLength(500, ErrorMessage = "As anotações devem ter no máximo 500 caracteres.")]
        public required string Anotacoes { get; set; }

        [Required(ErrorMessage = "O cliente é obrigatório.")]
        public required Guid ClienteId { get; set; } // Alterado para ClienteId em vez do objeto Cliente
    }
}