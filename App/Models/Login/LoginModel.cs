using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class LoginModel
    {
        [Display(Name = "E-mail Registrado", Description = "Um e-mail de usuário registrado para autenticação")]
        [Required(ErrorMessage = "O campo E-mail é requerido")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Senha", Description = "Uma senha válida para acesso")]
        [MinLength(length: 8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres")]
        [Required(ErrorMessage = "Preencha sua senha para acessar")]
        public string Password { get; set; }

        [Display(Name = "Lembrar-me", Description = "Opção para lembrar dados de acesso do usuário")]
        public bool RememberMe { get; set; }
    }
}