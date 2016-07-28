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
