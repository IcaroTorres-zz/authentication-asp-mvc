using System.ComponentModel.DataAnnotations;

namespace Services.Email.Models
{
    public class EmailModel
    {
        public string Title { get; set; }

        [Display(Name = "Recipient", Description = "Valid email address of destination email recipient")]
        [Required(ErrorMessage = "Campo'Recipient' é requerido!")]
        [EmailAddress]
        public string RecipientEmail { get; set; }

        public string RecipientDisplayName { get; set; }

        [Display(Name = "Content", Description = "Body with email's message")]
        [Required(ErrorMessage = "Campo'Content' é requerido!")]
        public string QuoteMessage { get; set; }

        public string Site { get; set; }
        public string Compliment { get; set; } = "Dear";
        public string InstitutionName { get; set; } = "Rede Synapse - IPTI - Instituto de Pesquisas em Tecnologia e Inovação";
        public string WrittenBy { get; set; } = "Rede Synapse";
        public string City { get; set; } = "Aracaju";
        public string County { get; set; } = "SE";
        public string Country { get; set; } = "BR";
        public string Address { get; set; } = "Ed. Horizonte Jardins Offices & Hotel, Av. Dr. José Machado de Souza, 120 - sala 716 - 7º andar";
        public string CEP { get; set; } = "CEP 49025-740 / 49025-790";
    }
}