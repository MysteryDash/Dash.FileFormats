// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

namespace MysteryDash.FileFormats.IdeaFactory.CL3
{
    public class FileLink : IEntry
    {
        public uint LinkedFiledId { get; set; }
        public uint LinkId { get; set; }

        public FileLink(uint linkedFileId, uint linkId)
        {
            LinkedFiledId = linkedFileId;
            LinkId = linkId;
        }
    }
}