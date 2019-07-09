using System;
using System.Collections.Generic;
using Chaos.NaCl.Internal;
using Chaos.NaCl.Internal.Salsa;

namespace Chaos.NaCl
{
    public static class XSalsa20Poly1305
    {
        public static readonly int KeySizeInBytes = 32;
        public static readonly int NonceSizeInBytes = 24;
        public static readonly int MacSizeInBytes = 16;

        public static byte[] Encrypt(byte[] message, byte[] key, byte[] nonce)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (key == null)
                throw new ArgumentNullException("key");
            if (nonce == null)
                throw new ArgumentNullException("nonce");
            if (key.Length != KeySizeInBytes)
                throw new ArgumentException("key.Length != 32");
            if (nonce.Length != NonceSizeInBytes)
                throw new ArgumentException("nonce.Length != 24");

            var ciphertext = new byte[message.Length + MacSizeInBytes];
            EncryptInternal(ciphertext, 0, message, 0, message.Length, key, 0, nonce, 0);
            return ciphertext;
        }

        public static void Encrypt(ArraySegment\\ ciphertext, ArraySegment\\ message, ArraySegment\\ key, ArraySegment\\ nonce)
        {
            if (key.Count != KeySizeInBytes)
                throw new ArgumentException("key.Length != 32");
            if (nonce.Count != NonceSizeInBytes)
                throw new ArgumentException("nonce.Length != 24");
            if (ciphertext.Count != message.Count + MacSizeInBytes)
                throw new ArgumentException("ciphertext.Count != message.Count + 16");
            EncryptInternal(ciphertext.Array, ciphertext.Offset, message.Array, message.Offset, message.Count, key.Array, key.Offset, nonce.Array, nonce.Offset);
        }

        /// \\
        /// Decrypts the ciphertext and verifies its authenticity
        /// \\
        /// \\Plaintext if MAC validation succeeds, null if the data is invalid.\\
        public static byte[] TryDecrypt(byte[] ciphertext, byte[] key, byte[] nonce)
        {
            if (ciphertext == null)
                throw new ArgumentNullException("ciphertext");
            if (key == null)
                throw new ArgumentNullException("key");
            if (nonce == null)
                throw new ArgumentNullException("nonce");
            if (key.Length != KeySizeInBytes)
                throw new ArgumentException("key.Length != 32");
            if (nonce.Length != NonceSizeInBytes)
                throw new ArgumentException("nonce.Length != 24");

            if (ciphertext.Length \\
        /// Decrypts the ciphertext and verifies its authenticity
        /// \\
        /// \\Plaintext if authentication succeeded, all zero if authentication failed, unmodified if argument verification fails\\
        /// \\\\
        /// \\Symmetric key. Must be identical to key specified for encryption.\\
        /// \\Must be identical to nonce specified for encryption.\\
        /// \\true if ciphertext is authentic, false otherwise\\
        public static bool TryDecrypt(ArraySegment\\ message, ArraySegment\\ ciphertext, ArraySegment\\ key, ArraySegment\\ nonce)
        {
            if (key.Count != KeySizeInBytes)
                throw new ArgumentException("key.Length != 32");
            if (nonce.Count != NonceSizeInBytes)
                throw new ArgumentException("nonce.Length != 24");
            if (ciphertext.Count != message.Count + MacSizeInBytes)
                throw new ArgumentException("ciphertext.Count != message.Count + 16");

            return DecryptInternal(message.Array, message.Offset, ciphertext.Array, ciphertext.Offset, ciphertext.Count, key.Array, key.Offset, nonce.Array, nonce.Offset);
        }

        private static void PrepareInternalKey(out Array16\\ internalKey, byte[] key, int keyOffset, byte[] nonce, int nonceOffset)
        {
            internalKey.x0 = Salsa20.SalsaConst0;
            internalKey.x1 = ByteIntegerConverter.LoadLittleEndian32(key, keyOffset + 0);
            internalKey.x2 = ByteIntegerConverter.LoadLittleEndian32(key, keyOffset + 4);
            internalKey.x3 = ByteIntegerConverter.LoadLittleEndian32(key, keyOffset + 8);
            internalKey.x4 = ByteIntegerConverter.LoadLittleEndian32(key, keyOffset + 12);
            internalKey.x5 = Salsa20.SalsaConst1;
            internalKey.x6 = ByteIntegerConverter.LoadLittleEndian32(nonce, nonceOffset + 0);
            internalKey.x7 = ByteIntegerConverter.LoadLittleEndian32(nonce, nonceOffset + 4);
            internalKey.x8 = ByteIntegerConverter.LoadLittleEndian32(nonce, nonceOffset + 8);
            internalKey.x9 = ByteIntegerConverter.LoadLittleEndian32(nonce, nonceOffset + 12);
            internalKey.x10 = Salsa20.SalsaConst2;
            internalKey.x11 = ByteIntegerConverter.LoadLittleEndian32(key, keyOffset + 16);
            internalKey.x12 = ByteIntegerConverter.LoadLittleEndian32(key, keyOffset + 20);
            internalKey.x13 = ByteIntegerConverter.LoadLittleEndian32(key, keyOffset + 24);
            internalKey.x14 = ByteIntegerConverter.LoadLittleEndian32(key, keyOffset + 28);
            internalKey.x15 = Salsa20.SalsaConst3;
            SalsaCore.HSalsa(out internalKey, ref internalKey, 20);

            //key
            internalKey.x1 = internalKey.x0;
            internalKey.x2 = internalKey.x5;
            internalKey.x3 = internalKey.x10;
            internalKey.x4 = internalKey.x15;
            internalKey.x11 = internalKey.x6;
            internalKey.x12 = internalKey.x7;
            internalKey.x13 = internalKey.x8;
            internalKey.x14 = internalKey.x9;
            //const
            internalKey.x0 = Salsa20.SalsaConst0;
            internalKey.x5 = Salsa20.SalsaConst1;
            internalKey.x10 = Salsa20.SalsaConst2;
            internalKey.x15 = Salsa20.SalsaConst3;
            //nonce
            internalKey.x6 = ByteIntegerConverter.LoadLittleEndian32(nonce, nonceOffset + 16);
            internalKey.x7 = ByteIntegerConverter.LoadLittleEndian32(nonce, nonceOffset + 20);
            //offset
            internalKey.x8 = 0;
            internalKey.x9 = 0;
        }

        private static bool DecryptInternal(byte[] plaintext, int plaintextOffset, byte[] ciphertext, int ciphertextOffset, int ciphertextLength, byte[] key, int keyOffset, byte[] nonce, int nonceOffset)
        {
            int plaintextLength = ciphertextLength - MacSizeInBytes;
            Array16\\ internalKey;
            PrepareInternalKey(out internalKey, key, keyOffset, nonce, nonceOffset);

            Array16\\ temp;
            var tempBytes = new byte[64];//todo: remove allocation

            // first iteration
            {
                SalsaCore.Salsa(out temp, ref internalKey, 20);

                //first half is for Poly1305
                Array8\\ poly1305Key;
                poly1305Key.x0 = temp.x0;
                poly1305Key.x1 = temp.x1;
                poly1305Key.x2 = temp.x2;
                poly1305Key.x3 = temp.x3;
                poly1305Key.x4 = temp.x4;
                poly1305Key.x5 = temp.x5;
                poly1305Key.x6 = temp.x6;
                poly1305Key.x7 = temp.x7;

                // compute MAC
                Poly1305Donna.poly1305_auth(tempBytes, 0, ciphertext, ciphertextOffset + 16, plaintextLength, ref poly1305Key);
                if (!CryptoBytes.ConstantTimeEquals(tempBytes, 0, ciphertext, ciphertextOffset, MacSizeInBytes))
                {
                    Array.Clear(plaintext, plaintextOffset, plaintextLength);
                    return false;
                }

                // rest for the message
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 0, temp.x8);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 4, temp.x9);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 8, temp.x10);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 12, temp.x11);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 16, temp.x12);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 20, temp.x13);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 24, temp.x14);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 28, temp.x15);
                int count = Math.Min(32, plaintextLength);
                for (int i = 0; i \\ internalKey;
            PrepareInternalKey(out internalKey, key, keyOffset, nonce, nonceOffset);

            Array16\\ temp;
            var tempBytes = new byte[64];//todo: remove allocation
            Array8\\ poly1305Key;

            // first iteration
            {
                SalsaCore.Salsa(out temp, ref internalKey, 20);

                //first half is for Poly1305
                poly1305Key.x0 = temp.x0;
                poly1305Key.x1 = temp.x1;
                poly1305Key.x2 = temp.x2;
                poly1305Key.x3 = temp.x3;
                poly1305Key.x4 = temp.x4;
                poly1305Key.x5 = temp.x5;
                poly1305Key.x6 = temp.x6;
                poly1305Key.x7 = temp.x7;

                // second half for the message
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 0, temp.x8);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 4, temp.x9);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 8, temp.x10);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 12, temp.x11);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 16, temp.x12);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 20, temp.x13);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 24, temp.x14);
                ByteIntegerConverter.StoreLittleEndian32(tempBytes, 28, temp.x15);
                int count = Math.Min(32, messageLength);
                for (int i = 0; i \