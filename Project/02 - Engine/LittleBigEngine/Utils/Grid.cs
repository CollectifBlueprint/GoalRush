using Microsoft.Xna.Framework;

namespace LBE
{
    public class Grid<T>
    {
        int m_width;
        public int Width
        {
            get { return m_width; }
        }

        int m_height;
        public int Height
        {
            get { return m_height; }
        }

        T[] m_data;
        public T[] Data
        {
            get { return m_data; }
        }

        public T this[Point p]
        {
            get { return Get(p.X, p.Y); }
            set { Set(p.X, p.Y, value); }
        }

        public T this[int x, int y]
        {
            get { return Get(x, y); }
            set { Set(x, y, value); }
        }

        public T this[Vector2 pos]
        {
            get { return Get((int)(m_width * pos.X), (int)(m_height * pos.Y)); }
            set { Set((int)(m_width * pos.X), (int)(m_height * pos.Y), value); }
        }

        public Grid(int width, int height)
        {
            m_width = width;
            m_height = height;

            m_data = new T[m_width * m_height];
        }

        public T Get(int x, int y)
        {
            return m_data[y * m_width + x];
        }

        public void Set(int x, int y, T value)
        {
            m_data[y * m_width + x] = value;
        }

        public bool TestBounds(Point p)
        {
            return TestBounds(p.X, p.Y);
        }

        public bool TestBounds(int x, int y)
        {
            return x >= 0 && x < m_width && y >= 0 && y < m_height;
        }

        public int GetLinearIndex(Point index2D)
        {
            return index2D.Y * m_width + index2D.X;
        }
    }
}
