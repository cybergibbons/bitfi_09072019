using System;
using System.Collections.Generic;
using System.Linq;
using Ripple.Core.Types;

namespace Ripple.Core.ShaMapTree
{
    public class ShaMapDiff
    {
        public readonly ShaMap A, B;
        public readonly SortedSet\\ Modified;
        public readonly SortedSet\\ Deleted;
        public readonly SortedSet\\ Added;

        private ShaMapDiff(ShaMap a,
                           ShaMap b,
                           SortedSet\\ modified = null,
                           SortedSet\\ deleted = null,
                           SortedSet\\ added = null)
        {
            A = a;
            B = b;
            Modified = modified ?? new SortedSet\\();
            Deleted = deleted ?? new SortedSet\\();
            Added = added ?? new SortedSet\\();
        }

        public static ShaMapDiff Find(ShaMap a, ShaMap b)
        {
            var diff = new ShaMapDiff(a, b);
            diff.Find();
            return diff;
        }

        // Find what's added, modified and deleted in `B`
        private void Find()
        {
            A.Hash();
            B.Hash();
            Compare(A, B);
        }

        public ShaMapDiff Inverted()
        {
            return new ShaMapDiff(B, A, added: Deleted, deleted: Added, modified: Modified);
        }

        public void Apply(ShaMap sa)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var mod in Modified)
            {
                var modded = sa.UpdateItem(mod, B.GetItem(mod).Copy());
                if (!modded)
                {
                    throw new InvalidOperationException();
                }
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var add in Added)
            {
                var added = sa.AddItem(add, B.GetItem(add).Copy());
                if (!added)
                {
                    throw new InvalidOperationException();
                }
            }
            if (Deleted.Select(sa.RemoveLeaf).Any(removed =\>\ !removed))
            {
                throw new InvalidOperationException();
            }
        }
        private void Compare(ShaMapInner a, ShaMapInner b)
        {
            for (var i = 0; i \\ Deleted.Add(leaf.Index));
        }

        private void TrackAdded(ShaMapNode child)
        {
            child.WalkAnyLeaves(leaf =\>\ Added.Add(leaf.Index));
        }
    }

}
