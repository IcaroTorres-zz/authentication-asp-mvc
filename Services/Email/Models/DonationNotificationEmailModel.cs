using System.Globalization;

namespace Services.Email.Models
{
    public class DonationNotificationEmailModel : EmailModel
    {
        public DonationNotificationEmailModel()
        {
            Title = "Synapse - Notificação de Doação";
        }
        public decimal Amount { get; set; }
        public string GeneratedPassword { get; set; }
        public string AmountAsCurrency => Amount.ToString("C", CultureInfo.CurrentCulture);
    }
}