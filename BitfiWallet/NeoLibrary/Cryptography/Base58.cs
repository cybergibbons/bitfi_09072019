using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace NeoGasLibrary.Cryptography
{
  public static class Base58
  {
    /// \\
    /// base58编码的字母表
    /// \\
    public const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

    /// \\
    /// 解码
    /// \\
    /// \\要解码的字符串\\
    /// \\返回解码后的字节数组\\
    public static byte[] Decode(string input)
    {
      BigInteger bi = BigInteger.Zero;
      for (int i = input.Length - 1; i \>\= 0; i--)
      {
        int index = Alphabet.IndexOf(input[i]);
        if (index == -1)
          throw new FormatException();
        bi += index * BigInteger.Pow(58, input.Length - 1 - i);
      }
      byte[] bytes = bi.ToByteArray();
      Array.Reverse(bytes);
      bool stripSignByte = bytes.Length \>\ 1 && bytes[0] == 0 && bytes[1] \>\= 0x80;
      int leadingZeros = 0;
      for (int i = 0; i \\
    /// 编码
    /// \\
    /// \\要编码的字节数组\\
    /// \\返回编码后的字符串\\
    public static string Encode(byte[] input)
    {
      BigInteger value = new BigInteger(new byte[1].Concat(input).Reverse().ToArray());
      StringBuilder sb = new StringBuilder();
      while (value \>\= 58)
      {
        BigInteger mod = value % 58;
        sb.Insert(0, Alphabet[(int)mod]);
        value /= 58;
      }
      sb.Insert(0, Alphabet[(int)value]);
      foreach (byte b in input)
      {
        if (b == 0)
          sb.Insert(0, Alphabet[0]);
        else
          break;
      }
      return sb.ToString();
    }
  }
}
