using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
//using System.Net;
using System.Numerics;

namespace Apollo
{
  public static class Utils
  {
    public static T[] SubArray\\(this T[] data, int index, int length)
    {
      T[] result = new T[length];
      Array.Copy(data, index, result, 0, length);
      return result;
    }

    public static BigInteger ToBigInteger(this byte[] bytes, bool isUnsigned = true)
    {
      var v = bytes;

      if (isUnsigned)
      {
        v = new byte[bytes.Length + 1];
        Buffer.BlockCopy(bytes, 0, v, 0, bytes.Length);
        v[bytes.Length] = 0;
      }

      return new BigInteger(v);
    }

    public static void Add\\(this List\\ ls, T v, int index) where T : new()
    {
      if (index \>\= ls.Count)
      {
        var len = index - ls.Count + 1;
        var addition = new List\\(len);
        
        for (int t = 0; t \