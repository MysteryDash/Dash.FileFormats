// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System.IO;
using System.Text;

namespace MysteryDash.FileFormats.IO
{
    public unsafe class BeBinaryWriter : BinaryWriter
    {
        protected readonly byte[] buffer;

        public BeBinaryWriter(Stream s) : this(s, new UTF8Encoding(false, true))
        {
        }

        public BeBinaryWriter(Stream s, Encoding e) : this(s, e, false)
        {
        }

        public BeBinaryWriter(Stream s, Encoding e, bool leaveOpen) : base(s, new UTF8Encoding(false, true), leaveOpen)
        {
            buffer = new byte[16];
        }

        public override void Write(decimal value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteDecimal(p, value);
            OutStream.Write(buffer, 0, 16);
        }

        public override void Write(double value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt64(p, Reinterpret.DoubleAsInt64(value));
            OutStream.Write(buffer, 0, 8);
        }

        public override void Write(float value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteDecimal(p, Reinterpret.FloatAsInt32(value));
            OutStream.Write(buffer, 0, 4);
        }

        public override void Write(int value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt32(p, value);
            OutStream.Write(buffer, 0, 4);
        }

        public override void Write(long value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt64(p, value);
            OutStream.Write(buffer, 0, 8);
        }

        public override void Write(short value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt16(p, value);
            OutStream.Write(buffer, 0, 2);
        }

        public override void Write(uint value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt32(p, (int)value);
            OutStream.Write(buffer, 0, 4);
        }

        public override void Write(ulong value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt64(p, (long)value);
            OutStream.Write(buffer, 0, 8);
        }

        public override void Write(ushort value)
        {
            fixed (byte* p = buffer)
                BigEndian.WriteInt16(p, (short)value);
            OutStream.Write(buffer, 0, 2);
        }
    }
}
