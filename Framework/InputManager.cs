using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

namespace Framework
{
    public enum InputState
    {
        JustPressed, BeingPressed, JustReleased, Idle
    }

    public enum MouseButtons
    {
        Left, Right, Middle
    }

    class InputManager
    {
        static private KeyboardState previousKeyboardState;
        static private KeyboardState currentKeyboardState;
        static private MouseState previousMouseState;
        static private MouseState currentMouseState;


        static public void Update(KeyboardState keyboardState, MouseState mouseState)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = keyboardState;

            previousMouseState = currentMouseState;
            currentMouseState = mouseState;
        }

        static public InputState GetState(Keys key)
        {
            bool wasPressed = previousKeyboardState.IsKeyDown(key);
            bool isPressed = currentKeyboardState.IsKeyDown(key);

            if (wasPressed && isPressed) return InputState.BeingPressed;
            else if (!wasPressed && isPressed) return InputState.JustPressed;
            else if (wasPressed && !isPressed) return InputState.JustReleased;
            else return InputState.Idle;
        }

        static public InputState GetState(MouseButtons button)
        {
            bool wasPressed = false, isPressed = false;
            switch (button)
            {
                case MouseButtons.Left:
                    if (previousMouseState.LeftButton == ButtonState.Pressed) wasPressed = true;
                    if (currentMouseState.LeftButton == ButtonState.Pressed) isPressed = true;
                break;
                case MouseButtons.Right:
                    if (previousMouseState.RightButton == ButtonState.Pressed) wasPressed = true;
                    if (currentMouseState.RightButton == ButtonState.Pressed) isPressed = true;
                break;
                case MouseButtons.Middle:
                    if (previousMouseState.MiddleButton == ButtonState.Pressed) wasPressed = true;
                    if (currentMouseState.MiddleButton == ButtonState.Pressed) isPressed = true;
                break;
            }
            

            if (wasPressed && isPressed) return InputState.BeingPressed;
            else if (!wasPressed && isPressed) return InputState.JustPressed;
            else if (wasPressed && !isPressed) return InputState.JustReleased;
            else return InputState.Idle;
        }
    }
}