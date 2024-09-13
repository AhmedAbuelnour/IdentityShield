using IdentityShield.Domain.Entities;
using IdentityShield.Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace IdentityShield.Application.Shields
{
    public class ShieldPasswordHasher(IOptions<ShieldOptions> options) : IPasswordHasher<ApplicationUser>
    {
        public string HashPassword(ApplicationUser user, string password)
        {
            return Hash(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        {
            return hashedPassword.Equals(Hash(providedPassword)) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        private string Hash(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            byte[] toEncryptArray = Encoding.UTF8.GetBytes(value);

            // Use TripleDES for encryption
            using (TripleDES tdes = TripleDES.Create())
            {
                tdes.Key = MD5.HashData(Encoding.UTF8.GetBytes(options.Value.HashKey));  // Compute MD5 hash of the key
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                // Create the encryptor
                using (ICryptoTransform cTransform = tdes.CreateEncryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                    // Convert the result to a base64 string
                    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
                }
            }
        }
    }
}
