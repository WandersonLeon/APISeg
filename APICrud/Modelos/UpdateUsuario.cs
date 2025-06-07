using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace APICrud.Modelos
{
    public class UpdateUsuario
    {
        [Required]
        public required string EmailNovo { get; set; }

        [Required]
        public required string SenhaAtual { get; set; }

        public string? NovaSenha { get; set; }
    }
}