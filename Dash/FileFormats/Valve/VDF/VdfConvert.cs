// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2017.
//

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Dash.FileFormats.Valve.VDF
{
    /// <summary>
    /// Provides methods for converting between .NET types and VDF types.
    /// </summary>
    public static class VdfConvert
    {
        private static readonly char[] Whitespaces = { ' ', '\r', '\n', '\t' };
        private static readonly Dictionary<char, char> EscapeSequences = new Dictionary<char, char>()
        {
            { 'n', '\n' },
            { 't', '\t' },
            { '"', '"' },
            { '\\', '\\' }
        };

        /// <summary>
        /// Serializes the specified object to a VDF string.
        /// </summary>
        /// <param name="values">The object to serialize.</param>
        /// <returns>A VDF string representation of the object.</returns>
        public static string Serialize(IDictionary<string, object> values)
        {
            return Serialize(values, 0);
        }

        /// <summary>
        /// Serializes the specified object to a VDF string.
        /// </summary>
        /// <param name="values">The object to serialize.</param>
        /// <param name="indentationLevel">The indentation level for the current values.</param>
        /// <returns>A VDF string representation of the object.</returns>
        public static string Serialize(IDictionary<string, object> values, int indentationLevel)
        {
            Contract.Requires<ArgumentNullException>(values != null);
            Contract.Requires<ArgumentException>(indentationLevel >= 0, $"{nameof(indentationLevel)} must be a positive integer.");

            StringBuilder serializedVdf = new StringBuilder();
            var indentation = new string('\t', indentationLevel);

            foreach (var pair in values)
            {
                switch (pair.Value)
                {
                    case IDictionary<string, object> subValues:
                        serializedVdf.AppendLine($"{indentation}\"{pair.Key}\"");
                        serializedVdf.AppendLine($"{indentation}{{");
                        serializedVdf.Append(Serialize(subValues, indentationLevel + 1));
                        serializedVdf.AppendLine($"{indentation}}}");
                        break;
                    default:
                        serializedVdf.AppendLine($"{indentation}\"{pair.Key}\"\t\"{pair.Value}\"");
                        break;
                }
            }

            return serializedVdf.ToString();
        }

        /// <summary>
        /// Deserializes the VDF to a .NET <see cref="ExpandoObject"/>
        /// </summary>
        /// <param name="value">The VDF to deserialize.</param>
        /// <returns>The deserialized <see cref="ExpandoObject"/> from the VDF string.</returns>
        public static ExpandoObject Deserialize(string value)
        {
            Contract.Requires<ArgumentNullException>(value != null);

            IDictionary<string, object> keyValues = new ExpandoObject();

            var states = new Stack<DeserializationStates>();
            states.Push(DeserializationStates.ExpectKey);
            states.Push(DeserializationStates.IgnoreWhitespaces);

            var stack = new Stack<IDictionary<string, object>>();
            stack.Push(keyValues);

            string currentKey = null;
            StringBuilder currentToken = null;
            bool currentTokenQuoted = false;

            foreach (var c in value)
            {
                var currentState = states.Peek();
                var currentKeyValues = stack.Peek();

                switch (currentState)
                {
                    case DeserializationStates.IgnoreWhitespaces:
                        if (c == '/')
                        {
                            states.Push(DeserializationStates.ExpectCommentSequence);
                        }
                        else if (!Whitespaces.Contains(c))
                        {
                            states.Pop();
                        }
                        break;
                    case DeserializationStates.ExpectCommentSequence:
                        if (c == '/')
                        {
                            states.Pop();
                            states.Push(DeserializationStates.ReadSinglelineComment);
                        }
                        else if (c == '*')
                        {
                            states.Pop();
                            states.Push(DeserializationStates.ReadMultilineComment);
                        }
                        else
                        {
                            throw new VdfException.VdfReaderException();
                        }
                        break;
                    case DeserializationStates.ReadSinglelineComment:
                        if (c == '\n')
                        {
                            states.Pop();
                        }
                        break;
                    case DeserializationStates.ReadMultilineComment:
                        if (c == '*')
                        {
                            states.Push(DeserializationStates.ExpectMultilineCommentSequenceEnd);
                        }
                        break;
                    case DeserializationStates.ExpectMultilineCommentSequenceEnd:
                        if (c == '/')
                        {
                            states.Pop();
                            states.Pop();
                        }
                        break;
                }

                currentState = states.Peek();

                switch (currentState)
                {
                    case DeserializationStates.ExpectKey:
                        if (c == '"')
                        {
                            currentToken = new StringBuilder();
                            currentTokenQuoted = true;

                            states.Pop();
                            states.Push(DeserializationStates.ReadKey);
                        }
                        else if (c == '}')
                        {
                            stack.Pop();
                            states.Push(DeserializationStates.IgnoreWhitespaces);
                        }
                        else if (!Whitespaces.Contains(c))
                        {
                            currentToken = new StringBuilder();
                            currentTokenQuoted = false;

                            states.Pop();
                            states.Push(DeserializationStates.ReadKey);
                        }
                        else
                        {
                            throw new VdfException.VdfReaderException();
                        }
                        break;
                    case DeserializationStates.ExpectValue:
                        if (c == '"')
                        {
                            currentToken = new StringBuilder();
                            currentTokenQuoted = true;

                            states.Pop();
                            states.Push(DeserializationStates.ReadValue);
                        }
                        else if (c == '{')
                        {
                            IDictionary<string, object> subKeyValues = new ExpandoObject();
                            currentKeyValues.Add(currentKey, subKeyValues);

                            stack.Push(subKeyValues);

                            states.Pop();
                            states.Push(DeserializationStates.ExpectKey);
                            states.Push(DeserializationStates.IgnoreWhitespaces);
                        }
                        else if (!Whitespaces.Contains(c))
                        {
                            currentToken = new StringBuilder();
                            currentTokenQuoted = false;

                            states.Pop();
                            states.Push(DeserializationStates.ReadValue);
                        }
                        else
                        {
                            throw new VdfException.VdfReaderException();
                        }
                        break;
                    case DeserializationStates.ReadKey when (currentTokenQuoted && c == '"') || (!currentTokenQuoted && Whitespaces.Contains(c)):
                        currentKey = currentToken?.ToString();
                        states.Pop();
                        states.Push(DeserializationStates.ExpectValue);
                        states.Push(DeserializationStates.IgnoreWhitespaces);
                        break;
                    case DeserializationStates.ReadValue when currentTokenQuoted && c == '"' || (!currentTokenQuoted && Whitespaces.Contains(c)):
                        currentKeyValues.Add(currentKey, currentToken?.ToString());
                        states.Pop();
                        states.Push(DeserializationStates.ExpectKey);
                        states.Push(DeserializationStates.IgnoreWhitespaces);
                        break;
                    case DeserializationStates.ReadKey when c == '\\':
                    case DeserializationStates.ReadValue when c == '\\':
                        states.Push(DeserializationStates.ExpectEscapeSequence);
                        break;
                    case DeserializationStates.ReadKey:
                    case DeserializationStates.ReadValue:
                        currentToken?.Append(c);
                        break;
                    case DeserializationStates.ExpectEscapeSequence:
                        if (EscapeSequences.ContainsKey(c))
                        {
                            currentToken?.Append(EscapeSequences[c]);
                            states.Pop();
                        }
                        else
                        {
                            throw new VdfException.VdfReaderException();
                        }
                        break;
                }
            }

            return (ExpandoObject)keyValues;
        }

        private enum DeserializationStates
        {
            ExpectKey,
            ReadKey,
            ExpectValue,
            ReadValue,
            ExpectEscapeSequence,
            IgnoreWhitespaces,
            ExpectCommentSequence,
            ReadSinglelineComment,
            ReadMultilineComment,
            ExpectMultilineCommentSequenceEnd
        }
    }
}
