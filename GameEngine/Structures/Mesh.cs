using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using SharpDX;

using GameEngine;

namespace GameEngine
{
    public class Mesh
    {

        public Vector4[] Vertices { get; set; }
        public int[] Triangles { get; set; }
        public Transform Transform { get; set; }

        public Vector3 Size { get; private set; }
        public Vector3 Extents { get; private set; }
        public Vector3 OriginNearPoint { get; private set; }
        public Vector3 OriginFarPoint { get; private set; }

        public Mesh()
        {

        }

        public void CalculateBounds()
        {
            float lowX = float.MaxValue;
            float lowY = float.MaxValue;
            float lowZ = float.MaxValue;
            float highX = float.MinValue;
            float highY = float.MinValue;
            float highZ = float.MinValue;

            float shortestDistance = float.MaxValue;
            float farthestDistance = float.MinValue;
            Vector3 shortestPoint = Vector3.Zero;
            Vector3 farthestPoint = Vector3.Zero;

            for (int i = 0; i < Vertices.Length; i++)
            {
                Vector3 v = (Vector3)Vertices[i];

                float distance = MathHelper.CalculateDistance(Transform.WorldPosition, v);

                if(distance < shortestDistance)
                {
                    shortestDistance = distance;
                    shortestPoint = v;
                }
                else if(distance > farthestDistance)
                {
                    farthestDistance = distance;
                    farthestPoint = v;
                }

                if (v.X < lowX)
                    lowX = v.X;
                if (v.X > highX)
                    highX = v.X;
                if (v.Y < lowY)
                    lowY = v.Y;
                if (v.Y > highY)
                    highY = v.Y;
                if (v.Z < lowZ)
                    lowZ = v.Z;
                if (v.Z > highZ)
                    highZ = v.Z;
            }

            Size = new Vector3(Math.Abs(highX - lowX), Math.Abs(highY - lowY), Math.Abs(highZ - lowZ));
            Extents = new Vector3(Size.X / 2, Size.Y / 2, Size.Z / 2);
            OriginNearPoint = shortestPoint;
            OriginFarPoint = farthestPoint;
        }

        #region Statics
        public static Mesh From3DPrimitive(Primitive3D type)
        {
            Mesh m = new Mesh();

            Vector4[] verts = new Vector4[0];
            Vector4[] colors = new Vector4[0];
            int[] tris = new int[0];

            switch (type)
            {
                case Primitive3D.Cube:
                    verts = new Vector4[]
                    {
                        //Front
                        new Vector4(-0.5f,-0.5f,-0.5f,1f), //Bottom left: 0
                        new Vector4(-0.5f,0.5f,-0.5f,1f), //Top left: 1
                        new Vector4(0.5f,-0.5f,-0.5f,1f), //Bottom right: 2
                        new Vector4(0.5f,0.5f,-0.5f,1f), //Top right: 3

                        //Back
                        new Vector4(-0.5f,-0.5f,0.5f,1f),//Bottom left: 4
                        new Vector4(-0.5f,0.5f,0.5f,1f), //Top left: 5
                        new Vector4(0.5f,-0.5f,0.5f,1f), //Bottom right: 6
                        new Vector4(0.5f,0.5f,0.5f,1f) //Top right: 7
                    };

                    colors = new Vector4[]
                    {
                        new Vector4(1,0,0,1),
                        new Vector4(0,1,0,1),
                        new Vector4(0,0,1,1),
                        new Vector4(1,1,0,1),
                        new Vector4(1,0,1,1),
                        new Vector4(0,1,1,0),
                        new Vector4(0,0,0,1),
                        new Vector4(1,1,1,1)
                    };

                    tris = new int[]
                    {
                        //Front face
                        0,1,2,
                        1,3,2,

                        //Back face
                        7,5,6,
                        6,5,4,

                        //Right face
                        2,3,6,
                        3,7,6,

                        //Left face
                        1,0,4,
                        4,5,1,

                        //Top face
                        1,5,7,
                        1,7,3,

                        //Bottom face
                        0,6,4,
                        0,2,6
                    };
                    break;
            }
            m.Vertices = verts;
            m.Triangles = tris;

            return m;

        }
        public static Mesh From2DPrimitive(Primitive2D type)
        {
            Mesh m = new Mesh();

            Vector4[] verts = new Vector4[0];
            int[] tris = new int[0];

            switch (type)
            {
                case Primitive2D.Triangle:
                    verts = new Vector4[]
                    {
                        new Vector4(-0.5f,-0.25f,0f,1f),
                        new Vector4(0f,0.5f,0f,1f),
                        new Vector4(0.5f,-0.25f,0f,1f)
                    };

                    tris = new int[] { 0, 1, 2 };
                    break;
                case Primitive2D.Square:
                    verts = new Vector4[]
                    {
                        new Vector4(-0.5f,-0.5f,0f,1f),
                        new Vector4(-0.5f,0.5f,0f,1f),
                        new Vector4(0.5f,0.5f,0f,1f),
                        new Vector4(0.5f,-0.5f,0f,1f)
                    };
                    tris = new int[] { 0, 1, 2, 0, 2, 3 };
                    break;
                case Primitive2D.Plane:
                    verts = new Vector4[]
                    {
                        new Vector4(-0.5f,0,0.5f,1f),//Far left
                        new Vector4(0.5f,0,0.5f,1f),//Far right
                        new Vector4(0.5f,0,-0.5f,1f),//Near right
                        new Vector4(-0.5f,0,-0.5f,1f)//Near left
                    };
                    tris = new int[] { 0, 2, 3, 0, 1, 2 };
                    break;
            }

            m.Vertices = verts;
            m.Triangles = tris;

            return m;
        }
        #endregion

    }
}
