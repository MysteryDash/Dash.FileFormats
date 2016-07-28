using System.Text;

namespace MysteryDash.FileFormats.Utils
{
    public struct MixedString
    {
        private byte[] _dataBytes;
        private string _dataString;
        private readonly Encoding _encoding;

        public MixedString(byte[] value) : this(value, Encoding.UTF8)
        {
        }

        public MixedString(string value) : this(value, Encoding.UTF8)
        {
        }

        public MixedString(byte[] value, Encoding encoding)
        {
            _dataBytes = value;
            _dataString = null;
            _encoding = encoding;
        }

        public MixedString(string value, Encoding encoding)
        {
            _dataBytes = null;
            _dataString = value;
            _encoding = encoding;
        }

        public byte[] GetCustomLength(int length)
        {
            byte[] output = new byte[length];
            byte[] value = this;
            for (int i = 0; i < value.Length && i < length; i++)
            {
                output[i] = value[i];
            }
            return output;
        }

        public static implicit operator MixedString(string value) => new MixedString(value);

        public static implicit operator MixedString(byte[] value) => new MixedString(value);

        public static implicit operator string(MixedString value) => value._dataString ?? (value._dataString = value._encoding.GetString(value._dataBytes));

        public static implicit operator byte[] (MixedString value) => value._dataBytes ?? (value._dataBytes = value._encoding.GetBytes(value._dataString));
    }
}
