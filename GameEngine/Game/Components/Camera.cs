using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class Camera : Component
    {
        public static GameObject MainCamera
        {
            get;set;
        }

        public static void Init()
        {
            GameObject camera = new GameObject();
            camera.AddComponent<Camera>();
            Camera.MainCamera = camera;
        }

    }
}
