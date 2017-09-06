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
        private static Mouse _mouse;

        private static DirectInput _di;

        private static bool[] _currentKeyStates = new bool[Enum.GetNames(typeof(Key)).Length];
        private static bool[] _previousKeyStates = new bool[_currentKeyStates.Length];

        private static bool[] _currentMouseStates = new bool[3];//3 mouse buttons
        private static bool[] _previousMouseStates = new bool[_currentMouseStates.Length];

        public static Vector2 MouseDelta { get; private set; }

        internal static void Init()
        {
            _di = new DirectInput();

            _keyboard = new Keyboard(_di);
            _mouse = new Mouse(_di);

            _keyboard.Properties.BufferSize = 128;
            _keyboard.Acquire();

            _mouse.Properties.BufferSize = 128;
            _mouse.Acquire();
        }

        internal static void Update()
        {
            #region KeyboardUpdate
            _keyboard.Poll();
            var _keyboardStateArray = _keyboard.GetBufferedData();

            Array.Copy(_currentKeyStates, _previousKeyStates, _currentKeyStates.Length);
            foreach (KeyboardUpdate update in _keyboardStateArray)
            {
                _currentKeyStates[(int)update.Key] = update.IsPressed;
            }
            #endregion
            #region MouseUpdate
            _mouse.Poll();
            var _mouseStateArray = _mouse.GetBufferedData();

            int xSum = 0;
            int ySum = 0;

            Array.Copy(_currentMouseStates, _previousMouseStates, _currentMouseStates.Length);
            foreach (MouseUpdate update in _mouseStateArray)
            {
                int buttonIndex = (int)update.Offset - 12;
                if (buttonIndex < 0)
                {
                    //Compute mouse delta
                    if (update.Offset == MouseOffset.X)
                        xSum += update.Value;
                    else if (update.Offset == MouseOffset.Y)
                        ySum += update.Value;
                }
                else
                    _currentMouseStates[buttonIndex] = update.Value > 0;
            }
            #endregion

            //Update MouseDelta
            MouseDelta = new Vector2(xSum, ySum);
        }

        public static bool GetKey(Key key)
        {
            if ((int)key < 0 || (int)key >= _currentKeyStates.Length)
                return false;

            return _currentKeyStates[(int)key];
        }
        public static bool GetKeyDown(Key key)
        {
            if ((int)key < 0 || (int)key >= _currentKeyStates.Length)
                return false;

            return _currentKeyStates[(int)key] & !_previousKeyStates[(int)key];
        }
        public static bool GetKeyUp(Key key)
        {
            if ((int)key < 0 || (int)key >= _currentKeyStates.Length)
                return false;

            return !_currentKeyStates[(int)key] & _previousKeyStates[(int)key];
        }

        public static bool GetMouseButton(int ButtonIndex)
        {
            if (ButtonIndex < 0 || ButtonIndex >= _currentMouseStates.Length)
                return false;

            return _currentMouseStates[ButtonIndex];
        }
        public static bool GetMouseButtonDown(int ButtonIndex)
        {
            if (ButtonIndex < 0 || ButtonIndex >= _currentMouseStates.Length)
                return false;

            //If the button is currently pressed but not pressed last frame
            return _currentMouseStates[ButtonIndex] & !_previousMouseStates[ButtonIndex];
        }
        public static bool GetMouseButtonUp(int ButtonIndex)
        {
            if (ButtonIndex < 0 || ButtonIndex >= _currentMouseStates.Length)
                return false;

            return !_currentMouseStates[ButtonIndex] & _previousMouseStates[ButtonIndex];
        }



        public static void Dispose()
        {
            _di.Dispose();
            _keyboard.Dispose();
            _mouse.Dispose();
        }

    }
}

