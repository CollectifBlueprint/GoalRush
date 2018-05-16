using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace LBE.Input
{
    public class KeyControl : IControl
    {
        IInput m_input;

        public KeyControl(IInput input)
        {
            m_input = input;
        }

        public KeyControl(Keys key)
        {
            m_input = new KeyboardInput(key);
        }

        public KeyControl(MouseButtons button)
        {
            m_input = new MouseButtonInput(button);
        }

        public KeyControl(Buttons button)
        {
            m_input = new GamepadButtonInput(button);
        }

        public bool KeyPressed()
        {
            return m_input.Value != 0 && m_input.PreviousValue == 0;
        }

        public bool KeyReleased()
        {
            return m_input.Value == 0 && m_input.PreviousValue != 0;
        }

        public bool KeyDown()
        {
            return m_input.Value != 0;
        }

        public bool KeyUp()
        {
            return m_input.Value == 0;
        }
    }
}
