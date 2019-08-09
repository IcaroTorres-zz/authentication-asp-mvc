using Ninject.Modules;
using Services.Email;
using System.Configuration;

namespace Services.Modules
{
    public class Module : NinjectModule
    {
        private readonly bool _TestEnvironment;
        /// <summary>
        /// Constructor for a class inheriting NinjectModule, used by Ninject kernel to bind dependencies.
        /// </summary>
        /// <param name="TestEnvironment">If true, set the module environment to test.</param>
        public Module(bool TestEnvironment = false) => _TestEnvironment = TestEnvironment;

        /// <summary>
        /// Load defined custom binders for a NinjectModule
        /// </summary>
        public override void Load()
        {
#if RELEASE
            Bind<ISmtp>().ToMethod(s => new Smtp(ConfigurationManager.AppSettings["debug_email_host"],
                                                     ConfigurationManager.AppSettings["debug_email_port"],
                                                     ConfigurationManager.AppSettings["debug_email_user"],
                                                     ConfigurationManager.AppSettings["debug_email_password"]));
#else
            if (_TestEnvironment)
            {
                Bind<ISmtp>().ToMethod(s => new FakeSmtp(ConfigurationManager.AppSettings["test_email_host"],
                                                         ConfigurationManager.AppSettings["test_email_port"],
                                                         ConfigurationManager.AppSettings["test_email_user"])).Named("test");
            }
            else
            {
                Bind<ISmtp>().ToMethod(s => new FakeSmtp(ConfigurationManager.AppSettings["debug_email_host"],
                                                         ConfigurationManager.AppSettings["debug_email_port"],
                                                         ConfigurationManager.AppSettings["debug_email_user"])).Named("debug");
            }
#endif
            Bind<IEmailWorker>().To<EmailWorker>().Named(_TestEnvironment ? "test" : "");
        }
    }
}