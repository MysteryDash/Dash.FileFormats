// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using System.IO;

namespace Dash.Compression.DXT
{
    public static class DxtDecompressor
    {
        public static byte[] DecompressDxt(byte[] compressed, int width, int height, DxtCompression compression)
        {
            using (var memoryStream = new MemoryStream(compressed))
            {
                return DecompressDxt(memoryStream, width, height, compression);
            }
        }

        public static byte[] DecompressDxt(Stream compressed, int width, int height, DxtCompression compression)
        {
            if (compressed == null) throw new ArgumentNullException(nameof(compressed));
            if (!compressed.CanRead) throw new ArgumentException(nameof(compressed));
            if ((compressed.Length - compressed.Position) * 8 < width * height * 4) throw new ArgumentException($"{nameof(compressed)} does not contain enough data for specified size.", nameof(compressed));
            if (!Enum.IsDefined(typeof(DxtCompression), compression)) throw new ArgumentException("Invalid compression specified.", nameof(compression));

            byte[] image = new byte[height * width * 4];

            using (var reader = new BinaryReader(compressed))
            {
                for (int baseY = 0; baseY < height; baseY += 4)
                {
                    for (int baseX = 0; baseX < width; baseX += 4)
                    {
                        DxtTexel texel;

                        switch (compression)
                        {
                            case DxtCompression.Dxt1:
                                texel = new Dxt1Texel(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt32());
                                break;
                            case DxtCompression.Dxt5:
                                texel = new Dxt5Texel(reader.ReadByte(),
                                    reader.ReadByte(),
                                    (ulong)reader.ReadUInt16() << 0 | (ulong)reader.ReadUInt32() << 16,
                                    reader.ReadUInt16(),
                                    reader.ReadUInt16(),
                                    reader.ReadUInt32());
                                break;
                            default:
                                throw new InvalidOperationException($"Unreachable code reached. {nameof(compression)}'s value ({compression}) is incorrect.");
                        }

                        var currentPixelIndex = 0;
                        for (int y = baseY; y < baseY + 4 && y < height; y++)
                        {
                            for (int x = baseX; x < baseX + 4 && x < width; x++)
                            {
                                int currentLocation = y * width * 4 + x * 4;
                                image[currentLocation + 0] = texel.Pixels[currentPixelIndex].B;
                                image[currentLocation + 1] = texel.Pixels[currentPixelIndex].G;
                                image[currentLocation + 2] = texel.Pixels[currentPixelIndex].R;
                                image[currentLocation + 3] = texel.Pixels[currentPixelIndex].A;
                                currentPixelIndex++;
                            }
                        }
                    }
                }
            }

            return image;
        }
    }
}