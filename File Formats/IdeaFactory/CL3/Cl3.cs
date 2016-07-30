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
using MysteryDash.FileFormats.IO;
using MysteryDash.FileFormats.Utils;

namespace MysteryDash.FileFormats.IdeaFactory.CL3
{
    public class Cl3 : IFileFormat, IArchive
    {
        private static readonly byte[] Magic = { 0x43, 0x4C, 0x33 };

        public bool Loaded { get; private set; }
        public bool IsLittleEndian { get; set; }
        public ContentType ContentType { get; set; }
        public List<Section> Sections { get; set; }

        public Cl3()
        {
            Sections = new List<Section>();
        }

        public Cl3(bool isLittleEndian, ContentType contentType, List<Section> sections)
        {
            IsLittleEndian = isLittleEndian;
            ContentType = contentType;
            Sections = sections;

            Loaded = true;
        }

        public void LoadFile(string path)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path));
            Contract.Requires<FileNotFoundException>(File.Exists(path));

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                LoadFromStream(stream);
            }
        }

        public void LoadBytes(byte[] data)
        {
            Contract.Requires<ArgumentNullException>(data != null);

            using (var stream = new MemoryStream(data))
            {
                LoadFromStream(stream);
            }
        }

        public void LoadFromStream(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires(stream.CanRead);
            Contract.Requires(stream.CanSeek);
            Contract.Requires(stream.Length - stream.Position >= 0x18); // Header Size, Minimum Required

            using (var reader = new EndianBinaryReader(stream))
            {
                if (!reader.ReadBytes(0x03).SequenceEqual(Magic))
                    throw new InvalidDataException("This isn't a CL3 file.");

                reader.IsLittleEndian = reader.ReadChar() == 'L';

                stream.Seek(0x08, SeekOrigin.Current);
                var sectionsCount = reader.ReadUInt32();
                var sectionsOffset = reader.ReadUInt32();
                var contentType = (ContentType)reader.ReadUInt32();

                var sections = new List<Section>();
                for (uint i = 0; i < sectionsCount; i++)
                    sections.Add(ReadSection(reader, sectionsOffset + i * 0x50));

                IsLittleEndian = reader.IsLittleEndian;
                ContentType = contentType;
                Sections = sections;
            }

            Loaded = true;
        }

        private static Section ReadSection(EndianBinaryReader reader, uint offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            MixedString name = reader.ReadBytes(0x20);
            var count = reader.ReadInt32();
            var dataSize = reader.ReadInt32();
            var dataOffset = reader.ReadInt32();

            string realName = name.ZeroTerminatedString;
            if (realName == "FILE_COLLECTION")
            {
                var fileEntries = new List<FileEntry>();
                for (int i = 0; i < count; i++)
                {
                    reader.BaseStream.Seek(dataOffset + i * 0x230, SeekOrigin.Begin);
                    var fileName = reader.ReadBytes(0x200);
                    var fileId = reader.ReadInt32();
                    var fileOffset = reader.ReadInt32();
                    var fileSize = reader.ReadInt32();
                    var linkIndex = reader.ReadInt32();
                    var linkCount = reader.ReadInt32();

                    reader.BaseStream.Seek(fileOffset + dataOffset, SeekOrigin.Begin);
                    var file = reader.ReadBytes(fileSize);

                    fileEntries.Add(new FileEntry(fileName, file, linkIndex, linkCount));
                }

                return new Section<FileEntry>(name, fileEntries);
            }
            if (realName == "FILE_LINK")
            {
                var fileLinks = new List<FileLink>();
                for (int i = 0; i < count; i++)
                {
                    reader.BaseStream.Seek(dataOffset + i * 0x20 + 0x04, SeekOrigin.Begin);
                    fileLinks.Add(new FileLink(reader.ReadUInt32(), reader.ReadUInt32()));
                }
                return new Section<FileLink>(name, fileLinks);
            }

            reader.BaseStream.Seek(dataOffset, SeekOrigin.Begin);
            return new UnknownSection(name, reader.ReadBytes(dataSize), count);
        }

        public void LoadFolder(string path, bool ignoreFileOnInvalidPath = false)
        {
            path = path.TrimEnd('\\') + '\\';
            var pathLength = path.Length;

            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories).ToArray();
            var relativePaths = files.Select(file => Encoding.UTF8.GetBytes(file.Remove(0, pathLength))).ToArray();

            var section = (Section<FileEntry>)Sections.FirstOrDefault(sec => ((string) sec?.Name).StartsWith("FILE_COLLECTION"));
            if (section == null)
            {
                section = new Section<FileEntry>("FILE_COLLECTION", new List<FileEntry>());
                Sections.Add(section);
            }

            for (int i = 0; i < files.Length; i++)
            {
                if (relativePaths[i].Length > 0x104)
                {
                    if (ignoreFileOnInvalidPath)
                        continue;
                    throw new PathTooLongException();
                }
                
                var file = File.ReadAllBytes(files[i]);
                section.Entries.Add(new FileEntry(relativePaths[i], file, 0, 0));
            }

            if (Sections.FirstOrDefault(sec => ((string)sec?.Name).StartsWith("FILE_LINK")) == null)
            {
                Sections.Add(new Section<FileLink>("FILE_LINK", new List<FileLink>()));
            }
        }

        public void WriteFile(string path)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path));

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
            WriteToStream(stream, 0x40);
        }

        public void WriteToStream(Stream stream, int customOffsetAlignment)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentException>(stream.CanWrite);
            Contract.Requires<ArgumentException>(stream.CanSeek);
            Contract.Requires<DivideByZeroException>(customOffsetAlignment > 0);
            Contract.Requires(Sections.Count(section => ((string)section?.Name).StartsWith("FILE_COLLECTION")) == 1, "You must have one and only one FILE_COLLECTION section.");
            Contract.Requires(Sections.Count(section => ((string)section?.Name).StartsWith("FILE_LINK")) == 1, "You must have one and only one FILE_LINK section.");

            var origin = stream.Position;

            using (var writer = new EndianBinaryWriter(stream, new UTF8Encoding(false, true), true, IsLittleEndian))
            {
                int headerSize = 0x18;
                int startSectionsOffset = ((headerSize + customOffsetAlignment - 1) / customOffsetAlignment) * customOffsetAlignment;
                int endSectionsOffset = startSectionsOffset + Sections.Count * 0x50;

                writer.Write(Magic);
                writer.Write(IsLittleEndian ? 'L' : 'B');
                writer.Write(0x00);
                writer.Write(0x03);
                writer.Write(Sections.Count);
                writer.Write(startSectionsOffset);
                writer.Write((int)ContentType);

                int totalDataWritten = 0;
                for (int i = 0; i < Sections.Count; i++)
                {
                    int startSectionOffset = startSectionsOffset + i * 0x50;

                    if (Sections[i].Name == "FILE_COLLECTION")
                    {
                        var fileCollection = (Section<FileEntry>)Sections[i];
                        var fileEntries = fileCollection.Entries;
                        int startFileEntriesOffset = ((endSectionsOffset + totalDataWritten + customOffsetAlignment - 1) / customOffsetAlignment) * customOffsetAlignment;
                        int fileEntriesLength = fileEntries.Count * 0x230;
                        int endFileEntriesOffset = startFileEntriesOffset + fileEntriesLength;

                        int baseDataWritten = totalDataWritten;
                        for (int j = 0; j < fileEntries.Count; j++)
                        {
                            int fileOffset = ((endFileEntriesOffset + totalDataWritten + customOffsetAlignment - 1) / customOffsetAlignment) * customOffsetAlignment;
                            totalDataWritten += fileEntries[j].File.Length;

                            stream.Seek(startFileEntriesOffset + 0x230 * j + origin, SeekOrigin.Begin);
                            
                            writer.Write(fileEntries[j].Name.GetCustomLength(0x200));
                            writer.Write(j);
                            writer.Write(fileOffset - startFileEntriesOffset);
                            writer.Write(fileEntries[j].File.Length);
                            writer.Write(fileEntries[j].LinkStartIndex);
                            writer.Write(fileEntries[j].LinkCount);

                            stream.Seek(fileOffset + origin, SeekOrigin.Begin);
                            writer.Write(fileEntries[j].File);
                        }

                        totalDataWritten += fileEntriesLength;

                        stream.Seek(startSectionOffset + origin, SeekOrigin.Begin);
                        writer.Write(Sections[i].Name.GetCustomLength(0x20));
                        writer.Write(fileEntries.Count);
                        writer.Write(totalDataWritten - baseDataWritten);
                        writer.Write(startFileEntriesOffset);
                    }
                    else if (Sections[i].Name == "FILE_LINK")
                    {
                        var fileLinks = (Section<FileLink>)Sections[i];
                        var linkEntries = fileLinks.Entries;
                        int startLinkEntriesOffset = ((endSectionsOffset + totalDataWritten + customOffsetAlignment - 1) / customOffsetAlignment) * customOffsetAlignment;

                        stream.Seek(startSectionOffset + origin, SeekOrigin.Begin);
                        writer.Write(fileLinks.Name.GetCustomLength(0x20));
                        writer.Write(linkEntries.Count);
                        writer.Write(linkEntries.Count * 0x20);
                        writer.Write(startLinkEntriesOffset);

                        for (int j = 0; j < linkEntries.Count; j++)
                        {
                            stream.Seek(startLinkEntriesOffset + 0x20 * j + 0x04 + origin, SeekOrigin.Begin);
                            writer.Write(linkEntries[j].LinkedFiledId);
                            writer.Write(linkEntries[j].LinkId);
                        }

                        totalDataWritten += linkEntries.Count * 0x20;
                    }
                    else if (Sections[i] is UnknownSection)
                    {
                        var section = (UnknownSection)Sections[i];
                        int startDataOffset = ((endSectionsOffset + totalDataWritten + customOffsetAlignment - 1) / customOffsetAlignment) * customOffsetAlignment;

                        stream.Seek(startSectionOffset + origin, SeekOrigin.Begin);
                        writer.Write(section.Name.GetCustomLength(0x20));
                        writer.Write(section.Count);
                        writer.Write(section.Data.Length);
                        writer.Write(startDataOffset);

                        stream.Seek(startDataOffset + origin, SeekOrigin.Begin);
                        writer.Write(section.Data);

                        totalDataWritten += section.Data.Length;
                    }

                    totalDataWritten = ((totalDataWritten + customOffsetAlignment - 1) / customOffsetAlignment) * customOffsetAlignment; // Realign total data written with offset alignment
                }

                // Align file size
                if (stream.Length % customOffsetAlignment != 0)
                {
                    stream.Seek(((stream.Length + customOffsetAlignment - 1) / customOffsetAlignment) * customOffsetAlignment - 1 + origin, SeekOrigin.Begin);
                    stream.WriteByte(0x00);
                }
            }
        }

        public void WriteFolder(string path)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path));

            var files = (Section<FileEntry>)Sections.FirstOrDefault(section => ((string) section?.Name).StartsWith("FILE_COLLECTION"));
            if (files == null)
            {
                return;
            }
            foreach (var file in files.Entries)
            {
                var realPath = Path.Combine(path, file.Name.ZeroTerminatedString);
                Directory.CreateDirectory(Path.GetDirectoryName(realPath));
                File.WriteAllBytes(realPath, file.File);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!Loaded || Sections != null);
        }

        public void Dispose()
        {
            Sections = null;
        }
    }
}