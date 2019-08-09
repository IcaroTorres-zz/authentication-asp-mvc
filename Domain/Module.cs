using Domain.Context;
using Ninject.Modules;
using System.Data.Common;
using System.Data.Entity;

namespace Domain.Modules
{
    public class Module : NinjectModule
    {
        private readonly bool _TestEnvironment;
        /// <summary>
        /// Constructor for a class inheriting NinjectModule, used by Ninject kernel to bind dependencies.
        /// </summary>
        /// <param name="isTestEnvironment">If true, set the module environment to test.</param>
        public Module(bool TestEnvironment = false) => _TestEnvironment = TestEnvironment;

        /// <summary>
        /// Load defined custom binders for a NinjectModule
        /// </summary>
        public override void Load()
        {
#if RELEASE
                Bind<DbContext>().To<AuthContext>();
                Bind<AuthContext>().ToSelf();
#else
            DbConnection DonationConnection;

            if (_TestEnvironment)
            {
                DonationConnection = Effort.DbConnectionFactory.CreateTransient();
                Bind<DbContext>().ToMethod(c => new AuthContext(DonationConnection)).Named("test");
                Bind<AuthContext>().ToMethod(c => new AuthContext(DonationConnection)).Named("test");
            }
            else
            {
                DonationConnection = Effort.DbConnectionFactory.CreatePersistent($"debug_{nameof(AuthContext)}");
                Bind<DbContext>().ToMethod(c => new AuthContext(DonationConnection)).Named("debug");
                Bind<AuthContext>().ToMethod(c => new AuthContext(DonationConnection)).Named("debug");
            }
#endif
        }
    }
}
