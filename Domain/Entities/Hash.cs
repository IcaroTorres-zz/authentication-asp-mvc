using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Hash : Entity<int>
    {
        [Required, Column(TypeName = "VARCHAR"), StringLength(128), Index(IsUnique = false)]
        public string HashCode { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public int Attempts { get; set; } = 0;

        [Required]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public DateTime? BlockExpirationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

    /// <summary>
    /// Types of authentication Hash
    /// </summary>
    public enum Hashs
    {
        /// <summary>
        /// Login Hash type, used to authenticate sign-in users
        /// </summary>
        Login = 1,

        /// <summary>
        /// Recover password Hash type, used to generate link urls for users password recovery
        /// </summary>
        Recovery,

        /// <summary>
        /// Hash to validate donations, shown in the donation proof document emitted by donor end user
        /// </summary>
        DonationProof
    }
}