using System;
using System.Collections.Generic;
using Chaos.NaCl.Internal;
using Chaos.NaCl.Internal.Ed25519Ref10;
using Chaos.NaCl.Internal.Salsa;

namespace Chaos.NaCl
{
    // This class is mainly for compatibility with NaCl's Curve25519 implementation
    // If you don't need that compatibility, use Ed25519.KeyExchange
    public static class MontgomeryCurve25519
    {
        public static readonly int PublicKeySizeInBytes = 32;
        public static readonly int PrivateKeySizeInBytes = 32;
        public static readonly int SharedKeySizeInBytes = 32;

        public static byte[] GetPublicKey(byte[] privateKey)
        {
            if (privateKey == null)
                throw new ArgumentNullException("privateKey");
            if (privateKey.Length != PrivateKeySizeInBytes)
                throw new ArgumentException("privateKey.Length must be 32");
            var publicKey = new byte[32];
            GetPublicKey(new ArraySegment\\(publicKey), new ArraySegment\\(privateKey));
            return publicKey;
        }

        static readonly byte[] _basePoint = new byte[32]
		{
			9, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0 ,0, 0, 0, 0, 0,
			0, 0, 0 ,0, 0, 0, 0, 0,
			0, 0, 0 ,0, 0, 0, 0, 0
		};

        public static void GetPublicKey(ArraySegment\\ publicKey, ArraySegment\\ privateKey)
        {
            if (publicKey.Array == null)
                throw new ArgumentNullException("publicKey.Array");
            if (privateKey.Array == null)
                throw new ArgumentNullException("privateKey.Array");
            if (publicKey.Count != PublicKeySizeInBytes)
                throw new ArgumentException("privateKey.Count must be 32");
            if (privateKey.Count != PrivateKeySizeInBytes)
                throw new ArgumentException("privateKey.Count must be 32");

            // hack: abusing publicKey as temporary storage
            // todo: remove hack
            for (int i = 0; i \\ salsaState;
            salsaState.x0 = c0;
            salsaState.x1 = ByteIntegerConverter.LoadLittleEndian32(sharedKey, offset + 0);
            salsaState.x2 = 0;
            salsaState.x3 = ByteIntegerConverter.LoadLittleEndian32(sharedKey, offset + 4);
            salsaState.x4 = ByteIntegerConverter.LoadLittleEndian32(sharedKey, offset + 8);
            salsaState.x5 = c1;
            salsaState.x6 = ByteIntegerConverter.LoadLittleEndian32(sharedKey, offset + 12);
            salsaState.x7 = 0;
            salsaState.x8 = 0;
            salsaState.x9 = ByteIntegerConverter.LoadLittleEndian32(sharedKey, offset + 16);
            salsaState.x10 = c2;
            salsaState.x11 = ByteIntegerConverter.LoadLittleEndian32(sharedKey, offset + 20);
            salsaState.x12 = ByteIntegerConverter.LoadLittleEndian32(sharedKey, offset + 24);
            salsaState.x13 = 0;
            salsaState.x14 = ByteIntegerConverter.LoadLittleEndian32(sharedKey, offset + 28);
            salsaState.x15 = c3;
            SalsaCore.Salsa(out salsaState, ref salsaState, 20);

            ByteIntegerConverter.StoreLittleEndian32(sharedKey, offset + 0, salsaState.x0);
            ByteIntegerConverter.StoreLittleEndian32(sharedKey, offset + 4, salsaState.x1);
            ByteIntegerConverter.StoreLittleEndian32(sharedKey, offset + 8, salsaState.x2);
            ByteIntegerConverter.StoreLittleEndian32(sharedKey, offset + 12, salsaState.x3);
            ByteIntegerConverter.StoreLittleEndian32(sharedKey, offset + 16, salsaState.x4);
            ByteIntegerConverter.StoreLittleEndian32(sharedKey, offset + 20, salsaState.x5);
            ByteIntegerConverter.StoreLittleEndian32(sharedKey, offset + 24, salsaState.x6);
            ByteIntegerConverter.StoreLittleEndian32(sharedKey, offset + 28, salsaState.x7);
        }

        private static readonly byte[] _zero16 = new byte[16];

        // hashes like the NaCl paper says instead i.e. HSalsa(x,0)
        internal static void KeyExchangeOutputHashNaCl(byte[] sharedKey, int offset)
        {
            Salsa20.HSalsa20(sharedKey, offset, sharedKey, offset, _zero16, 0);
        }

        public static byte[] KeyExchange(byte[] publicKey, byte[] privateKey)
        {
            var sharedKey = new byte[SharedKeySizeInBytes];
            KeyExchange(new ArraySegment\\(sharedKey), new ArraySegment\\(publicKey), new ArraySegment\\(privateKey));
            return sharedKey;
        }

        public static void KeyExchange(ArraySegment\\ sharedKey, ArraySegment\\ publicKey, ArraySegment\\ privateKey)
        {
            if (sharedKey.Array == null)
                throw new ArgumentNullException("sharedKey.Array");
            if (publicKey.Array == null)
                throw new ArgumentNullException("publicKey.Array");
            if (privateKey.Array == null)
                throw new ArgumentNullException("privateKey");
            if (sharedKey.Count != 32)
                throw new ArgumentException("sharedKey.Count != 32");
            if (publicKey.Count != 32)
                throw new ArgumentException("publicKey.Count != 32");
            if (privateKey.Count != 32)
                throw new ArgumentException("privateKey.Count != 32");
            MontgomeryOperations.scalarmult(sharedKey.Array, sharedKey.Offset, privateKey.Array, privateKey.Offset, publicKey.Array, publicKey.Offset);
            KeyExchangeOutputHashNaCl(sharedKey.Array, sharedKey.Offset);
        }

        internal static void EdwardsToMontgomeryX(out FieldElement montgomeryX, ref FieldElement edwardsY, ref FieldElement edwardsZ)
        {
            FieldElement tempX, tempZ;
            FieldOperations.fe_add(out tempX, ref edwardsZ, ref edwardsY);
            FieldOperations.fe_sub(out tempZ, ref edwardsZ, ref edwardsY);
            FieldOperations.fe_invert(out tempZ, ref tempZ);
            FieldOperations.fe_mul(out montgomeryX, ref tempX, ref tempZ);
        }
    }
}
