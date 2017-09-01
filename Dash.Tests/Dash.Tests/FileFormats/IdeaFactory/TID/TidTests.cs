// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2017.
//

using System;
using System.Drawing;
using System.DrawingCore.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xunit;
using Assert = Xunit.Assert;
using BitmapCore = System.DrawingCore.Bitmap;

namespace Dash.FileFormats.IdeaFactory.TID
{
    public class TidTests
    {
        private string BasePath { get; } = @"Samples\TID\";

        [Theory]
        [InlineData("win.tid", "win.png")]
        public void Load_FromTid(string inputFilename, string modelFilename, double similarityLevel = 1.0)
        {
            var inputPath = Path.Combine(BasePath, inputFilename);
            var modelPath = Path.Combine(BasePath, modelFilename);

            using (var tid = new Tid())
            using (var model = new Bitmap(modelPath))
            {
                tid.LoadFile(inputPath);

                Assert.Equal(tid.Height, model.Height);
                Assert.Equal(tid.Width, model.Width);

                var rectangle = new Rectangle(0, 0, tid.Width, tid.Height);
                var similarity = CompareBitmaps(GetBitmapBytes(tid.Bitmap, rectangle, tid.Bitmap.PixelFormat), GetBitmapBytes(model, rectangle, tid.Bitmap.PixelFormat));

                Assert.True(similarity >= similarityLevel);
            }
        }

        [Theory]
        [InlineData("win.png", "win.tid", CompressionAlgorithm.None, 0x90)]
        [InlineData("win.png", "win.tid", CompressionAlgorithm.Dxt1, 0x90, true)]
        [InlineData("win.png", "win.tid", CompressionAlgorithm.Dxt5, 0x90, true)]
        public void Write_FromPng(string inputFilename, string modelFilename, CompressionAlgorithm compression, byte version, bool failExpected = false)
        {
            var inputPath = Path.Combine(BasePath, inputFilename);
            var modelPath = Path.Combine(BasePath, modelFilename);

            using (var tid = new Tid(new BitmapCore(inputPath), modelFilename, compression, version))
            using (var tidOutput = new MemoryStream())
            using (var model = new Tid())
            using (var modelOutput = new MemoryStream())
            {
                model.LoadFile(modelPath);

                // This is not part of the test.
                model.Filename = tid.Filename;

                model.Compression = compression;
                model.Version = version;

                void WriteStreams()
                {
                    tid.WriteToStream(tidOutput);
                    model.WriteToStream(modelOutput);
                }

                if (failExpected)
                {
                    Assert.Throws<NotImplementedException>((Action)WriteStreams);
                }
                else
                {
                    WriteStreams();
                }

                CollectionAssert.AreEqual(tidOutput.ToArray(), modelOutput.ToArray());
            }
        }

        private byte[] GetBitmapBytes(Bitmap bitmap, Rectangle rectangle, PixelFormat pixelFormat)
        {
            var data = bitmap.LockBits(rectangle,
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                (System.Drawing.Imaging.PixelFormat)pixelFormat);

            var values = new byte[Math.Abs(data.Stride) * data.Height];
            Marshal.Copy(data.Scan0, values, 0, values.Length);
            bitmap.UnlockBits(data);
            return values;
        }

        private byte[] GetBitmapBytes(BitmapCore bitmap, Rectangle rectangle, PixelFormat pixelFormat)
        {
            var data = bitmap.LockBits(new System.DrawingCore.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height),
                ImageLockMode.ReadOnly,
                pixelFormat);

            var values = new byte[Math.Abs(data.Stride) * data.Height];
            Marshal.Copy(data.Scan0, values, 0, values.Length);
            bitmap.UnlockBits(data);
            return values;
        }

        private double CompareBitmaps(byte[] a, byte[] b)
        {
            int equals = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == b[i]) equals++;
            }
            return (double)equals / a.Length;
        }
    }
}