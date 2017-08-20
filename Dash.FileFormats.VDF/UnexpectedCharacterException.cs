using System;

namespace Dash.FileFormats.VDF
{
    public class UnexpectedCharacterException : Exception
    {
        public char Character { get; }
        public int Index { get; }

        public UnexpectedCharacterException()
        {

        }

        public UnexpectedCharacterException(char character) : this(character, -1)
        {
            
        }

        public UnexpectedCharacterException(char character, int index)
        {
            Character = character;
            Index = index;
        }

        public UnexpectedCharacterException(string message) : base(message)
        {

        }

        public UnexpectedCharacterException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}