using System.ComponentModel.DataAnnotations;

namespace APICrud.Modelos;

public class CreateUsuario
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(6)]
    public required string Senha { get; set; }
}
