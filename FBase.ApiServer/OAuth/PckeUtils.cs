using System;
using System.Security.Cryptography;
using System.Text;

namespace FBase.ApiServer.OAuth
{
    public class PckeUtils
    {
        public static string GenerateCodeVerifier(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static string GenerateCodeChallengeFromPlainText(string plainText)
        {
            var base64CodeVerifier = GenerateCodeVerifier(plainText);
            return GenerateCodeChallengeFromValidCodeVerifier(base64CodeVerifier);
        }

        public static string GenerateCodeChallengeFromValidCodeVerifier(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                return Convert.ToBase64String(challengeBytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
        }

        public static bool Check(string codeVerifier, string codeChallenge)
        {
            return GenerateCodeChallengeFromValidCodeVerifier(codeVerifier) == codeChallenge;
        }
    }
}
