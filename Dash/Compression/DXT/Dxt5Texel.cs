// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

namespace Dash.Compression.DXT
{
    internal sealed class Dxt5Texel : DxtTexel
    {
        public Dxt5Texel(byte alpha0, byte alpha1, ulong alphaIndices, ushort packedC0, ushort packedC1, uint colorIndices) : base(packedC0, packedC1, colorIndices)
        {
            SetAlpha(alpha0, alpha1, alphaIndices);
        }

        public override Color[] InterpolateColors(ushort packedC0, ushort packedC1)
        {
            var c0 = Color.FromRgb565(packedC0);
            var c1 = Color.FromRgb565(packedC1);
            var c2 = new Color();
            var c3 = new Color();

            c2.A = 0xFF;
            c2.R = (byte)((2 * c0.R + c1.R) / 3);
            c2.G = (byte)((2 * c0.G + c1.G) / 3);
            c2.B = (byte)((2 * c0.B + c1.B) / 3);

            c3.A = 0xFF;
            c3.R = (byte)((c0.R + 2 * c1.R) / 3);
            c3.G = (byte)((c0.G + 2 * c1.G) / 3);
            c3.B = (byte)((c0.B + 2 * c1.B) / 3);

            return new[] { c0, c1, c2, c3 };
        }

        public void SetAlpha(byte alpha0, byte alpha1, ulong alphaIndices)
        {
            byte[] alphas;

            if (alpha0 > alpha1)
            {
                alphas = new[]
                {
                    alpha0,
                    alpha1,
                    (byte)((6 * alpha0 + 1 * alpha1) / 7),
                    (byte)((5 * alpha0 + 2 * alpha1) / 7),
                    (byte)((4 * alpha0 + 3 * alpha1) / 7),
                    (byte)((3 * alpha0 + 4 * alpha1) / 7),
                    (byte)((2 * alpha0 + 5 * alpha1) / 7),
                    (byte)((1 * alpha0 + 6 * alpha1) / 7)
                };
            }
            else
            {
                alphas = new[]
                {
                    alpha0,
                    alpha1,
                    (byte)((4 * alpha0 + 1 * alpha1) / 5),
                    (byte)((3 * alpha0 + 2 * alpha1) / 5),
                    (byte)((2 * alpha0 + 3 * alpha1) / 5),
                    (byte)((1 * alpha0 + 4 * alpha1) / 5),
                    (byte)0x00,
                    (byte)0xFF
                };
            }

            for (int i = 0; i < 16; i++, alphaIndices >>= 3)
            {
                Pixels[i].A = alphas[alphaIndices & 0b111];
            }
        }
    }
}