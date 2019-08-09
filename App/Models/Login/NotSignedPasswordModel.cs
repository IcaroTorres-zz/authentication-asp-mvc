using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class NotSignedPasswordModel
    {
        [Display(Name = "Seu e-mail registrado", Description = "E-mail registrado do usuário utilizado para recuperar senha")]
        [Required(ErrorMessage = "O campo e-mail é requerido")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Informe a nova senha abaixo:", Description = "Uma senha válida para acesso")]
        [MinLength(length: 8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres")]
        [Required(ErrorMessage = "Digite sua nova senha")]
        public string NewPassword { get; set; }

        [Required]
        public string HashCode { get; set; }
    }
}