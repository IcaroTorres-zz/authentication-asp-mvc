using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class ProfileModel
    {
        [Display(Name = "Nome", Description = "User's name")]
        [Required(ErrorMessage = "Campo 'Nome' é requerido")]
        [MinLength(3, ErrorMessage = "Nome muito curto")]
        public string Name { get; set; }

        [Display(Name = "E-mail", Description = "A valid user e-mail for authentication")]
        [Required(ErrorMessage = "Campo E-mail é requerido")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Quer uma nova senha?", Description = "A valid password to access validation")]
        [MinLength(length: 8, ErrorMessage = "Senha deve ter ao menos 8 characters")]
        public string Password { get; set; }

        [Display(Name = "Não divulgar meu nome publicamente", Description = "Option to make user anonymous in Donations")]
        public bool IsAnonymous { get; set; }
    }
}