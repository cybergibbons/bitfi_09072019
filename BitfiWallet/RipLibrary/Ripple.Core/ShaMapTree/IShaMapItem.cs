using Ripple.Core.Binary;
using Ripple.Core.Hashing;

namespace Ripple.Core.ShaMapTree
{
    public interface IShaMapItem\\
    {
        void ToBytes(IBytesSink sink);
        IShaMapItem\\ Copy();
        T Value();
        HashPrefix Prefix();
    }
}
