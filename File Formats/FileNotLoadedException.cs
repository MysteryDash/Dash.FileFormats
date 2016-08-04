// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

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