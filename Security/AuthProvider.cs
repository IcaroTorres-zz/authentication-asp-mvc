using Domain.Context;
using Domain.Entities;
using Services;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Security
{
    public class AuthProvider : Service<AuthContext>, IAuthProvider
    {
        private readonly string _implementation;

        public AuthProvider(AuthContext context) : base(context) => _implementation = "System.Security.Cryptography.SHA512CryptoServiceProvider";

        /// <summary>
        /// Generate SHA256 Hash over given option strings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Encrypt(string value = "")
        {
            using (SHA512 mySHA512 = SHA512.Create(_implementation))
            {
                value = string.IsNullOrEmpty(value) ? GenerateLengthString() : value;
                byte[] hash = mySHA512.ComputeHash(Encoding.UTF8.GetBytes(value));
                var hashString = hash.Aggregate("", (final, actual) => final + string.Format("{0:x2}", actual));
                return hashString;
            }
        }

        /// <summary>
        /// Verify existence and validity of relation between userId, hash code and hash type
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="hashCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool Validate(string userEmail, string hashCode, Hashs type) => Find<Hash>(h => !h.Disabled && h.User.Email == userEmail && h.HashCode.Equals(hashCode) && h.Type.Equals((int)type) &&
                                                                                                (
                                                                                                    h.ExpirationDate == null ||
                                                                                                    h.ExpirationDate.Value.CompareTo(DateTime.UtcNow) > 0
                                                                                                ) &&
                                                                                                (
                                                                                                    h.BlockExpirationDate == null ||
                                                                                                    h.BlockExpirationDate.Value.CompareTo(DateTime.UtcNow) <= 0
                                                                                                ) &&
                                                                                                h.Attempts <= 3, includes: "User", isreadonly: true).Any();

        /// <summary>
        /// Return a registered user corresponding to given email or null
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetCredentials(string email) => Find<Hash>(h => h.User.Email == email &&
                                                                    h.Type.Equals((int)Hashs.Login) &&
                                                                    !h.Disabled, includes: "User,User.Role").Select(h => h.User).SingleOrDefault();

        /// <summary>
        /// Provide hash of type using given strings
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Hash ProvideLinkToRecovery(User user)
        {
            if (string.IsNullOrEmpty(user.Email))
                throw new ArgumentNullException("User email can't be Null or empty", nameof(user.Email));

            if (user.Id > 0)
            {
                var previousPaswordRecoveryHash = Find<Hash>(h => h.UserId == user.Id && !h.Disabled && h.Type.Equals((int)Hashs.Recovery)).SingleOrDefault();
                if (previousPaswordRecoveryHash != null)
                {
                    previousPaswordRecoveryHash.Disabled = true;
                    Update(previousPaswordRecoveryHash);
                }
            }

            var hash = new Hash
            {
                HashCode = Encrypt(),
                Type = (int)Hashs.Recovery,
                Attempts = 0,
                ExpirationDate = DateTime.UtcNow.AddDays(3),
                User = user,
                UserId = user.Id
            };

            // committing makes the shared context updates both alterations in not yet recorded user,
            // and it's new related hash
            Add(hash);
            Commit();

            return hash;
        }

        /// <summary>
        /// Provide login hash to a new recently created user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Hash ProvideLogin(User user, string password)
        {
            if (string.IsNullOrEmpty(user.Email))
                throw new ArgumentNullException("User email can't be Null or empty", nameof(user.Email));

            if (user.Id > 0)
            {
                var previousLoginHash = Find<Hash>(h => h.UserId == user.Id && !h.Disabled && h.Type.Equals((int)Hashs.Login)).SingleOrDefault();
                if (previousLoginHash != null)
                {
                    previousLoginHash.Disabled = true;
                    Update(previousLoginHash);
                }
            }

            var hash = new Hash
            {
                HashCode = Encrypt(password),
                Type = (int)Hashs.Login,
                Attempts = 0,
                User = user,
                UserId = user.Id
            };

            // committing makes the shared context updates both alterations in not yet recorded user,
            // and it's new related hash
            Add(hash);
            Commit();

            return hash;
        }

        /// <summary>
        /// Generate 8 length string password and a login hash over it associated to given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GenerateUserWithPassword(User user)
        {
            var password = Encrypt(GenerateLengthString(8)).Substring(0, 8);
            ProvideLogin(user, password);
            return password;
        }

        /// <summary>
        /// Authenticate given mail and password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>Return authenticated user instance</returns>
        public User Authenticate(string email, string password)
        {
            var user = Find<User>(u => u.Email == email && !u.Disabled, includes: "Role", isreadonly: true).SingleOrDefault();

            if (user == null)
                throw new InvalidOperationException("Invalid email or password");

            var hash = Find<Hash>(h => h.UserId == user.Id && h.Type.Equals((int)Hashs.Login) && !h.Disabled).SingleOrDefault();

            if (hash == null)
                throw new InvalidOperationException("Invalid email or password");

            if (hash.BlockExpirationDate.HasValue && hash.BlockExpirationDate.Value.CompareTo(DateTime.UtcNow) > 0)
            {
                var blockEnds = hash.BlockExpirationDate.Value.ToLocalTime().ToString("HH:mm:ss");
                throw new InvalidOperationException($"Login blocked until {blockEnds} minutes due to wrong password attempts exceeded.");
            }

            if (hash.Attempts > 3)
            {
                hash.Attempts = 0;
                hash.BlockExpirationDate = DateTime.UtcNow.AddMinutes(5);
                Update(hash);
                Commit();
                throw new InvalidOperationException($"Login blocked for {5} minutes due to wrong password attempts exceeded.");
            }

            if (!hash.HashCode.Equals(Encrypt(password)))
            {
                hash.Attempts += 1;
                Update(hash);
                Commit();
                throw new InvalidOperationException("Invalid email or password");
            }

            hash.Attempts = 0;
            hash.BlockExpirationDate = null;
            Update(hash);
            Commit();

            return user;
        }

        /// <summary>
        /// Updates user data, and if given a password, your respective credentials,
        /// committing changes and returning updated user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User UpdateCredentials(User user, string password)
        {
            if (!string.IsNullOrEmpty(password))
                ProvideLogin(user, password);
            else
            {
                Update(user);
                Commit();
            }
            return user;
        }

        /// <summary>
        /// Generate string of given length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private string GenerateLengthString(int length = 12)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[length];
                crypto.GetNonZeroBytes(data);
            }
            var result = new StringBuilder(length);
            foreach (byte b in data)
                result.Append(chars[b % (chars.Length)]);

            return result.ToString();
        }
    }
}