using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace LBE.Input
{
    public class KeyboardInput : KeyInput
    {
        Keys m_key;

        public KeyboardInput(Keys key)
        {
            m_key = key;
        }

        public override bool GetState()
        {
            return Engine.Input.KeyboardState().IsKeyDown(m_key);
        }

        public override bool GetPreviousState()
        {
            return Engine.Input.PreviousKeyboardState().IsKeyDown(m_key);
        }
    }
}
