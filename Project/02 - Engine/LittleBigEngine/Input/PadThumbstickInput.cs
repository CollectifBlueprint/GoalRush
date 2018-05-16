using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LBE.Input
{
    public enum ThumbSticks
    {
        Left,
        Right,
    }

    public class PadThumbstickInput : IInput2D
    {
        ThumbSticks m_thumbstick;
        PlayerIndex m_playerIndex;

        public PadThumbstickInput(ThumbSticks thumbstick, PlayerIndex playerIndex = PlayerIndex.One)
        {
            m_thumbstick = thumbstick;
            m_playerIndex = playerIndex;
        }

        public Vector2 Value
        {
            get 
            {
                if (m_thumbstick == ThumbSticks.Left)
                    return Engine.Input.GamePadState(m_playerIndex).ThumbSticks.Left;
                else
                    return Engine.Input.GamePadState(m_playerIndex).ThumbSticks.Right;
            }
        }

        public Vector2 PreviousValue
        {
            get
            {
                if (m_thumbstick == ThumbSticks.Left)
                    return Engine.Input.PreviousGamePadState(m_playerIndex).ThumbSticks.Left;
                else
                    return Engine.Input.PreviousGamePadState(m_playerIndex).ThumbSticks.Right;
            }
        }
    }
}
