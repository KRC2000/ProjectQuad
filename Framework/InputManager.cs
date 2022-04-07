using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

namespace Framework
{
    class InputManager
    {
        public enum InputState
        {
            JustPressed, BeingPressed, JustReleased, Idle
        }

        static private Dictionary<Keys, InputState> input = new Dictionary<Keys, InputState>();

        static public void Init()
        {
            Array keys = Enum.GetValues(typeof(Keys));  
            foreach (var key in keys)
                input.Add((Keys)key, InputState.Idle);
        }

        static public void Update()
        {
            
        }

        static public InputState GetState(Keys key)
        {
            InputState state;
            input.TryGetValue(key, out state);
            return state;
        }
    }
}