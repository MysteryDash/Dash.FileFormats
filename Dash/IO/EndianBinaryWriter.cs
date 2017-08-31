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
    public class EndianBinaryWriter : IDisposable
    {
        public Stream BaseStream { get; private set; }

        private BinaryWriter _writer;
        private BinaryWriter _littleEndianWriter;
        private BinaryWriter _bigEndianWriter;

        private readonly bool _leaveOpen;
        private bool _disposed = false;

        public bool IsLittleEndian
        {
            get => _writer == _littleEndianWriter;
            set => _writer = value ? _littleEndianWriter : _bigEndianWriter;
        }

        public EndianBinaryWriter(Stream input) : this(input, new UTF8Encoding(false, true), false, true)
        {
        }

        public EndianBinaryWriter(Stream input, Encoding encoding) : this(input, encoding, false, true)
        {
        }

        public EndianBinaryWriter(Stream input, Encoding encoding, bool leaveOpen) : this(input, encoding, leaveOpen, true)
        {
        }

        public EndianBinaryWriter(Stream input, Encoding encoding, bool leaveOpen, bool isLittleEndian)
        {
            Contract.Requires<ArgumentNullException>(input != null);
            Contract.Requires<ArgumentException>(input.CanSeek);
            Contract.Requires<ArgumentNullException>(encoding != null);

            BaseStream = input;
            _littleEndianWriter = new BinaryWriter(input, encoding, leaveOpen);
            _bigEndianWriter = new BeBinaryWriter(input, encoding, leaveOpen);
            _leaveOpen = leaveOpen;
            IsLittleEndian = isLittleEndian;
        }

        public void Dispose()
        {
            Contract.Assume(_littleEndianWriter != null);
            Contract.Assume(_bigEndianWriter != null);

            if (!_disposed)
            {
                _littleEndianWriter.Dispose();
                _bigEndianWriter.Dispose();

                Stream copyOfStream = BaseStream;
                BaseStream = null;
                if (copyOfStream != null && !_leaveOpen)
                    copyOfStream.Close();

                _littleEndianWriter = null;
                _bigEndianWriter = null;
                _writer = null;
            }
        }
        
        public void Write(bool value)
        {
            _writer.Write(value);
        }

        public void Write(byte value)
        {
            _writer.Write(value);
        }

        public void Write(params byte[] buffer)
        {
            _writer.Write(buffer);
        }

        public void Write(byte[] buffer, int index, int count)
        {
            _writer.Write(buffer, index, count);
        }

        public void Write(sbyte value)
        {
            _writer.Write(value);
        }

        public void Write(char value)
        {
            _writer.Write(value);
        }

        public void Write(params char[] chars)
        {
            _writer.Write(chars);
        }

        public void Write(char[] chars, int index, int count)
        {
            _writer.Write(chars, index, count);
        }

        public void Write(short value)
        {
            _writer.Write(value);
        }

        public void Write(ushort value)
        {
            _writer.Write(value);
        }

        public void Write(int value)
        {
            _writer.Write(value);
        }

        public void Write(uint value)
        {
            _writer.Write(value);
        }

        public void Write(long value)
        {
            _writer.Write(value);
        }

        public void Write(ulong value)
        {
            _writer.Write(value);
        }

        public void Write(decimal value)
        {
            _writer.Write(value);
        }

        public void Write(double value)
        {
            _writer.Write(value);
        }

        public void Write(float value)
        {
            _writer.Write(value);
        }

        public void Write(string value)
        {
            _writer.Write(value);
        }
    }
}
