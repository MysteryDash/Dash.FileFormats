// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using Neptoolia.DataLayer;

namespace MysteryDash.FileFormats.IdeaFactory.PAC
{
    /// <summary>
    /// Provides access to pac archives. Currently this class preloads everything in RAM, so be careful with the RAM usage.
    /// </summary>
    public class Pac
    {
        public List<PacEntry> Files { get; set; }
        public bool FullyLoadedInRam => true;

        public Pac()
        {
            Files = new List<PacEntry>();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Files != null);
        }

        public static Pac Load(string path)
        {
            Contract.Requires<FileNotFoundException>(File.Exists(path));

            using (var stream = File.OpenRead(path))
            using (var reader = new BinaryReader(stream))
            {
                if (new string(reader.ReadChars(8)) != "DW_PACK\0")
                    throw new InvalidDataException("This isn't a PAC file.");

                stream.Seek(0x04, SeekOrigin.Current);
                var fileCount = reader.ReadInt32();

                var archive = new Pac();
                var dataOffset = 0x14 + 0x120 * fileCount;

                for (int i = 0; i < fileCount; i++)
                {
                    stream.Seek(0x14 + 0x120 * i + 0x08, SeekOrigin.Begin); // Header + Entries already read + Padding + Id
                    var filePath = reader.ReadBytes(0x104);
                    stream.Seek(0x04, SeekOrigin.Current);
                    var size = reader.ReadInt32();
                    var extractedSize = reader.ReadInt32();
                    var isCompressed = reader.ReadInt32() != 0;
                    var relativedDataOffset = reader.ReadInt32(); // Relative offset to beginning of data

                    stream.Seek(dataOffset + relativedDataOffset, SeekOrigin.Begin);
                    var file = reader.ReadBytes(size);
                    if (i == 0)
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(filePath));
                        Console.WriteLine($"{size} {extractedSize} {dataOffset}");
                        Console.WriteLine(file.Length);
                    }

                    if (isCompressed)
                    {
                        var decompressedFile = new byte[extractedSize];
                        Decompressor.Decompress(file, decompressedFile);
                        file = decompressedFile;
                    }
                    
                    archive.Files.Add(new PacEntry(archive, filePath, file, isCompressed));
                }

                return archive;
            }
        }

        public static Pac LoadFolder(string path, bool compressAll = false, bool ignoreFileOnInvalidPath = false)
        {
            Contract.Requires<DirectoryNotFoundException>(Directory.Exists(path));

            path = path.TrimEnd('\\') + '\\';

            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories).ToArray();
            var relativePaths = files.Select(file => Encoding.UTF8.GetBytes(file.Remove(0, path.Length))).ToArray();

            var archive = new Pac();
            for (int i = 0; i < files.Length; i++)
            {
                if (relativePaths[i].Length > 0x104)
                {
                    if (ignoreFileOnInvalidPath)
                        continue;
                    throw new PathTooLongException();
                }

                byte[] filePath = new byte[0x104];
                relativePaths[i].CopyTo(filePath, 0);

                archive.Files.Add(new PacEntry(archive, filePath, File.ReadAllBytes(files[i]), compressAll));
            }

            return archive;
        }

        public void WriteArchive(string path)
        {
            throw new NotImplementedException();
        }

        public void WriteFolder(string path)
        {
            throw new NotImplementedException();
        }
    }
}
