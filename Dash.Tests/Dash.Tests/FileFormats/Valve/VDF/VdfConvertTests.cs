// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2017.
//

using System.IO;
using Xunit;

namespace Dash.FileFormats.Valve.VDF
{
    public class VdfConvertTests
    {
        private string BasePath { get; } = @"Samples\VDF\";

        [Theory]
        [InlineData("KSP.acf", true)]
        [InlineData("KSP_OneLine.acf", true)]
        [InlineData("KSP_NoQuotes.acf", true)]
        public void Deserialize_File(string filename, bool shouldWork)
        {
            try
            {
                var path = Path.Combine(BasePath, filename);
                VdfConvert.Deserialize(File.ReadAllText(path));
            }
            catch
            {
                if (shouldWork)
                    throw;
            }
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", true)]
        public void Deserialize_Inline(string data, bool shouldWork)
        {
            try
            {
                VdfConvert.Deserialize(data);
            }
            catch
            {
                if (shouldWork)
                    throw;
            }
        }
    }
}