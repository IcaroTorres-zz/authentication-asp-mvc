using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Services.Email;

namespace Services.Tests
{
    [TestClass]
    public class ModuleTest
    {
        private readonly StandardKernel _kernel;

        public ModuleTest()
        {
            _kernel = new StandardKernel(new Domain.Modules.Module(TestEnvironment: true), new Modules.Module(TestEnvironment: true));
        }

        [TestMethod]
        public void SMTP_DI_Test()
        {
            // arrange in ctor

            // act
            var testSmtp = _kernel.Get<ISmtp>();

            // assert
            Assert.IsInstanceOfType(testSmtp, typeof(FakeSmtp));
        }

        [TestMethod]
        public void EmailWorker_DI_Test()
        {
            // arrange in ctor

            // act
            var actual = _kernel.Get<IEmailWorker>();

            // assert
            Assert.IsInstanceOfType(actual, typeof(EmailWorker));
        }
    }
}