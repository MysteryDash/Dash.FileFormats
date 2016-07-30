using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using MysteryDash.FileFormats.IdeaFactory.CL3;
using MysteryDash.FileFormats.IdeaFactory.PAC;
using MysteryDash.FileFormats.IdeaFactory.TID;

namespace MysteryDash.FileFormats
{
    public static class Reader
    {
        private static readonly Dictionary<string, Type> Formats = new Dictionary<string, Type>()
        {
            ["CL3"] = typeof(Cl3),
            ["TID"] = typeof(Tid),
            ["DW_PACK\0"] = typeof(Pac)
        }; 

        public static IFileFormat LoadFile(string path)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path));
            Contract.Requires<FileNotFoundException>(File.Exists(path));

            return LoadFromStream(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        public static IFileFormat LoadBytes(byte[] data)
        {
            Contract.Requires<ArgumentNullException>(data != null);

            return LoadFromStream(new MemoryStream(data));
        }

        public static IFileFormat LoadFromStream(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires(stream.CanRead);
            Contract.Requires(stream.CanSeek);

            var type = GuessFileType(stream);
            if (type == null)
            {
                return null;
            }

            var file = (IFileFormat)Activator.CreateInstance(type);
            file.LoadFromStream(stream);
            return file;
        }

        public static Type GuessFileType(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires(stream.CanRead);
            Contract.Requires(stream.CanSeek);

            var startingOffset = stream.Position;

            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var header = new string(reader.ReadChars(8));

                stream.Seek(startingOffset, SeekOrigin.Begin);
                
                foreach (var format in Formats)
                {
                    if (header.StartsWith(format.Key))
                    {
                        return format.Value;
                    }
                }
            }

            return null;
        }
    }
}