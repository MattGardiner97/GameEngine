using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using SharpDX;
using GameEngine.Utilities;

namespace GameEngine
{
    public class Transform : Component
    {
        public Vector3 Position
        {
            get;
            set;
        }
        public Vector3 WorldPosition
        {
            get
            {
                if (GameObject.Parent == null)
                {
                    return Position;
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
        public Vector3 Forward
        {
            get
            {
                return MathHelper.CalculateVectorDirection(Rotation);
            }
            set
            {
                throw new NotImplementedException("Coming soon");
            }
        }
        public Vector3 Backward
        {
            get
            {
                return Forward * -1;
            }
            set
            {
                throw new NotImplementedException("Coming soon");
            }
        }
        public Vector3 Right
        {
            get
            {
                return new Vector3(Forward.Z, Position.Y, Forward.X*-1);
            }
        }
        public Vector3 Left
        {
            get
            {
                return -Right;
            }
        }

        public Vector3 Up
        {
            get
            {
                return new Vector3(Forward.X, Forward.Z, Forward.Y * -1);
            }
        }

        public void Translate(Vector3 Value)
        {
            Translate(Value, false);
        }
        public void Translate(Vector3 Value, bool RelativeToSelf)
        {
            if (RelativeToSelf == false)
                Position += Value;
            else
            {
                Position += Value.Z * Forward;
                Position += Value.X * new Vector3(Right.X,0,Right.Z);
                Position += Value.Y * Up;
            }

        }

        public void Rotate(Vector3 value)
        {
            Rotation += value;
        }
    }
}
