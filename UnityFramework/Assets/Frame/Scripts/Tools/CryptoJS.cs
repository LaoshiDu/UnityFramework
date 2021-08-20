using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CryptoJS
{
    private static string key = "1234567891234567";

    /// <summary>
    /// AES解密 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string DecryptByAES(string input)
    {
        byte[] inputBytes = Convert.FromBase64String(input);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 16));
        using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
        {
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Key = keyBytes;
            aesAlg.IV = keyBytes;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msEncrypt = new MemoryStream(inputBytes))
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srEncrypt = new StreamReader(csEncrypt))
                    {
                        return srEncrypt.ReadToEnd();
                    }
                }
            }
        }
    }

    /// <summary>
    /// AES加密算法
    /// </summary>
    /// <param name="input"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string EncryptByAES(string input)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 16));
        using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
        {
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Key = keyBytes;
            aesAlg.IV = keyBytes;
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(input);
                    }
                    byte[] bytes = msEncrypt.ToArray();
                    var ddc = Convert.ToBase64String(bytes);

                    byte[] inputBytes = Convert.FromBase64String(ddc);
                    return ddc;
                }
            }
        }
    }

    /// <summary>
    /// 将指定的16进制字符串转换为byte数组
    /// </summary>
    /// <param name="s">16进制字符串(如：“7F 2C 4A”或“7F2C4A”都可以)</param>
    /// <returns>16进制字符串对应的byte数组</returns>
    public static byte[] HexStringToByteArray(string s)
    {
        s = s.Replace(" ", "");
        byte[] buffer = new byte[s.Length / 2];
        for (int i = 0; i < s.Length; i += 2)
            buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
        return buffer;
    }

    /// <summary>
    /// 将一个byte数组转换成一个格式化的16进制字符串
    /// </summary>
    /// <param name="data">byte数组</param>
    /// <returns>格式化的16进制字符串</returns>
    public static string ByteArrayToHexString(byte[] data)
    {
        StringBuilder sb = new StringBuilder(data.Length * 3);
        foreach (byte b in data)
        {
            //16进制数字
            sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            //16进制数字之间以空格隔开
            //sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
        }
        return sb.ToString().ToUpper();
    }
}