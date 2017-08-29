// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

namespace Dash.IO
{
    public class EndianBinaryReader : IDisposable
    {
        public Stream BaseStream { get; private set; }

        private BinaryReader _reader;
        private BinaryReader _littleEndianReader;
        private BinaryReader _bigEndianReader;

        private readonly bool _leaveOpen;
        private bool _disposed = false;
        
        public bool IsLittleEndian
        {
            get { return _reader == _littleEndianReader; }
            set { _reader = value ? _littleEndianReader : _bigEndianReader; }
        }

        public EndianBinaryReader(Stream input) : this(input, new UTF8Encoding(), false, true)
        {
        }

        public EndianBinaryReader(Stream input, Encoding encoding) : this(input, encoding, false, true)
        {
        }

        public EndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : this(input, encoding, leaveOpen, true)
        {
        }

        public EndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen, bool isLittleEndian)
        {
            Contract.Requires<ArgumentNullException>(input != null);
            Contract.Requires<ArgumentException>(input.CanRead);
            Contract.Requires<ArgumentNullException>(encoding != null);

            BaseStream = input;
            _littleEndianReader = new BinaryReader(input, encoding, leaveOpen);
            _bigEndianReader = new BeBinaryReader(input, encoding, leaveOpen);
            _leaveOpen = leaveOpen;
            IsLittleEndian = isLittleEndian;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _littleEndianReader.Dispose();
                _bigEndianReader.Dispose();

                Stream copyOfStream = BaseStream;
                BaseStream = null;
                if (copyOfStream != null && !_leaveOpen)
                    copyOfStream.Close();

                _littleEndianReader = null;
                _bigEndianReader = null;
                _reader = null;
            }
        }

        public int Read()
        {
            return _reader.Read();
        }

        public bool ReadBoolean()
        {
            return _reader.ReadBoolean();
        }
        
        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        public sbyte ReadSByte()
        {
            return _reader.ReadSByte();
        }

        public int Read(byte[] buffer, int index, int count)
        {
            return _reader.Read(buffer, index, count);
        }

        public byte[] ReadBytes(int count)
        {
            return _reader.ReadBytes(count);
        }

        public int PeekChar()
        {
            return _reader.PeekChar();
        }

        public char ReadChar()
        {
            return _reader.ReadChar();
        }

        public int Read(char[] buffer, int index, int count)
        {
            return _reader.Read(buffer, index, count);
        }
        
        public char[] ReadChars(int count)
        {
            return _reader.ReadChars(count);
        }

        public short ReadInt16()
        {
            return _reader.ReadInt16();
        }

        public ushort ReadUInt16()
        {
            return _reader.ReadUInt16();
        }

        public int ReadInt32()
        {
            return _reader.ReadInt32();
        }

        public uint ReadUInt32()
        {
            return _reader.ReadUInt32();
        }

        public long ReadInt64()
        {
            return _reader.ReadInt64();
        }

        public ulong ReadUInt64()
        {
            return _reader.ReadUInt64();
        }
        
        public float ReadSingle()
        {
            return _reader.ReadSingle();
        }

        public double ReadDouble()
        {
            return _reader.ReadDouble();
        }

        public decimal ReadDecimal()
        {
            return _reader.ReadDecimal();
        }

        public string ReadString()
        {
            return _reader.ReadString();
        }
    }
}
