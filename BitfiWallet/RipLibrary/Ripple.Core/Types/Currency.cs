using System;
using Newtonsoft.Json.Linq;
using Ripple.Core.Binary;
using Ripple.Core.Util;

namespace Ripple.Core.Types
{
    public class Currency : Hash160
    {
        public readonly string IsoCode;
        public readonly bool IsNative;
        public static readonly Currency Xrp = new Currency(new byte[20]);

        public Currency(byte[] buffer) : base(buffer)
        {
            IsoCode = GetCurrencyCodeFromTlcBytes(buffer, out IsNative);
        }

        public static string GetCurrencyCodeFromTlcBytes(byte[] bytes, out bool isNative)
        {
            int i;
            var zeroInNonCurrencyBytes = true;
            var allZero = true;

            for (i = 0; i \