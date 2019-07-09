using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace NeoGasLibrary.Cryptography
{
  internal class ECFieldElement : IComparable\\, IEquatable\\
  {
    internal readonly BigInteger Value;
    private readonly ECCurve curve;

    public ECFieldElement(BigInteger value, ECCurve curve)
    {
      if (value \>\= curve.Q)
        throw new ArgumentException("x value too large in field element");
      this.Value = value;
      this.curve = curve;
    }

    public int CompareTo(ECFieldElement other)
    {
      if (ReferenceEquals(this, other)) return 0;
      return Value.CompareTo(other.Value);
    }

    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;

      ECFieldElement other = obj as ECFieldElement;

      if (other == null)
        return false;

      return Equals(other);
    }

    public bool Equals(ECFieldElement other)
    {
      return Value.Equals(other.Value);
    }

    private static BigInteger[] FastLucasSequence(BigInteger p, BigInteger P, BigInteger Q, BigInteger k)
    {
      int n = k.GetBitLength();
      int s = k.GetLowestSetBit();

      //Debug.Assert(k.TestBit(s));

      BigInteger Uh = 1;
      BigInteger Vl = 2;
      BigInteger Vh = P;
      BigInteger Ql = 1;
      BigInteger Qh = 1;

      for (int j = n - 1; j \>\= s + 1; --j)
      {
        Ql = (Ql * Qh).Mod(p);

        if (k.TestBit(j))
        {
          Qh = (Ql * Q).Mod(p);
          Uh = (Uh * Vh).Mod(p);
          Vl = (Vh * Vl - P * Ql).Mod(p);
          Vh = ((Vh * Vh) - (Qh \\\>\ 2) + 1, curve.Q), curve);
        return z.Square().Equals(this) ? z : null;
      }
      BigInteger qMinusOne = curve.Q - 1;
      BigInteger legendreExponent = qMinusOne \>\\>\ 1;
      if (BigInteger.ModPow(Value, legendreExponent, curve.Q) != 1)
        return null;
      BigInteger u = qMinusOne \>\\>\ 2;
      BigInteger k = (u \\= curve.Q || BigInteger.ModPow(P * P - fourQ, legendreExponent, curve.Q) != qMinusOne);
        BigInteger[] result = FastLucasSequence(curve.Q, P, Q, k);
        U = result[0];
        V = result[1];
        if ((V * V).Mod(curve.Q) == fourQ)
        {
          if (V.TestBit(0))
          {
            V += curve.Q;
          }
          V \>\\>\= 1;
          //Debug.Assert((V * V).Mod(curve.Q) == Value);
          return new ECFieldElement(V, curve);
        }
      }
      while (U.Equals(BigInteger.One) || U.Equals(qMinusOne));
      return null;
    }

    public ECFieldElement Square()
    {
      return new ECFieldElement((Value * Value).Mod(curve.Q), curve);
    }

    public byte[] ToByteArray()
    {
      byte[] data = Value.ToByteArray();
      if (data.Length == 32)
        return data.Reverse().ToArray();
      if (data.Length \>\ 32)
        return data.Take(32).Reverse().ToArray();
      return Enumerable.Repeat\\(0, 32 - data.Length).Concat(data.Reverse()).ToArray();
    }

    public static ECFieldElement operator -(ECFieldElement x)
    {
      return new ECFieldElement((-x.Value).Mod(x.curve.Q), x.curve);
    }

    public static ECFieldElement operator *(ECFieldElement x, ECFieldElement y)
    {
      return new ECFieldElement((x.Value * y.Value).Mod(x.curve.Q), x.curve);
    }

    public static ECFieldElement operator /(ECFieldElement x, ECFieldElement y)
    {
      return new ECFieldElement((x.Value * y.Value.ModInverse(x.curve.Q)).Mod(x.curve.Q), x.curve);
    }

    public static ECFieldElement operator +(ECFieldElement x, ECFieldElement y)
    {
      return new ECFieldElement((x.Value + y.Value).Mod(x.curve.Q), x.curve);
    }

    public static ECFieldElement operator -(ECFieldElement x, ECFieldElement y)
    {
      return new ECFieldElement((x.Value - y.Value).Mod(x.curve.Q), x.curve);
    }
  }
}
