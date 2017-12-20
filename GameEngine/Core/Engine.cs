using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using SharpDX;

using Math = GameEngine.Utilities.MathHelper;

using GameEngine.Utilities;
using System.Timers;
using GameEngine.Structures;
using SharpDX.Windows;

namespace GameEngine
{
    public class Engine
    {
        private Graphics _graphics;

        internal bool _finished { get; private set; } = false;
        //internal List<GameObject> ObjectList { get; private set; } = new List<GameObject>();

        public bool Finished { get; private set; }
        public long FrameCount { get; private set; }


        public static Engine Current { get; private set; }

        public Engine()
        {
            Engine.Current = this;
            _graphics = new Graphics(this);
        }

        public void Init()
        {
            Input.Init();
            _graphics.Init();
            ShaderManager.Init(_graphics.GraphicsDevice);

            //Must be last
            Camera.Init();
        }

        public void Start()
        {
            Time.Start();

            RenderLoop.Run(_graphics.Form, EngineLoop);
        }

        public void Exit()
        {
            _graphics.Form.Close();
        }

        internal void Shutdown()
        {
            Dispose();
        }

        internal void EngineLoop()
        {
            Update();
            Draw();
        }


        internal void Update()
        {
            Time.Update();
            if (_graphics.Form.Focused)
                Input.Update();
            Debug.Update();
            Cursor.Update();


            GameObject.UpdateAll();

            FrameCount++;
            if(FrameCount %10 == 0)
                _graphics.Form.Text = $"{1 / Time.DeltaTime}FPS";

        }

        internal void Draw()
        {
            _graphics.Draw();
        }

        internal void Dispose()
        {
            _graphics.Dispose();
            ShaderManager.Dispose();
            Input.Dispose();
        }
    }
}
