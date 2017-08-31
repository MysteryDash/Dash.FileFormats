// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

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
