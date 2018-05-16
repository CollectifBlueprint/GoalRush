using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace LBE.Input
{
    public enum MouseButtons
    {
        Left, 
        Middle,
        Right,

    }

    public class MouseButtonInput : KeyInput
    {
        private MouseButtons m_button;

        public MouseButtonInput(MouseButtons button)
        {
            m_button = button;
        }

        public override bool GetState()
        {
            if(m_button == MouseButtons.Left)
                return Engine.Input.MouseState().LeftButton == ButtonState.Pressed;
            if (m_button == MouseButtons.Middle)
                return Engine.Input.MouseState().MiddleButton == ButtonState.Pressed;
            if (m_button == MouseButtons.Right)
                return Engine.Input.MouseState().RightButton == ButtonState.Pressed;

            return false;
        }

        public override bool GetPreviousState()
        {
            if (m_button == MouseButtons.Left)
                return Engine.Input.PreviousMouseState().LeftButton == ButtonState.Pressed;
            if (m_button == MouseButtons.Middle)
                return Engine.Input.PreviousMouseState().MiddleButton == ButtonState.Pressed;
            if (m_button == MouseButtons.Right)
                return Engine.Input.PreviousMouseState().RightButton == ButtonState.Pressed;

            return false;
        }
    }
    
}
