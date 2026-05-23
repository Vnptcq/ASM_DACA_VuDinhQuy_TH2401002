using System.Security.Cryptography;
using System.Text;

const int SaltSize = 16;
const int HashSize = 32;
const int Iterations = 100_000;

var password = args.Length > 0 ? args[0] : "Admin@1234";
var salt = RandomNumberGenerator.GetBytes(SaltSize);
var hash = Rfc2898DeriveBytes.Pbkdf2(
    Encoding.UTF8.GetBytes(password),
    salt,
    Iterations,
    HashAlgorithmName.SHA256,
    HashSize);

Console.WriteLine($"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}");
