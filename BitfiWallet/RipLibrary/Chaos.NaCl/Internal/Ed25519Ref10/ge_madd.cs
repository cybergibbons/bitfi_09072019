using System;

namespace Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class GroupOperations
	{
		/*
		r = p + q
		*/
		public static void ge_madd(out GroupElementP1P1 r, ref  GroupElementP3 p, ref GroupElementPreComp q)
		{
			FieldElement t0;

			/* qhasm: enter ge_madd */

			/* qhasm: fe X1 */

			/* qhasm: fe Y1 */

			/* qhasm: fe Z1 */

			/* qhasm: fe T1 */

			/* qhasm: fe ypx2 */

			/* qhasm: fe ymx2 */

			/* qhasm: fe xy2d2 */

			/* qhasm: fe X3 */

			/* qhasm: fe Y3 */

			/* qhasm: fe Z3 */

			/* qhasm: fe T3 */

			/* qhasm: fe YpX1 */

			/* qhasm: fe YmX1 */

			/* qhasm: fe A */

			/* qhasm: fe B */

			/* qhasm: fe C */

			/* qhasm: fe D */

			/* qhasm: YpX1 = Y1+X1 */
			/* asm 1: fe_add(\>\YpX1=fe#1,\\YpX1=r.X,\\YmX1=fe#2,\\YmX1=r.Y,\\A=fe#3,\\A=r.Z,\\B=fe#2,\\B=r.Y,\\C=fe#4,\\C=r.T,\\D=fe#5,\\D=t0,\\X3=fe#1,\\X3=r.X,\\Y3=fe#2,\\Y3=r.Y,\\Z3=fe#3,\\Z3=r.Z,\\T3=fe#4,\\T3=r.T,\