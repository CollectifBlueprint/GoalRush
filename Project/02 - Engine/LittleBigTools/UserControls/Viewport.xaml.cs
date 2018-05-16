using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework.Graphics;
using LBE;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Interop;

namespace LBT
{
    public partial class Viewport : UserControl, IDisposable
    {
        public Action OnRender;

        RenderTarget2D m_renderTarget;
        public RenderTarget2D RenderTarget
        {
            get { return m_renderTarget; }
        }

        WriteableBitmap m_writableBitmap;
        public WriteableBitmap WritableBitmap
        {
            get { return m_writableBitmap; }
        }

        bool m_realTime;
        public bool RealTime
        {
            get { return m_realTime; }
            set { m_realTime = value; }
        }

        Nullable<Int32Rect> m_dirtyRectangle;
        public Nullable<Int32Rect> DirtyRectangle
        {
            get { return m_dirtyRectangle; }
            set { m_dirtyRectangle = value; }
        }

        bool m_needRedraw;

        GraphicsDevice m_device;
        Byte[] m_buffer;

        public Viewport()
        {
            InitializeComponent();

            //Dummy bitmap
            m_writableBitmap = new WriteableBitmap(
                32, 32, 96, 96,
                PixelFormats.Bgr32, null);

            Image.Source = m_writableBitmap;

            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                Loaded += new RoutedEventHandler(OnLoaded);
            }

            m_realTime = false;
            m_needRedraw = true;

            m_dirtyRectangle = null;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                Tool.Instance.Viewports.Add(this);
                Reset((int)ActualWidth, (int)ActualHeight);
            }
        }

        public void Reset(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            if (m_renderTarget != null)
                Dispose();

            m_device = Engine.Renderer.Device;

            m_renderTarget = new RenderTarget2D(
                m_device,
                width, height);

            m_writableBitmap = new WriteableBitmap(
                width, height, 96, 96,
                PixelFormats.Bgr32, null);

            m_buffer = new byte[width * height * 4];

            Image.Source = m_writableBitmap;

            m_needRedraw = true;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                Dispose();
                if (IsLoaded)
                    Reset((int)ActualWidth, (int)ActualHeight);
            }
        }

        public void Invalidate()
        {
            m_needRedraw = true;
        }


        public void Render()
        {
            if (!m_needRedraw)
                return;

            if (OnRender != null) OnRender();
        }

        public void Commit()
        {
            if (!m_needRedraw)
                return;

            Int32Rect dirtyRect;
            if (m_dirtyRectangle == null)
                dirtyRect = new Int32Rect(0, 0, (int)m_writableBitmap.PixelWidth, (int)m_writableBitmap.PixelHeight);
            else
                dirtyRect = m_dirtyRectangle.Value;

            if (dirtyRect.IsEmpty)
                return;

            m_renderTarget.GetData<byte>(m_buffer);

            for (int i = 0; i < m_buffer.Length - 2; i += 4)
            {
                byte r = m_buffer[i];
                m_buffer[i] = m_buffer[i + 2];
                m_buffer[i + 2] = r;
            }

            m_writableBitmap.Lock();
            Marshal.Copy(m_buffer, 0, m_writableBitmap.BackBuffer, m_buffer.Length);
            m_writableBitmap.AddDirtyRect(dirtyRect);
            m_writableBitmap.Unlock();

            if (!m_realTime)
                m_needRedraw = false;
        }

        public void Dispose()
        {
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                if (m_renderTarget != null)
                    m_renderTarget.Dispose();
            }
        }
    }
}
