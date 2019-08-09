using Domain.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Domain.Context
{
    public class InMemoryInitializer : DropCreateDatabaseAlways<AuthContext>
    {

        /// <summary>
        /// Seed the Context with initial data, starting when it is used
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(AuthContext context)
        {
            InitialProfiles(context);

            InitialUsers(context);

            InitialHashs(context);

            base.Seed(context);
        }

        /// <summary>
        /// Generate dummy initial Users
        /// </summary>
        /// <param name="context"></param>
        public void InitialUsers(AuthContext context)
        {
            const string emailSuffix = "@dummy.com";

            context.Users.AddRange(
                new string[] { "admin", "customer" }
                .Select((n, index) => new User
                {
                    Name = n,
                    Email = n + emailSuffix,
                    RoleId = index == 0 ? 1 : 2
                })
            );
            context.SaveChanges();
        }

        /// <summary>
        /// Generate dummy initial Profiles
        /// </summary>
        /// <param name="context"></param>
        public void InitialProfiles(AuthContext context)
        {
            context.Roles.AddRange(
                new string[] { "Admin", "customer" }
                .Select(n => new Role
                {
                    Name = n,
                    Description = $"The {n}'s system role."
                })
            );
            context.SaveChanges();
        }

        /// <summary>
        /// Generate initial Hash.
        /// </summary>
        /// <param name="context"></param>
        public void InitialHashs(AuthContext context)
        {
            // hash equivalent to password 12345678
            var hashcode = "fa585d89c851dd338a70dcf535aa2a92fee7836dd6aff1226583e88e0996293f16bc009c652826e0fc5c706695a03cddce372f139eff4d13959da6f1f5d3eabe";

            // hash previously generated to use in unit test
            var recoveryhashcode = "ab7a0c2ced52887eba412b6841241ed10e998a27155e9e6ff10dabcd92b82e0bc7a4abf26f7fdf3add9ddb5a431a8d96e56a5ca9cc0e8ff58ab66d67f9457a92";

            var hashs = context.Users.Select(u => u.Id)
                                     .AsEnumerable()
                                     .Select(uid => 
                                     new Hash
                                     {
                                         HashCode = hashcode,
                                         Type = (int)Hashs.Login,
                                         UserId = uid
                                     }).ToList();

            context.Hashs.AddRange(hashs);

            context.Hashs.Add(new Hash
            {
                HashCode = recoveryhashcode,
                Type = (int)Hashs.Recovery,
                UserId = 1
            });

            context.SaveChanges();
        }
    }
}
