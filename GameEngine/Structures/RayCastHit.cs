using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace GameEngine.Structures
{
    public class RayCastHit
    {
        public GameObject GameObject { get; private set; }
        public Vector3 Point { get; private set; }

        public RayCastHit(GameObject HitObject,Vector3 hitPoint)
        {
            this.GameObject = HitObject;
            this.Point = hitPoint;
        }

    }
}
