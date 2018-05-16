using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;

namespace Ball.Gameplay
{
    public class PropertyStack
    {
        int m_stack;

        public bool Value
        {
            get { return m_stack > 0; }
        }

        public PropertyStack()
        {
            m_stack = 0;
        }

        public void Set()
        {
            m_stack++;
        }

        public void Unset()
        {
            if (m_stack > 0)
                m_stack--;
            else
                Engine.Log.Write("The PropertieStack was already unset");
        }

        public static implicit operator bool(PropertyStack stack)
        {
            return stack.Value;
        }
    }
}
