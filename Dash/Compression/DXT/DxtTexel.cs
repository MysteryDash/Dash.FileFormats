// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

namespace Dash.Compression.DXT
{
    internal abstract class DxtTexel
    {
        public Color[] Pixels { get; private set; }

        protected DxtTexel(ushort packedC0, ushort packedC1, uint colorIndices)
        {
            var colors = InterpolateColors(packedC0, packedC1);
            SetPixels(colors, colorIndices);
        }

        public abstract Color[] InterpolateColors(ushort packedC0, ushort packedC1);

        public void SetPixels(Color[] colors, uint colorIndices)
        {
            Pixels = new Color[16];
            for (int i = 0; i < 16; i++, colorIndices >>= 2)
            {
                Pixels[i] = colors[colorIndices & 0b11];
            }
        }
    }
}