using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Utilities
{
    public static class Math
    {
        public static float Wrap(float min, float max, float value)
        {
            if(value < min)
            {
                if(System.Math.Abs(min- value) < System.Math.Abs(max- value))
                {
                    return max;
                }
            }
            else if(value > max)
            {
                if(System.Math.Abs(max- value) < System.Math.Abs(min- value))
                {
                    return min;
                }
            }
            return value;
        }

    }
}
