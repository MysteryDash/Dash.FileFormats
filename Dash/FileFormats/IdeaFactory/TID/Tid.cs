// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Dash.Compression;
using Dash.Helpers;
using Dash.IO;
using ManagedSquish;

namespace Dash.FileFormats.IdeaFactory.TID
{
    /// <summary>
    /// This class provides methods to handle .TID files.
    /// </summary>
    public class Tid
    {
        public static readonly Dictionary<int, CompressionState> Versions = new Dictionary<int, CompressionState>
        {
            {0x80, CompressionState.Both},
            {0x81, CompressionState.CompressedOnly},
            {0x82, CompressionState.UncompressedOnly},
            {0x88, CompressionState.CompressedOnly},
            {0x89, CompressionState.CompressedOnly},
            {0x90, CompressionState.Both},
            {0x91, CompressionState.CompressedOnly},
            {0x92, CompressionState.UncompressedOnly},
            {0x93, CompressionState.UncompressedOnly},
            {0x98, CompressionState.Both},
            {0x99, CompressionState.CompressedOnly},
            {0x9A, CompressionState.UncompressedOnly}
        };
        
        public bool Loaded { get; private set; }
        public Bitmap Bitmap { get; set; }
        public CompressionAlgorithm Compression { get; set; }
        public MixedString Filename { get; set; }
        public int Height => Bitmap.Height;
        public bool IsLittleEndian => (Version & 0x01) == 0;
        public uint UncompressedSize => 0x80 + (uint) Width*(uint) Height*4;
        public byte Version { get; set; }
        public int Width => Bitmap.Width;

        public Tid()
        {
            
        }

        public Tid(Bitmap bitmap, MixedString filename, CompressionAlgorithm compression = CompressionAlgorithm.None, byte version = 0x90)
        {
            Bitmap = bitmap;
            Filename = filename.GetCustomLength(0x20);
            Compression = compression;
            Version = version;
            Loaded = true;
        }
    
        public void LoadFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                LoadFromStream(stream);
            }
        }

        public void LoadBytes(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                LoadFromStream(stream);
            }
        }

        public void LoadFromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead || !stream.CanSeek || stream.Length < 0x80) throw new ArgumentException(nameof(stream)); // Header Size, Minimum Required

            var origin = stream.Position;

            if (stream.ReadByte() != 0x54 || stream.ReadByte() != 0x49 || stream.ReadByte() != 0x44)
                throw new InvalidDataException("This is not a TID file.");

            var version = (byte)stream.ReadByte();
            if (!Versions.ContainsKey(version))
                throw new VersionException($"{version} is not a version that can currently be read.");

            using (var reader = new EndianBinaryReader(stream, Encoding.UTF8, true, (version & 0x01) == 0))
            {
                stream.Seek(0x20 + origin, SeekOrigin.Begin);
                var filename = reader.ReadBytes(32);

                stream.Seek(0x44 + origin, SeekOrigin.Begin);
                var width = reader.ReadInt32();
                var height = reader.ReadInt32();

                stream.Seek(0x58 + origin, SeekOrigin.Begin);
                var dataLength = reader.ReadInt32();

                stream.Seek(0x64 + origin, SeekOrigin.Begin);
                int compressionValue = reader.ReadInt32();
                if (!Enum.IsDefined(typeof (CompressionAlgorithm), compressionValue))
                    throw new CompressionException($"{compressionValue} represents an incorrect or unknown compression algorithm.");
                CompressionAlgorithm compression = (CompressionAlgorithm) compressionValue;

                if (stream.Length < 0x80 + dataLength)
                    throw new EndOfStreamException();

                stream.Seek(0x80 + origin, SeekOrigin.Begin);
                byte[] data;
                if (version == 0x9A)
                {
                    data = reader.ReadBytes(width*height*4);
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
                        data = BitmapArrayTools.Swap32BppColorChannels(data, 3, 2, 1, 0);
                    }
                    else
                    {
                        data = BitmapArrayTools.Swap32BppColorChannels(data, 2, 1, 0, 3);
                    }
                }
                else
                {
                    data = Squish.DecompressImage(data, width, height, compression.ToSquishFlags());
                    data = BitmapArrayTools.Swap32BppColorChannels(data, 2, 1, 0, 3);
                }

                if (compression == CompressionAlgorithm.Dxt1)
                {
                    data = BitmapArrayTools.Fill32BppAlpha(data, 0, 0xFF);
                }

                var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
                bitmap.UnlockBits(bitmapData);

                Bitmap = bitmap;
                Filename = filename;
                Compression = compression;
                Version = version;

                Loaded = true;
            }
        }

        public void WriteFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                WriteToStream(stream);
            }
        }

        public byte[] WriteBytes()
        {
            using (var stream = new MemoryStream())
            {
                WriteToStream(stream);
                return stream.ToArray();
            }
        }

        public void WriteToStream(Stream stream)
        {
            if (!Loaded) throw new Exception($"{nameof(Tid)} is not loaded.");
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite || !stream.CanSeek) throw new ArgumentException(nameof(stream)); // Header Size, Minimum Required

            if (Compression != CompressionAlgorithm.None) throw new NotImplementedException("Compression is not yet supported.");
            if (!(Compression == CompressionAlgorithm.None ? Versions[Version] == CompressionState.Both || Versions[Version] == CompressionState.UncompressedOnly : Versions[Version] == CompressionState.Both || Versions[Version] == CompressionState.CompressedOnly))
                throw new CompressionException("Incorrect algorithm for specified verison.");

            var origin = stream.Position;

            using (var writer = new EndianBinaryWriter(stream, new UTF8Encoding(false, true), false, IsLittleEndian))
            {
                writer.Write('T', 'I', 'D');
                writer.Write(Version);

                stream.Seek(0x04, SeekOrigin.Current);
                writer.Write(0x80);
                writer.Write(0x01);
                writer.Write(0x01);
                writer.Write(0x20);

                stream.Seek(0x20 + origin, SeekOrigin.Begin);
                writer.Write(Filename.GetCustomLength(0x20));
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
                        data = BitmapArrayTools.Swap32BppColorChannels(data, 3, 2, 1, 0);
                    }
                    else
                    {
                        data = BitmapArrayTools.Swap32BppColorChannels(data, 2, 1, 0, 3);
                    }
                }
                else
                {
                    data = BitmapArrayTools.Swap32BppColorChannels(data, 2, 1, 0, 3);
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
                stream.Seek(0x78 + origin, SeekOrigin.Begin);
                writer.Write(0x101);

                stream.Seek(0x80 + origin, SeekOrigin.Begin);
                writer.Write(data);

                int fileSize = (int)stream.Position;
                stream.Seek(0x04 + origin, SeekOrigin.Begin);
                writer.Write(Compression == CompressionAlgorithm.None ? fileSize : 0x80);
            }
        }

        /*
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!Loaded || Bitmap != null);
            Contract.Invariant(!Loaded || Filename.Length <= 0x20);
            Contract.Invariant(!Loaded || Versions.ContainsKey(Version));
        }
        */
    }
}