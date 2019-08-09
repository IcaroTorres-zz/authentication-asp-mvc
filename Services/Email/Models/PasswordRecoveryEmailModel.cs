using System;

namespace Services.Email.Models
{
    public class PasswordRecoveryEmailModel : EmailModel
    {
        public PasswordRecoveryEmailModel()
        {
            Title = "Synapse - Recuperação de senha requisitada";
        }
        public string RecoveryLink { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
