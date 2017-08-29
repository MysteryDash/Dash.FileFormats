// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.1.0.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System;

namespace Dash.FileFormats.IdeaFactory.CL3
{
    public enum ContentType : uint
    {
        Unknown = 0,
        Script = 1,
        Effect = 2,
        Texture = 3
    }

    public static class ContentTypeExtensions
    {
        public static bool IsUnknown(this ContentType type) => !Enum.IsDefined(typeof(ContentType), type);
    }
}
