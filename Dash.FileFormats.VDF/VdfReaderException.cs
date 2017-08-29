// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2017.
//

using System;

namespace Dash.FileFormats.VDF
{
    public partial class VdfException
    {
        /// <summary>
        /// The exception thrown when an error occurs while reading VDF text.
        /// </summary>
        public class VdfReaderException : VdfException
        {
            public VdfReaderException()
            {
            }

            public VdfReaderException(char character) : base(character)
            {
            }

            public VdfReaderException(string message) : base(message)
            {
            }

            public VdfReaderException(char character, int index) : base(character, index)
            {
            }

            public VdfReaderException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
    }
}