using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace GameEngine
{
    public class Transform : Component
    {
        public Vector3 LocalPosition
        {
            get;
            set;
        }
        public Vector3 WorldPosition
        {
            get
            {
                if(GameObject.Parent == null)
                {
                    return LocalPosition;
                }
                else
                {
                    return GameObject.Parent.Transform.WorldPosition;
                }
            }
        }
        public Vector3 Scale
        {
            get;
            set;
        } = Vector3.One;
        public Vector3 Rotation
        {
            get;
            set;
        }

        public void Translate(Vector3 value)
        {
            LocalPosition += value;
        }

        public void Rotate(Vector3 value)
        {
            Rotation += value;
        }
    }
}
