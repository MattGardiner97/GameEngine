using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using SharpDX.Mathematics.Interop;

namespace GameEngine.Utilities
{    
    public static class ColorExtensions
    {
        public static RawColor4 ToRawColor4(this Color c)
        {
            RawColor4 rc = new RawColor4();
            rc.R = c.R;
            rc.G = c.G;
            rc.B = c.B;
            rc.A = c.A;

            return rc;
        }
    }
    
}
