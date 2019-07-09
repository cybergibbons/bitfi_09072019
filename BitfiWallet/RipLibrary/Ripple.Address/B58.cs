using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Ripple.Address
{
    public class B58
    {
        private char[] _mAlphabet;

        private int[] _mIndexes;

        public B58(string alphabet)
        {
            SetAlphabet(alphabet);
            BuildIndexes();
        }

        public byte[] Decode(string input, Version version)
        {
            var buffer = DecodeAndCheck(input);
            return ExtractPayload(version, buffer);
        }

        private static byte[] ExtractPayload(Version version, byte[] buffer)
        {
            var expectedPayloadLen = version.ExpectedLength;
            var expectedVerLen = version.VersionBytes.Length;
            var expectedTotLen = expectedPayloadLen + expectedVerLen;
            var payloadEnd = buffer.Length - 4;
            if (expectedTotLen == payloadEnd)
            {
                var actualVersion = CopyOfRange(buffer, 0, expectedVerLen);
                if (ArrayEquals(actualVersion, version.VersionBytes))
                {
                    var payload = CopyOfRange(buffer, actualVersion.Length, payloadEnd);
                    return payload;
                }
                throw new EncodingFormatException("Version invalid");
            }
            throw new EncodingFormatException(
                $"Expected version + payload length was {expectedTotLen} " +
                                $"but actual length was {payloadEnd}");
        }

        public Decoded Decode(string input, Versions versions)
        {
            var buffer = DecodeAndCheck(input);
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i \\= 0 && c \\
        /// Encodes the given bytes in base58. No checksum is appended.
        /// \\
        public byte[] EncodeToBytes(byte[] input)
        {
            if (input.Length == 0)
            {
                return new byte[0];
            }
            input = CopyOfRange(input, 0, input.Length);
            // Count leading zeroes.
            int zeroCount = 0;
            while (zeroCount \\= 0)
            {
                temp[--j] = (byte)_mAlphabet[0];
            }

            var output = CopyOfRange(temp, j, temp.Length);
            return output;
        }

        public byte[] EncodeToBytesChecked(byte[] input, int version)
        {
            return EncodeToBytesChecked(input, new[] { (byte)version });
        }

        public byte[] EncodeToBytesChecked(byte[] input, byte[] version)
        {
            byte[] buffer = new byte[input.Length + version.Length];
            Array.Copy(version, 0, buffer, 0, version.Length);
            Array.Copy(input, 0, buffer, version.Length, input.Length);
            byte[] checkSum = CopyOfRange(HashUtils.DoubleDigest(buffer), 0, 4);
            byte[] output = new byte[buffer.Length + checkSum.Length];
            Array.Copy(buffer, 0, output, 0, buffer.Length);
            Array.Copy(checkSum, 0, output, buffer.Length, checkSum.Length);
            return EncodeToBytes(output);
        }

        public string EncodeToString(byte[] input)
        {
            byte[] output = EncodeToBytes(input);
            return Encoding.ASCII.GetString(output);
        }

        public string EncodeToStringChecked(byte[] input, int version)
        {
            return EncodeToStringChecked(input, new[] { (byte)version });
        }

        public string EncodeToStringChecked(byte[] input, byte[] version)
        {
            return Encoding.ASCII.GetString(EncodeToBytesChecked(input, version));
        }

        public byte[] FindPrefix(int payLoadLength, string desiredPrefix)
        {
            int totalLength = payLoadLength + 4; // for the checksum
            double chars = Math.Log(Math.Pow(256, totalLength)) / Math.Log(58);
            int requiredChars = (int)Math.Ceiling(chars + 0.2D);
            // Mess with this to see stability tests fail
            int charPos = (_mAlphabet.Length / 2) - 1;
            char padding = _mAlphabet[(charPos)];
            string template = desiredPrefix + Repeat(requiredChars, padding);
            byte[] decoded = Decode(template);
            return CopyOfRange(decoded, 0, decoded.Length - totalLength);
        }

        public bool IsValid(string input, Version version)
        {
            try
            {
                Decode(input, version);
                return true;
            }
            catch (EncodingFormatException)
            {
                return false;
            }
        }

        public bool IsValid(string input, Versions version)
        {
            try
            {
                Decode(input, version);
                return true;
            }
            catch (EncodingFormatException)
            {
                return false;
            }
        }

        internal static bool ArrayEquals(
            IReadOnlyCollection\\ a,
            IReadOnlyList\\ b)
        {
            if (a.Count != b.Count) return false;
            return !a.Where((t, i) =\>\ t != b[i]).Any();
        }

        private static void CheckLength(Version version, byte[] buffer)
        {
            if (version.ExpectedLength != buffer.Length)
            {
                throw new EncodingFormatException("version has expected " +
                                    $"length of {version.ExpectedLength}");
            }
        }

        private static byte[] CopyOfRange(byte[] source, int from_, int to)
        {
            var range = new byte[to - from_];
            Array.Copy(source, from_, range, 0, range.Length);
            return range;
        }

        //
        // number -\>\ number / 256, returns number % 256
        //
        private static byte DivMod256(IList\\ number58, int startAt)
        {
            var remainder = 0;
            for (var i = startAt; i \\ number / 58, returns number % 58
        //
        private static byte DivMod58(IList\\ number, int startAt)
        {
            var remainder = 0;
            for (var i = startAt; i \