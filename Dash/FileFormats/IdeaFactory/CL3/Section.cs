// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;
using Dash.Helpers;

namespace Dash.FileFormats.IdeaFactory.CL3
{
    public abstract class Section
    {
        private MixedString _name = "";

        public MixedString Name
        {
            get => _name;
            set
            {
                if (value.Length > 0x20) throw new ArgumentException($"{nameof(value.Length)} of {nameof(value)} must be equal or lower than 32 characters.");
                _name = value;
            }
        }
    }
}