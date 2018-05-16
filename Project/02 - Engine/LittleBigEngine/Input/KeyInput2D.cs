using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LBE.Input
{
    public class KeyInput2D : IInput2D
    {
        IInput m_up, m_down, m_left, m_right;

        public KeyInput2D()
        {
            m_up = new NullInput();
            m_down = new NullInput();
            m_left = new NullInput();
            m_right = new NullInput();
        }

        public KeyInput2D(IInput up, IInput down, IInput left, IInput right)
        {
            m_up = up;
            m_down = down;
            m_left = left;
            m_right = right;
        }

        public static KeyInput2D FromKeyboardArrows()
        {
            return new KeyInput2D(
                new KeyboardInput(Keys.Up),
                new KeyboardInput(Keys.Down),
                new KeyboardInput(Keys.Left), 
                new KeyboardInput(Keys.Right));
        }

        public static KeyInput2D FromKeyboardZQSD()
        {
            return new KeyInput2D(
                new InputCombiner(new KeyboardInput(Keys.Z), new KeyboardInput(Keys.W)),
                new KeyboardInput(Keys.S),
                new InputCombiner(new KeyboardInput(Keys.A), new KeyboardInput(Keys.Q)),
                new KeyboardInput(Keys.D));
        }

        public Vector2 Value
        {
            get
            {
                return new Vector2(
                    m_right.Value - m_left.Value,
                    m_up.Value - m_down.Value);
            }
        }

        public Vector2 PreviousValue
        {
            get
            {
                return new Vector2(
                    m_right.PreviousValue - m_left.PreviousValue,
                    m_up.PreviousValue - m_down.PreviousValue);
            }
        }
    }

}
