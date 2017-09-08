// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2017.
//

// This is a port of rygDXT by Fabian "ryg" Giesen.
// Fabian "ryg" Giesen's Github : https://github.com/rygorous
// Latest rygDXT C implementation can be found here : https://github.com/nothings/stb


using System;
using System.Runtime.InteropServices;

namespace Dash.Compression.DXT
{
    public static class DxtCompressor
    {
        private static readonly byte[] Expand5;
        private static readonly byte[] Expand6;
        private static readonly byte[][] OMatch5;
        private static readonly byte[][] OMatch6;
        private static readonly byte[] QuantRBTab;
        private static readonly byte[] QuantGTab;

        static DxtCompressor()
        {
            Expand5 = new byte[32];
            Expand6 = new byte[64];
            OMatch5 = new byte[256][];
            for (int i = 0; i < 256; i++) OMatch5[i] = new byte[2];
            OMatch6 = new byte[256][];
            for (int i = 0; i < 256; i++) OMatch6[i] = new byte[2];
            QuantRBTab = new byte[256 + 16];
            QuantGTab = new byte[256 + 16];

            for (uint i = 0; i < 32; i++)
                Expand5[i] = (byte)((i << 3) | (i >> 2));

            for (uint i = 0; i < 64; i++)
                Expand6[i] = (byte)((i << 2) | (i >> 4));

            for (int i = 0; i < 256 + 16; i++)
            {
                int v = i - 8 < 0 ? 0 : i - 8 > 255 ? 255 : i - 8;
                QuantRBTab[i] = Expand5[Mul8Bit(v, 31)];
                QuantGTab[i] = Expand6[Mul8Bit(v, 63)];
            }

            var flatOMatch5 = new byte[512];
            PrepareOptTable(flatOMatch5, Expand5);
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    OMatch5[i][j] = flatOMatch5[i * 2 + j];
                }
            }

            var flatOMatch6 = new byte[512];
            PrepareOptTable(flatOMatch6, Expand6);
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    OMatch6[i][j] = flatOMatch6[i * 2 + j];
                }
            }
        }

        private static int Mul8Bit(int a, int b)
        {
            int t = a * b + 128;
            return (t + (t >> 8)) >> 8;
        }

        [StructLayout(LayoutKind.Explicit)]
        private unsafe struct Pixel
        {
            [FieldOffset(0x00)]
            public uint Quad;

            [FieldOffset(0x00)]
            public fixed byte Bytes[4];

            [FieldOffset(0x00)]
            public byte B;

            [FieldOffset(0x01)]
            public byte G;

            [FieldOffset(0x02)]
            public byte R;

            [FieldOffset(0x03)]
            public byte A;

            public void From16Bit(ushort value)
            {
                int rv = (value & 0xF800) >> 11;
                int gv = (value & 0x07E0) >> 5;
                int bv = (value & 0x001F) >> 0;

                A = 0;
                R = Expand5[rv];
                G = Expand6[gv];
                B = Expand5[bv];
            }

            public ushort As16Bit()
            {
                return (ushort)((Mul8Bit(R, 31) << 11) + (Mul8Bit(G, 63) << 5) + Mul8Bit(B, 31));
            }

            public void LerpRGB(ref Pixel p1, ref Pixel p2, int f)
            {
                R = (byte)(p1.R + Mul8Bit(p2.R - p1.R, f));
                G = (byte)(p1.R + Mul8Bit(p2.R - p1.R, f));
                B = (byte)(p1.R + Mul8Bit(p2.R - p1.R, f));
            }

            public byte this[int i]
            {
                get
                {
                    fixed (byte* ptr = Bytes)
                    {
                        return *(ptr + i);
                    }
                }
                set
                {
                    fixed (byte* ptr = Bytes)
                    {
                        *(ptr + i) = value;
                    }
                }
            }
        }

        static void PrepareOptTable(byte[] table, byte[] expand)
        {
            for (int i = 0; i < 256; i++)
            {
                int bestError = 256;

                for (int min = 0; min < expand.Length; min++)
                {
                    for (int max = 0; max < expand.Length; max++)
                    {
                        int mini = expand[min];
                        int maxi = expand[max];

                        int err = Math.Abs(maxi + Mul8Bit(mini - maxi, 0x55) - i);

                        if (err < bestError)
                        {
                            table[i * 2 + 0] = (byte)max;
                            table[i * 2 + 1] = (byte)min;
                            bestError = err;
                        }
                    }
                }
            }
        }

        static void EvalColors(Pixel[] colors, ushort c0, ushort c1)
        {
            colors[0].From16Bit(c0);
            colors[1].From16Bit(c1);
            colors[2].LerpRGB(ref colors[0], ref colors[1], 0x55);
            colors[2].LerpRGB(ref colors[0], ref colors[1], 0xAA);
        }

        static void DitherBlock(Pixel[] destination, Pixel[] block)
        {
            for (int channel = 0; channel < 3; channel++)
            {
                int[] ep1 = new int[4];
                int[] ep2 = new int[4];

                var quant = (channel == 1) ? QuantGTab : QuantRBTab;
                var quantBaseIndex = 8;

                for (int pixelIndex = 0; pixelIndex < 16; pixelIndex += 4)
                {
                    destination[pixelIndex + 0][channel] = quant[quantBaseIndex + block[pixelIndex + 0][channel] + ((3 * ep2[1] + 5 * ep2[0]) >> 4)];
                    ep1[0] = block[pixelIndex + 0][channel] - destination[pixelIndex + 0][channel];

                    destination[pixelIndex + 1][channel] = quant[quantBaseIndex + block[pixelIndex + 1][channel] + ((7 * ep1[0] + 3 * ep2[2] + 5 * ep2[1] + ep2[0]) >> 4)];
                    ep1[1] = block[pixelIndex + 1][channel] - destination[pixelIndex + 1][channel];

                    destination[pixelIndex + 2][channel] = quant[quantBaseIndex + block[pixelIndex + 2][channel] + ((7 * ep1[1] + 3 * ep2[3] + 5 * ep2[2] + ep2[1]) >> 4)];
                    ep1[2] = block[pixelIndex + 2][channel] - destination[pixelIndex + 2][channel];

                    destination[pixelIndex + 3][channel] = quant[quantBaseIndex + block[pixelIndex + 3][channel] + ((7 * ep1[2] + 5 * ep2[3] + ep2[2]) >> 4)];
                    ep1[3] = block[pixelIndex + 3][channel] - destination[pixelIndex + 3][channel];

                    var tmp = ep1;
                    ep1 = ep2;
                    ep2 = tmp;
                }
            }
        }

        static uint MatchColorsBlock(Pixel[] block, Pixel[] colors, bool dither)
        {
            uint mask = 0;
            int dirr = colors[0].R - colors[1].R;
            int dirg = colors[0].G - colors[1].G;
            int dirb = colors[0].B - colors[1].B;

            int[] dots = new int[16];
            for (int i = 0; i < 16; i++)
                dots[i] = block[i].R * dirr + block[i].G * dirg + block[i].B * dirb;

            int[] stops = new int[4];
            for (int i = 0; i < 4; i++)
                stops[i] = colors[i].R * dirr + colors[i].G * dirg + colors[i].B * dirb;

            int c0Point = (stops[1] + stops[3]) >> 1;
            int halfPoint = (stops[3] + stops[2]) >> 1;
            int c3Point = (stops[2] + stops[0]) >> 1;

            if (!dither)
            {
                for (int i = 15; i >= 0; i--)
                {
                    mask <<= 2;
                    int dot = dots[i];

                    if (dot < halfPoint)
                        mask |= unchecked((uint)((dot < c0Point) ? 1 : 3));
                    else
                        mask |= unchecked((uint)((dot < c3Point) ? 2 : 0));
                }
            }
            else
            {
                int[] ep1 = new int[4];
                int[] ep2 = new int[4];

                c0Point <<= 4;
                halfPoint <<= 4;
                c3Point <<= 4;

                int currentDot = 0;

                for (int y = 0; y < 4; y++)
                {
                    int dot, lmask, step;

                    // Pixel 0
                    dot = (dots[currentDot + 0] << 4) + (3 * ep2[1] + 5 * ep2[0]);
                    if (dot < halfPoint)
                        step = (dot < c0Point) ? 1 : 3;
                    else
                        step = (dot < c3Point) ? 2 : 0;

                    ep1[0] = dots[currentDot + 0] - stops[step];
                    lmask = step;

                    // Pixel 1
                    dot = (dots[currentDot + 1] << 4) + (7 * ep1[0] + 3 * ep2[2] + 5 * ep2[1] + ep2[0]);
                    if (dot < halfPoint)
                        step = (dot < c0Point) ? 1 : 3;
                    else
                        step = (dot < c3Point) ? 2 : 0;

                    ep1[1] = dots[currentDot + 1] - stops[step];
                    lmask |= step << 2;

                    // Pixel 2
                    dot = (dots[currentDot + 2] << 4) + (7 * ep1[1] + 3 * ep2[3] + 5 * ep2[2] + ep2[1]);
                    if (dot < halfPoint)
                        step = (dot < c0Point) ? 1 : 3;
                    else
                        step = (dot < c3Point) ? 2 : 0;

                    ep1[2] = dots[currentDot + 2] - stops[step];
                    lmask |= step << 4;

                    // Pixel 3
                    dot = (dots[currentDot + 3] << 4) + (7 * ep1[2] + 5 * ep2[3] + ep2[2]);
                    if (dot < halfPoint)
                        step = (dot < c0Point) ? 1 : 3;
                    else
                        step = (dot < c3Point) ? 2 : 0;

                    ep1[2] = dots[currentDot + 2] - stops[step];
                    lmask |= step << 6;

                    var tmp = ep1;
                    ep1 = ep2;
                    ep2 = tmp;

                    currentDot += 4;

                    mask |= unchecked((uint)lmask << (y * 8));
                }
            }

            return mask;
        }

        static void OptimizeColorsBlock(Pixel[] block, ref ushort max16, ref ushort min16)
        {
            const int nIterPower = 4;

            int[] mu = new int[3], min = new int[3], max = new int[3];

            for (int channel = 0; channel < 3; channel++)
            {
                int muv, minv, maxv;

                muv = minv = maxv = block[0][channel];

                for (int i = 1; i < 16; i++)
                {
                    muv += block[i][channel];
                    minv = Math.Min(minv, block[i][channel]);
                    maxv = Math.Min(maxv, block[i][channel]);
                }

                mu[channel] = (muv + 8) >> 4;
                min[channel] = minv;
                max[channel] = maxv;
            }

            int[] cov = new int[6];

            for (int i = 0; i < 16; i++)
            {
                int r = block[i].R - mu[2];
                int g = block[i].G - mu[1];
                int b = block[i].B - mu[0];

                cov[0] += r * r;
                cov[1] += r * g;
                cov[2] += r * b;
                cov[3] += g * g;
                cov[4] += g * b;
                cov[5] += b * b;
            }

            float[] covf = new float[6];
            float vfr, vfg, vfb;

            for (int i = 0; i < 6; i++)
                covf[i] = cov[i] / 255.0f;

            vfr = max[2] - min[2];
            vfg = max[1] - min[1];
            vfb = max[0] - min[0];

            for (int i = 0; i < nIterPower; i++)
            {
                float r = vfr * covf[0] + vfg * covf[1] + vfb * covf[2];
                float g = vfr * covf[1] + vfg * covf[3] + vfb * covf[4];
                float b = vfr * covf[2] + vfg * covf[4] + vfb * covf[5];

                vfr = r;
                vfg = g;
                vfb = b;
            }

            float magn = Math.Max(Math.Max(Math.Abs(vfr), Math.Abs(vfg)), Math.Abs(vfb));
            int v_r, v_g, v_b;

            if (magn < 4.0f)
            {
                v_r = 148;
                v_g = 300;
                v_b = 58;
            }
            else
            {
                magn = 512.0f / magn;
                v_r = (int)(vfr * magn);
                v_g = (int)(vfg * magn);
                v_b = (int)(vfb * magn);
            }

            int mind = 0x7FFFFFFF, maxd = -0x7FFFFFFF;
            Pixel minp = default(Pixel), maxp = default(Pixel);

            for (int i = 0; i < 16; i++)
            {
                int dot = block[i].R * v_r + block[i].G * v_g + block[i].B * v_b;

                if (dot < mind)
                {
                    mind = dot;
                    minp = block[i];
                }

                if (dot > maxd)
                {
                    maxd = dot;
                    maxp = block[i];
                }
            }

            max16 = maxp.As16Bit();
            min16 = minp.As16Bit();
        }

        static int Sclamp(float y, int p0, int p1)
        {
            int x = (int)y;
            if (x < p0) return p0;
            if (x > p1) return p1;
            return x;
        }

        static bool RefineBlock(Pixel[] block, ref ushort max16, ref ushort min16, uint mask)
        {
            int[] wlTab = { 3, 0, 2, 1 };
            int[] prods = { 0x090000, 0x000900, 0x040102, 0x010402 };

            int akku = 0;
            int at1_r, at1_g, at1_b;
            int at2_r, at2_g, at2_b;
            uint cm = mask;

            at1_r = at1_g = at1_b = 0;
            at2_r = at2_g = at2_b = 0;

            for (int i = 0; i < 16; i++, cm >>= 2)
            {
                int step = (int)(cm & 3);
                int w1 = wlTab[step];
                int r = block[i].R;
                int g = block[i].G;
                int b = block[i].B;

                akku += prods[step];
                at1_r += w1 * r;
                at1_g += w1 * g;
                at1_b += w1 * b;
                at2_r += r;
                at2_g += g;
                at2_b += b;
            }

            at2_r = 3 * at2_r - at1_r;
            at2_g = 3 * at2_g - at1_g;
            at2_b = 3 * at2_b - at1_b;

            int xx = akku >> 16;
            int yy = (akku >> 8) & 0xFF;
            int xy = (akku >> 0) & 0xFF;

            if (yy == 0 || xx == 0 || xx * yy == xy * xy)
                return false;

            float frb = 3.0f * 31.0f / 255.0f / (xx * yy - xy * xy);
            float fg = frb * 63.0f / 31.0f;

            int oldMin = min16;
            int oldMax = max16;

            unchecked
            {
                max16 = (ushort)(Sclamp((at1_r * yy - at2_r * xy) * frb + 0.5f, 0, 31) << 11);
                max16 |= (ushort)(Sclamp((at1_g * yy - at2_g * xy) * fg + 0.5f, 0, 63) << 5);
                max16 |= (ushort)(Sclamp((at1_b * yy - at2_b * xy) * frb + 0.5f, 0, 31) << 0);

                min16 = (ushort)(Sclamp((at2_r * xx - at1_r * xy) * frb + 0.5f, 0, 31) << 11);
                min16 |= (ushort)(Sclamp((at2_g * xx - at1_g * xy) * fg + 0.5f, 0, 63) << 5);
                min16 |= (ushort)(Sclamp((at2_b * xx - at1_b * xy) * frb + 0.5f, 0, 31) << 0);
            }

            return oldMin != min16 || oldMax != max16;
        }

        public static void CompressColorBlock(byte[] dest, int baseDest, uint[] src, int quality)
        {
            var block = new Pixel[16];
            for (int i = 0; i < 16; i++)
                block[i].Quad = src[i];

            var dblock = new Pixel[16];
            var color = new Pixel[4];

            uint min, max;
            min = max = block[0].Quad;

            for (int i = 1; i < 16; i++)
            {
                min = Math.Min(min, block[i].Quad);
                max = Math.Max(max, block[i].Quad);
            }

            ushort min16 = 0, max16 = 0;
            uint mask;

            if (min != max)
            {
                if (quality != 0)
                    DitherBlock(dblock, block);

                OptimizeColorsBlock(quality != 0 ? dblock : block, ref max16, ref min16);
                if (max16 != min16)
                {
                    EvalColors(color, max16, min16);
                    mask = MatchColorsBlock(block, color, quality != 0);
                }
                else
                {
                    mask = 0;
                }

                if (RefineBlock(quality != 0 ? dblock : block, ref max16, ref min16, mask))
                {
                    if (max16 != min16)
                    {
                        EvalColors(color, max16, min16);
                        mask = MatchColorsBlock(block, color, quality != 0);
                    }
                    else
                    {
                        mask = 0;
                    }
                }
            }
            else
            {
                byte r = block[0].R;
                byte g = block[0].G;
                byte b = block[0].B;

                mask = 0xAAAAAAAA;
                max16 = unchecked((ushort)((OMatch5[r][0] << 11) | OMatch6[g][0] << 5 | OMatch5[b][0]));
                min16 = unchecked((ushort)((OMatch5[r][1] << 11) | OMatch6[g][1] << 5 | OMatch5[b][1]));
            }

            if (max16 < min16)
            {
                var tmp = max16;
                max16 = min16;
                min16 = tmp;

                mask ^= 0x55555555;
            }

            unchecked
            {
                dest[baseDest + 0] = (byte)(max16 >> 0);
                dest[baseDest + 1] = (byte)(max16 >> 8);

                dest[baseDest + 2] = (byte)(min16 >> 0);
                dest[baseDest + 3] = (byte)(min16 >> 8);

                dest[baseDest + 4] = (byte)(mask >> 0);
                dest[baseDest + 5] = (byte)(mask >> 8);
                dest[baseDest + 6] = (byte)(mask >> 16);
                dest[baseDest + 7] = (byte)(mask >> 24);
            }
        }

        public static void CompressAlphaBlock(byte[] dest, uint[] src, int quality)
        {
            var block = new Pixel[16];
            for (int i = 0; i < 16; i++)
                block[i].Quad = src[i];

            byte min, max;
            min = max = block[0].A;

            for (int i = 1; i < 16; i++)
            {
                min = Math.Min(min, block[i].A);
                max = Math.Max(max, block[i].A);
            }

            int destPtr = 0;

            dest[destPtr++] = max;
            dest[destPtr++] = min;

            int dist = max - min;
            int bias = min * 7 - (dist >> 1);
            int dist4 = dist * 4;
            int dist2 = dist * 2;
            int bits = 0, mask = 0;

            for (int i = 0; i < 16; i++)
            {
                int a = block[i].A * 7 - bias;
                int ind, t;

                t = (dist4 - a) >> 31;
                ind = t & 4;
                a -= dist4 & t;

                t = (dist2 - a) >> 31;
                ind += t & 2;
                a -= dist2 & t;

                t = (dist - a) >> 31;
                ind += t & 1;

                ind = -ind & 7;
                ind ^= (2 > ind ? 1 : 0);

                mask |= ind << bits;
                if ((bits += 3) >= 8)
                {
                    dest[destPtr++] = unchecked((byte)mask);
                    mask >>= 8;
                    bits -= 8;
                }
            }
        }

        public static void CompressDXTBlock(byte[] dest, uint[] src, bool alpha, int quality)
        {
            int baseDest = 0;
            if (alpha)
            {
                CompressAlphaBlock(dest, src, quality);
                baseDest += 8;
            }
            CompressColorBlock(dest, baseDest, src, quality);
        }
    }
}
