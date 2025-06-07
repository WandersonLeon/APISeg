using System.ComponentModel.DataAnnotations;

namespace APICrud.Modelos
{
    public class ModeloLogin
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Senha { get; set; }
    }
}