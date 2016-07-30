// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System.IO;

namespace MysteryDash.FileFormats
{
    public interface IFileFormat
    {
        bool Loaded { get; }

        void LoadFile(string path);
        void LoadBytes(byte[] data);
        void LoadFromStream(Stream stream);

        void WriteFile(string path);
        byte[] WriteBytes();
        void WriteToStream(Stream stream);
    }
}
