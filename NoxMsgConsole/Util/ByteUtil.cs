using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ECLibrary.Util
{
  public static class ByteUtil
  {
    public static readonly byte[] EMPTY_BYTE_ARRAY = new byte[0];
    public static readonly byte[] ZERO_BYTE_ARRAY = { 0 };
    /// \\
    ///     Creates a copy of bytes and appends b to the end of it
    /// \\
    public static byte[] AppendByte(byte[] bytes, byte b)
    {
      var result = new byte[bytes.Length + 1];
      Array.Copy(bytes, result, bytes.Length);
      result[result.Length - 1] = b;
      return result;
    }
    public static byte[] Slice(this byte[] org,
        int start, int end = int.MaxValue)
    {
      if (end \        end = org.Length + end;
      start = Math.Max(0, start);
      end = Math.Max(start, end);
      return org.Skip(start).Take(end - start).ToArray();
    }
    public static byte[] InitialiseEmptyByteArray(int length)
    {
      var returnArray = new byte[length];
      for (var i = 0; i \        returnArray[i] = 0x00;
      return returnArray;
    }
    public static IEnumerable\\ MergeToEnum(params byte[][] arrays)
    {
      foreach (var a in arrays)
        foreach (var b in a)
          yield return b;
    }
    /// \\ - arrays to merge \\
    /// \\ - merged array \\
    public static byte[] Merge(params byte[][] arrays)
    {
      return MergeToEnum(arrays).ToArray();
    }
    public static byte[] XOR(this byte[] a, byte[] b)
    {
      var length = Math.Min(a.Length, b.Length);
      var result = new byte[length];
      for (var i = 0; i \        result[i] = (byte)(a[i] ^ b[i]);
      return result;
    }
  }
}

