using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Drawing;

namespace LBE.Input
{
    public class InputManager : BaseEngineComponent
    {
        /// <summary>
        /// GamePad states
        /// </summary>
        GamePadState[] m_gamePadState = new GamePadState[4];
        GamePadState[] m_previousGamePadState = new GamePadState[4];

        public GamePadState GamePadState()
        {
            return GamePadState(PlayerIndex.One);
        }

        public GamePadState GamePadState(PlayerIndex playerIndex)
        {
            return m_gamePadState[(int)playerIndex];
        }

        public GamePadState PreviousGamePadState()
        {
            return PreviousGamePadState(PlayerIndex.One);
        }

        public GamePadState PreviousGamePadState(PlayerIndex playerIndex)
        {
            return m_previousGamePadState[(int)playerIndex];
        }

        /// <summary>
        /// Keyboard states
        /// </summary>
        KeyboardState[] m_keyboardState = new KeyboardState[4];
        KeyboardState[] m_previousKeyboardState = new KeyboardState[4];

        public KeyboardState KeyboardState()
        {
            return KeyboardState(PlayerIndex.One);
        }

        public KeyboardState KeyboardState(PlayerIndex playerIndex)
        {
            return m_keyboardState[(int)playerIndex];
        }

        public KeyboardState PreviousKeyboardState()
        {
            return PreviousKeyboardState(PlayerIndex.One);
        }

        public KeyboardState PreviousKeyboardState(PlayerIndex playerIndex)
        {
            return m_previousKeyboardState[(int)playerIndex];
        }

        /// <summary>
        /// Mouse state
        /// </summary>
        MouseState m_mouseState;
        MouseState m_previousMouseState;

        public MouseState MouseState()
        {
            return m_mouseState;
        }

        public MouseState PreviousMouseState()
        {
            return m_previousMouseState;
        }

        Point m_mouseOrigin;
        public Point MouseOrigin
        {
            get { return m_mouseOrigin; }
            set { m_mouseOrigin = value; }
        }

        public InputManager()
        {
        }

        public override void Startup()
        {
            m_mouseOrigin = Point.Zero;
        }

        public override void Shutdown()
        {
        }

        public override void StartFrame()
        {
            for (int i = 0; i < 4; i++)
            {
                m_previousKeyboardState[i] = m_keyboardState[i];
                m_keyboardState[i] = Keyboard.GetState((PlayerIndex)i);

                m_previousGamePadState[i] = m_gamePadState[i];
                m_gamePadState[i] = GamePad.GetState((PlayerIndex)i, GamePadDeadZone.Circular);
            }
            m_previousMouseState = m_mouseState;
            m_mouseState = Mouse.GetState();
        }
    }
}
