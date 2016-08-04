// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.Diagnostics.Contracts;
using System.IO;
using MysteryDash.FileFormats.Utils;
using Neptoolia.DataLayer;

namespace MysteryDash.FileFormats.IdeaFactory.PAC
{
    public class PacEntry : IDisposable
    {
        public Pac Archive { get; }
        public MixedString Path { get; set; }
        public int DecompressedSize { get; private set; }
        public bool CurrentlyCompressed { get; private set; }
        public bool KeepCompressed { get; set; }
        public bool CacheFromStream { get; set; }

        private byte[] _file;
        private Stream _fileStream;
        private int _fileOffset;
        private int _fileSize;

        public byte[] File
        {
            get
            {
                if (_file != null)
                {
                    if (CurrentlyCompressed && !KeepCompressed)
                    {
                        var decompressedFile = new byte[DecompressedSize];
                        Decompressor.Decompress(_file, decompressedFile);
                        _file = decompressedFile;
                        CurrentlyCompressed = false;
                    }
                    return _file;
                }

                _fileStream.Seek(_fileOffset, SeekOrigin.Begin);

                byte[] buffer = new byte[_fileSize];
                int read = 0;
                while ((read += _fileStream.Read(buffer, read, _fileSize - read)) < _fileSize) { }

                if (CurrentlyCompressed && !KeepCompressed)
                {
                    byte[] file = new byte[DecompressedSize];
                    Decompressor.Decompress(buffer, file);
                    buffer = file;
                }

                if (CacheFromStream)
                {
                    _file = buffer;
                    CurrentlyCompressed = false;
                    _fileStream = null;
                }

                return buffer;
            }
        }
        
        public void SetFile(bool compressed, int decompressedSize, byte[] file, bool keepCompressed)
        {
            Contract.Requires(file != null);
            Contract.Requires(compressed || decompressedSize == file.Length, "decompressedSize and file.Length must be the same if the file isn't compressed.");
            Contract.Requires(decompressedSize >= 0);

            CurrentlyCompressed = compressed;
            DecompressedSize = decompressedSize;
            _file = file;
            KeepCompressed = keepCompressed;
        }

        public void SetFile(bool compressed, int decompressedSize, Stream fileStream, int fileOffset, int fileSize, bool cacheFromStream, bool keepCompressed)
        {
            Contract.Requires(compressed || decompressedSize == fileSize, "decompressedSize and fileSize must be the same if the file isn\'t compressed.");
            Contract.Requires(decompressedSize >= 0);
            Contract.Requires(fileStream != null);
            Contract.Requires(fileOffset >= 0);
            Contract.Requires(fileSize >= 0);

            CurrentlyCompressed = compressed;
            DecompressedSize = decompressedSize;
            _fileStream = fileStream;
            _fileOffset = fileOffset;
            _fileSize = fileSize;
            CacheFromStream = cacheFromStream;
            KeepCompressed = keepCompressed;
        }

        public PacEntry(Pac archive, MixedString path, bool compressed, int decompressedSize, byte[] file, bool keepCompressed)
        {
            Contract.Requires<ArgumentNullException>(archive != null);

            Archive = archive;
            Path = path;
            SetFile(compressed, decompressedSize, file, keepCompressed);
        }

        public PacEntry(Pac archive, MixedString path, bool compressed, int decompressedSize, Stream fileStream, int fileOffset, int fileSize, bool cacheFromStream, bool keepCompressed)
        {
            Contract.Requires<ArgumentNullException>(archive != null);

            Archive = archive;
            Path = path;
            SetFile(compressed, decompressedSize, fileStream, fileOffset, fileSize, cacheFromStream, keepCompressed);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Path.Length <= 0x104);
        }

        public void Dispose()
        {
            Path = string.Empty;
            _file = null;
            _fileStream = null;
        }
    }
}