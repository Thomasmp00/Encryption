using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Encrypter;

public class Encryption
{
    private Boolean appOn = true;
    private Boolean messageIsnotEncrypted = true;
    private String usedOption;
    public byte[] nonce;
    public byte[] ciphertext;
    public byte[] tag;
    
    
    
    public void Main()
    {
        
        while (appOn)
        {
            Console.WriteLine("Choose an option");
            Console.WriteLine("1: Encrypt a message");
            Console.WriteLine("2: Decrypt a message");
            Console.WriteLine("3: Reset and exit the app");
            usedOption = Console.ReadLine();
            cryptoOption();
        }
        

    }

    public void cryptoOption()
    {
        if (usedOption.Equals("1"))
        {
            EncryptMessage();
        }else if (usedOption.Equals("2"))
        {
            if (messageIsnotEncrypted = false)
            {
                DecryptionOption();
            }
            else
            {
                Console.WriteLine("You have to encrypt a message before you can decrypt one...");
            }
            
        }else if (usedOption.Equals("3"))
        {
            appOn = false;
        }
        
    }

    public void EncryptMessage()
    {
        Console.WriteLine("Enter the message you want to encrypt:");
        string myString = Console.ReadLine();
        
        // var key = new byte[32];
        // RandomNumberGenerator.Fill(key);
        Console.WriteLine("Write a 32 character long key for the encryption");
        var keyText = Console.ReadLine();

        Byte[] keyByte = Encoding.ASCII.GetBytes(keyText);

        nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        RandomNumberGenerator.Fill(nonce);

        
         var plaintextBytes = Encoding.UTF8.GetBytes(myString);
         ciphertext = new byte[plaintextBytes.Length];
         tag = new byte[AesGcm.TagByteSizes.MaxSize];
        
        if(keyText.Length == 32){
        using (var aes = new AesGcm(keyByte))
        {
            aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);
            SecretMessage secretMessage = new SecretMessage
            {
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
            messageIsnotEncrypted = false;
        }}
        else
        {
            Console.WriteLine("Key is not the right length");
        }
        
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

    public void DecryptionOption()
    {
        Console.WriteLine("Write the correct key to decrypt the message");
        var decryptionKey = Console.ReadLine();
        var decryptionKeyByte = Encoding.ASCII.GetBytes(decryptionKey);
        
        string decryptedMessage = Decrypt(ciphertext, nonce, tag, decryptionKeyByte);

        
        var path = Path.Combine( "NotSoSecretMessage.json");
        string filePath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  path);
        
        File.WriteAllText(filePath2, decryptedMessage);
        ReadAndPrintNotSoSecretMessage();
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

