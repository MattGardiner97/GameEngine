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
        private const float RadianRatio = (float)Math.PI / 180f;
        private const float DegreeRatio = 180f / (float)Math.PI;

        public Matrix TranslationMatrix { get; private set; } = Matrix.Translation(Vector3.Zero);
        public Matrix RotationMatrix
        {
            get
            {
                Vector3 rot = Rotation.ToRadians();
                return Matrix.RotationX(rot.X) * Matrix.RotationY(rot.Y) * Matrix.RotationZ(rot.Z);
            }
        }
        public Matrix ScaleMatrix { get; private set; } = Matrix.Scaling(1);
        public Matrix WorldMatrix { get { return TranslationMatrix * RotationMatrix * ScaleMatrix; } }

        public Vector3 Position
        {
            get { return (Vector3)TranslationMatrix.TranslationVector; }
            set { TranslationMatrix = Matrix.Translation(value); }
        }
        public Vector3 WorldPosition { get { return GameObject.Parent?.Transform.WorldPosition + Position ?? Position; } }
        public Vector3 Scale
        {
            get { return (Vector3)ScaleMatrix.ScaleVector; }
            set { ScaleMatrix = Matrix.Scaling(value); }
        }
        //public Vector3 Rotation
        //{
        //    get
        //    {
        //        return Vector3.Zero;
        //    }
        //    set
        //    {
        //        value = value.ToRadians();
        //        RotationMatrix = Matrix.RotationX(value.X) * Matrix.RotationY(value.Y) * Matrix.RotationZ(value.Z);
        //    }
        //}
        public Vector3 Rotation { get; set; }
        public Vector3 Forward
        {
            get
            {
                Matrix forwardMatrix = Matrix.Translation(Vector3.ForwardLH) * RotationMatrix;
                return this.WorldPosition + forwardMatrix.TranslationVector;
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
                return new Vector3(Forward.Z, Position.Y, Forward.X * -1);
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
                Matrix translateMatrix = Matrix.Translation(Value) * RotationMatrix;
                Position += translateMatrix.TranslationVector;
            }

        }
        public void Rotate(Vector3 value)
        {
            this.Rotation+= value;
        }
    }
}
