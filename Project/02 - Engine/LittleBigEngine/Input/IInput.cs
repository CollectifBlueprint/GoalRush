using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Input
{
    public interface IInput
    {
        float Value { get; }
        float PreviousValue { get; }    
    }
}
