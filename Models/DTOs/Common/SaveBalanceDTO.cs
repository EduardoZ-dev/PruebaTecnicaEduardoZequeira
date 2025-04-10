﻿using System.ComponentModel.DataAnnotations;

namespace RouletteTechTest.API.Models.DTOs.Common
{
    public class SaveBalanceDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        public decimal Amount { get; set; }
    }
}
