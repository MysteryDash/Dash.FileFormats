// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

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
