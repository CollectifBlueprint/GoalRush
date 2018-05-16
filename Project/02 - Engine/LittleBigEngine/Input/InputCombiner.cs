using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Input
{
    public class InputCombiner : IInput
    {
        List<IInput> m_inputs;

        public InputCombiner()
        {
            m_inputs = new List<IInput>();
        }

        public InputCombiner(IInput i1, IInput i2)
        {
            m_inputs = new List<IInput>();
            m_inputs.Add(i1);
            m_inputs.Add(i2);
        }

        public InputCombiner(IInput i1, IInput i2, IInput i3)
        {
            m_inputs = new List<IInput>();
            m_inputs.Add(i1);
            m_inputs.Add(i2);
            m_inputs.Add(i3);
        }

        public InputCombiner(IEnumerable<IInput> inputs)
        {
            m_inputs = new List<IInput>(inputs);
        }

        public void Add(IInput input)
        {
            m_inputs.Add(input);
        }

        public float Value
        {
            get {
                float acc = 0;
                foreach (var input in m_inputs)
                    acc += input.Value;
                return acc; 
            }
        }

        public float PreviousValue
        {
            get
            {
                float acc = 0;
                foreach (var input in m_inputs)
                    acc += input.PreviousValue;
                return acc;
            }
        }
    }
}
