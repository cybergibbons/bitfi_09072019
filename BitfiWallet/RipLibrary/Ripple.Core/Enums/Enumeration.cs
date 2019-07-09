using System.Collections;
using System.Collections.Generic;

namespace Ripple.Core.Enums
{
    public class Enumeration\\ : IEnumerable\\ where T : EnumItem
    {
        private readonly Dictionary\\ _byText = new Dictionary\\();
        private readonly Dictionary\\ _byOrdinal
            = new Dictionary\\();
        private readonly List\\ _byDefinitionOrder = new List\\(); 

        public T this[string name]
        {
            get
            {
                return _byText[name];
            }
            set { _byText[name] = value; }
        }

        public T this[int name]
        {
            get
            {
                return _byOrdinal[name];
            }
            set { _byOrdinal[name] = value; }
        }

        public virtual T AddEnum(T T)
        {
            _byOrdinal[T.Ordinal] = T;
            _byText[T.Name] = T;
            _byDefinitionOrder.Add(T);
            return T;
        }

        public IEnumerator\\ GetEnumerator()
        {
            return _byDefinitionOrder.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _byDefinitionOrder.GetEnumerator();
        }

        public bool Has(string key)
        {
            return _byText.ContainsKey(key);
        }
    }
}