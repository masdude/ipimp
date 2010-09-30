using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;


public class CryptoHelper
{
  private const string encryptedPassword = "TsB3BRgxKK3N5Yjv6evDJdUvWdnk2/Dia9Z6Dzo53gE=";
  private static string cryptoPwd = "";
  public static byte[] u8_Salt = new byte[] { 0x26, 0x19, 0x81, 0x4E, 0xA0, 0x6D, 0x95, 0x34, 0x26, 0x75, 0x64, 0x05, 0xF6 };

  public static string CryptoPwd()
  {
    if (cryptoPwd != "")
      cryptoPwd = CryptoHelper.Crypt(encryptedPassword, false);
    return cryptoPwd;
  }

  public static string Crypt(string s_Data, bool b_Encrypt)
  {
    if (!b_Encrypt)
      s_Data = HexToStr(s_Data);
    PasswordDeriveBytes i_Pass = new PasswordDeriveBytes(CryptoPwd(), u8_Salt);

    Rijndael i_Alg = Rijndael.Create();
    i_Alg.Key = i_Pass.GetBytes(32);
    i_Alg.IV = i_Pass.GetBytes(16);

    ICryptoTransform i_Trans = (b_Encrypt) ? i_Alg.CreateEncryptor() : i_Alg.CreateDecryptor();

    MemoryStream i_Mem = new MemoryStream();
    CryptoStream i_Crypt = new CryptoStream(i_Mem, i_Trans, CryptoStreamMode.Write);

    byte[] u8_Data;
    try
    {
      if (b_Encrypt)
        u8_Data = Encoding.Unicode.GetBytes(s_Data);
      else
        u8_Data = Convert.FromBase64String(s_Data);
    }
    catch (Exception)
    {
      return "";
    }

    try
    {
      i_Crypt.Write(u8_Data, 0, u8_Data.Length);
      i_Crypt.Close();
      if (b_Encrypt)
        return StrToHex(Convert.ToBase64String(i_Mem.ToArray()));
      else
        return Encoding.Unicode.GetString(i_Mem.ToArray());
    }
    catch
    {
      return null;
    }
  }
  private static string StrToHex(string str)
  {
    StringBuilder sb = new StringBuilder();
    for (int i = 0; i < str.Length; i++)
      sb.AppendFormat("{0:X2}", (byte)str[i]);
    return sb.ToString();
  }
  private static string HexToStr(string str)
  {
    StringBuilder sb = new StringBuilder();
    int p = 0;
    for (int i = 0; i < str.Length / 2; i++)
    {
      byte b = Convert.ToByte(str.Substring(p, 2), 16);
      sb.Append((char)b);
      p += 2;
    }
    return sb.ToString();
  }
}
