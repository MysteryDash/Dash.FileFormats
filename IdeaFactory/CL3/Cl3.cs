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

namespace MysteryDash.FileFormats.IdeaFactory.CL3
{
    public class Cl3
    {
        private static readonly byte[] Magic = {0x43, 0x4C, 0x33};

        public bool IsLittleEndian { get; set; }
        public ContentType ContentType { get; set; }
        public List<Section> Sections { get; set; }

        public Cl3(bool isLittleEndian, ContentType contentType, List<Section> sections)
        {
            IsLittleEndian = isLittleEndian;
            ContentType = contentType;
            Sections = sections;
        }

        public static Cl3 FromFile(string path)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path));
            Contract.Requires<FileNotFoundException>(File.Exists(path));

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return FromStream(stream);
            }
        }

        public static Cl3 FromBytes(byte[] data)
        {
            Contract.Requires<ArgumentNullException>(data != null);

            using (var stream = new MemoryStream(data))
            {
                return FromStream(stream);
            }
        }

        public static Cl3 FromStream(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires(stream.CanRead);
            Contract.Requires(stream.CanSeek);
            Contract.Requires(stream.Length >= 0x18); // Header Size, Minimum Required
            Contract.Ensures(Contract.Result<Cl3>() != null);
            
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

                return new Cl3(reader.IsLittleEndian, contentType, sections);
            }
        }

        private static Section ReadSection(EndianBinaryReader reader, uint offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            var name = reader.ReadBytes(0x20);
            var count = reader.ReadInt32();
            var dataSize = reader.ReadInt32();
            var dataOffset = reader.ReadInt32();

            string realName = Encoding.UTF8.GetString(name.TakeWhile(b => b != '\0').ToArray());
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

        public void ToStream(Stream stream, int offsetAlignment = 0x40)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentException>(stream.CanWrite);
            Contract.Requires<ArgumentException>(stream.CanSeek);
            Contract.Requires<DivideByZeroException>(offsetAlignment > 0);
            Contract.Requires(Sections.Count(section => section?.Name == "FILE_COLLECTION") == 1, "You must have one FILE_COLLECTION section.");
            Contract.Requires(Sections.Count(section => section?.Name == "FILE_LINK") == 1, "You must have one FILE_LINK section.");
            
            using (var writer = new EndianBinaryWriter(stream, new UTF8Encoding(false, true), true, IsLittleEndian))
            {
                int headerSize = 0x18;
                int startSectionsOffset = ((headerSize + offsetAlignment - 1) / offsetAlignment) * offsetAlignment;
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
                        int startFileEntriesOffset = ((endSectionsOffset + totalDataWritten + offsetAlignment - 1) / offsetAlignment) * offsetAlignment;
                        int fileEntriesLength = fileEntries.Count * 0x230;
                        int endFileEntriesOffset = startFileEntriesOffset + fileEntriesLength;

                        int baseDataWritten = totalDataWritten;
                        for (int j = 0; j < fileEntries.Count; j++)
                        {
                            int fileOffset = ((endFileEntriesOffset + totalDataWritten + offsetAlignment - 1) / offsetAlignment) * offsetAlignment;
                            totalDataWritten += fileEntries[j].File.Length;

                            stream.Seek(startFileEntriesOffset + 0x230 * j, SeekOrigin.Begin);
                            writer.Write(fileEntries[j].NameBytes);
                            writer.Write(j);
                            writer.Write(fileOffset - startFileEntriesOffset);
                            writer.Write(fileEntries[j].File.Length);
                            writer.Write(fileEntries[j].LinkStartIndex);
                            writer.Write(fileEntries[j].LinkCount);

                            stream.Seek(fileOffset, SeekOrigin.Begin);
                            writer.Write(fileEntries[j].File);
                        }

                        totalDataWritten += fileEntriesLength;

                        stream.Seek(startSectionOffset, SeekOrigin.Begin);
                        writer.Write(Sections[i].NameBytes);
                        writer.Write(fileEntries.Count);
                        writer.Write(totalDataWritten - baseDataWritten);
                        writer.Write(startFileEntriesOffset);
                    }
                    else if (Sections[i].Name == "FILE_LINK")
                    {
                        var fileLinks = (Section<FileLink>)Sections[i];
                        var linkEntries = fileLinks.Entries;
                        int startLinkEntriesOffset = ((endSectionsOffset + totalDataWritten + offsetAlignment - 1) / offsetAlignment) * offsetAlignment;

                        stream.Seek(startSectionOffset, SeekOrigin.Begin);
                        writer.Write(fileLinks.NameBytes);
                        writer.Write(linkEntries.Count);
                        writer.Write(linkEntries.Count * 0x20);
                        writer.Write(startLinkEntriesOffset);

                        for (int j = 0; j < linkEntries.Count; j++)
                        {
                            stream.Seek(startLinkEntriesOffset + 0x20 * j + 0x04, SeekOrigin.Begin);
                            writer.Write(linkEntries[j].LinkedFiledId);
                            writer.Write(linkEntries[j].LinkId);
                        }
                        
                        totalDataWritten += linkEntries.Count * 0x20;
                    }
                    else if (Sections[i] is UnknownSection)
                    {
                        var section = (UnknownSection)Sections[i];
                        int startDataOffset = ((endSectionsOffset + totalDataWritten + offsetAlignment - 1) / offsetAlignment) * offsetAlignment;

                        stream.Seek(startSectionOffset, SeekOrigin.Begin);
                        writer.Write(section.NameBytes);
                        writer.Write(section.Count);
                        writer.Write(section.Data.Length);
                        writer.Write(startDataOffset);

                        stream.Seek(startDataOffset, SeekOrigin.Begin);
                        writer.Write(section.Data);

                        totalDataWritten += section.Data.Length;
                    }

                    totalDataWritten = ((totalDataWritten + offsetAlignment - 1) / offsetAlignment) * offsetAlignment; // Realign total data written with offset alignment
                }

                // Align file size
                if (stream.Length % offsetAlignment != 0)
                {
                    stream.Seek(((stream.Length + offsetAlignment - 1) / offsetAlignment) * offsetAlignment - 1, SeekOrigin.Begin);
                    stream.WriteByte(0x00);
                }
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Sections != null);
        }
    }
}
