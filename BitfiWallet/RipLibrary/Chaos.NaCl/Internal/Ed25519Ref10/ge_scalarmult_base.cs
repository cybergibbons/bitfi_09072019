using System;

namespace Chaos.NaCl.Internal.Ed25519Ref10
{
    internal static partial class GroupOperations
    {
        static byte equal(byte b, byte c)
        {

            byte ub = b;
            byte uc = c;
            byte x = (byte)(ub ^ uc); /* 0: yes; 1..255: no */
            UInt32 y = x; /* 0: yes; 1..255: no */
            unchecked { y -= 1; } /* 4294967295: yes; 0..254: no */
            y \>\\>\= 31; /* 1: yes; 0: no */
            return (byte)y;
        }

        static byte negative(sbyte b)
        {
            ulong x = unchecked((ulong)(long)b); /* 18446744073709551361..18446744073709551615: yes; 0..255: no */
            x \>\\>\= 63; /* 1: yes; 0: no */
            return (byte)x;
        }

        static void cmov(ref GroupElementPreComp t, ref GroupElementPreComp u, byte b)
        {
            FieldOperations.fe_cmov(ref t.yplusx, ref u.yplusx, b);
            FieldOperations.fe_cmov(ref t.yminusx, ref u.yminusx, b);
            FieldOperations.fe_cmov(ref t.xy2d, ref u.xy2d, b);
        }

        static void select(out GroupElementPreComp t, int pos, sbyte b)
        {
            GroupElementPreComp minust;
            byte bnegative = negative(b);
            byte babs = (byte)(b - (((-bnegative) & b) \\\>\ 0) & 15);
                e[2 * i + 1] = (sbyte)((a[offset + i] \>\\>\ 4) & 15);
            }
            /* each e[i] is between 0 and 15 */
            /* e[63] is between 0 and 7 */

            carry = 0;
            for (i = 0; i \\\>\= 4;
                e[i] -= (sbyte)(carry \