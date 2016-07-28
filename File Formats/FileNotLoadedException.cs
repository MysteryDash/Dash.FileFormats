using System;
using System.Runtime.Serialization;

namespace MysteryDash.FileFormats.IdeaFactory.TID
{
    [Serializable]
    public class FileNotLoadedException : Exception
    {
        public FileNotLoadedException()
        {
        }

        public FileNotLoadedException(string message) : base(message)
        {
        }

        public FileNotLoadedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FileNotLoadedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}