// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace MysteryDash.FileFormats.IdeaFactory.CL3
{
    public class Section<T> : Section where T : IEntry
    {
        public List<T> Entries { get; set; }

        public Section(byte[] nameBytes, List<T> entries)
        {
            NameBytes = nameBytes;
            Entries = entries;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Entries != null);
        }
    }
}
