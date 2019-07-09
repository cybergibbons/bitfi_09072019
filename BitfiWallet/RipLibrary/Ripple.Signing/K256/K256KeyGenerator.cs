using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Ripple.Signing.Utils;

namespace Ripple.Signing.K256
{
    public class K256KeyGenerator
    {
        // See https://wiki.ripple.com/Account_Family
        public static K256KeyPair From128Seed(byte[] seedBytes, int keyIndex)
        {
            // The private generator (aka root private key, master private key)
            var privateGen = ComputePrivateGen(seedBytes);
            if (keyIndex == -1)
            {
                // The root keyPair
                return new K256KeyPair(privateGen);
            }
            var secret = ComputeSecretKey(privateGen, (uint) keyIndex);
            return new K256KeyPair(secret);
        }

        public static BigInteger ComputePrivateGen(byte[] seedBytes)
        {
            return ComputeScalar(seedBytes, null);
        }

        public static BigInteger ComputeSecretKey(
            BigInteger privateGen,
            uint accountNumber)
        {
            ECPoint publicGen = ComputePublicGenerator(privateGen);
            return ComputeScalar(publicGen.GetEncoded(true), accountNumber)
                            .Add(privateGen).Mod(Secp256K1.Order());
        }

        ///
        /// \\ secret scalar\\
        /// \\ the corresponding public key is the public generator
        ///         (aka public root key, master public key).
        /// \\
        public static ECPoint ComputePublicGenerator(BigInteger privateGen)
        {
            return ComputePublicKey(privateGen);
        }

        public static byte[] ComputePublicKey(byte[] publicGenBytes, uint accountNumber)
        {
            ECPoint rootPubPoint = Secp256K1.Curve().DecodePoint(publicGenBytes);
            BigInteger scalar = ComputeScalar(publicGenBytes, accountNumber);
            ECPoint point = Secp256K1.BasePoint().Multiply(scalar);
            ECPoint offset = rootPubPoint.Add(point);
            return offset.GetEncoded(true);
        }

        /// \\ - a bytes sequence of arbitrary length which will be hashed \\
        /// \\ - nullable optional uint32 to hash \\
        /// \\ a number between [1, order -1] suitable as a private key
        ///  \\
        public static BigInteger ComputeScalar(byte[] seedBytes, uint? discriminator)
        {
            BigInteger key = null;
            for (uint i = 0; i \\ secret point on the curve as BigInteger \\
        /// \\ corresponding public point \\
        public static ECPoint ComputePublicKey(BigInteger secretKey)
        {
            return Secp256K1.BasePoint().Multiply(secretKey);
        }
    }
}