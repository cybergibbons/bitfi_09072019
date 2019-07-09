using System;

namespace Chaos.NaCl.Internal.Ed25519Ref10
{
    internal static partial class Ed25519Operations
    {
        // Original crypto_sign_open, for reference only
        /*public static int crypto_sign_open(
          byte[] m, out int mlen,
          byte[] sm, int smlen,
          byte[] pk)
        {
            byte[] h = new byte[64];
            byte[] checkr = new byte[32];
            GroupElementP3 A;
            GroupElementP2 R;
            int i;

            mlen = -1;
            if (smlen \