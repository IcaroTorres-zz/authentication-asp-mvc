using Ninject.Modules;

namespace Security.Modules
{
    public class Module : NinjectModule
    {
        /// <summary>
        /// Load defined custom binders for a NinjectModule
        /// </summary>
        public override void Load()
        {
            Bind<IAuthProvider>().To<AuthProvider>();
        }
    }
}
