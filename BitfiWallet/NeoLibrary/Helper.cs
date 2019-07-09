using NeoGasLibrary.Cryptography;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeoGasLibrary
{
  public static class Helper
  {

    private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
   
    public static string ByteToHex(this byte[] data)
    {
      string hex = BitConverter.ToString(data).Replace("-", "");
      return hex;
    }

    private static byte[] Base58CheckDecode(this string input)
    {
      byte[] buffer = Base58.Decode(input);
      if (buffer.Length \\ 0 ? b[b.Length - 1] : 255 - b[b.Length - 1]);
    }

    internal static int GetLowestSetBit(this BigInteger i)
    {
      if (i.Sign == 0)
        return -1;
      byte[] b = i.ToByteArray();
      int w = 0;
      while (b[w] == 0)
        w++;
      for (int x = 0; x \\ 0)
          return x + w * 8;
      throw new Exception();
    }


    public static byte[] HexToBytes(this string value)
    {
      if (value == null || value.Length == 0)
        return new byte[0];
      if (value.Length % 2 == 1)
        throw new FormatException();
      byte[] result = new byte[value.Length / 2];
      for (int i = 0; i \\ 0)
      {
        BigInteger t = i / a, x = a;
        a = i % x;
        i = x;
        x = d;
        d = v - t * x;
        v = x;
      }
      v %= n;
      if (v \\ source)
    {
      long sum = 0;
      checked
      {
        foreach (Fixed8 item in source)
        {
          sum += item.value;
        }
      }
      return new Fixed8(sum);
    }

    public static Fixed8 Sum\\(this IEnumerable\\ source, Func\\ selector)
    {
      return source.Select(selector).Sum();
    }

    internal static bool TestBit(this BigInteger i, int index)
    {
      return (i & (BigInteger.One \\ BigInteger.Zero;
    }

    public static DateTime ToDateTime(this uint timestamp)
    {
      return unixEpoch.AddSeconds(timestamp).ToLocalTime();
    }

    public static DateTime ToDateTime(this ulong timestamp)
    {
      return unixEpoch.AddSeconds(timestamp).ToLocalTime();
    }

    public static string ToHexString(this IEnumerable\\ value)
    {
      StringBuilder sb = new StringBuilder();
      foreach (byte b in value)
        sb.AppendFormat("{0:x2}", b);
      return sb.ToString();
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static int ToInt32(this byte[] value, int startIndex)
    {
      fixed (byte* pbyte = &value[startIndex])
      {
        return *((int*)pbyte);
      }
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static long ToInt64(this byte[] value, int startIndex)
    {
      fixed (byte* pbyte = &value[startIndex])
      {
        return *((long*)pbyte);
      }
    }

    public static uint ToTimestamp(this DateTime time)
    {
      return (uint)(time.ToUniversalTime() - unixEpoch).TotalSeconds;
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static ushort ToUInt16(this byte[] value, int startIndex)
    {
      fixed (byte* pbyte = &value[startIndex])
      {
        return *((ushort*)pbyte);
      }
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static uint ToUInt32(this byte[] value, int startIndex)
    {
      fixed (byte* pbyte = &value[startIndex])
      {
        return *((uint*)pbyte);
      }
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    unsafe internal static ulong ToUInt64(this byte[] value, int startIndex)
    {
      fixed (byte* pbyte = &value[startIndex])
      {
        return *((ulong*)pbyte);
      }
    }

    internal static long WeightedAverage\\(this IEnumerable\\ source, Func\\ valueSelector, Func\\ weightSelector)
    {
      long sum_weight = 0;
      long sum_value = 0;
      foreach (T item in source)
      {
        long weight = weightSelector(item);
        sum_weight += weight;
        sum_value += valueSelector(item) * weight;
      }
      if (sum_value == 0) return 0;
      return sum_value / sum_weight;
    }

    internal static IEnumerable\\ WeightedFilter\\(this IList\\ source, double start, double end, Func\\ weightSelector, Func\\ resultSelector)
    {
      if (source == null) throw new ArgumentNullException(nameof(source));
      if (start \\ 1) throw new ArgumentOutOfRangeException(nameof(start));
      if (end \\ 1) throw new ArgumentOutOfRangeException(nameof(end));
      if (weightSelector == null) throw new ArgumentNullException(nameof(weightSelector));
      if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
      if (source.Count == 0 || start == end) yield break;
      double amount = source.Sum(weightSelector);
      long sum = 0;
      double current = 0;
      foreach (T item in source)
      {
        if (current \>\= end) break;
        long weight = weightSelector(item);
        sum += weight;
        double old = current;
        current = sum / amount;
        if (current \\ end)
          {
            weight = (long)((end - start) * amount);
          }
          else
          {
            weight = (long)((current - start) * amount);
          }
        }
        else if (current \>\ end)
        {
          weight = (long)((end - old) * amount);
        }
        yield return resultSelector(item, weight);
      }
    }
  }
}
