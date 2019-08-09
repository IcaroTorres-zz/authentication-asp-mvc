using Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System.Linq;

namespace Security.Tests
{
    [TestClass]
    public class AuthProviderTest
    {
        private readonly IAuthProvider _auth;

        public AuthProviderTest()
        {
            var kernel = new StandardKernel(new Domain.Modules.Module(TestEnvironment: true), new Modules.Module());
            _auth = kernel.Get<IAuthProvider>();
        }

        [TestMethod]
        public void ProvideHashCode_Empty_Test()
        {
            // arrange in ctor

            // act
            var hashCode1 = _auth.Encrypt();
            var hashCode2 = _auth.Encrypt();

            // assert
            Assert.IsInstanceOfType(hashCode1, typeof(string));
            Assert.IsInstanceOfType(hashCode2, typeof(string));
            Assert.AreNotEqual(hashCode1, hashCode2);
        }

        [TestMethod]
        public void ProvideHashCode_Empty_Exaustion_Test()
        {
            // arrange in ctor
            var loopRounds = 10000;

            // act
            var hashList = Enumerable.Range(0, loopRounds).Select(_ => _auth.Encrypt()).ToList();
            var hashSet = hashList.ToHashSet();

            // assert
            Assert.AreEqual(hashList.Count, hashSet.Count);
        }

        [TestMethod]
        public void ProvideHashCode_WithValues_Test()
        {
            // arrange in ctor
            var value = "87654321";
            var otherValue = "12345678";
            // act
            var hashCode1 = _auth.Encrypt(value);
            var hashCode2 = _auth.Encrypt(otherValue);

            // assert
            Assert.IsInstanceOfType(hashCode1, typeof(string));
            Assert.IsInstanceOfType(hashCode2, typeof(string));
            Assert.AreNotEqual(hashCode1, hashCode2);
        }

        [TestMethod]
        public void GetCredentials_Test()
        {
            // arrange
            var expectedEmail = "admin@dummy.com";
            var invalidEmail = "nooneuser@nothing.net";

            // act
            var userWithEmail = _auth.GetCredentials(expectedEmail);
            var userWithInvalidEmail = _auth.GetCredentials(invalidEmail);

            // assert
            Assert.IsNotNull(userWithEmail);
            Assert.IsInstanceOfType(userWithEmail, typeof(User));
            Assert.AreEqual(expectedEmail, userWithEmail.Email);
            Assert.IsNull(userWithInvalidEmail);
        }

        [TestMethod]
        public void ProvideLinkToRecovery_Test()
        {
            // arrange in ctor
            var expectedUser = _auth.GetCredentials("admin@dummy.com");
            var expectedHashType = (int)Hashs.Recovery;

            // act
            var hash = _auth.ProvideLinkToRecovery(expectedUser);

            // assert
            Assert.IsNotNull(hash);
            Assert.IsInstanceOfType(hash, typeof(Hash));
            Assert.AreEqual(expectedUser.Id, hash.UserId);
            Assert.AreEqual(expectedHashType, hash.Type);
        }

        [TestMethod]
        public void ProvideLogin_Test()
        {
            // arrange in ctor
            var expectedUser = _auth.GetCredentials("admin@dummy.com");
            var expectedHashType = (int)Hashs.Login;
            var password = "iuwdn132n1n3-1ntdn v-n2fm";
            var expectedHashCode = _auth.Encrypt(password);

            // act
            var hash = _auth.ProvideLogin(expectedUser, password);

            // assert
            Assert.IsNotNull(hash);
            Assert.IsInstanceOfType(hash, typeof(Hash));
            Assert.AreEqual(expectedUser.Id, hash.UserId);
            Assert.AreEqual(expectedHashType, hash.Type);
            Assert.AreEqual(expectedHashCode, hash.HashCode);
        }

        [TestMethod]
        public void Authenticate_Test()
        {
            // arrange in ctor
            var email = "admin@dummy.com";
            var password = "12345678";

            // act
            var authenticatedUser = _auth.Authenticate(email, password);

            // assert
            Assert.IsNotNull(authenticatedUser);
            Assert.IsInstanceOfType(authenticatedUser, typeof(User));
            Assert.AreEqual(email, authenticatedUser.Email);
        }

        [TestMethod]
        public void Validate_Test()
        {
            // arrange in ctor
            // valid user data
            var expectedUser = _auth.GetCredentials("admin@dummy.com");
            var validPassword = "12345678";

            // invalid user data
            var invalidPassword = "doij12-93jr1isdiof";
            var invalidPasswordRecoverHash = "ecc0d46a5e19d21fc46f9b4189125dei686wn0a176887fea6fce02e37e4bf2c9";

            // equivalent login hashes
            var expectedLoginHashCode = _auth.Encrypt(validPassword);
            var expectedPasswordRecoverHash = "ab7a0c2ced52887eba412b6841241ed10e998a27155e9e6ff10dabcd92b82e0bc7a4abf26f7fdf3add9ddb5a431a8d96e56a5ca9cc0e8ff58ab66d67f9457a92";
            var invalidHashCode = _auth.Encrypt(invalidPassword);

            // act
            var isLoginHashVerified = _auth.Validate(expectedUser.Email, expectedLoginHashCode, Hashs.Login);
            var isPasswordRecoverHashVerified = _auth.Validate(expectedUser.Email, expectedPasswordRecoverHash, Hashs.Recovery);

            var isLoginHashRecoverToo = _auth.Validate(expectedUser.Email, expectedLoginHashCode, Hashs.Recovery);
            var isPasswordRecoverHashLoginToo = _auth.Validate(expectedUser.Email, expectedPasswordRecoverHash, Hashs.Login);

            var isFalseLoginHashVerified = _auth.Validate(expectedUser.Email, invalidHashCode, Hashs.Login);
            var isFalsePasswordRecoverHashVerified = _auth.Validate(expectedUser.Email, invalidPasswordRecoverHash, Hashs.Recovery);

            // assert
            Assert.AreEqual(true, isLoginHashVerified);
            Assert.AreEqual(true, isPasswordRecoverHashVerified);

            Assert.AreEqual(false, isLoginHashRecoverToo);
            Assert.AreEqual(false, isPasswordRecoverHashLoginToo);

            Assert.AreEqual(false, isFalseLoginHashVerified);
            Assert.AreEqual(false, isFalsePasswordRecoverHashVerified);
        }

        [TestMethod]
        public void GeneratePassword_Test()
        {
            // arrange
            var expectedUser = _auth.GetCredentials("admin@dummy.com");

            // act
            var newPassword = _auth.GenerateUserWithPassword(expectedUser);
            var newHashCode = _auth.Encrypt(newPassword);
            var actualAuthenticatedUser = _auth.Authenticate(expectedUser.Email, newPassword);

            // assert
            Assert.AreEqual(true, _auth.Validate(expectedUser.Email, newHashCode, Hashs.Login));
            Assert.AreEqual(expectedUser.Email, actualAuthenticatedUser.Email);
            Assert.AreEqual(expectedUser.Id, actualAuthenticatedUser.Id);
        }
    }
}
