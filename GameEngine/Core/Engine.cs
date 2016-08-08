using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

using Math = GameEngine.Utilities.Math;

using GameEngine.Utilities;
using System.Timers;

namespace GameEngine
{
    public static class Engine
    {
        internal static bool _finished = false;

        internal static List<GameObject> ObjectList = new List<GameObject>();


        public static bool Finished
        {
            get
            {
                return _finished;
            }
        }

        public static void Init()
        {
            Input.Init();
            Graphics.Init();
        }

        public static void Start()
        {
            Time.Start();

            Graphics.Start();
        }

        public static void Exit()
        {
            Graphics.Form.Close();
        }

        internal static void Shutdown()
        {
            Dispose();
        }

        internal static void EngineLoop()
        {
            Update();
            Draw();
        }


        public static void Update()
        {
            Time.Update();
            Input.Update();


            foreach(GameObject go in ObjectList)
            {
                go.Update();
            }

            float fps = (1000 / (Time.DeltaTime * 1000));
            Graphics.Form.Text = $"Game engine ({fps.ToString()} FPS)";
        }

        public static void Draw()
        {
            foreach(GameObject go in ObjectList)
            {
                go.Draw();
            }

            Graphics.Draw();
        }

        internal static void Dispose()
        {
            Graphics.Dispose();
        }
    }
}
