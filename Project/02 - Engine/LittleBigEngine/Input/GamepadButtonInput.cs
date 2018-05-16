using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace LBE.Input
{
    public class GamepadButtonInput : KeyInput
    {
        PlayerIndex m_playerIndex;
        Buttons m_button;

        public GamepadButtonInput(Buttons button, PlayerIndex playerIndex = PlayerIndex.One)
        {
            m_playerIndex = playerIndex;
            m_button = button;
        }

        public override bool GetState()
        {
            return Engine.Input.GamePadState(m_playerIndex).IsButtonDown(m_button);
        }

        public override bool GetPreviousState()
        {
            return Engine.Input.PreviousGamePadState(m_playerIndex).IsButtonDown(m_button);
        }
    }
}
