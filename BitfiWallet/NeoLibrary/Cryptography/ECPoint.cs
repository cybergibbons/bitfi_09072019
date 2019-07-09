using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace NeoGasLibrary.Cryptography
{
  public class ECPoint : IComparable\\, IEquatable\\
  {
    internal ECFieldElement X, Y;
    internal readonly ECCurve Curve;

    /// \\
    /// 判断是否为无穷远点
    /// \\
    public bool IsInfinity
    {
      get { return X == null && Y == null; }
    }

    public int Size =\>\ IsInfinity ? 1 : 33;

    public ECPoint()
        : this(null, null, ECCurve.Secp256r1)
    {
    }

    internal ECPoint(ECFieldElement x, ECFieldElement y, ECCurve curve)
    {
      if ((x != null && y == null) || (x == null && y != null))
        throw new ArgumentException("Exactly one of the field elements is null");
      this.X = x;
      this.Y = y;
      this.Curve = curve;
    }

    /// \\
    /// 与另一对象进行比较
    /// \\
    /// \\另一对象\\
    /// \\返回比较的结果\\
    public int CompareTo(ECPoint other)
    {
      if (ReferenceEquals(this, other)) return 0;
      int result = X.CompareTo(other.X);
      if (result != 0) return result;
      return Y.CompareTo(other.Y);
    }

    /// \\
    /// 从字节数组中解码
    /// \\
    /// \\要解码的字节数组\\
    /// \\曲线参数\\
    /// \\\\
    public static ECPoint DecodePoint(byte[] encoded, ECCurve curve)
    {
      ECPoint p = null;
      int expectedLength = (curve.Q.GetBitLength() + 7) / 8;
      switch (encoded[0])
      {
        case 0x00: // infinity
          {
            if (encoded.Length != 1)
              throw new FormatException("Incorrect length for infinity encoding");
            p = curve.Infinity;
            break;
          }
        case 0x02: // compressed
        case 0x03: // compressed
          {
            if (encoded.Length != (expectedLength + 1))
              throw new FormatException("Incorrect length for compressed encoding");
            int yTilde = encoded[0] & 1;
            BigInteger X1 = new BigInteger(encoded.Skip(1).Reverse().Concat(new byte[1]).ToArray());
            p = DecompressPoint(yTilde, X1, curve);
            break;
          }
        case 0x04: // uncompressed
        case 0x06: // hybrid
        case 0x07: // hybrid
          {
            if (encoded.Length != (2 * expectedLength + 1))
              throw new FormatException("Incorrect length for uncompressed/hybrid encoding");
            BigInteger X1 = new BigInteger(encoded.Skip(1).Take(expectedLength).Reverse().Concat(new byte[1]).ToArray());
            BigInteger Y1 = new BigInteger(encoded.Skip(1 + expectedLength).Reverse().Concat(new byte[1]).ToArray());
            p = new ECPoint(new ECFieldElement(X1, curve), new ECFieldElement(Y1, curve), curve);
            break;
          }
        default:
          throw new FormatException("Invalid point encoding " + encoded[0]);
      }
      return p;
    }

    private static ECPoint DecompressPoint(int yTilde, BigInteger X1, ECCurve curve)
    {
      ECFieldElement x = new ECFieldElement(X1, curve);
      ECFieldElement alpha = x * (x.Square() + curve.A) + curve.B;
      ECFieldElement beta = alpha.Sqrt();

      //
      // if we can't find a sqrt we haven't got a point on the
      // curve - run!
      //
      if (beta == null)
        throw new ArithmeticException("Invalid point compression");

      BigInteger betaValue = beta.Value;
      int bit0 = betaValue.IsEven ? 0 : 1;

      if (bit0 != yTilde)
      {
        // Use the other root
        beta = new ECFieldElement(curve.Q - betaValue, curve);
      }

      return new ECPoint(x, beta, curve);
    }

    /// \\
    /// 反序列化
    /// \\
    /// \\数据来源\\
    /// \\椭圆曲线参数\\
    /// \\\\
    public static ECPoint DeserializeFrom(BinaryReader reader, ECCurve curve)
    {
      int expectedLength = (curve.Q.GetBitLength() + 7) / 8;
      byte[] buffer = new byte[1 + expectedLength * 2];
      buffer[0] = reader.ReadByte();
      switch (buffer[0])
      {
        case 0x00:
          return curve.Infinity;
        case 0x02:
        case 0x03:
          reader.Read(buffer, 1, expectedLength);
          return DecodePoint(buffer.Take(1 + expectedLength).ToArray(), curve);
        case 0x04:
        case 0x06:
        case 0x07:
          reader.Read(buffer, 1, expectedLength * 2);
          return DecodePoint(buffer, curve);
        default:
          throw new FormatException("Invalid point encoding " + buffer[0]);
      }
    }

    /// \\
    /// 将对象编码到字节数组
    /// \\
    /// \\是否为压缩格式的编码\\
    /// \\返回编码后的字节数组\\
    public byte[] EncodePoint(bool commpressed)
    {
      if (IsInfinity) return new byte[1];
      byte[] data;
      if (commpressed)
      {
        data = new byte[33];
      }
      else
      {
        data = new byte[65];
        byte[] yBytes = Y.Value.ToByteArray().Reverse().ToArray();
        Buffer.BlockCopy(yBytes, 0, data, 65 - yBytes.Length, yBytes.Length);
      }
      byte[] xBytes = X.Value.ToByteArray().Reverse().ToArray();
      Buffer.BlockCopy(xBytes, 0, data, 33 - xBytes.Length, xBytes.Length);
      data[0] = commpressed ? Y.Value.IsEven ? (byte)0x02 : (byte)0x03 : (byte)0x04;
      return data;
    }

    /// \\
    /// 比较与另一个对象是否相等
    /// \\
    /// \\另一个对象\\
    /// \\返回比较的结果\\
    public bool Equals(ECPoint other)
    {
      if (ReferenceEquals(this, other)) return true;
      if (ReferenceEquals(null, other)) return false;
      if (IsInfinity && other.IsInfinity) return true;
      if (IsInfinity || other.IsInfinity) return false;
      return X.Equals(other.X) && Y.Equals(other.Y);
    }

    /// \\
    /// 比较与另一个对象是否相等
    /// \\
    /// \\另一个对象\\
    /// \\返回比较的结果\\
    public override bool Equals(object obj)
    {
      return Equals(obj as ECPoint);
    }

    /// \\   
    /// 从指定的字节数组中解析出公钥，这个字节数组可以是任意形式的公钥编码、或者包含私钥的内容
    /// \\
    /// \\要解析的字节数组\\
    /// \\椭圆曲线参数\\
    /// \\返回解析出的公钥\\
    public static ECPoint FromBytes(byte[] pubkey, ECCurve curve)
    {
      switch (pubkey.Length)
      {
        case 33:
        case 65:
          return DecodePoint(pubkey, curve);
        case 64:
        case 72:
          return DecodePoint(new byte[] { 0x04 }.Concat(pubkey.Skip(pubkey.Length - 64)).ToArray(), curve);
        case 96:
        case 104:
          return DecodePoint(new byte[] { 0x04 }.Concat(pubkey.Skip(pubkey.Length - 96).Take(64)).ToArray(), curve);
        default:
          throw new FormatException();
      }
    }

    /// \\
    /// 获取HashCode
    /// \\
    /// \\返回HashCode\\
    public override int GetHashCode()
    {
      return X.GetHashCode() + Y.GetHashCode();
    }

    internal static ECPoint Multiply(ECPoint p, BigInteger k)
    {
      // floor(log2(k))
      int m = k.GetBitLength();

      // width of the Window NAF
      sbyte width;

      // Required length of precomputation array
      int reqPreCompLen;

      // Determine optimal width and corresponding length of precomputation
      // array based on literature values
      if (m \\= 0; i--)
      {
        q = q.Twice();

        if (wnaf[i] != 0)
        {
          if (wnaf[i] \>\ 0)
          {
            q += preComp[(wnaf[i] - 1) / 2];
          }
          else
          {
            // wnaf[i] \\ 0)
      {
        if (!k.IsEven)
        {
          BigInteger remainder = k % pow2wB;
          if (remainder.TestBit(width - 1))
          {
            wnaf[i] = (sbyte)(remainder - pow2wB);
          }
          else
          {
            wnaf[i] = (sbyte)remainder;
          }
          k -= wnaf[i];
          length = i;
        }
        else
        {
          wnaf[i] = 0;
        }
        k \>\\>\= 1;
        i++;
      }
      length++;
      sbyte[] wnafShort = new sbyte[length];
      Array.Copy(wnaf, 0, wnafShort, 0, length);
      return wnafShort;
    }

    public static ECPoint operator -(ECPoint x)
    {
      return new ECPoint(x.X, -x.Y, x.Curve);
    }

    public static byte[] ReverseSecure(byte[] arr)
    {
      byte[] newarr = new byte[arr.Length + 1];

      for (int i = 0; i \