using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LBE.Graphics
{
    public class DynamicMesh<T> : Mesh, IDisposable where T : struct
    {
        private int m_capacity;
        public int Capacity
        {
            get { return m_capacity; }
        }

        T[] m_vertexArray;
        int[] m_indexArray;

        private int m_vertexCount;
        private int m_indexCount;

        public DynamicMesh(GraphicsDevice device, VertexDeclaration vertexDeclaration, PrimitiveType primitiveType, int capacity)
        {
            m_vertexArray = new T[capacity];
            m_indexArray = new int[capacity];

            m_primitiveType = primitiveType;
            m_capacity = capacity;

            m_vertexBuffer = new VertexBuffer(device, vertexDeclaration, Capacity, BufferUsage.WriteOnly);
            m_indexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, Capacity, BufferUsage.WriteOnly);
        }

        public int Vertex(T vertex)
        {
            m_vertexArray[m_vertexCount] = vertex;
            return m_vertexCount++;
        }

        public void Index(int index)
        {
            m_indexArray[m_indexCount] = index;
            m_indexCount++;
        }

        public void Reset()
        {
            m_verticesCount = 0;
            m_primitiveCount = 0;

            m_vertexCount = 0;
            m_indexCount = 0;
        }

        public void PrepareDraw()
        {
            if (m_vertexCount > 0 && m_indexCount > 0)
            {
                if (m_primitiveType == PrimitiveType.LineList)
                    m_primitiveCount = m_indexCount / 2;

                if (m_primitiveType == PrimitiveType.TriangleList)
                    m_primitiveCount = m_indexCount / 3;

                if (m_primitiveType == PrimitiveType.TriangleStrip)
                    m_primitiveCount = m_indexCount-2;

                m_vertexBuffer.SetData<T>(m_vertexArray, 0, m_vertexCount);
                m_indexBuffer.SetData(m_indexArray, 0, m_indexCount);
            }
        }

        public void Dispose()
        {
            m_vertexArray = null;
            m_indexArray = null;

            m_vertexBuffer.Dispose();
            m_indexBuffer.Dispose();
        }
    }
}
