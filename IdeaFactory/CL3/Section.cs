// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System.Text;

namespace MysteryDash.FileFormats.IdeaFactory.CL3
{
    public abstract class Section
    {
        public byte[] NameBytes { get; set; }
        public string Name => Encoding.UTF8.GetString(NameBytes.TakeWhile(b => b != '\0').ToArray());

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(NameBytes != null);
            Contract.Invariant(NameBytes.Length == 0x20);
        }
    }
}
