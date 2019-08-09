namespace Services.Email.Models
{
    public class ChangePasswordEmailModel : EmailModel
    {
        public ChangePasswordEmailModel()
        {
            Title = "Synapse - Password change successful";
        }
        public string NewPassword { get; set; }
    }
}