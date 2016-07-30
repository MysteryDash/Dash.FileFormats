// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System.Diagnostics.Contracts;
using MysteryDash.FileFormats.Utils;

namespace MysteryDash.FileFormats.IdeaFactory.CL3
{
    public class FileEntry : IEntry
    {
        public MixedString Name { get; set; }
        public byte[] File { get; set; }
        public int LinkStartIndex { get; set; }
        public int LinkCount { get; set; }
        
        public FileEntry(MixedString name, byte[] file, int linkStartIndex, int linkCount)
        {
            Name = name;
            File = file;
            LinkStartIndex = linkStartIndex;
            LinkCount = linkCount;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Name.Length <= 0x200);
            Contract.Invariant(File != null);
            Contract.Invariant(LinkStartIndex >= 0);
            Contract.Invariant(LinkCount >= 0);
        }
    }
}
