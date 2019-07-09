using System;

namespace Chaos.NaCl.Internal.Ed25519Ref10
{
	public static class MontgomeryOperations
	{
		public static void scalarmult(
			byte[] q, int qoffset,
			byte[] n, int noffset,
			byte[] p, int poffset)
		{
			FieldElement p0;
			FieldElement q0;
			FieldOperations.fe_frombytes2(out p0, p, poffset);
			scalarmult(out q0, n, noffset, ref p0);
			FieldOperations.fe_tobytes(q, qoffset, ref q0);
		}

		internal static void scalarmult(
			out FieldElement q,
			byte[] n, int noffset,
			ref FieldElement p)
		{
			byte[] e = new byte[32];//ToDo: remove allocation
			UInt32 i;
			FieldElement x1;
			FieldElement x2;
			FieldElement z2;
			FieldElement x3;
			FieldElement z3;
			FieldElement tmp0;
			FieldElement tmp1;
			int pos;
			UInt32 swap;
			UInt32 b;

			for (i = 0; i \\= 0; --pos)
			{
				b = (uint)(e[pos / 8] \>\\>\ (pos & 7));
				b &= 1;
				swap ^= b;
				FieldOperations.fe_cswap(ref x2, ref x3, swap);
				FieldOperations.fe_cswap(ref z2, ref z3, swap);
				swap = b;
				/* qhasm: fe X2 */

				/* qhasm: fe Z2 */

				/* qhasm: fe X3 */

				/* qhasm: fe Z3 */

				/* qhasm: fe X4 */

				/* qhasm: fe Z4 */

				/* qhasm: fe X5 */

				/* qhasm: fe Z5 */

				/* qhasm: fe A */

				/* qhasm: fe B */

				/* qhasm: fe C */

				/* qhasm: fe D */

				/* qhasm: fe E */

				/* qhasm: fe AA */

				/* qhasm: fe BB */

				/* qhasm: fe DA */

				/* qhasm: fe CB */

				/* qhasm: fe t0 */

				/* qhasm: fe t1 */

				/* qhasm: fe t2 */

				/* qhasm: fe t3 */

				/* qhasm: fe t4 */

				/* qhasm: enter ladder */

				/* qhasm: D = X3-Z3 */
				/* asm 1: fe_sub(\>\D=fe#5,\\D=tmp0,\\B=fe#6,\\B=tmp1,\\A=fe#1,\\A=x2,\\C=fe#2,\\C=z2,\\DA=fe#4,\\DA=z3,\\CB=fe#2,\\CB=z2,\\BB=fe#5,\\BB=tmp0,\\AA=fe#6,\\AA=tmp1,\\t0=fe#3,\\t0=x3,\\t1=fe#2,\\t1=z2,\\X4=fe#1,\\X4=x2,\\E=fe#6,\\E=tmp1,\\t2=fe#2,\\t2=z2,\\t3=fe#4,\\t3=z3,\\X5=fe#3,\\X5=x3,\\t4=fe#5,\\t4=tmp0,\\Z5=fe#4,x1,\\Z5=z3,x1,\\Z4=fe#2,\\Z4=z2,\