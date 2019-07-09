using System;

namespace Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class GroupOperations
	{
		/*
		r = 2 * p
		*/

		public static void ge_p2_dbl(out GroupElementP1P1 r, ref GroupElementP2 p)
		{
			FieldElement t0;

			/* qhasm: enter ge_p2_dbl */

			/* qhasm: fe X1 */

			/* qhasm: fe Y1 */

			/* qhasm: fe Z1 */

			/* qhasm: fe A */

			/* qhasm: fe AA */

			/* qhasm: fe XX */

			/* qhasm: fe YY */

			/* qhasm: fe B */

			/* qhasm: fe X3 */

			/* qhasm: fe Y3 */

			/* qhasm: fe Z3 */

			/* qhasm: fe T3 */

			/* qhasm: XX=X1^2 */
			/* asm 1: fe_sq(\>\XX=fe#1,\\XX=r.X,\\YY=fe#3,\\YY=r.Z,\\B=fe#4,\\B=r.T,\\A=fe#2,\\A=r.Y,\\AA=fe#5,\\AA=t0,\\Y3=fe#2,\\Y3=r.Y,\\Z3=fe#3,\\Z3=r.Z,\\X3=fe#1,\\X3=r.X,\\T3=fe#4,\\T3=r.T,\