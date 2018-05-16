using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LBE.Graphics
{
    public abstract class Mesh
    {
        protected VertexBuffer m_vertexBuffer = null;
        public VertexBuffer VertexBuffer
        {
            get { return m_vertexBuffer; }
        }

        protected IndexBuffer m_indexBuffer = null;
        public IndexBuffer IndexBuffer
        {
            get { return m_indexBuffer; }
        }

        protected int m_primitiveCount = 0;
        public int PrimitiveCount
        {
            get { return m_primitiveCount; }
        }

        protected int m_verticesCount = 0;
        public int VerticesCount
        {
            get { return m_verticesCount; }
        }

        protected BoundingSphere m_boundingSphere;
        public BoundingSphere BoundingSphere
        {
            get { return m_boundingSphere; }
            set { m_boundingSphere = value; }
        }

        protected PrimitiveType m_primitiveType;
        public PrimitiveType PrimitiveType
        {
            get { return m_primitiveType; }
            set { m_primitiveType = value; }
        }
    }
}
