using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameEngine.Structures;
using SharpDX;

namespace GameEngine
{
    public static class Physics
    {
        private static bool Raycast(Vector3 StartPosition, Vector3 Direction, float MaxDistance, int LayerIndex,out RayCastHit HitInfo)
        {
            float closestDistance = float.MaxValue;
            Vector3 closestPoint = Vector3.Zero;
            GameObject closestObject = null;

            GameObject[] _objectsInLayer = Layer.GetAllObjectsInLayer(LayerIndex);
            foreach (GameObject go in _objectsInLayer.Where(x=>x.HasComponent<MeshRenderer>()))
            {
                Mesh m = go.GetComponent<MeshRenderer>().Mesh;

                //Check distance
                float objectDistance = MathHelper.CalculateDistance(go.Transform.WorldPosition, StartPosition);
                objectDistance -= MathHelper.CalculateDistance(go.Transform.WorldPosition, m.OriginFarPoint);

                if (objectDistance > MaxDistance)
                    continue;

                for (int i = 0; i < m.Triangles.Length; i += 3)
                {
                    Vector3 p1 = (Vector3)m.Vertices[m.Triangles[i]];
                    Vector3 p2 = (Vector3)m.Vertices[m.Triangles[i + 1]];
                    Vector3 p3 = (Vector3)m.Vertices[m.Triangles[i + 2]];

                    Vector3 e1 = (Vector3)(p2 - p1);
                    Vector3 e2 = (Vector3)(p3 - p1);

                    Vector3 p = Vector3.Cross(Direction, e2);
                    float det = Vector3.Dot(e1, p);

                    if (det > -float.Epsilon && det < float.Epsilon)
                        continue;

                    float invDet = 1.0f / det;

                    Vector3 t = StartPosition - p1;

                    float u = Vector3.Dot(t, p) * invDet;

                    if (u < 0 || u > 1)
                        continue;

                    Vector3 q = Vector3.Cross(t, e1);

                    float v = Vector3.Dot(Direction, q) * invDet;

                    if (v < 0 || u + v > 1)
                        continue;

                    if ((Vector3.Dot(e2, q) * invDet) > float.Epsilon)
                    {
                        Vector3 hitPoint = p1 + (u * e1) + (v * e2);

                        float distance = MathHelper.CalculateDistance(StartPosition, hitPoint);

                        if (distance > MaxDistance)
                            continue;

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestPoint = hitPoint;
                            closestObject = go;
                        }
                    }
                }
            }

            if (closestObject == null)
            {
                HitInfo = null;
                return false;
            }
            else
            {
                HitInfo = new RayCastHit(closestObject, closestPoint);
                return true;
            }
        }

        public static bool Raycast(Vector3 StartPosition, Vector3 Direction, out RayCastHit HitInfo)
        {
            return Raycast(StartPosition, Direction, float.MaxValue, 0, out HitInfo);
        }
    }
}
