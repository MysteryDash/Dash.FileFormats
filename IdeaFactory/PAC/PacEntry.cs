// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.Diagnostics.Contracts;

namespace MysteryDash.FileFormats.IdeaFactory.PAC
{
    public class PacEntry
    {
        public Pac Archive { get; }
        public byte[] Path { get; set; }
        public byte[] File { get; set; }
        public bool IsCompressed { get; set; }

        public PacEntry(Pac archive, byte[] path, byte[] file, bool isCompressed)
        {
            Contract.Requires<ArgumentNullException>(archive != null);
            Archive = archive;
            Path = path;
            File = file;
            IsCompressed = isCompressed;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Path != null);
            Contract.Invariant(Path.Length == 260);
            Contract.Invariant(File != null);
        }
    }
}