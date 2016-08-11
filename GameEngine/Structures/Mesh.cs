using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

using GameEngine;

namespace GameEngine
{
    public class Mesh
    {
        public Vector3[] Vertices
        {
            get;set;
        }
        public int[] Triangles
        {
            get;set;
        }
        public Vector2[] UV
        {
            get;set;
        }
        public Transform Transform
        {
            get;set;
        }

        public Material Material
        {
            get;set;
        }

        public Mesh()
        {

        }

        public static Mesh From3DPrimitive(Primitive3D type)
        {
            Mesh m = new Mesh();

            Vector3[] verts = new Vector3[0];
            int[] tris = new int[0];

            switch(type)
            {
                case Primitive3D.Cube:
                    verts = new Vector3[]
                    {
                        //Front
                        new Vector3(-0.5f,-0.5f,-0.5f), //Bottom left: 0
                        new Vector3(-0.5f,0.5f,-0.5f), //Top left: 1
                        new Vector3(0.5f,-0.5f,-0.5f), //Bottom right: 2
                        new Vector3(0.5f,0.5f,-0.5f), //Top right: 3

                        //Back
                        new Vector3(-0.5f,-0.5f,0.5f), //Bottom left: 4
                        new Vector3(-0.5f,0.5f,0.5f), //Top left: 5
                        new Vector3(0.5f,-0.5f,0.5f), //Bottom right: 6
                        new Vector3(0.5f,0.5f,0.5f), //Top right: 7
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

            Vector3[] verts = new Vector3[0];
            int[] tris = new int[0];

            switch (type)
            {
                case Primitive2D.Triangle:
                    verts = new Vector3[]
                    {
                        new Vector3(-0.5f,-0.25f,0f),
                        new Vector3(0f,0.5f,0f),
                        new Vector3(0.5f,-0.25f,0f)
                    };

                    tris = new int[] { 0, 1, 2 };
                    break;
                case Primitive2D.Square:
                    verts = new Vector3[]
                    {
                        new Vector3(-0.5f,-0.5f,0f),
                        new Vector3(-0.5f,0.5f,0f),
                        new Vector3(0.5f,0.5f,0f),
                        new Vector3(0.5f,-0.5f,0f)
                    };
                    tris = new int[] { 0, 1, 2, 0, 2, 3};
                    break;
            }

            m.Vertices = verts;
            m.Triangles = tris;

            return m;
        }

    }
}
