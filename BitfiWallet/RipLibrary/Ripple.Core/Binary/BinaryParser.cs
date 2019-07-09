using System;
using System.Diagnostics.Contracts;
using Ripple.Core.Enums;

namespace Ripple.Core.Binary
{
    public abstract class BinaryParser
    {
        protected internal int Size;
        protected internal int Cursor;
        public bool End() =\>\ Cursor \>\= Size;
        public int Pos() =\>\ Cursor;
        public int ReadOneInt() =\>\ ReadOne() & 0xFF;

        public abstract void Skip(int n);
        public abstract byte ReadOne();
        public abstract byte[] Read(int n);

        public Field ReadField()
        {
            var fieldCode = ReadFieldCode();
            var field = Field.Values[fieldCode];
            if (field == null)
            {
                throw new InvalidOperationException(
                    "Couldn't parse field from " +
                    $"{fieldCode.ToString("x")}");
            }

            return field;
        }

        public int ReadFieldCode()
        {
            var tagByte = ReadOne();

            var typeBits = (tagByte & 0xFF) \>\\>\ 4;
            if (typeBits == 0)
            {
                typeBits = ReadOne();
            }

            var fieldBits = tagByte & 0x0F;
            if (fieldBits == 0)
            {
                fieldBits = ReadOne();
            }

            return typeBits \\= Size ||
                   (customEnd != null && Cursor \>\= customEnd);
        }
    }
}