using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Utilities
{
    public static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] src, int Offset)
        {
            return src.SubArray(Offset, src.Length - Offset);
        }

        public static T[] SubArray<T>(this T[] src, int Offset, int Count)
        {
            T[] result = new T[Count];
            Buffer.BlockCopy(src, Offset, result, 0, Count);
            return result;
        }

    }
}
