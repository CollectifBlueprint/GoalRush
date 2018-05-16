using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using LBE.Debug;
using LBE.Graphics;
using LBE.Log;
using Microsoft.Xna.Framework;
using LBE.Graphics.Sprites;
using LBE.Input;
using System.Windows.Forms;
using LBE.Gameplay;
using MonoApp;
using LBE.Physics;
using LBE.Audio;

namespace LBE
{
    public class Engine
    {
        static Engine m_instance;
        public static Engine Instance
        {
            get { return Engine.m_instance; }
        }

        IntPtr m_host;
        public static IntPtr Host
        {
            get { return Instance.m_host; }
            set { Instance.m_host = value; }
        }

        List<IEngineComponent> m_components;

        Renderer m_renderer;
        public static Renderer Renderer
        {
            get { return Instance.m_renderer; }
            set { Instance.m_renderer = value; }
        }

        LBE.Core.Application m_application;
        public static LBE.Core.Application Application
        {
            get { return Instance.m_application; }
            set { Instance.m_application = value; }
        }

        PhysicsManager m_physicsManager;
        public static PhysicsManager PhysicsManager
        {
            get { return Instance.m_physicsManager; }
        }

        InputManager m_inputManager;
        public static InputManager Input
        {
            get { return Instance.m_inputManager; }
        }

        LogManager m_logManager;
        public static LogManager Log
        {
            get { return Instance.m_logManager; }
            set { Instance.m_logManager = value; }
        }

        DebugManager m_debugManager;
        public static DebugManager Debug
        {
            get { return Instance.m_debugManager; }
            set { Instance.m_debugManager = value; }
        }

        AudioManager m_audioManager;
        public static AudioManager Audio
        {
            get { return Instance.m_audioManager; }
            set { Instance.m_audioManager = value; }
        }

        MusicManager m_musicManager;
        public static MusicManager MusicManager
        {
            get { return Instance.m_musicManager; }
            set { Instance.m_musicManager = value; }
        }

        AudioStreamManager m_audioStreamManager;
        public static AudioStreamManager AudioStreamManager
        {
            get { return Instance.m_audioStreamManager; }
            set { Instance.m_audioStreamManager = value; }
        }

        AssetManager m_assetManager;
        public static AssetManager AssetManager
        {
            get { return Instance.m_assetManager; }
            set { Instance.m_assetManager = value; }
        }
        
        World m_world;
        public static World World
        {
            get { return Instance.m_world; }
            set { Instance.m_world = value; }
        }

        Random m_random;
        public static Random Random
        {
            get { return Instance.m_random; }
            set { Instance.m_random = value; }
        }

        Form m_form;
        public static Form Form
        {
            get { return Instance.m_form; }
            set { Instance.m_form = value; }
        }

        TimeSource m_timeSource;
        TimeSource m_gameTimeSource;

        float m_timeCoef = 1.0f;
        public static float TimeCoef
        {
            get { return Instance.m_timeCoef; }
            set { Instance.m_timeCoef = value; }
        }

        Clock m_realTime;
        public static Clock RealTime
        {
            get { return Instance.m_realTime; }
            set { Instance.m_realTime = value; }
        }

        Clock m_gameTime;
        public static Clock GameTime
        {
            get { return Instance.m_gameTime; }
            set { Instance.m_gameTime = value; }
        }

        float m_targetFrameTimeMS;
        public static float TargetFrameTime
        {
            get { return Instance.m_targetFrameTimeMS; }
            set { Instance.m_targetFrameTimeMS = value; }
        }

        int m_frameCount;
        public static int FrameCount
        {
            get { return Instance.m_frameCount; }
        }

        bool m_started;
        public bool Started
        {
            get { return m_started; }
        }

        bool m_paused;
        public bool Paused
        {
            get { return m_paused; }
            set { m_paused = value; }
        }

        SmoothValue m_frameRate;

        public Engine()
        {
            m_instance = this;
            m_started = false;
            m_components = new List<IEngineComponent>();

            m_random = new System.Random();

            m_timeSource = new TimeSource();
            m_gameTimeSource = new TimeSource();

            m_realTime = new Clock(m_timeSource);
            m_realTime.Start();

            m_gameTime = new Clock(m_gameTimeSource);
            m_gameTime.Start();

            m_targetFrameTimeMS = 16.6f;

            m_frameRate = new SmoothValue(1000 / m_targetFrameTimeMS, 0.9f);
            m_frameRate.Strength = 0.9f;

            Init();
        }

        public void Init()
        {
            //Create LogManager
            m_logManager = new LogManager();
            AddComponent(m_logManager);

            Engine.Log.Write("Welcome to Little Big Engine");
            Engine.Log.IndentMore();
            Engine.Log.Write("Version: 0.02");
            Engine.Log.IndentLess();

            //Create AssetManager
            m_assetManager = new AssetManager();
            AddComponent(m_assetManager);

            m_assetManager.TypeDatabase.AddAssembly(typeof(Engine).Assembly);
            m_assetManager.TypeDatabase.AddAssembly(typeof(Vector2).Assembly);
            m_assetManager.TypeDatabase.AddAssembly(typeof(FarseerPhysics.Dynamics.Body).Assembly);

            //Create InputManager
            m_inputManager = new InputManager();
            AddComponent(m_inputManager);

            //Create World
            m_world = new World();
            AddComponent(m_world);

            //Create Physic Simulation
            m_physicsManager = new PhysicsManager();
            AddComponent(m_physicsManager);
        }

        public void Start()
        {
            m_frameCount = 0;
            m_started = true;
        }

        public void BeginFrame(float elapsedMS)
        {
            //Update stats
            m_frameCount++;
            m_frameRate.Update(1000/elapsedMS);

            if (Engine.FrameCount % 4 == 0)
                Engine.Log.Debug("Framerate", m_frameRate.Value.ToString("0.00") + " fps");

            float timeCoef = m_timeCoef;
            if (Engine.Debug.Flags.SlowPhysics)
                timeCoef *= 0.1f;

            if (m_paused)
                timeCoef *= 0;

            //Update time sources
            m_timeSource.TickMS(elapsedMS);
            m_gameTimeSource.TickMS(elapsedMS * timeCoef);            

            //Update components
            foreach (var engineCmp in m_components)
            {
                if (!engineCmp.Started)
                {
                    engineCmp.Startup();
                    engineCmp.Started = true;
                }
                engineCmp.StartFrame();
            }
        }

        public void BeginScene()
        {
            m_renderer.BeginScene();
        }

        public void Render()
        {
            m_renderer.Render();
        }

        public void EndScene()
        {
            m_renderer.EndScene();
        }

        public void EndFrame()
        {
            foreach (var engineCmp in m_components)
            {
                engineCmp.EndFrame();
            }
        }

        public void Stop()
        {
            m_started = false;
            foreach (var engineCmp in m_components)
            {
                engineCmp.Shutdown();
            }
        }

        public void AddComponent(IEngineComponent component)
        {
            AddComponent(component, false);
        }

        public void AddComponentDelayed(IEngineComponent component)
        {
            AddComponent(component, true);
        }

        private void AddComponent(IEngineComponent component, bool delayStartUp)
        {
            Engine.Log.Write("Adding sub-system:");
            Engine.Log.IndentMore();
            Engine.Log.Write("* " + component.GetType().Name);
            Engine.Log.IndentMore();
            m_components.Add(component);
            if (!delayStartUp)
            {
                component.Startup();
                component.Started = true;
            }
            Engine.Log.IndentLess();
            Engine.Log.IndentLess();
        }
    }
}
