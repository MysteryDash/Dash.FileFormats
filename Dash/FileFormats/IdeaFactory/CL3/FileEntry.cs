// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using Dash.Helpers;

namespace Dash.FileFormats.IdeaFactory.CL3
{
    public class FileEntry : IEntry
    {
        private MixedString _name;
        private byte[] _file;
        private int _linkStartIndex;
        private int _linkCount;

        public MixedString Name
        {
            get => _name;
            set
            {
                if (value.Length > 0x200) throw new ArgumentException($"{nameof(value.Length)} of {nameof(value)} must be equal or lower than 512 characters.");
                _name = value;
            }
        }

        public byte[] File
        {
            get => _file;
            set => _file = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int LinkStartIndex
        {
            get => _linkStartIndex;
            set
            {
                if (value < 0) throw new ArgumentException($"{nameof(value)} must be a positive integer.");
                _linkStartIndex = value;
            }
        }

        public int LinkCount
        {
            get => _linkCount;
            set
            {
                if (value < 0) throw new ArgumentException($"{nameof(value)} must be a positive integer.");
                _linkCount = value;
            }
        }

        public FileEntry(MixedString name, byte[] file, int linkStartIndex, int linkCount)
        {
            Name = name;
            File = file;
            LinkStartIndex = linkStartIndex;
            LinkCount = linkCount;
        }
    }
}
