using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using LBE.Graphics.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Interop;
using MonoApp;
using LBE.Debug;
using System.Windows;
using System.Windows.Media;

using Color = Microsoft.Xna.Framework.Color;

namespace LBT
{
    public class Tool
    {
        static Tool m_instance;
        public static Tool Instance
        {
            get { return m_instance; }
        }

        ToolWindow m_toolWindow;
        public static ToolWindow Window
        {
            get { return m_instance.m_toolWindow; }
        }

        Engine m_engine;
        public Engine Engine
        {
            get { return m_engine; }
        }

        private OpenTK.GameWindow m_dummyWindow;

         GraphicsDeviceService m_graphicsService;

         DateTime m_currentTime;
         float m_targetFrameTime;

         List<Viewport> m_viewports;
         public List<Viewport> Viewports
         {
             get { return m_viewports; }
         }

        public Tool()
        {
            m_instance = this;
            m_viewports = new List<Viewport>();

            m_toolWindow = new ToolWindow();
            m_toolWindow.Loaded += new RoutedEventHandler(m_toolWindow_Loaded);

            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

            m_currentTime = DateTime.Now;
            m_targetFrameTime = 16.6f;

            m_engine = new Engine();

            //Set the content directory to the project root
            Engine.AssetManager.ContentRoot = ".";

            //Start the engine
            m_engine.Start();
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            Update();
        }

        void m_toolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        public void Run()
        {
            Application app = new Application();
            app.Run(m_toolWindow);
        }

        public void Initialize()
        {
            m_dummyWindow = new OpenTK.GameWindow();

            //Add a reference to the graphics device
            IntPtr hwnd = new WindowInteropHelper(m_toolWindow).Handle;
            m_graphicsService = GraphicsDeviceService.AddRef(hwnd);

            var renderer = new Renderer();
            renderer.Device = m_graphicsService.GraphicsDevice;

            Engine.AddComponent(renderer);
            Engine.Renderer = renderer;

            var debug = new DebugManager();
            Engine.AddComponent(debug);
            Engine.Debug = debug;

            Engine.TargetFrameTime = m_targetFrameTime;
        }

        public void Update()
        {
            DateTime newTime = DateTime.Now;
            TimeSpan deltaTime = DateTime.Now - m_currentTime;

            if(deltaTime.TotalMilliseconds > m_targetFrameTime)
            {
                m_currentTime = newTime;
                Engine.BeginFrame((float)deltaTime.TotalMilliseconds);
                Draw();
            }
        }
        

        public void Draw()
        {
            foreach (var viewport in m_viewports)
            {
                if (viewport.IsVisible)
                    RenderViewport(viewport);
            }

            Engine.EndFrame();
        }

        public void End()
        {
            m_dummyWindow.Close();
        }

        public void RenderViewport(Viewport viewport)
        {
            if (viewport.RenderTarget == null)
                return;

            //Set the render target
            m_graphicsService.GraphicsDevice.SetRenderTarget(viewport.RenderTarget);

            Engine.Renderer.CurrentCamera = new Camera2D(Vector2.Zero, viewport.RenderTarget.Width, viewport.RenderTarget.Height);
            Engine.Renderer.Device.Clear(ClearOptions.Target, Color.Black, 0, 0);

            viewport.Render();
            Engine.Debug.Screen.Draw();
            Engine.Debug.Screen.Reset();

            //Resolve the render target
            m_graphicsService.GraphicsDevice.SetRenderTarget(null);

            //Commit the change to the image source
            viewport.Commit();
        }
    }
}
