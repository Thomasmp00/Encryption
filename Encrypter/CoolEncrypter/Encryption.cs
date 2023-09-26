using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Encrypter;

public class Encryption
{
    
    public void Main()
    {
        Console.WriteLine("Enter the message you want to encrypt:");
        string userMessage = Console.ReadLine();

        EncryptMessage(userMessage);
        
        ReadAndPrintNotSoSecretMessage();

    }

    public void EncryptMessage(string myString)
    {
        var key = new byte[32];
        RandomNumberGenerator.Fill(key);

        var nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);

        var plaintextBytes = Encoding.UTF8.GetBytes(myString);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize];

        using (var aes = new AesGcm(key))
        {
            aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);
        }

        SecretMessage secretMessage = new SecretMessage
        {
            Key = key,
            Nonce = nonce,
            CipherText = ciphertext,
            Tag = tag
        };

        string json = JsonSerializer.Serialize(secretMessage);
        Console.WriteLine("Encrypted JSON:");
        Console.WriteLine(json);

        string fileName = @"MySacredText.json";
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

        File.WriteAllText(filePath, json);

        
        string decryptedMessage = Decrypt(ciphertext, nonce, tag, key);

        
        var path = Path.Combine( "NotSoSecretMessage.json");
        string filePath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  path);
        
        File.WriteAllText(filePath2, decryptedMessage);
    }
    static string Decrypt(byte[] ciphertext, byte[] nonce, byte[] tag, byte[] key)
    {
        using (var aes2 = new AesGcm(key))
        {
            var plaintextBytes = new byte[ciphertext.Length];

            aes2.Decrypt(nonce,ciphertext,tag,plaintextBytes);

            return Encoding.UTF8.GetString(plaintextBytes);
        }
    }
    
    public void ReadAndPrintNotSoSecretMessage()
    {
        string fileName2 = "NotSoSecretMessage.json";
        string filePath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName2);

        try
        {
            string decryptedMessage = File.ReadAllText(filePath2);
            Console.WriteLine("Contents of NotSoSecretMessage.json:");
            Console.WriteLine(decryptedMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading NotSoSecretMessage.json: " + ex.Message);
        }
    }
    
    
    
    
}