// Neptoolia.DataLayer.Decompressor
//
// Copyright(c) 2016 NepIsLife and ps-auxw
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Runtime.InteropServices;

namespace Neptoolia.DataLayer
{
    /// <summary>
    /// Provides a static function to decompress .pac entries.
    /// </summary>
    internal static class Decompressor
    {
        /// <summary>
        /// Decompresses a .pac archive entry.
        /// </summary>
        /// <param name="input">The compressed bytes of the archive entry.</param>
        /// <param name="output">The buffer into which the content should be uncompressed. The size of this buffer can be taken from the header in the pac archive.</param>
        public static void Decompress(byte[] input, byte[] output)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            InflatePacEntry(input, output);
        }

        /// <summary>
        /// Decompresses a .pac archive entry.
        /// </summary>
        /// <param name="input">Bytes that represent the archive entry.</param>
        /// <param name="output">Buffer into which the entry should be inflated to.</param>
        [DllImport("Neptoolia.DataLayer.Compression.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void InflatePacEntry(byte[] input, [Out]byte[] output);
    }
}