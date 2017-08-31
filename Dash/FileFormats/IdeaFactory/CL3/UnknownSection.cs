// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using Dash.Helpers;

namespace Dash.FileFormats.IdeaFactory.CL3
{
    public class UnknownSection : Section
    {
        private byte[] _data;
        private int _count;

        public byte[] Data
        {
            get => _data;
            set => _data = value ?? throw new ArgumentNullException(nameof(value));
        }

        public int Count
        {
            get => _count;
            set
            {
                if (value < 0) throw new ArgumentException($"{nameof(value)} must be a positive integer.");
                _count = value;
            }
        }

        public UnknownSection(MixedString name, byte[] data, int count)
        {
            Name = name;
            Data = data;
            Count = count;
        }
    }
}