namespace LBE
{
    /// <summary>
    /// Represents a value that update slowly over time, from its current value to a target value.
    /// It effectively implments a low-pass filter.
    /// </summary>
    public class SmoothValue
    {
        float m_strength;
        public float Strength
        {
            get { return m_strength; }
            set { m_strength = value; }
        }

        float m_value;
        public float Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public SmoothValue()
        {
            m_value = 0;
            m_strength = 0.5f;
        }

        public SmoothValue(float value, float strength)
        {
            m_strength = strength;
            m_value = value;
        }

        public void Update(float value)
        {
            if(!float.IsNaN(value) && !float.IsInfinity(value))
                m_value = m_strength * m_value + (1 - m_strength) * value;

        }
    }
}
