using System;
using System.Runtime.Serialization;

namespace Dash.Compression
{
    public partial class CompressionException : Exception
    {
        public CompressionException()
        {
        }

        public CompressionException(string message) : base(message)
        {
        }

        public CompressionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CompressionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
