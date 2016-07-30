using System;

namespace MysteryDash.FileFormats
{
    public interface IArchive : IDisposable
    {
        void WriteFolder(string path);
        void LoadFolder(string path, bool ignoreFileOnInvalidPath = false);
    }
}
