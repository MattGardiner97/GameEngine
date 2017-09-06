using SharpDX;
using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Utilities
{
    public static class MathHelper
    {
        public static float Clamp(float min, float max, float value)
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

        public static Vector3 CalculateVectorDirection(Vector3 Rotation)
        {
            //Y axis rotation clamped from 0 to 360
            float yDeg = Rotation.Y % 360;
            float yRads = (yDeg * (float)Math.PI) / 180f;

            //X axis rotation clamped from 0 to 360
            float xDeg = Rotation.X % 360;
            float xRads = (xDeg * (float)Math.PI) / 180f;

            //We Cosine for the z axis because our rotation is shifted by 90 degrees
            float zResult = (float)Math.Cos(yRads);
            float xResult = (float)Math.Sin(yRads);
            float yResult = (float)Math.Sin(xRads);

            //If X or Z are very close to zero then set them to zero
            if (Math.Abs(xResult) < 1 / 1000000f)
                xResult = 0;
            if (Math.Abs(zResult) < 1 / 1000000f)
                zResult = 0;
            if (Math.Abs(yResult) < 1 / 1000000f)
                yResult = 0;

            return new Vector3(xResult, yResult, zResult);
        }

    }
}
