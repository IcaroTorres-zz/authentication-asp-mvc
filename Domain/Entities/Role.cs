using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Role : Entity<int>
    {
        [Required, Column(TypeName = "VARCHAR"), MaxLength(10), Index(IsUnique = true)]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<User> User { get; set; }
    }

    public enum Roles
    {
        /// <summary>
        /// System Admin Role
        /// </summary>
        ADMIN = 1,

        /// <summary>
        /// End user Donor Role
        /// </summary>
        DONOR
    }
}