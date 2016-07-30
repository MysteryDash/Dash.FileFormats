// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System.Diagnostics.Contracts;
using MysteryDash.FileFormats.Utils;

namespace MysteryDash.FileFormats.IdeaFactory.CL3
{
    public abstract class Section
    {
        public MixedString Name { get; set; } = "";
        
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Name.Length <= 0x20);
        }
    }
}