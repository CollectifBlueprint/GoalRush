using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LBE.Graphics.Camera
{
    public class Camera2D : ICamera
    {
        Matrix m_view;
        public Matrix View
        {
            get { return m_view; }
        }

        Matrix m_projection;
        public Matrix Projection
        {
            get { return m_projection; }
        }

        int m_width;
        public int Width
        {
            set { m_width = value; }
            get { return m_width; }
        }

        int m_height;
        public int Height
        {
            set { m_height = value; }
            get { return m_height; }
        }

        Vector2 m_position;
        public Vector2 Position
        {
            set { m_position = value; }
            get { return m_position; }
        }

        float m_zoom = 1.0f;
        public float Zoom
        {
            set { m_zoom = value; }
            get { return m_zoom; }
        }

        public Camera2D(Vector2 cameraPosition, int width, int height)
        {
            m_width = width;
            m_height = height;
            SetProjection();

            m_position = cameraPosition;
            SetView();
        }


        public void SetView()
        {
            m_view = Matrix.CreateLookAt(
                    new Vector3(m_position, -1),
                    new Vector3(m_position, 1),
                    -Vector3.UnitY);
        }


        public void SetProjection()
        {
            m_projection = Matrix.CreateOrthographic(
                m_width,
                -m_height,
                0,
                2);
        }

        public void SetProjection(int width, int height)
        {
            m_projection = Matrix.CreateOrthographic(
                width / m_zoom,
                -height / m_zoom,
                0,
                2);

            m_width = width;
            m_height = height;
        }

        public Point WorldToScreen(Vector2 world)
        {
            float screenWidth = m_width;// Engine.Application.Window.ClientBounds.Width;// 1920;// Engine.Renderer.Device.PresentationParameters.BackBufferWidth;
            float screenHeigth = m_height;// Engine.Application.Window.ClientBounds.Height;// = 1080;// Engine.Renderer.Device.PresentationParameters.BackBufferHeight;
            return new Vector2(screenWidth * 0.5f, screenHeigth * 0.5f) + Engine.Renderer.CameraZoom * m_zoom * new Vector2(world.X - m_position.X, -world.Y + m_position.Y);
        }

        public Vector2 ScreenToWorld(Point screen)
        {
            float screenWidth = m_width;// Engine.Application.Window.ClientBounds.Width;// 1920;// Engine.Renderer.Device.PresentationParameters.BackBufferWidth;
            float screenHeigth = m_height;// Engine.Application.Window.ClientBounds.Height;// = 1080;// Engine.Renderer.Device.PresentationParameters.BackBufferHeight;
            float bufferWidth = Engine.Renderer.MainBuffer.Width;
            float bufferHeight = Engine.Renderer.MainBuffer.Height;

            Vector2 screenRatio = new Vector2(screen.X / screenWidth, screen.Y / screenHeigth);
            return m_position + (1/m_zoom) * new Vector2(screenRatio.X * bufferWidth - bufferWidth * 0.5f, -screenRatio.Y * bufferHeight + bufferHeight * 0.5f);
        }
    }
}
