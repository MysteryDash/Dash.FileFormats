using System;
using System.Runtime.Serialization;

namespace Dash.FileFormats
{
    public class VersionException : FormatException
    {
        public VersionException()
        {
        }

        public VersionException(string message) : base(message)
        {
        }

        public VersionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected VersionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
