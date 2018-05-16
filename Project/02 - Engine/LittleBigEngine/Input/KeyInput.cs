using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Input
{
    public class KeyInput : IInput
    {
        public KeyInput()
        {
        }

        public virtual bool GetState()
        {
            return false;
        }

        public virtual bool GetPreviousState()
        {
            return false;
        }
        
        public float Value
        {
            get
            {
                if (GetState())
                {
                    return 1.0f;
                }
                else
                {
                    return 0.0f;
                }
            }
        }

        public float PreviousValue
        {
            get
            {
                if (GetPreviousState())
                {
                    return 1.0f;
                }
                else
                {
                    return 0.0f;
                }
            }
        }
    }
    
}
