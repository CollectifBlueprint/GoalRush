using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LBE.Input
{
    public class StickControl
    {
        List<IInput2D> m_inputs;
        public List<IInput2D> Inputs
        {
            get { return m_inputs; }
        }

        public StickControl()
        {
            m_inputs = new List<IInput2D>();
        }

        public StickControl(IInput2D input)
        {
            m_inputs = new List<IInput2D>();
            m_inputs.Add(input);
        }

        public StickControl(IInput2D[] inputs)
        {
            m_inputs = new List<IInput2D>();
            m_inputs.AddRange(inputs);
        }

        public Vector2 Get()
        {
            Vector2 acc = new Vector2();
            foreach (var input in m_inputs)
                acc += input.Value;

            if (acc.LengthSquared() > 1)
                acc.Normalize();

            return acc;
        }
    }
}
