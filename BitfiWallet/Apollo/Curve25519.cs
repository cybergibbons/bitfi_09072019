using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo
{
  public struct Key
  {
    public byte[] P { get; set; }
    public byte[] K { get; set; }
    public byte[] S { get; set; }
  }

  public static class Curve25519
  {
    private const int UNPACKED_SIZE = 16;
    private const int KEY_SIZE = 32;
    private const int P25 = 33554431; /* (1 \\= 2^255 */
    private static readonly byte[] ORDER_TIMES_8 = new byte[] {
        104, 159, 174, 231,
        210, 24, 147, 192,
        178, 230, 188, 23,
        245, 206, 247, 166,
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 128
    };

    /* group order (a prime near 2^252+2^124) */
    private static readonly byte[] ORDER = new byte[] {
        237, 211, 245, 92,
        26, 99, 18, 88,
        214, 156, 247, 162,
        222, 249, 222, 20,
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 16
    };


    private static void Clamp(byte[] k)
    {
      k[31] &= 0x7F;
      k[31] |= 0x40;
      k[0] &= 0xF8;
    }

    private static UInt16[] CreateUnpackedArray()
    {
      return new UInt16[UNPACKED_SIZE];
    }

    /* Convert to internal format from little-endian byte format */
    private static void Unpack(UInt16[] x, byte[] m)
    {
      for (var i = 0; i \\= 2^255-19 */
    private static bool IsOverflow(UInt16[] x)
    {
      return (
            ((x[0] \>\ P26 - 19)) &&
                ((x[1] & x[3] & x[5] & x[7] & x[9]) == P25) &&
                ((x[2] & x[4] & x[6] & x[8]) == P26)
            ) || (x[9] \>\ P25);
    }

    /* checks if x is "negative", requires reduced input */
    private static Int64 IsNegative(UInt16[] x)
    {
      var isOverflowOrNegative = IsOverflow(x) || x[9] \\\>\= 8;
      }

      return v;
    }

    private static void Core(byte[] Px, byte[] s, byte[] k, byte[] Gx)
    {
      var dx = CreateUnpackedArray();
      var t1 = CreateUnpackedArray();
      var t2 = CreateUnpackedArray();
      var t3 = CreateUnpackedArray();
      var t4 = CreateUnpackedArray();
      var x = new UInt16[2][] { CreateUnpackedArray(), CreateUnpackedArray() };
      var z = new UInt16[2][] { CreateUnpackedArray(), CreateUnpackedArray() };
      int i = 0;
      int j = 0;

      /* unpack the base */
      if (Gx != null)
        Unpack(dx, Gx);
      else
        Set(dx, 9);

      /* 0G = point-at-infinity */
      Set(x[0], 1);
      Set(z[0], 0);

      /* 1G = G */
      Cpy(x[1], dx);
      Set(z[1], 1);

      for (i = 32; i-- != 0;)
      {
        for (j = 8; j-- != 0;)
        {
          /* swap arguments depending on bit */
          var bit1 = (k[i] & 0xFF) \>\\>\ j & 1;
          var bit0 = ~(k[i] & 0xFF) \>\\>\ j & 1;
          var ax = x[bit0];
          var az = z[bit0];
          var bx = x[bit1];
          var bz = z[bit1];

          /* a' = a + b	*/
          /* b' = 2 b	*/
          MontPrep(t1, t2, ax, az);
          MontPrep(t3, t4, bx, bz);
          MontAdd(t1, t2, t3, t4, ax, az, dx);
          MontDbl(t1, t2, t3, t4, bx, bz);

        }
      }

      Recip(t1, z[0], 0);
      Mul(dx, x[0], t1);

      Pack(dx, Px);

      /* calculate s such that s abs(P) = G  .. assumes G is std base point */
      if (s != null)
      {
        XtoY2(t2, t1, dx); /* t1 = Py^2  */
        Recip(t3, z[1], 0); /* where Q=P+G ... */
        Mul(t2, x[1], t3); /* t2 = Qx  */
        Add(t2, t2, dx); /* t2 = Qx + Px  */
        Add(t2, t2, C486671); /* t2 = Qx + Px + Gx + 486662  */
        Sub(dx, dx, C9); /* dx = Px - Gx  */
        Sqr(t3, dx); /* t3 = (Px - Gx)^2  */
        Mul(dx, t2, t3); /* dx = t2 (Px - Gx)^2  */
        Sub(dx, dx, t1); /* dx = t2 (Px - Gx)^2 - Py^2  */
        Sub(dx, dx, C39420360); /* dx = t2 (Px - Gx)^2 - Py^2 - Gy^2  */
        Mul(t1, dx, BASE_R2Y); /* t1 = -Py  */

        if (IsNegative(t1) != 0)    /* sign is 1, so just copy  */
          Cpy32(s, k);
        else            /* sign is -1, so negate  */
          MulaSmall(s, ORDER_TIMES_8, 0, k, 32, -1);

        /* reduce s mod q
         * (is this needed?  do it just in case, it's fast anyway) */
        //divmod((dstptr) t1, s, 32, order25519, 32);

        /* take reciprocal of s mod q */
        var temp1 = new byte[32];
        var temp2 = new byte[64];
        var temp3 = new byte[64];
        Cpy32(temp1, ORDER);
        Cpy32(s, Egcd32(temp2, temp3, s, temp1));
        if ((s[31] & 0x80) != 0)
          MulaSmall(s, s, 0, ORDER, 32, 1);

      }
    }

    private static int Numsize(byte[] x, int n)
    {
      while (n-- != 0 && x[n] == 0) { }
      return n + 1;
    }

    /* divide r (size n) by d (size t), returning quotient q and remainder r
     * quotient is size n-t+1, remainder is size t
     * requires t \>\ 0 && d[t-1] !== 0
     * requires that r[-1] and d[-1] are valid memory locations
     * q may overlap with r+t */
    private static void Divmod(byte[] q, byte[] r, int n, byte[] d, int t)
    {
      n = n | 0;
      t = t | 0;

      var rn = 0;
      var dt = (d[t - 1] & 0xFF) \\ 1)
        dt |= (d[t - 2] & 0xFF);

      while (n-- \>\= t)
      {
        var z = (rn \\ 0)
          z |= (r[n - 1] & 0xFF);

        var i = n - t + 1;
        z /= dt;
        rn += MulaSmall(r, r, i, d, t, -z);
        q[i] = (byte)((z + rn) & 0xFF);
        /* rn is 0 or -1 (underflow) */
        MulaSmall(r, r, i, d, t, -rn);
        rn = r[n] & 0xFF;
        r[n] = 0;
      }

      r[t - 1] = (byte)(rn & 0xFF);
    }

    /* p += x * y * z  where z is a small integer
     * x is size 32, y is size t, p is size 32+t
     * y is allowed to overlap with p+32 if you don't care about the upper half  */
    private static int Mula32(byte[] p, byte[] x, byte[] y, int t, int z)
    {
      t = t | 0;
      z = z | 0;

      var n = 31;
      var w = 0;
      var i = 0;
      for (; i \\\>\= 8;
      }
      p[i + n] = (byte)((w + (p[i + n] & 0xFF)) & 0xFF);
      return w \>\\>\ 8;
    }

    /* Signature generation primitive, calculates (x-h)s mod q
     *   h  [in]  signature hash (of message, signature pub key, and context data)
     *   x  [in]  signature private key
     *   s  [in]  private key for signing
     * returns signature value on success, undefined on failure (use different x or h)
     */
    public static byte[] Sign(byte[] h, byte[] x, byte[] s)
    {
      // v = (x - h) s  mod q
      int w = 0;
      int i = 0;
      var h1 = new byte[32];
      var x1 = new byte[32];
      var tmp1 = new byte[64];
      var tmp2 = new byte[64];

      // Don't clobber the arguments, be nice!
      Cpy32(h1, h);
      Cpy32(x1, x);

      // Reduce modulo group order
      var tmp3 = new byte[32];
      Divmod(tmp3, h1, 32, ORDER, 32);
      Divmod(tmp3, x1, 32, ORDER, 32);

      // v = x1 - h1
      // If v is negative, add the group order to it to become positive.
      // If v was already positive we don't have to worry about overflow
      // when adding the order because v \\\>\ 8);
      }
    }

    public static Key Keygen(byte[] k)
    {
      var P = new byte[32];
      var s = new byte[32];
      Clamp(k);
      Core(P, s, k, null);

      return new Key
      {
        P = P,
        S = s,
        K = k
      };
    }
  }
}
