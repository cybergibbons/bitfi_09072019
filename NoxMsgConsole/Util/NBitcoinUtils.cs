using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
namespace NoxMsgConsole.Util
{
    //NBitcoin utility //FormatMessageForSigning//Bitcoin Stream
    public class NBitcoinUtils
    {
        internal static String BITCOIN_SIGNED_MESSAGE_HEADER = "Bitcoin Signed Message:\n";
        internal static byte[] BITCOIN_SIGNED_MESSAGE_HEADER_BYTES = Encoding.UTF8.GetBytes(BITCOIN_SIGNED_MESSAGE_HEADER);
        //http://bitcoinj.googlecode.com/git-history/keychain/core/src/main/java/com/google/bitcoin/core/Utils.java
        public static byte[] CreateEncodedMessageD2(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            MemoryStream ms = new MemoryStream();
            ms.WriteByte((byte)BITCOIN_SIGNED_MESSAGE_HEADER_BYTES.Length);
            Write(ms, BITCOIN_SIGNED_MESSAGE_HEADER_BYTES);
            VarInt size = new VarInt((ulong)messageBytes.Length);
            Write(ms, size.ToBytes());
            Write(ms, messageBytes);
            byte[] data = ms.ToArray();
            using (var sha = new SHA256Managed())
            {
                var h = sha.ComputeHash(data, 0, data.Length);
                return sha.ComputeHash(h, 0, h.Length);
            }
        }
        private static void Write(MemoryStream ms, byte[] bytes)
        {
            ms.Write(bytes, 0, bytes.Length);
        }
        private static byte[] WriteBts(byte[] bytes, int start, int lenght)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(bytes, start, lenght);
            return ms.ToArray();
        }
    }
    public class VarInt : IBitcoinSerializable
    {
        private byte _PrefixByte = 0;
        private ulong _Value = 0;
        public VarInt()
          : this(0)
        {
        }
        public VarInt(ulong value)
        {
            SetValue(value);
        }
        internal void SetValue(ulong value)
        {
            this._Value = value;
            if (_Value < 0xFD)
                _PrefixByte = (byte)(int)_Value;
            else if (_Value <= 0xffff)
                _PrefixByte = 0xFD;
            else if (_Value <= 0xffffffff)
                _PrefixByte = 0xFE;
            else
                _PrefixByte = 0xFF;
        }
        public ulong ToLong()
        {
            return _Value;
        }
        #region IBitcoinSerializable Members
        public void ReadWrite(BitcoinStream stream)
        {
            stream.ReadWrite(ref _PrefixByte);
            if (_PrefixByte < 0xFD)
            {
                _Value = _PrefixByte;
            }
            else if (_PrefixByte == 0xFD)
            {
                var value = (ushort)_Value;
                stream.ReadWrite(ref value);
                _Value = value;
            }
            else if (_PrefixByte == 0xFE)
            {
                var value = (uint)_Value;
                stream.ReadWrite(ref value);
                _Value = value;
            }
            else
            {
                var value = (ulong)_Value;
                stream.ReadWrite(ref value);
                _Value = value;
            }
        }
        #endregion
    }
    public class CompactVarInt : IBitcoinSerializable
    {
        private ulong _Value = 0;
        private int _Size;
        public CompactVarInt(int size)
        {
            _Size = size;
        }
        public CompactVarInt(ulong value, int size)
        {
            _Value = value;
            _Size = size;
        }
        #region IBitcoinSerializable Members
        public void ReadWrite(BitcoinStream stream)
        {
            if (stream.Serializing)
            {
                ulong n = _Value;
                byte[] tmp = new byte[(_Size * 8 + 6) / 7];
                int len = 0;
                while (true)
                {
                    byte a = (byte)(n & 0x7F);
                    byte b = (byte)(len != 0 ? 0x80 : 0x00);
                    tmp[len] = (byte)(a | b);
                    if (n <= 0x7F)
                        break;
                    n = (n >> 7) -1;
                    len++;
                }
                do
                {
                    byte b = tmp[len];
                    stream.ReadWrite(ref b);
                } while (len-- != 0);
            }
            else
            {
                ulong n = 0;
                while (true)
                {
                    byte chData = 0;
                    stream.ReadWrite(ref chData);
                    ulong a = (n << 7);
                    byte b = (byte)(chData & 0x7F);
                    n = (a | b);
                    if ((chData & 0x80) != 0)
                        n++;
                    else
                        break;
                }
                _Value = n;
            }
        }
        #endregion
        public ulong ToLong()
        {
            return _Value;
        }
    }
    public interface IBitcoinSerializable
    {
        void ReadWrite(BitcoinStream stream);
    }
    public enum TransactionOptions : uint
    {
        None = 0x00000000,
        Witness = 0x40000000,
        All = Witness
    }
    public static class BitcoinSerializableExtensions
    {
        public static void ReadWrite(this IBitcoinSerializable serializable, Stream stream, bool serializing, ProtocolVersion version = ProtocolVersion.PROTOCOL_VERSION)
        {
            BitcoinStream s = new BitcoinStream(stream, serializing)
            {
                ProtocolVersion = version
            };
            serializable.ReadWrite(s);
        }
        public static int GetSerializedSize(this IBitcoinSerializable serializable, ProtocolVersion version, SerializationType serializationType)
        {
            BitcoinStream s = new BitcoinStream(Stream.Null, true);
            s.Type = Util.SerializationType.Disk;
            s.ReadWrite(serializable);
            return (int)s.Counter.WrittenBytes;
        }
        public static int GetSerializedSize(this IBitcoinSerializable serializable, TransactionOptions options)
        {
            var bms = new BitcoinStream(Stream.Null, true);
            bms.TransactionOptions = options;
            serializable.ReadWrite(bms);
            return (int)bms.Counter.WrittenBytes;
        }
        public static int GetSerializedSize(this IBitcoinSerializable serializable, ProtocolVersion version = ProtocolVersion.PROTOCOL_VERSION)
        {
            return GetSerializedSize(serializable, version, SerializationType.Disk);
        }
        public static void ReadWrite(this IBitcoinSerializable serializable, byte[] bytes, ProtocolVersion version = ProtocolVersion.PROTOCOL_VERSION)
        {
            ReadWrite(serializable, new MemoryStream(bytes), false, version);
        }
        public static void FromBytes(this IBitcoinSerializable serializable, byte[] bytes, ProtocolVersion version = ProtocolVersion.PROTOCOL_VERSION)
        {
            serializable.ReadWrite(new BitcoinStream(bytes)
            {
                ProtocolVersion = version
            });
        }
        public static T Clone<T>(this T serializable, ProtocolVersion version = ProtocolVersion.PROTOCOL_VERSION) where T : IBitcoinSerializable, new()
        {
            var instance = new T();
        instance.FromBytes(serializable.ToBytes(version), version);
            return instance;
        }
    public static byte[] ToBytes(this IBitcoinSerializable serializable, ProtocolVersion version = ProtocolVersion.PROTOCOL_VERSION)
    {
        MemoryStream ms = new MemoryStream();
        serializable.ReadWrite(new BitcoinStream(ms, true)
        {
            ProtocolVersion = version
        });
        return ToArrayEfficient(ms);
    }
    public static byte[] ToArrayEfficient(this MemoryStream ms)
    {
#if !(PORTABLE || NETCORE)
        var bytes = ms.GetBuffer();
        Array.Resize(ref bytes, (int)ms.Length);
        //     Console.WriteLine(BitConverter.ToString(bytes));
        return bytes;
#else
			return ms.ToArray();
#endif
    }
    public enum SerializationType
    {
        Disk,
        Network,
        Hash
    }
    public class Scope : IDisposable
    {
        Action close;
        public Scope(Action open, Action close)
        {
            this.close = close;
            open();
        }
        #region IDisposable Members
        public void Dispose()
        {
            close();
        }
        #endregion
        public static IDisposable Nothing
        {
            get
            {
                return new Scope(() =>
                    {
                }, () =>
                    {
                });
            }
        }
    }
}
public partial class BitcoinStream
{
    int _MaxArraySize = 1024 * 1024;
    public int MaxArraySize
    {
        get
        {
            return _MaxArraySize;
        }
        set
        {
            _MaxArraySize = value;
        }
    }
    //ReadWrite<T>(ref T data)
    static MethodInfo _ReadWriteTyped;
    static BitcoinStream()
    {
        _ReadWriteTyped = typeof(BitcoinStream)
        .GetTypeInfo()
        .DeclaredMethods
        .Where(m => m.Name == "ReadWrite")
        .Where(m => m.IsGenericMethodDefinition)
        .Where(m => m.GetParameters().Length == 1)
        .Where(m => m.GetParameters().Any(p => p.ParameterType.IsByRef && p.ParameterType.HasElementType && !p.ParameterType.GetElementType().IsArray))
        .First();
    }
    private readonly Stream _Inner;
    public Stream Inner
    {
        get
        {
            return _Inner;
        }
    }
    private readonly bool _Serializing;
    public bool Serializing
    {
        get
        {
            return _Serializing;
        }
    }
    public BitcoinStream(Stream inner, bool serializing)
    {
        _Serializing = serializing;
        _Inner = inner;
    }
    public BitcoinStream(byte[] bytes)
        : this(new MemoryStream(bytes), false)
    {
    }

    public T ReadWrite<T>(T data) where T : IBitcoinSerializable
        {
            ReadWrite<T>(ref data);
            return data;
        }
public void ReadWriteAsVarString(ref byte[] bytes)
{
    if (Serializing)
    {
        VarString str = new VarString(bytes);
        str.ReadWrite(this);
    }
    else
    {
        VarString str = new VarString();
        str.ReadWrite(this);
        bytes = str.GetString(true);
    }
}
public void ReadWrite(Type type, ref object obj)
{
    try
    {
        var parameters = new object[] { obj };
        _ReadWriteTyped.MakeGenericMethod(type).Invoke(this, parameters);
        obj = parameters[0];
    }
    catch (TargetInvocationException ex)
    {
        throw ex.InnerException;
    }
}
public void ReadWrite(ref byte data)
{
    ReadWriteByte(ref data);
}
public byte ReadWrite(byte data)
{
    ReadWrite(ref data);
    return data;
}
public void ReadWrite(ref bool data)
{
    byte d = data ? (byte)1 : (byte)0;
    ReadWriteByte(ref d);
    data = (d == 0 ? false : true);
}
public void ReadWriteStruct<T>(ref T data) where T : struct, IBitcoinSerializable
        {
            data.ReadWrite(this);
}
public void ReadWriteStruct<T>(T data) where T : struct, IBitcoinSerializable
        {
            data.ReadWrite(this);
}
public void ReadWrite<T>(ref T data) where T : IBitcoinSerializable
        {
            var obj = data;
            if (obj == null)
                obj = Activator.CreateInstance<T>();
            obj.ReadWrite(this);
            if (!Serializing)
                data = obj;
}
public void ReadWrite<T>(ref List<T> list) where T : IBitcoinSerializable, new()
        {
            ReadWriteList<List<T>, T>(ref list);
        }
        public void ReadWrite<TList, TItem>(ref TList list)
            where TList : List<TItem>, new()
            where TItem : IBitcoinSerializable, new()
        {
            ReadWriteList<TList, TItem>(ref list);
        }
        private void ReadWriteList<TList, TItem>(ref TList data)
            where TList : List<TItem>, new()
            where TItem : IBitcoinSerializable, new()
        {
            var dataArray = data == null ? null : data.ToArray();
            if (Serializing && dataArray == null)
            {
                dataArray = new TItem[0];
            }
            ReadWriteArray(ref dataArray);
            if (!Serializing)
            {
                if (data == null)
                    data = new TList();
                else
                    data.Clear();
                data.AddRange(dataArray);
            }
        }
        public void ReadWrite(ref byte[] arr)
{
    ReadWriteBytes(ref arr);
}
public void ReadWrite(ref byte[] arr, int offset, int count)
{
    ReadWriteBytes(ref arr, offset, count);
}
public void ReadWrite<T>(ref T[] arr) where T : IBitcoinSerializable, new()
        {
            ReadWriteArray<T>(ref arr);
        }
        private void ReadWriteNumber(ref long value, int size)
{
    ulong uvalue = unchecked((ulong)value);
    ReadWriteNumber(ref uvalue, size);
    value = unchecked((long)uvalue);
}
private void ReadWriteNumber(ref ulong value, int size)
{
    var bytes = new byte[size];
    for (int i = 0; i < size; i++)
            {
        bytes[i] = (byte)(value >> i * 8);
    }
    if (IsBigEndian)
        Array.Reverse(bytes);
    ReadWriteBytes(ref bytes);
    if (IsBigEndian)
        Array.Reverse(bytes);
    ulong valueTemp = 0;
    for (int i = 0; i < bytes.Length; i++)
            {
        var v = (ulong)bytes[i];
        valueTemp += v << (i * 8);
    }
    value = valueTemp;
}
private void ReadWriteBytes(ref byte[] data, int offset = 0, int count = -1)
{
    if (data == null) throw new ArgumentNullException("data");
    if (data.Length == 0) return;
    count = count == -1 ? data.Length : count;
    if (count == 0) return;
    if (Serializing)
    {
        Inner.Write(data, offset, count);
        Counter.AddWritten(count);
    }
    else
    {
        var readen = Inner.ReadEx(data, offset, count, ReadCancellationToken);
        if (readen == 0)
            throw new EndOfStreamException("No more byte to read");
        Counter.AddReaden(readen);
    }
}
private PerformanceCounter _Counter;
public PerformanceCounter Counter
{
    get
    {
        if (_Counter == null)
            _Counter = new PerformanceCounter();
        return _Counter;
    }
}
private void ReadWriteByte(ref byte data)
{
    if (Serializing)
    {
        Inner.WriteByte(data);
        Counter.AddWritten(1);
    }
    else
    {
        var readen = Inner.ReadByte();
        if (readen == -1)
            throw new EndOfStreamException("No more byte to read");
        data = (byte)readen;
        Counter.AddReaden(1);
    }
}
public bool IsBigEndian
{
    get;
    set;
}
public IDisposable BigEndianScope()
{
    var old = IsBigEndian;
    return new Scope(() =>
            {
        IsBigEndian = true;
},
            () =>
            {
                IsBigEndian = old;
            });
        }
        ProtocolVersion _ProtocolVersion = ProtocolVersion.PROTOCOL_VERSION;
public ProtocolVersion ProtocolVersion
{
    get
    {
        return _ProtocolVersion;
    }
    set
    {
        _ProtocolVersion = value;
    }
}
TransactionOptions _TransactionSupportedOptions = TransactionOptions.All;
public TransactionOptions TransactionOptions
{
    get
    {
        return _TransactionSupportedOptions;
    }
    set
    {
        _TransactionSupportedOptions = value;
    }
}
public IDisposable ProtocolVersionScope(ProtocolVersion version)
{
    var old = ProtocolVersion;
    return new Scope(() =>
            {
        ProtocolVersion = version;
},
            () =>
            {
                ProtocolVersion = old;
            });
        }
        public void CopyParameters(BitcoinStream stream)
{
    if (stream == null)
        throw new ArgumentNullException("stream");
    ProtocolVersion = stream.ProtocolVersion;
    IsBigEndian = stream.IsBigEndian;
    MaxArraySize = stream.MaxArraySize;
    Type = stream.Type;
}
public SerializationType Type
{
    get;
    set;
}
public IDisposable SerializationTypeScope(SerializationType value)
{
    var old = Type;
    return new Scope(() =>
            {
        Type = value;
}, () =>
            {
                Type = old;
            });
        }
        public System.Threading.CancellationToken ReadCancellationToken
{
    get;
    set;
}
public void ReadWriteAsVarInt(ref uint val)
{
    ulong vallong = val;
    ReadWriteAsVarInt(ref vallong);
    if (!Serializing)
        val = (uint)vallong;
}
public void ReadWriteAsVarInt(ref ulong val)
{
    var value = new VarInt(val);
    ReadWrite(ref value);
    if (!Serializing)
        val = value.ToLong();
}
public void ReadWriteAsCompactVarInt(ref uint val)
{
    var value = new CompactVarInt(val, sizeof(uint));
    ReadWrite(ref value);
    if (!Serializing)
        val = (uint)value.ToLong();
}
public void ReadWriteAsCompactVarInt(ref ulong val)
{
    var value = new CompactVarInt(val, sizeof(ulong));
    ReadWrite(ref value);
    if (!Serializing)
        val = value.ToLong();
}
    }
    public enum SerializationType
{
    Disk,
    Network,
    Hash
}
public class Scope : IDisposable
{
    Action close;
    public Scope(Action open, Action close)
    {
        this.close = close;
        open();
    }
    #region IDisposable Members
    public void Dispose()
    {
        close();
    }
    #endregion
    public static IDisposable Nothing
    {
        get
        {
            return new Scope(() =>
                {
            }, () =>
                {
            });
        }
    }
}
public partial class BitcoinStream
{
    VarInt _VarInt = new VarInt(0);
    private void ReadWriteArray<T>(ref T[] data) where T : IBitcoinSerializable
        {
            if (data == null && Serializing)
                throw new ArgumentNullException("Impossible to serialize a null array");
    _VarInt.SetValue(data == null ? 0 : (ulong) data.Length);
    ReadWrite(ref _VarInt);
            if (_VarInt.ToLong() > (uint) MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
            if (!Serializing)
                data = new T[_VarInt.ToLong()];
            for (int i = 0; i < data.Length; i++)
            {
                T obj = data[i];
    ReadWrite(ref obj);
    data[i] = obj;
            }
        }
        private void ReadWriteArray(ref ulong[] data)
{
    if (data == null && Serializing)
        throw new ArgumentNullException("Impossible to serialize a null array");
    _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
    ReadWrite(ref _VarInt);
    if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
    if (!Serializing)
        data = new ulong[_VarInt.ToLong()];
    for (int i = 0; i < data.Length; i++)
            {
        ulong obj = data[i];
        ReadWrite(ref obj);
        data[i] = obj;
    }
}
private void ReadWriteArray(ref ushort[] data)
{
    if (data == null && Serializing)
        throw new ArgumentNullException("Impossible to serialize a null array");
    _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
    ReadWrite(ref _VarInt);
    if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
    if (!Serializing)
        data = new ushort[_VarInt.ToLong()];
    for (int i = 0; i < data.Length; i++)
            {
        ushort obj = data[i];
        ReadWrite(ref obj);
        data[i] = obj;
    }
}
private void ReadWriteArray(ref uint[] data)
{
    if (data == null && Serializing)
        throw new ArgumentNullException("Impossible to serialize a null array");
    _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
    ReadWrite(ref _VarInt);
    if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
    if (!Serializing)
        data = new uint[_VarInt.ToLong()];
    for (int i = 0; i < data.Length; i++)
            {
        uint obj = data[i];
        ReadWrite(ref obj);
        data[i] = obj;
    }
}
private void ReadWriteArray(ref byte[] data)
{
    if (data == null && Serializing)
        throw new ArgumentNullException("Impossible to serialize a null array");
    _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
    ReadWrite(ref _VarInt);
    if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
    if (!Serializing)
        data = new byte[_VarInt.ToLong()];
    for (int i = 0; i < data.Length; i++)
            {
        byte obj = data[i];
        ReadWrite(ref obj);
        data[i] = obj;
    }
}
private void ReadWriteArray(ref long[] data)
{
    if (data == null && Serializing)
        throw new ArgumentNullException("Impossible to serialize a null array");
    _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
    ReadWrite(ref _VarInt);
    if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
    if (!Serializing)
        data = new long[_VarInt.ToLong()];
    for (int i = 0; i < data.Length; i++)
            {
        long obj = data[i];
        ReadWrite(ref obj);
        data[i] = obj;
    }
}
private void ReadWriteArray(ref short[] data)
{
    if (data == null && Serializing)
        throw new ArgumentNullException("Impossible to serialize a null array");
    _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
    ReadWrite(ref _VarInt);
    if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
    if (!Serializing)
        data = new short[_VarInt.ToLong()];
    for (int i = 0; i < data.Length; i++)
            {
        short obj = data[i];
        ReadWrite(ref obj);
        data[i] = obj;
    }
}
private void ReadWriteArray(ref int[] data)
{
    if (data == null && Serializing)
        throw new ArgumentNullException("Impossible to serialize a null array");
    _VarInt.SetValue(data == null ? 0 : (ulong)data.Length);
    ReadWrite(ref _VarInt);
    if (_VarInt.ToLong() > (uint)MaxArraySize)
                throw new ArgumentOutOfRangeException("Array size not big");
    if (!Serializing)
        data = new int[_VarInt.ToLong()];
    for (int i = 0; i < data.Length; i++)
            {
        int obj = data[i];
        ReadWrite(ref obj);
        data[i] = obj;
    }
}
public void ReadWrite(ref ulong[] data)
{
    ReadWriteArray(ref data);
}
public void ReadWrite(ref ushort[] data)
{
    ReadWriteArray(ref data);
}
public void ReadWrite(ref uint[] data)
{
    ReadWriteArray(ref data);
}
public void ReadWrite(ref long[] data)
{
    ReadWriteArray(ref data);
}
public void ReadWrite(ref short[] data)
{
    ReadWriteArray(ref data);
}
public void ReadWrite(ref int[] data)
{
    ReadWriteArray(ref data);
}

public void ReadWrite(ref ulong data)
{
    ulong l = (ulong)data;
    ReadWriteNumber(ref l, sizeof(ulong));
    if (!Serializing)
        data = (ulong)l;
}
public ulong ReadWrite(ulong data)
{
    ReadWrite(ref data);
    return data;
}
public void ReadWrite(ref ushort data)
{
    ulong l = (ulong)data;
    ReadWriteNumber(ref l, sizeof(ushort));
    if (!Serializing)
        data = (ushort)l;
}
public ushort ReadWrite(ushort data)
{
    ReadWrite(ref data);
    return data;
}
public void ReadWrite(ref uint data)
{
    ulong l = (ulong)data;
    ReadWriteNumber(ref l, sizeof(uint));
    if (!Serializing)
        data = (uint)l;
}
public uint ReadWrite(uint data)
{
    ReadWrite(ref data);
    return data;
}

public void ReadWrite(ref long data)
{
    long l = (long)data;
    ReadWriteNumber(ref l, sizeof(long));
    if (!Serializing)
        data = (long)l;
}
public long ReadWrite(long data)
{
    ReadWrite(ref data);
    return data;
}
public void ReadWrite(ref short data)
{
    long l = (long)data;
    ReadWriteNumber(ref l, sizeof(short));
    if (!Serializing)
        data = (short)l;
}
public short ReadWrite(short data)
{
    ReadWrite(ref data);
    return data;
}
public void ReadWrite(ref int data)
{
    long l = (long)data;
    ReadWriteNumber(ref l, sizeof(int));
    if (!Serializing)
        data = (int)l;
}
public int ReadWrite(int data)
{
    ReadWrite(ref data);
    return data;
}
    }
    public class PerformanceCounter
{
    public PerformanceCounter()
    {
        _Start = DateTime.UtcNow;
    }
    long _WrittenBytes;
    public long WrittenBytes
    {
        get
        {
            return _WrittenBytes;
        }
    }
    public void AddWritten(long count)
    {
        Interlocked.Add(ref _WrittenBytes, count);
    }
    public void AddReaden(long count)
    {
        Interlocked.Add(ref _ReadenBytes, count);
    }
    long _ReadenBytes;
    public long ReadenBytes
    {
        get
        {
            return _ReadenBytes;
        }
    }
    public PerformanceSnapshot Snapshot()
    {
#if !(PORTABLE || NETCORE)
        Thread.MemoryBarrier();
#endif
        var snap = new PerformanceSnapshot(ReadenBytes, WrittenBytes)
        {
            Start = Start,
            Taken = DateTime.UtcNow
        };
        return snap;
    }
    DateTime _Start;
    public DateTime Start
    {
        get
        {
            return _Start;
        }
    }
    public TimeSpan Elapsed
    {
        get
        {
            return DateTime.UtcNow - Start;
        }
    }
    public override string ToString()
    {
        return Snapshot().ToString();
    }
    internal void Add(PerformanceCounter counter)
    {
        AddWritten(counter.WrittenBytes);
        AddReaden(counter.ReadenBytes);
    }
}
public class PerformanceSnapshot
{
    public PerformanceSnapshot(long readen, long written)
    {
        _TotalWrittenBytes = written;
        _TotalReadenBytes = readen;
    }
    private readonly long _TotalWrittenBytes;
    public long TotalWrittenBytes
    {
        get
        {
            return _TotalWrittenBytes;
        }
    }
    long _TotalReadenBytes;
    public long TotalReadenBytes
    {
        get
        {
            return _TotalReadenBytes;
        }
        set
        {
            _TotalReadenBytes = value;
        }
    }
    public TimeSpan Elapsed
    {
        get
        {
            return Taken - Start;
        }
    }
    public ulong ReadenBytesPerSecond
    {
        get
        {
            return (ulong)((double)TotalReadenBytes / Elapsed.TotalSeconds);
        }
    }
    public ulong WrittenBytesPerSecond
    {
        get
        {
            return (ulong)((double)TotalWrittenBytes / Elapsed.TotalSeconds);
        }
    }
    public static PerformanceSnapshot operator -(PerformanceSnapshot end, PerformanceSnapshot start)
    {
        if (end.Start != start.Start)
        {
            throw new InvalidOperationException("Performance snapshot should be taken from the same point of time");
        }
        if (end.Taken < start.Taken)
            {
            throw new InvalidOperationException("The difference of snapshot can't be negative");
        }
        return new PerformanceSnapshot(end.TotalReadenBytes - start.TotalReadenBytes,
                        end.TotalWrittenBytes - start.TotalWrittenBytes)
        {
            Start = start.Taken,
            Taken = end.Taken
        };
    }
    public override string ToString()
    {
        return "Read : " + ToKBSec(ReadenBytesPerSecond) + ", Write : " + ToKBSec(WrittenBytesPerSecond);
    }
    private string ToKBSec(ulong bytesPerSec)
    {
        double speed = ((double)bytesPerSec / 1024.0);
        return speed.ToString("0.00") + " KB/S)";
    }
    public DateTime Start
    {
        get;
        set;
    }
    public DateTime Taken
    {
        get;
        set;
    }
}
public enum ProtocolVersion : uint
{
    PROTOCOL_VERSION = 70012,
    /// <summary>
    /// Initial protocol version, to be increased after version/verack negotiation
    /// </summary>
    INIT_PROTO_VERSION = 209,
    /// <summary>
    /// Disconnect from peers older than this protocol version
    /// </summary>
    MIN_PEER_PROTO_VERSION = 209,
    /// <summary>
    /// nTime field added to CAddress, starting with this version;
    /// if possible, avoid requesting addresses nodes older than this
    /// </summary>
    CADDR_TIME_VERSION = 31402,
    /// <summary>
    /// Only request blocks from nodes outside this range of versions (START)
    /// </summary>
    NOBLKS_VERSION_START = 32000,
    /// <summary>
    /// Only request blocks from nodes outside this range of versions (END)
    /// </summary>
    NOBLKS_VERSION_END = 32400,
    /// <summary>
    /// BIP 0031, pong message, is enabled for all versions AFTER this one
    /// </summary>
    BIP0031_VERSION = 60000,
    /// <summary>
    /// "mempool" command, enhanced "getdata" behavior starts with this version
    /// </summary>
    MEMPOOL_GD_VERSION = 60002,
    /// <summary>
    /// "reject" command
    /// </summary>
    REJECT_VERSION = 70002,
    /// <summary>
    /// ! "filter*" commands are disabled without NODE_BLOOM after and including this version
    /// </summary>
    NO_BLOOM_VERSION = 70011,
    /// <summary>
    /// ! "sendheaders" command and announcing blocks with headers starts with this version
    /// </summary>
    SENDHEADERS_VERSION = 70012,
    /// <summary>
    /// ! Version after which witness support potentially exists
    /// </summary>
    WITNESS_VERSION = 70012,
    /// <summary>
    /// shord-id-based block download starts with this version
    /// </summary>
    SHORT_IDS_BLOCKS_VERSION = 70014
}
public static class NBUtil
{
    public static int ReadEx(this Stream stream, byte[] buffer, int offset, int count, CancellationToken cancellation = default(CancellationToken))
    {
        if (stream == null)
            throw new ArgumentNullException("stream");
        if (buffer == null)
            throw new ArgumentNullException("buffer");
        if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset");
        if (count <= 0 || count > buffer.Length)
                throw new ArgumentOutOfRangeException("count"); //Disallow 0 as a debugging aid.
        if (offset > buffer.Length - count)
                throw new ArgumentOutOfRangeException("count");
        int totalReadCount = 0;
        while (totalReadCount < count)
            {
            cancellation.ThrowIfCancellationRequested();
            int currentReadCount;
            //Big performance problem with BeginRead for other stream types than NetworkStream.
            //Only take the slow path if cancellation is possible.
            //IO interruption not supported in this path.
            currentReadCount = stream.Read(buffer, offset + totalReadCount, count - totalReadCount);
            if (currentReadCount == 0)
                return 0;
            totalReadCount += currentReadCount;
        }
        return totalReadCount;
    }
}
public class VarString : IBitcoinSerializable
{
    public VarString()
    {
    }
    byte[] _Bytes = new byte[0];
    public int Length
    {
        get
        {
            return _Bytes.Length;
        }
    }
    public VarString(byte[] bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException("bytes");
        _Bytes = bytes;
    }
    public byte[] GetString()
    {
        return GetString(false);
    }
    public byte[] GetString(bool @unsafe)
    {
        if (@unsafe)
            return _Bytes;
        return _Bytes.ToArray();
    }
    #region IBitcoinSerializable Members
    public void ReadWrite(BitcoinStream stream)
    {
        var len = new VarInt((ulong)_Bytes.Length);
        stream.ReadWrite(ref len);
        if (!stream.Serializing)
        {
            if (len.ToLong() > (uint)stream.MaxArraySize)
                    throw new ArgumentOutOfRangeException("Array size not big");
            _Bytes = new byte[len.ToLong()];
        }
        stream.ReadWrite(ref _Bytes);
    }
    #endregion
}
}