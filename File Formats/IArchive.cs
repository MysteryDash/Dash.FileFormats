// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;

namespace MysteryDash.FileFormats
{
    public interface IArchive : IDisposable
    {
        void WriteFolder(string path);
        void LoadFolder(string path, bool ignoreFileOnInvalidPath = false);
    }
}
