// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.Runtime.Serialization;

namespace MysteryDash.FileFormats.Exceptions
{
    [Serializable]
    public class InvalidVersionException : Exception
    {
        public InvalidVersionException()
        {
        }

        public InvalidVersionException(string message) : base(message)
        {
        }

        public InvalidVersionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}