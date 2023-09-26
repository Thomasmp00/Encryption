namespace Encrypter;

public class SecretMessage
{
    public byte[] Key { get; set; }
    public byte[] Nonce { get; set; }
    
    public byte[] CipherText { get; set; }
    public byte[] Tag { get; set; }
}