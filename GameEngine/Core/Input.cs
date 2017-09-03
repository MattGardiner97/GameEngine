using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.DirectInput;

namespace GameEngine
{
    public static class Input
    {
        private static Keyboard _keyboard;
       // private static Mouse _mouse;

        private static DirectInput _di;

        private static KeyboardUpdate[] stateArray;

        internal static void Init()
        {
            _di = new DirectInput();

            _keyboard = new Keyboard(_di);

            _keyboard.Properties.BufferSize = 128;
            _keyboard.Acquire();            
        }

        internal static void Update()
        {
            _keyboard.Poll();
            stateArray = _keyboard.GetBufferedData();

            _previousKeysDown = _currentKeysDown;
            foreach (KeyboardUpdate state in stateArray)
            {
                if(state.IsPressed == true)
                {
                    if(_currentKeysDown.Contains(state.Key) == false)
                    {
                        _currentKeysDown.Add(state.Key);
                    }
                }
                if(state.IsReleased == true)
                {
                    if(_currentKeysDown.Contains(state.Key) == true)
                    {
                        _currentKeysDown.Remove(state.Key);
                    }
                }
            }
        }


        private static List<Key> _previousKeysDown = new List<Key>();
        private static List<Key> _currentKeysDown = new List<Key>();

        public static bool GetKey(Key key)
        {
            if(_currentKeysDown.Contains(key) == true)
            {
                return true;
            }
            return false;
        }
        public static bool GetKeyDown(Key key)
        {
            if(_currentKeysDown.Contains(key) == true && _previousKeysDown.Contains(key) == false)
            {
                return true;
            }
            return false;
        }
        public static bool GetKeyUp(Key key)
        {
            if(_currentKeysDown.Contains(key) == false && _previousKeysDown.Contains(key) == true)
            {
                return true;
            }
            return false;
        }

        public static void Dispose()
        {
            Input._di.Dispose();
            Input._keyboard.Dispose();
        }

        
    }
}
