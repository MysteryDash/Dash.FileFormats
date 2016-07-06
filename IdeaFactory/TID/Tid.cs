// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ManagedSquish;
using MysteryDash.FileFormats.Exceptions;
using MysteryDash.FileFormats.IO;

namespace MysteryDash.FileFormats.IdeaFactory.TID
{
    /// <summary>
    /// This class provides methods to handle .TID files.
    /// </summary>
    public class Tid
    {
        public static readonly Dictionary<int, CompressionState> Versions = new Dictionary<int, CompressionState>
        {
            { 0x80, CompressionState.Both },
            { 0x81, CompressionState.CompressedOnly },
            { 0x82, CompressionState.UncompressedOnly },
            { 0x88, CompressionState.CompressedOnly },
            { 0x89, CompressionState.CompressedOnly },
            { 0x90, CompressionState.Both },
            { 0x91, CompressionState.CompressedOnly },
            { 0x92, CompressionState.UncompressedOnly },
            { 0x93, CompressionState.UncompressedOnly },
            { 0x98, CompressionState.Both },
            { 0x99, CompressionState.CompressedOnly },
            { 0x9A, CompressionState.UncompressedOnly }
        };

        public Bitmap Bitmap { get; set; }
        public CompressionAlgorithm Compression { get; set; }
        public byte[] Filename { get; set; }
        public string ReadableFilename => Encoding.ASCII.GetString(Filename.TakeWhile(c => c != '\0').ToArray());
        public int Height => Bitmap.Height;
        public bool IsLittleEndian => (Version & 0x01) == 0;
        public uint UncompressedSize => 0x80 + (uint)Width * (uint)Height * 4;
        public byte Version { get; set; }
        public int Width => Bitmap.Width;

        public Tid(Bitmap bitmap, byte[] filename) : this(bitmap, filename, CompressionAlgorithm.None)
        {

        }

        public Tid(Bitmap bitmap, byte[] filename, CompressionAlgorithm compression) : this(bitmap, filename, compression, 0x90)
        {

        }

        public Tid(Bitmap bitmap, byte[] filename, CompressionAlgorithm compression, byte version)
        {
            Bitmap = bitmap;
            Filename = filename;
            Compression = compression;
            Version = version;
        }

        public static Tid FromFile(string path)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path));
            Contract.Requires<FileNotFoundException>(File.Exists(path));

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return FromStream(stream);
            }
        }

        public static Tid FromBytes(byte[] data)
        {
            Contract.Requires<ArgumentNullException>(data != null);

            using (var stream = new MemoryStream(data))
            {
                return FromStream(stream);
            }
        }

        public static Tid FromStream(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires(stream.CanRead);
            Contract.Requires(stream.CanSeek);
            Contract.Requires(stream.Length >= 0x80); // Header Size, Minimum Required
            Contract.Ensures(Contract.Result<Tid>() != null);

            if (stream.ReadByte() != 0x54 || stream.ReadByte() != 0x49 || stream.ReadByte() != 0x44)
                throw new InvalidDataException("This isn't a TID file.");

            var version = (byte)stream.ReadByte();
            if (!Versions.ContainsKey(version))
                throw new InvalidVersionException($"{version} isn't a valid version that can be read.");

            using (var reader = new EndianBinaryReader(stream, Encoding.UTF8, true, (version & 0x01) == 0))
            {
                stream.Seek(0x20, SeekOrigin.Begin);
                var filename = reader.ReadBytes(32);

                stream.Seek(0x44, SeekOrigin.Begin);
                var width = reader.ReadInt32();
                var height = reader.ReadInt32();

                stream.Seek(0x58, SeekOrigin.Begin);
                var dataLength = reader.ReadInt32();

                stream.Seek(0x64, SeekOrigin.Begin);
                int compressionValue = reader.ReadInt32();
                if (!Enum.IsDefined(typeof(CompressionAlgorithm), compressionValue))
                    throw new InvalidCompressionMethodException($"{compressionValue} represents an incorrect or unknown compression algorithm.");
                CompressionAlgorithm compression = (CompressionAlgorithm)compressionValue;

                if (stream.Length < 0x80 + dataLength)
                    throw new EndOfStreamException();

                stream.Seek(0x80, SeekOrigin.Begin);
                byte[] data;
                if (version == 0x9A)
                {
                    data = reader.ReadBytes(width * height * 4);
                }
                else
                {
                    data = reader.ReadBytes(dataLength);
                }

                if (compression == CompressionAlgorithm.None)
                {
                    if (version == 0x9A)
                    {

                    }
                    else if ((version & 0x02) == 0x02)
                    {
                        data = SwapColorChannels(data, 3, 2, 1, 0);
                    }
                    else
                    {
                        data = SwapColorChannels(data, 2, 1, 0, 3);
                    }
                }
                else
                {
                    data = Squish.DecompressImage(data, width, height, compression.ToSquishFlags());
                    data = SwapColorChannels(data, 2, 1, 0, 3);
                }

                if (compression == CompressionAlgorithm.Dxt1)
                {
                    data = FillAlpha(data, 0, 0xFF);
                }

                var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
                bitmap.UnlockBits(bitmapData);

                return new Tid(bitmap, filename, compression, version);
            }
        }

        private static byte[] SwapColorChannels(byte[] raw, int first, int second, int third, int fourth)
        {
            Contract.Requires<ArgumentNullException>(raw != null);
            Contract.Requires<ArgumentException>(raw.Length % 4 == 0);
            Contract.Requires<ArgumentOutOfRangeException>(first >= 0 || first <= 3);
            Contract.Requires<ArgumentOutOfRangeException>(second >= 0 || second <= 3);
            Contract.Requires<ArgumentOutOfRangeException>(third >= 0 || third <= 3);
            Contract.Requires<ArgumentOutOfRangeException>(fourth >= 0 || fourth <= 3);

            var length = raw.Length;
            var output = new byte[length];
            for (int i = 0; i < length; i += 4)
            {
                output[i + 0] = raw[i + first];
                output[i + 1] = raw[i + second];
                output[i + 2] = raw[i + third];
                output[i + 3] = raw[i + fourth];
            }

            return output;
        }

        private static byte[] FillAlpha(byte[] raw, int index, byte value)
        {
            Contract.Requires<ArgumentNullException>(raw != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index <= 3);

            for (int i = 0; i < index; i += 4)
            {
                raw[i] = value;
            }
            return raw;
        }

        public void ToFile(string path)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path));

            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                ToStream(stream);
            }
        }

        public byte[] ToBytes()
        {
            using (var stream = new MemoryStream())
            {
                ToStream(stream);
                return stream.ToArray();
            }
        }

        public void ToStream(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentException>(stream.CanWrite);
            Contract.Requires<ArgumentException>(stream.CanSeek);
            Contract.Requires<InvalidCompressionMethodException>(Compression == CompressionAlgorithm.None, "Compressing is not yet supported !");

            using (var writer = new EndianBinaryWriter(stream, new UTF8Encoding(false, true), false, IsLittleEndian))
            {
                writer.Write('T', 'I', 'D');
                writer.Write(Version);

                stream.Seek(0x04, SeekOrigin.Current);
                writer.Write(0x80);
                writer.Write(0x01);
                writer.Write(0x01);
                writer.Write(0x20);

                stream.Seek(0x20, SeekOrigin.Begin);
                writer.Write(Filename);
                writer.Write(0x60);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(Compression == CompressionAlgorithm.None ? 0x20 : 0x00);

                // TODO : Understand and write correct data for offset 0x50
                writer.Write(0x010001);

                stream.Seek(0x04, SeekOrigin.Current);

                var bitmapData = Bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var data = new byte[Width * Height * 4];
                Marshal.Copy(bitmapData.Scan0, data, 0, bitmapData.Width * bitmapData.Height * 4);
                Bitmap.UnlockBits(bitmapData);

                if (Compression == CompressionAlgorithm.None)
                {
                    if (Version == 0x9A)
                    {

                    }
                    else if ((Version & 0x02) == 0x02)
                    {
                        data = SwapColorChannels(data, 3, 2, 1, 0);
                    }
                    else
                    {
                        data = SwapColorChannels(data, 2, 1, 0, 3);
                    }
                }
                else
                {
                    data = SwapColorChannels(data, 2, 1, 0, 3);
                    data = Squish.CompressImage(data, Width, Height, Compression.ToSquishFlags()); // TODO : This line throws a FatalExecutionEngineError for an unknown reason
                }

                writer.Write(data.Length);
                writer.Write(0x80);
                writer.Write(Compression == CompressionAlgorithm.None ? 0x00 : 0x04);

                if (Compression == CompressionAlgorithm.Dxt1)
                {
                    writer.Write(827611204);
                }
                else if (Compression == CompressionAlgorithm.Dxt5)
                {
                    writer.Write(894720068);
                }
                else
                {
                    writer.Write(0x00);
                }

                // TODO : Understand and write correct data for offsets 0x68 and 0x78
                stream.Seek(0x78, SeekOrigin.Begin);
                writer.Write(0x101);
                
                stream.Seek(0x80, SeekOrigin.Begin);
                writer.Write(data);

                int fileSize = (int)stream.Position;
                stream.Seek(0x04, SeekOrigin.Begin);
                writer.Write(Compression == CompressionAlgorithm.None ? fileSize : 0x80);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Bitmap != null);
            Contract.Invariant(Filename != null);
            Contract.Invariant(Filename.Length == 32);
            Contract.Invariant(Versions.ContainsKey(Version));
            Contract.Invariant(Compression == CompressionAlgorithm.None ? Versions[Version] == CompressionState.Both || Versions[Version] == CompressionState.UncompressedOnly : Versions[Version] == CompressionState.Both || Versions[Version] == CompressionState.CompressedOnly);
        }
    }
}
