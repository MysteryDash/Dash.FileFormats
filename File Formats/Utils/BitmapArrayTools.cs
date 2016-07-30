// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.Diagnostics.Contracts;

namespace MysteryDash.FileFormats.Utils
{
    public static class BitmapArrayTools
    {
        public static byte[] Swap32BppColorChannels(byte[] raw, int first, int second, int third, int fourth)
        {
            Contract.Requires<ArgumentNullException>(raw != null);
            Contract.Requires<ArgumentException>(raw.Length % 4 == 0);
            Contract.Requires<ArgumentOutOfRangeException>(first >= 0 || first <= 3);
            Contract.Requires<ArgumentOutOfRangeException>(second >= 0 || second <= 3);
            Contract.Requires<ArgumentOutOfRangeException>(third >= 0 || third <= 3);
            Contract.Requires<ArgumentOutOfRangeException>(fourth >= 0 || fourth <= 3);

            var length = raw.Length;
            var output = new byte[length];
            for (int i = 0; i < length; i += 4)
            {
                output[i + 0] = raw[i + first];
                output[i + 1] = raw[i + second];
                output[i + 2] = raw[i + third];
                output[i + 3] = raw[i + fourth];
            }

            return output;
        }

        public static byte[] Fill32BppAlpha(byte[] raw, int index, byte value)
        {
            Contract.Requires<ArgumentNullException>(raw != null);
            Contract.Requires<ArgumentOutOfRangeException>(index >= 0 && index <= 3);

            for (int i = 0; i < index; i += 4)
            {
                raw[i] = value;
            }
            return raw;
        }
    }
}
