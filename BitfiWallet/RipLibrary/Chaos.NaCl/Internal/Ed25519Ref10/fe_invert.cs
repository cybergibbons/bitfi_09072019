using System;

namespace Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class FieldOperations
	{
		internal static void fe_invert(out FieldElement result, ref FieldElement z)
		{
			FieldElement t0;
			FieldElement t1;
			FieldElement t2;
			FieldElement t3;
			int i;

			/* qhasm: fe z1 */

			/* qhasm: fe z2 */

			/* qhasm: fe z8 */

			/* qhasm: fe z9 */

			/* qhasm: fe z11 */

			/* qhasm: fe z22 */

			/* qhasm: fe z_5_0 */

			/* qhasm: fe z_10_5 */

			/* qhasm: fe z_10_0 */

			/* qhasm: fe z_20_10 */

			/* qhasm: fe z_20_0 */

			/* qhasm: fe z_40_20 */

			/* qhasm: fe z_40_0 */

			/* qhasm: fe z_50_10 */

			/* qhasm: fe z_50_0 */

			/* qhasm: fe z_100_50 */

			/* qhasm: fe z_100_0 */

			/* qhasm: fe z_200_100 */

			/* qhasm: fe z_200_0 */

			/* qhasm: fe z_250_50 */

			/* qhasm: fe z_250_0 */

			/* qhasm: fe z_255_5 */

			/* qhasm: fe z_255_21 */

			/* qhasm: enter pow225521 */

			/* qhasm: z2 = z1^2^1 */
			/* asm 1: fe_sq(\>\z2=fe#1,\\z2=fe#1,\>\z2=fe#1); */
			/* asm 2: fe_sq(\>\z2=t0,\\z2=t0,\>\z2=t0); */
			fe_sq(out t0, ref z); //for (i = 1; i \\z8=fe#2,\\z8=fe#2,\>\z8=fe#2); */
			/* asm 2: fe_sq(\>\z8=t1,\\z8=t1,\>\z8=t1); */
			fe_sq(out t1, ref t0); for (i = 1; i \\z9=fe#2,\\z9=t1,\\z11=fe#1,\\z11=t0,\\z22=fe#3,\\z22=fe#3,\>\z22=fe#3); */
			/* asm 2: fe_sq(\>\z22=t2,\\z22=t2,\>\z22=t2); */
			fe_sq(out t2, ref t0); //for (i = 1; i \\z_5_0=fe#2,\\z_5_0=t1,\\z_10_5=fe#3,\\z_10_5=fe#3,\>\z_10_5=fe#3); */
			/* asm 2: fe_sq(\>\z_10_5=t2,\\z_10_5=t2,\>\z_10_5=t2); */
			fe_sq(out t2, ref t1); for (i = 1; i \\z_10_0=fe#2,\\z_10_0=t1,\\z_20_10=fe#3,\\z_20_10=fe#3,\>\z_20_10=fe#3); */
			/* asm 2: fe_sq(\>\z_20_10=t2,\\z_20_10=t2,\>\z_20_10=t2); */
			fe_sq(out t2, ref t1); for (i = 1; i \\z_20_0=fe#3,\\z_20_0=t2,\\z_40_20=fe#4,\\z_40_20=fe#4,\>\z_40_20=fe#4); */
			/* asm 2: fe_sq(\>\z_40_20=t3,\\z_40_20=t3,\>\z_40_20=t3); */
			fe_sq(out t3, ref t2); for (i = 1; i \\z_40_0=fe#3,\\z_40_0=t2,\\z_50_10=fe#3,\\z_50_10=fe#3,\>\z_50_10=fe#3); */
			/* asm 2: fe_sq(\>\z_50_10=t2,\\z_50_10=t2,\>\z_50_10=t2); */
			fe_sq(out t2, ref t2); for (i = 1; i \\z_50_0=fe#2,\\z_50_0=t1,\\z_100_50=fe#3,\\z_100_50=fe#3,\>\z_100_50=fe#3); */
			/* asm 2: fe_sq(\>\z_100_50=t2,\\z_100_50=t2,\>\z_100_50=t2); */
			fe_sq(out t2, ref t1); for (i = 1; i \\z_100_0=fe#3,\\z_100_0=t2,\\z_200_100=fe#4,\\z_200_100=fe#4,\>\z_200_100=fe#4); */
			/* asm 2: fe_sq(\>\z_200_100=t3,\\z_200_100=t3,\>\z_200_100=t3); */
			fe_sq(out t3, ref t2); for (i = 1; i \\z_200_0=fe#3,\\z_200_0=t2,\\z_250_50=fe#3,\\z_250_50=fe#3,\>\z_250_50=fe#3); */
			/* asm 2: fe_sq(\>\z_250_50=t2,\\z_250_50=t2,\>\z_250_50=t2); */
			fe_sq(out t2, ref t2); for (i = 1; i \\z_250_0=fe#2,\\z_250_0=t1,\\z_255_5=fe#2,\\z_255_5=fe#2,\>\z_255_5=fe#2); */
			/* asm 2: fe_sq(\>\z_255_5=t1,\\z_255_5=t1,\>\z_255_5=t1); */
			fe_sq(out t1, ref t1); for (i = 1; i \\z_255_21=fe#12,\\z_255_21=out,\