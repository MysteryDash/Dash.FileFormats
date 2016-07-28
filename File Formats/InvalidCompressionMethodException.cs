// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.Runtime.Serialization;

namespace MysteryDash.FileFormats
{
    [Serializable]
    public class InvalidCompressionMethodException : Exception
    {
        public InvalidCompressionMethodException()
        {
        }

        public InvalidCompressionMethodException(string message) : base(message)
        {
        }

        public InvalidCompressionMethodException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidCompressionMethodException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}