using System;

namespace Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class GroupOperations
	{
		private static void slide(sbyte[] r, byte[] a)
		{
			int i;
			int b;
			int k;

			for (i = 0; i \\\>\ 3] \>\\>\ (i & 7)));

			for (i = 0; i \\= -15)
							{
								r[i] -= (sbyte)(r[i + b] \\= 0; --i)
			{
				if ((aslide[i] != 0) || (bslide[i] != 0)) break;
			}

			for (; i \>\= 0; --i)
			{
				ge_p2_dbl(out t, ref r);

				if (aslide[i] \>\ 0)
				{
					ge_p1p1_to_p3(out u, ref t);
					ge_add(out t, ref u, ref Ai[aslide[i] / 2]);
				}
				else if (aslide[i] \\ 0)
				{
					ge_p1p1_to_p3(out u, ref t);
					ge_madd(out t, ref u, ref Bi[bslide[i] / 2]);
				}
				else if (bslide[i] \