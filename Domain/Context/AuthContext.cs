using Domain.Entities;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Domain.Context
{
    public class AuthContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Hash> Hashs { get; set; }

        /// <summary>
        /// Test only constructor.
        /// </summary>
        /// <param name="connection"></param>
        public AuthContext(DbConnection connection) : base(connection, true)
        {
            Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer(new InMemoryInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            builder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            builder.Conventions.Remove<ForeignKeyIndexConvention>();
        }

        public override int SaveChanges()
        {
            AddAudit();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync()
        {
            AddAudit();
            return await base.SaveChangesAsync();
        }

        private void AddAudit()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is Entity<int>
                                                        && (x.State == EntityState.Added || x.State == EntityState.Modified));

            var currentUsername = HttpContext.Current != null && HttpContext.Current.User != null
                 ? HttpContext.Current.User.Identity.Name
                 : "Anonymous";

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((Entity<int>)entity.Entity).CreatedDate = DateTime.UtcNow;
                    ((Entity<int>)entity.Entity).CreatedBy = currentUsername;
                }

                ((Entity<int>)entity.Entity).ModifiedDate = DateTime.UtcNow;
                ((Entity<int>)entity.Entity).ModifiedBy = currentUsername;
            }
        }
    }
}
