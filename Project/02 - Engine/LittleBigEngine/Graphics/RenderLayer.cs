using LBE.Graphics.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Graphics
{
    public interface IRenderable
    {
        void Draw();
    }

    public class RenderLayer
    {
        protected ICamera m_camera;
        public ICamera Camera
        {
            get { return m_camera; }
        }

        List<IRenderable> m_renderables;
        public List<IRenderable> Renderables
        {
            get { return m_renderables; }
        }

        bool m_enabled;
        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }

        public RenderLayer()
        {
            m_renderables = new List<IRenderable>();
            m_enabled = true;
        }

        public RenderLayer(ICamera camera) : this()
        {
            m_camera = camera;
        }

        public void Draw()
        {
            Start();

            foreach (var renderable in m_renderables)
            {
                renderable.Draw();
            }

            End();
        }

        public virtual void Start() { }
        public virtual void End() { }
    }
}
