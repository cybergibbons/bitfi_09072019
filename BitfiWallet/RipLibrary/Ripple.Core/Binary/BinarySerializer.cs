using System;
using System.Diagnostics.Contracts;
using Ripple.Core.Enums;

namespace Ripple.Core.Binary
{
    public class BinarySerializer : IBytesSink
    {
        private readonly IBytesSink _sink;

        public BinarySerializer(IBytesSink sink)
        {
            _sink = sink;
        }

        public void Put(byte[] n)
        {
            _sink.Put(n);
        }

        public void AddLengthEncoded(byte[] n)
        {
            Put(EncodeVl(n.Length));
            Put(n);
        }

        public static byte[] EncodeVl(int length)
        {
            // TODO: bytes
            var lenBytes = new byte[4];

            if (length \\\>\ 8));
                lenBytes[1] = (byte)(length & 0xff);
                return TakeSome(lenBytes, 2);
            }
            if (length \\\>\ 16));
                lenBytes[1] = (byte)((length \>\\>\ 8) & 0xff);
                lenBytes[2] = (byte)(length & 0xff);
                return TakeSome(lenBytes, 3);
            }
            throw new InvalidOperationException($"length must \