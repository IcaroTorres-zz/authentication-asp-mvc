using Domain.Modules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Security.Modules;

namespace Security.Tests
{
    [TestClass]
    public class ModuleTest
    {
        [TestMethod]
        public void DependencyInjection_Test()
        {
            // arrange
            var kernel = new StandardKernel(new Domain.Modules.Module(TestEnvironment: true), new Modules.Module());

            // act
            var actual = kernel.Get<IAuthProvider>();

            // assert
            Assert.IsInstanceOfType(actual, typeof(AuthProvider));
        }
    }
}
