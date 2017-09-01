// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;

namespace Dash.Helpers
{
    public static class BitmapArrayTools
    {
        public static byte[] Swap32BppColorChannels(byte[] raw, int first, int second, int third, int fourth)
        {
            if (raw == null) throw new ArgumentNullException(nameof(raw));
            if (raw.Length % 4 != 0) throw new ArgumentException(nameof(raw.Length));
            if (first < 0 || first > 3) throw new ArgumentOutOfRangeException(nameof(first));
            if (second < 0 || second > 3) throw new ArgumentOutOfRangeException(nameof(first));
            if (third < 0 || third > 3) throw new ArgumentOutOfRangeException(nameof(first));
            if (fourth < 0 || fourth > 3) throw new ArgumentOutOfRangeException(nameof(first));

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
            if (raw == null) throw new ArgumentNullException(nameof(raw));
            if (index < 0 || index > 3) throw new ArgumentOutOfRangeException(nameof(index));

            for (int i = index; i < raw.Length; i += 4)
            {
                raw[i] = value;
            }

            return raw;
        }
    }
}
