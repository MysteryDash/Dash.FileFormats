// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.IO;
using System.Text;

namespace Dash.IO
{
    public unsafe class BeBinaryReader : BinaryReader
    {
        protected readonly byte[] buffer;

        public BeBinaryReader(Stream s) : this(s, new UTF8Encoding())
        { }

        public BeBinaryReader(Stream s, Encoding e) : this(s, e, false)
        { }

        public BeBinaryReader(Stream s, Encoding e, bool leaveOpen) : base(s, new UTF8Encoding(), leaveOpen)
        {
            int bufferSize = e.GetMaxByteCount(1);
            if (bufferSize < 16)
                bufferSize = 16;
            buffer = new byte[bufferSize];
        }

        public override decimal ReadDecimal()
        {
            FillBuffer(16);
            fixed (byte* p = buffer)
                return BigEndian.ReadDecimal(p);
        }

        public override double ReadDouble()
        {
            FillBuffer(8);
            fixed (byte* p = buffer)
                return Reinterpret.Int64AsDouble(BigEndian.ReadInt64(p));
        }

        public override short ReadInt16()
        {
            FillBuffer(2);
            fixed (byte* p = buffer)
                return BigEndian.ReadInt16(p);
        }

        public override int ReadInt32()
        {
            FillBuffer(4);
            fixed (byte* p = buffer)
                return BigEndian.ReadInt32(p);
        }

        public override long ReadInt64()
        {
            FillBuffer(8);
            fixed (byte* p = buffer)
                return BigEndian.ReadInt64(p);
        }

        public override float ReadSingle()
        {
            FillBuffer(4);
            fixed (byte* p = buffer)
                return Reinterpret.Int32AsFloat(BigEndian.ReadInt32(p));
        }

        public override ushort ReadUInt16()
        {
            FillBuffer(2);
            fixed (byte* p = buffer)
                return (ushort)BigEndian.ReadInt16(p);
        }

        public override uint ReadUInt32()
        {
            FillBuffer(4);
            fixed (byte* p = buffer)
                return (uint)BigEndian.ReadInt32(p);
        }

        public override ulong ReadUInt64()
        {
            FillBuffer(8);
            fixed (byte* p = buffer)
                return (ulong)BigEndian.ReadInt64(p);
        }

        protected override void FillBuffer(int numBytes)
        {
            if ((uint) numBytes > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(numBytes));

            int n, read = 0;
            do
            {
                n = BaseStream.Read(buffer, read, numBytes - read);
                if (n == 0)
                    throw new EndOfStreamException();
                read += n;
            } while (read < numBytes);
        }
    }
}