﻿using System;

namespace Chaos.NaCl.Internal.Ed25519Ref10
{
    internal static partial class FieldOperations
    {
        /*
        Preconditions:
          |h| bounded by 1.1*2^26,1.1*2^25,1.1*2^26,1.1*2^25,etc.

        Write p=2^255-19; q=floor(h/p).
        Basic claim: q = floor(2^(-255)(h + 19 2^(-25)h9 + 2^(-1))).

        Proof:
          Have |h|\\\>\ 0);
                s[offset + 1] = (byte) (h0 \>\\>\ 8);
                s[offset + 2] = (byte) (h0 \>\\>\ 16);
                s[offset + 3] = (byte) ((h0 \>\\>\ 24) | (h1 \\\>\ 6);
                s[offset + 5] = (byte) (h1 \>\\>\ 14);
                s[offset + 6] = (byte) ((h1 \>\\>\ 22) | (h2 \\\>\ 5);
                s[offset + 8] = (byte) (h2 \>\\>\ 13);
                s[offset + 9] = (byte) ((h2 \>\\>\ 21) | (h3 \\\>\ 3);
                s[offset + 11] = (byte) (h3 \>\\>\ 11);
                s[offset + 12] = (byte) ((h3 \>\\>\ 19) | (h4 \\\>\ 2);
                s[offset + 14] = (byte) (h4 \>\\>\ 10);
                s[offset + 15] = (byte) (h4 \>\\>\ 18);
                s[offset + 16] = (byte) (h5 \>\\>\ 0);
                s[offset + 17] = (byte) (h5 \>\\>\ 8);
                s[offset + 18] = (byte) (h5 \>\\>\ 16);
                s[offset + 19] = (byte) ((h5 \>\\>\ 24) | (h6 \\\>\ 7);
                s[offset + 21] = (byte) (h6 \>\\>\ 15);
                s[offset + 22] = (byte) ((h6 \>\\>\ 23) | (h7 \\\>\ 5);
                s[offset + 24] = (byte) (h7 \>\\>\ 13);
                s[offset + 25] = (byte) ((h7 \>\\>\ 21) | (h8 \\\>\ 4);
                s[offset + 27] = (byte) (h8 \>\\>\ 12);
                s[offset + 28] = (byte) ((h8 \>\\>\ 20) | (h9 \\\>\ 2);
                s[offset + 30] = (byte) (h9 \>\\>\ 10);
                s[offset + 31] = (byte) (h9 \>\\>\ 18);
            }
        }

        internal static void fe_reduce(out FieldElement hr, ref FieldElement h)
        {
            Int32 h0 = h.x0;
            Int32 h1 = h.x1;
            Int32 h2 = h.x2;
            Int32 h3 = h.x3;
            Int32 h4 = h.x4;
            Int32 h5 = h.x5;
            Int32 h6 = h.x6;
            Int32 h7 = h.x7;
            Int32 h8 = h.x8;
            Int32 h9 = h.x9;
            Int32 q;
            Int32 carry0;
            Int32 carry1;
            Int32 carry2;
            Int32 carry3;
            Int32 carry4;
            Int32 carry5;
            Int32 carry6;
            Int32 carry7;
            Int32 carry8;
            Int32 carry9;

            q = (19 * h9 + (((Int32)1) \\\>\ 25;
            q = (h0 + q) \>\\>\ 26;
            q = (h1 + q) \>\\>\ 25;
            q = (h2 + q) \>\\>\ 26;
            q = (h3 + q) \>\\>\ 25;
            q = (h4 + q) \>\\>\ 26;
            q = (h5 + q) \>\\>\ 25;
            q = (h6 + q) \>\\>\ 26;
            q = (h7 + q) \>\\>\ 25;
            q = (h8 + q) \>\\>\ 26;
            q = (h9 + q) \>\\>\ 25;

            /* Goal: Output h-(2^255-19)q, which is between 0 and 2^255-20. */
            h0 += 19 * q;
            /* Goal: Output h-2^255 q, which is between 0 and 2^255-20. */

            carry0 = h0 \>\\>\ 26; h1 += carry0; h0 -= carry0 \\\>\ 25; h2 += carry1; h1 -= carry1 \\\>\ 26; h3 += carry2; h2 -= carry2 \\\>\ 25; h4 += carry3; h3 -= carry3 \\\>\ 26; h5 += carry4; h4 -= carry4 \\\>\ 25; h6 += carry5; h5 -= carry5 \\\>\ 26; h7 += carry6; h6 -= carry6 \\\>\ 25; h8 += carry7; h7 -= carry7 \\\>\ 26; h9 += carry8; h8 -= carry8 \\\>\ 25; h9 -= carry9 \