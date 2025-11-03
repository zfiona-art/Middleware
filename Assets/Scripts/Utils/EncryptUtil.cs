using System.IO;
using System;
using System.Security.Cryptography;
using System.Text;

public class EncryptUtil
{
    #region 加密方法一 AES
    private static readonly string SecretKey = "secret";
    private static readonly byte[] DynamicKey = Encoding.UTF8.GetBytes("k7#mp!9z@2$vq5&s8*yd6%g4^hj3iewp"); // 32 字节(英文字母)密钥
    private static readonly byte[] DynamicIv = Encoding.UTF8.GetBytes("x5!k9@m3#p7$v2&0"); // 16 字节初始化向量
    
    private static Aes CreateCryptoProvider()
    {
        var provider = Aes.Create();
        provider.Mode = CipherMode.CBC;
        provider.Padding = PaddingMode.PKCS7;
        provider.KeySize = 256;
        provider.BlockSize = 128;
        return provider;
    }
    
    public static string AesEncrypt(string input)
    {
        using (var crypto = CreateCryptoProvider())
        using (var encryptor = crypto.CreateEncryptor(DynamicKey, DynamicIv))
        using (var memoryStream = new MemoryStream())
        {
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(input);
            }
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }

 
    public static string AesDecrypt(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        try
        {
            using (var crypto = CreateCryptoProvider())
            using (var decryptor = crypto.CreateDecryptor(DynamicKey, DynamicIv))
            using (var memoryStream = new MemoryStream(Convert.FromBase64String(input)))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cryptoStream))
            {
                return reader.ReadToEnd();
            }
        }
        catch
        {
            return string.Empty;
        }
    }
    
    public static byte[] AesEncrypt(byte[] input)
    {
        return TransformData(input, true);
    }
    
    public static byte[] AesDecrypt(byte[] input)
    {
        return TransformData(input, false);
    }
    
    private static byte[] TransformData(byte[] data, bool encrypt)
    {
        if (data == null || data.Length == 0) return data;

        using (var crypto = CreateCryptoProvider())
        {
            var transform = encrypt
                ? crypto.CreateEncryptor(DynamicKey, DynamicIv)
                : crypto.CreateDecryptor(DynamicKey, DynamicIv);

            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();
                return memoryStream.ToArray();
            }
        }
    }
    
    #endregion

    #region 加密方法二  字节取反
    public static byte[] Encrypt(byte[] bytes)
    {
        char[] key = SecretKey.ToCharArray();
        var len = key.Length;
        for (int i = 0; i < bytes.Length; i++)
        {
            var j = i % len;
            bytes[i] ^= (byte)key[j];
        }
        return bytes;
    }

    public static byte[] Decrypt(byte[] bytes)
    {
        return Encrypt(bytes);
    }
    #endregion

    #region 加密方法三 Base64
    public static string Base64Encode(string source)
    {
        string encode = string.Empty;
        byte[] bytes = Encoding.UTF8.GetBytes(source);
        try
        {
            encode = Convert.ToBase64String(bytes);
        }
        catch
        {
            encode = source;
        }
        return encode;
    }

    public static string Base64Decode(string result)
    {
        string decode = string.Empty;
        byte[] bytes = Convert.FromBase64String(result);
        try
        {
            decode = Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            decode = result;
        }
        return decode;
    }
    #endregion
}



