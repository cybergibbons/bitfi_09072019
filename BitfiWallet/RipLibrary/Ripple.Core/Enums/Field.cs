namespace Ripple.Core.Enums
{
    public class Field : EnumItem
    {
        #region members
        public readonly bool IsSigningField;
        public readonly bool IsSerialised;
        public readonly bool IsVlEncoded;
        public readonly int NthOfType;
        public readonly FieldType Type;
        public readonly byte[] Header;

        public FromJson FromJson;
        public FromParser FromParser;

        #endregion

        public static readonly Enumeration\\ Values = new Enumeration\\();
        public Field(string name,
            int nthOfType,
            FieldType type,
            bool isSigningField=true,
            bool isSerialised=true) :
                base(name,
                    (type.Ordinal \\ 0) && nthOfType \\ 0 && type.Ordinal \