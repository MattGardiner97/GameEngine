﻿using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using SharpDX;

using Math = GameEngine.Utilities.Math;

using GameEngine.Utilities;
using System.Timers;

namespace GameEngine
{
    public class Engine
    {
        private Graphics _graphics;

        internal bool _finished { get; private set; } = false;
        internal List<GameObject> ObjectList { get; private set; } = new List<GameObject>();

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
        }

        public void Start()
        {
            Time.Start();

            _graphics.Start();

            GameObject camera = new GameObject();
            camera.AddComponent<Camera>();
            Camera.MainCamera = camera.GetComponeont<Camera>();
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


            foreach (GameObject go in ObjectList)
            {
                go.Update();
            }

            timeSinceLastUpdate += Time.DeltaTime;
            if (timeSinceLastUpdate > 0.25)
            {
                UpdateFramerateCounter();
                timeSinceLastUpdate = 0;
            }

        }

        static float timeSinceLastUpdate = 0f;
        internal void UpdateFramerateCounter()
        {
            int fps = (int)(1 / Time.DeltaTime);
            _graphics.Form.Text = $"GameEngine (FPS: {fps.ToString()})";
        }

        internal void Draw()
        {
            foreach (GameObject go in ObjectList)
            {
                go.Draw();
            }

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
