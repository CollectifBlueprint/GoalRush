using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LBE.Input
{
    public class CustomInput : IInput
    {
        float m_newValue;
        float m_value;
        float m_previousValue;

        public CustomInput()
        {
        }

        public float Value
        {
            get { return m_value; }
        }

        public float PreviousValue
        {
            get { return m_previousValue; }
        }

        public void Set(float value)
        {
            m_newValue = value;
        }

        public void Update()
        {
            m_previousValue = m_value;
            m_value = m_newValue;
        }
    }

    public class CustomInput2D : IInput2D
    {
        Vector2 m_newValue;
        Vector2 m_value;
        Vector2 m_previousValue;

        public CustomInput2D()
        {
        }

        public Vector2 Value
        {
            get { return m_value; }
        }

        public Vector2 PreviousValue
        {
            get { return m_previousValue; }
        }

        public void Set(Vector2 value)
        {
            m_newValue = value;
        }

        public void Update()
        {
            m_previousValue = m_value;
            m_value = m_newValue;
        }
    }
    
}
