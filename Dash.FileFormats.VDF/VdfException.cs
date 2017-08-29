// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2017.
//

using System;

namespace Dash.FileFormats.VDF
{
    /// <summary>
    /// The exception thrown when an error occurs during VDF serialization or deserialization.
    /// </summary>
    public partial class VdfException : Exception
    {
        public char Character { get; }
        public int Index { get; }

        public VdfException()
        {

        }

        public VdfException(char character) : this(character, -1)
        {

        }

        public VdfException(char character, int index)
        {
            Character = character;
            Index = index;
        }

        public VdfException(string message) : base(message)
        {

        }

        public VdfException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}