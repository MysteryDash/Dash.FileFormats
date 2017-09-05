// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

namespace Dash.Compression.DXT
{
    internal sealed class Dxt1Texel : DxtTexel
    {
        public Dxt1Texel(ushort packedC0, ushort packedC1, uint colorIndices) : base(packedC0, packedC1, colorIndices)
        {

        }

        public override Color[] InterpolateColors(ushort packedC0, ushort packedC1)
        {
            var c0 = Color.FromRgb565(packedC0);
            var c1 = Color.FromRgb565(packedC1);
            var c2 = new Color();
            var c3 = new Color();

            if (packedC0 > packedC1)
            {
                c2.A = 0xFF;
                c2.R = (byte)((2 * c0.R + c1.R) / 3);
                c2.G = (byte)((2 * c0.G + c1.G) / 3);
                c2.B = (byte)((2 * c0.B + c1.B) / 3);

                c3.A = 0xFF;
                c3.R = (byte)((c0.R + 2 * c1.R) / 3);
                c3.G = (byte)((c0.G + 2 * c1.G) / 3);
                c3.B = (byte)((c0.B + 2 * c1.B) / 3);
            }
            else
            {
                c2.A = 0xFF;
                c2.R = (byte)((c0.R + c1.R) / 2);
                c2.G = (byte)((c0.G + c1.G) / 2);
                c2.B = (byte)((c0.B + c1.B) / 2);

                c3.Quad = 0x00000000;
            }

            return new[] { c0, c1, c2, c3 };
        }
    }
}