using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class EmailModel
    {
        [Display(Name = "E-mail Registrado", Description = "Um e-mail válido registrado para autenticação")]
        [Required(ErrorMessage = "O campo e-mail é requerido")]
        [EmailAddress]
        public string Email { get; set; }
    }
}