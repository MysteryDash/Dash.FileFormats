// 
// This file is licensed under the terms of the Simple Non Code License (SNCL) 2.0.2.
// The full license text can be found in the file named License.txt.
// Written originally by Alexandre Quoniou in 2016.
//

using System.Runtime.CompilerServices;

namespace MysteryDash.FileFormats.IO
{
    internal static unsafe class Reinterpret
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloatAsInt32(float f)
            => *(int*)&f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Int32AsFloat(int i)
            => *(float*)&i;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long DoubleAsInt64(double d)
            => *(long*)&d;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Int64AsDouble(long l)
            => *(double*)&l;
    }
}
