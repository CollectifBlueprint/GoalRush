using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Input
{
    public class NullInput : IInput
    {
        public NullInput()
        {
        }

        public float Value
        {
            get { return 0; }
        }

        public float PreviousValue
        {
            get { return 0; }
        }
    }
    
}
