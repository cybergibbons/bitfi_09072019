using System;

namespace Chaos.NaCl.Internal.Ed25519Ref10
{
    internal static partial class ScalarOperations
    {
        /*
        Input:
          s[0]+256*s[1]+...+256^63*s[63] = s

        Output:
          s[0]+256*s[1]+...+256^31*s[31] = s mod l
          where l = 2^252 + 27742317777372353535851937790883648493.
          Overwrites s in place.
        */

        public static void sc_reduce(byte[] s)
        {
            Int64 s0 = 2097151 & load_3(s, 0);
            Int64 s1 = 2097151 & (load_4(s, 2) \>\\>\ 5);
            Int64 s2 = 2097151 & (load_3(s, 5) \>\\>\ 2);
            Int64 s3 = 2097151 & (load_4(s, 7) \>\\>\ 7);
            Int64 s4 = 2097151 & (load_4(s, 10) \>\\>\ 4);
            Int64 s5 = 2097151 & (load_3(s, 13) \>\\>\ 1);
            Int64 s6 = 2097151 & (load_4(s, 15) \>\\>\ 6);
            Int64 s7 = 2097151 & (load_3(s, 18) \>\\>\ 3);
            Int64 s8 = 2097151 & load_3(s, 21);
            Int64 s9 = 2097151 & (load_4(s, 23) \>\\>\ 5);
            Int64 s10 = 2097151 & (load_3(s, 26) \>\\>\ 2);
            Int64 s11 = 2097151 & (load_4(s, 28) \>\\>\ 7);
            Int64 s12 = 2097151 & (load_4(s, 31) \>\\>\ 4);
            Int64 s13 = 2097151 & (load_3(s, 34) \>\\>\ 1);
            Int64 s14 = 2097151 & (load_4(s, 36) \>\\>\ 6);
            Int64 s15 = 2097151 & (load_3(s, 39) \>\\>\ 3);
            Int64 s16 = 2097151 & load_3(s, 42);
            Int64 s17 = 2097151 & (load_4(s, 44) \>\\>\ 5);
            Int64 s18 = 2097151 & (load_3(s, 47) \>\\>\ 2);
            Int64 s19 = 2097151 & (load_4(s, 49) \>\\>\ 7);
            Int64 s20 = 2097151 & (load_4(s, 52) \>\\>\ 4);
            Int64 s21 = 2097151 & (load_3(s, 55) \>\\>\ 1);
            Int64 s22 = 2097151 & (load_4(s, 57) \>\\>\ 6);
            Int64 s23 = (load_4(s, 60) \>\\>\ 3);
            Int64 carry0;
            Int64 carry1;
            Int64 carry2;
            Int64 carry3;
            Int64 carry4;
            Int64 carry5;
            Int64 carry6;
            Int64 carry7;
            Int64 carry8;
            Int64 carry9;
            Int64 carry10;
            Int64 carry11;
            Int64 carry12;
            Int64 carry13;
            Int64 carry14;
            Int64 carry15;
            Int64 carry16;

            s11 += s23 * 666643;
            s12 += s23 * 470296;
            s13 += s23 * 654183;
            s14 -= s23 * 997805;
            s15 += s23 * 136657;
            s16 -= s23 * 683901;
            s23 = 0;

            s10 += s22 * 666643;
            s11 += s22 * 470296;
            s12 += s22 * 654183;
            s13 -= s22 * 997805;
            s14 += s22 * 136657;
            s15 -= s22 * 683901;
            s22 = 0;

            s9 += s21 * 666643;
            s10 += s21 * 470296;
            s11 += s21 * 654183;
            s12 -= s21 * 997805;
            s13 += s21 * 136657;
            s14 -= s21 * 683901;
            s21 = 0;

            s8 += s20 * 666643;
            s9 += s20 * 470296;
            s10 += s20 * 654183;
            s11 -= s20 * 997805;
            s12 += s20 * 136657;
            s13 -= s20 * 683901;
            s20 = 0;

            s7 += s19 * 666643;
            s8 += s19 * 470296;
            s9 += s19 * 654183;
            s10 -= s19 * 997805;
            s11 += s19 * 136657;
            s12 -= s19 * 683901;
            s19 = 0;

            s6 += s18 * 666643;
            s7 += s18 * 470296;
            s8 += s18 * 654183;
            s9 -= s18 * 997805;
            s10 += s18 * 136657;
            s11 -= s18 * 683901;
            s18 = 0;

            carry6 = (s6 + (1 \\\>\ 21; s7 += carry6; s6 -= carry6 \\\>\ 21; s9 += carry8; s8 -= carry8 \\\>\ 21; s11 += carry10; s10 -= carry10 \\\>\ 21; s13 += carry12; s12 -= carry12 \\\>\ 21; s15 += carry14; s14 -= carry14 \\\>\ 21; s17 += carry16; s16 -= carry16 \\\>\ 21; s8 += carry7; s7 -= carry7 \\\>\ 21; s10 += carry9; s9 -= carry9 \\\>\ 21; s12 += carry11; s11 -= carry11 \\\>\ 21; s14 += carry13; s13 -= carry13 \\\>\ 21; s16 += carry15; s15 -= carry15 \\\>\ 21; s1 += carry0; s0 -= carry0 \\\>\ 21; s3 += carry2; s2 -= carry2 \\\>\ 21; s5 += carry4; s4 -= carry4 \\\>\ 21; s7 += carry6; s6 -= carry6 \\\>\ 21; s9 += carry8; s8 -= carry8 \\\>\ 21; s11 += carry10; s10 -= carry10 \\\>\ 21; s2 += carry1; s1 -= carry1 \\\>\ 21; s4 += carry3; s3 -= carry3 \\\>\ 21; s6 += carry5; s5 -= carry5 \\\>\ 21; s8 += carry7; s7 -= carry7 \\\>\ 21; s10 += carry9; s9 -= carry9 \\\>\ 21; s12 += carry11; s11 -= carry11 \\\>\ 21; s1 += carry0; s0 -= carry0 \\\>\ 21; s2 += carry1; s1 -= carry1 \\\>\ 21; s3 += carry2; s2 -= carry2 \\\>\ 21; s4 += carry3; s3 -= carry3 \\\>\ 21; s5 += carry4; s4 -= carry4 \\\>\ 21; s6 += carry5; s5 -= carry5 \\\>\ 21; s7 += carry6; s6 -= carry6 \\\>\ 21; s8 += carry7; s7 -= carry7 \\\>\ 21; s9 += carry8; s8 -= carry8 \\\>\ 21; s10 += carry9; s9 -= carry9 \\\>\ 21; s11 += carry10; s10 -= carry10 \\\>\ 21; s12 += carry11; s11 -= carry11 \\\>\ 21; s1 += carry0; s0 -= carry0 \\\>\ 21; s2 += carry1; s1 -= carry1 \\\>\ 21; s3 += carry2; s2 -= carry2 \\\>\ 21; s4 += carry3; s3 -= carry3 \\\>\ 21; s5 += carry4; s4 -= carry4 \\\>\ 21; s6 += carry5; s5 -= carry5 \\\>\ 21; s7 += carry6; s6 -= carry6 \\\>\ 21; s8 += carry7; s7 -= carry7 \\\>\ 21; s9 += carry8; s8 -= carry8 \\\>\ 21; s10 += carry9; s9 -= carry9 \\\>\ 21; s11 += carry10; s10 -= carry10 \\\>\ 0);
                s[1] = (byte)(s0 \>\\>\ 8);
                s[2] = (byte)((s0 \>\\>\ 16) | (s1 \\\>\ 3);
                s[4] = (byte)(s1 \>\\>\ 11);
                s[5] = (byte)((s1 \>\\>\ 19) | (s2 \\\>\ 6);
                s[7] = (byte)((s2 \>\\>\ 14) | (s3 \\\>\ 1);
                s[9] = (byte)(s3 \>\\>\ 9);
                s[10] = (byte)((s3 \>\\>\ 17) | (s4 \\\>\ 4);
                s[12] = (byte)(s4 \>\\>\ 12);
                s[13] = (byte)((s4 \>\\>\ 20) | (s5 \\\>\ 7);
                s[15] = (byte)((s5 \>\\>\ 15) | (s6 \\\>\ 2);
                s[17] = (byte)(s6 \>\\>\ 10);
                s[18] = (byte)((s6 \>\\>\ 18) | (s7 \\\>\ 5);
                s[20] = (byte)(s7 \>\\>\ 13);
                s[21] = (byte)(s8 \>\\>\ 0);
                s[22] = (byte)(s8 \>\\>\ 8);
                s[23] = (byte)((s8 \>\\>\ 16) | (s9 \\\>\ 3);
                s[25] = (byte)(s9 \>\\>\ 11);
                s[26] = (byte)((s9 \>\\>\ 19) | (s10 \\\>\ 6);
                s[28] = (byte)((s10 \>\\>\ 14) | (s11 \\\>\ 1);
                s[30] = (byte)(s11 \>\\>\ 9);
                s[31] = (byte)(s11 \>\\>\ 17);
            }
        }

    }
}