using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User : Entity<int>
    {
        [Required, Column(TypeName = "VARCHAR"), MaxLength(60)]
        public string Name { get; set; }

        [EmailAddress, Column(TypeName = "VARCHAR"), MaxLength(60), Index(IsUnique = true)]
        public string Email { get; set; }

        [Required]
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public bool IsAnonymous { get; set; } = false;
    }
}