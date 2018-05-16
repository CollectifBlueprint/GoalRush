#region Using Statements
using LBE.Debug;
using Microsoft.Xna.Framework;
using MonoApp;
using System;
using System.Threading;
using LBE.Audio;
using LBE.Assets;
using System.IO.Compression;
using System.Diagnostics;
#endregion

namespace LBE.Core
{
    class AppTimer
    {
        long m_startTime;

        public void Start()
        {
            m_startTime = System.Diagnostics.Stopwatch.GetTimestamp();
        }

        public float TimeMs()
        {
            long end = System.Diagnostics.Stopwatch.GetTimestamp();
            long delta = end - m_startTime;

            double freqInTickByMs = System.Diagnostics.Stopwatch.Frequency / 1000;
            return (float)(delta / freqInTickByMs);
        }
    }

    public class FPSDebug
    {
        public int SampleCount = 1;
        public int Frame = 0;
        public float Time = 0;

        public float FPS = 0;

        public void Tick(float time)
        {
            Frame++;
            Time += time;
            if (Frame == SampleCount)
            {
                FPS = 1000 * SampleCount / Time;
                Frame = 0;
                Time = 0;
            }
        }
    }

    public class Debug
    {
        public FPSDebug UpdateFPS = new FPSDebug();
        public FPSDebug DrawFPS = new FPSDebug();
    }

    public class Application : Game
    {
        GraphicsDeviceManager m_graphicsDeviceManager;
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return m_graphicsDeviceManager; }
        }

        private Engine m_engine;
        public Engine Engine
        {
            get { return m_engine; }
        }

        Debug m_debug;

        AppTimer m_frameTimer;
        public Application()
        {
            m_frameTimer = new AppTimer();

            m_debug = new Debug();
            m_graphicsDeviceManager = new GraphicsDeviceManager(this);
            m_graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            m_graphicsDeviceManager.PreferredBackBufferWidth = 1728;
            m_graphicsDeviceManager.PreferredBackBufferHeight = 1026;
            m_graphicsDeviceManager.ApplyChanges();

            m_engine = new Engine();

            Engine.Application = this;

            //Set the content directory to the project root
            Engine.AssetManager.ContentRoot = "./05 - Content";
            Engine.AssetManager.AssetSource = new FileSystemSource("./05 - Content");
            //Engine.AssetManager.AssetSource = new ZipFileSource("./05 - Content/content.dat");

            //Start the engine
            m_engine.Start();
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / 60f);
            Engine.TargetFrameTime = (float)TargetElapsedTime.Milliseconds;

            var renderer = new Renderer();
            renderer.Device = m_graphicsDeviceManager.GraphicsDevice;

            Engine.AddComponent(renderer);
            Engine.Renderer = renderer;

            var debug = new DebugManager();
            Engine.AddComponent(debug);
            Engine.Debug = debug;

            var audio = new AudioManager();
            Engine.AddComponent(audio);
            Engine.Audio = audio;

            var music = new MusicManager();
            Engine.AddComponent(music);
            Engine.MusicManager = music;

            var audioStreamManager = new AudioStreamManager();
            Engine.AddComponent(audioStreamManager);
            Engine.AudioStreamManager = audioStreamManager;

            Window.AllowUserResizing = false;

            base.Initialize();
        }

        bool update = false;
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Engine.Log.Debug("Gen 0", GC.CollectionCount(0));
            Engine.Log.Debug("Gen 1", GC.CollectionCount(1));
            Engine.Log.Debug("Gen 2", GC.CollectionCount(2));

            //Engine.Log.Debug("IsRunningSlowly", gameTime.IsRunningSlowly);

            float timeMs = m_frameTimer.TimeMs();
            m_frameTimer.Start();

            if (Engine.FrameCount == 0)
                timeMs = 0;

           // timeMs = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //Engine.Log.Write("Update");
            //Engine.Log.Write("GameTime: " + gameTime.ElapsedGameTime.TotalMilliseconds);
            //Engine.Log.Write("EngineTime: " + timeMs);

            //Engine.Log.Debug("Draw FPS", m_debug.DrawFPS.FPS);

            Engine.Log.Debug("Update FPS", m_debug.UpdateFPS.FPS);
            m_debug.UpdateFPS.Tick((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            Engine.BeginFrame((float)gameTime.ElapsedGameTime.TotalMilliseconds);

            update = true;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            m_debug.DrawFPS.Tick((float)gameTime.ElapsedGameTime.TotalMilliseconds);

            Engine.BeginScene();
            Engine.Render();
            Engine.Debug.Screen.Draw();
            Engine.EndScene();

            if (update == false)
                Engine.Log.Write("No update!!!");

            update = false;
        }

        protected override void EndDraw()
        {
            base.EndDraw();

            Engine.EndFrame();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Engine.Stop();
        }

        public void ExitGame()
        {
            this.Exit();
        }
    }
}
