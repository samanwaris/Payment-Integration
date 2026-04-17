using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace payment_service.Helper
{
    public class SignatureHelper
    {
        public static string HashSha256(string data)
        {
            try
            {
                using SHA256 sha256Hash = SHA256.Create();
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                // Convert byte array to hex string
                //StringBuilder builder = new StringBuilder();
                var builder = new StringBuilder();
                foreach (byte t in bytes)
                {
                    builder.Append(t.ToString("x2")); // lowercase hex
                }
                return builder.ToString();
            }
            catch (Exception ex)
            {
                new LoggingHelper().CreateFullLog(ex, "HashSha256");
            }

            return null;
        }

        //generate string to sign
        public static string StringToSign(string method, string relativeUrl, string minifyRequest, string xTimeStap)
        {
            try
            {
                string stringToSign = "";

                //minify request body
                new LoggingHelper().AppendTextFunction("StringToSign", $" MinifyRequestBody : {minifyRequest}");

                //hash sha 256
                string hashRequestBody = HashSha256(minifyRequest).ToLower();

                stringToSign = method + ":" + relativeUrl + ":" + hashRequestBody + ":" + xTimeStap;

                new LoggingHelper().AppendTextFunction("StringToSign", $"stringToSign : {stringToSign}");
                return stringToSign;
            }
            catch (Exception ex)
            {
                new LoggingHelper().CreateFullLog(ex, "StringToSign");
            }

            return null;
        }

        // Method to sign a string using an RSA private key and SHA256
        public static string GenerateSignature(string stringToSign, RSA privateKey)
        {
            try
            {
                // Convert the string to a byte array using UTF-8 encoding (or the required encoding)
                byte[] dataToSign = Encoding.UTF8.GetBytes(stringToSign);

                // The RSA class can sign the data directly. It calculates the hash internally.
                // RSASignaturePadding.PKCS1 is the standard padding for SHA256withRSA
                byte[] signatureBytes = privateKey.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                // Convert the signature to a Base64 encoded string
                string signature = Convert.ToBase64String(signatureBytes);

                new LoggingHelper().AppendTextFunction("GenerateSignature", $"{signature}");
                return signature;
            }
            catch (Exception ex)
            {
                new LoggingHelper().CreateFullLog(ex, "GenerateSignature");
            }
            return null;
        }

        public static bool VerifySignature(string stringToSign, string signatureBase64, RSA publicKey)
        {
            try
            {
                signatureBase64 = signatureBase64.Replace(" ", "+");
                new LoggingHelper().AppendTextFunction("VerifySignature", $"{stringToSign} - {signatureBase64}");
                byte[] dataToSign = Encoding.UTF8.GetBytes(stringToSign);
                byte[] signatureBytes = Convert.FromBase64String(signatureBase64);

                // Verify the signature using the public key
                bool hashVerify;
                using (var sha = SHA256.Create())
                {
                    var hash = sha.ComputeHash(dataToSign);
                    hashVerify = publicKey.VerifyHash(hash, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                }

                if (hashVerify)
                    return true;

                return publicKey.VerifyData(dataToSign, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            catch (Exception ex)
            {
                new LoggingHelper().CreateFullLog(ex, "VerifySignature");
            }

            return false;
        }

        public static RSA LoadPrivateKey(string pemFile)
        {
            try
            {
                string pem = File.ReadAllText(pemFile);
                var rsa = RSA.Create();
                rsa.ImportFromPem(pem.ToCharArray());

                return rsa;
            }
            catch (Exception ex)
            {
                new LoggingHelper().CreateFullLog(ex, "LoadPrivateKey");
            }

            return null;
        }

        public static RSA LoadPublicKey(string pemFile)
        {
            try
            {
                string pem = File.ReadAllText(pemFile);
                var rsa = RSA.Create();
                rsa.ImportFromPem(pem.ToCharArray());
                new LoggingHelper().AppendTextFunction("LoadPublicKey", "Public key loaded successfully");

                return rsa;
            }
            catch (Exception ex)
            {
                new LoggingHelper().CreateFullLog(ex, "LoadPublicKey");
            }

            return null;
        }

        public static bool IsValidAmount(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            // wajib angka + titik + tepat 2 digit desimal
            var pattern = @"^\d+\.\d{2}$";

            return Regex.IsMatch(value, pattern);
        }


    }
}

