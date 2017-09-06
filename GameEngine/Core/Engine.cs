using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using SharpDX;

using Math = GameEngine.Utilities.MathHelper;

using GameEngine.Utilities;
using System.Timers;
using GameEngine.Structures;

namespace GameEngine
{
    public class Engine
    {
        private Graphics _graphics;

        internal bool _finished { get; private set; } = false;
        //internal List<GameObject> ObjectList { get; private set; } = new List<GameObject>();

        public bool Finished { get; private set; }
        

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

            
            Camera.Init();
        }

        public void Start()
        {
            Time.Start();

            _graphics.Start();   
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
            Input.Update();
            Debug.Update();
            Cursor.Update();

            foreach (GameObject go in GameObject.ObjectList)
            {
                go.Update();
            }
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

        public void WriteConsole(string Message)
        {
            Console.WriteLine(Message);
        }
    }
}
