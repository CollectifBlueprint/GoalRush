#region Using Statements
using System;
using System.Collections.Generic;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using LBE.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LBE;
using LBE.Assets;
using LBE.Physics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using Ball.Gameplay;
using FarseerPhysics.Factories;
using LBE.Script;
using System.Reflection;
using Ball.MainMenu;
using Ball.Gameplay.Arenas;
using Ball.Stats;
using LBE.Audio;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using Ball.Audio;
using Ball.Graphics;
using Ball.Menus;
using LBE.Graphics.Particles;
using Ball.MainMenu.Scripts;
#endregion

namespace Ball
{
    public class GameStartInfo
    {
        //General debug switch
        public bool Release = false;

        //Debug options
        public bool RenderDebug = false;

        //Start
        public bool NoLoading = true;
        public bool NoMenu = false;
        public String DefaultArena;

        //Game  
        public bool NoHuman = false;
        public bool EnableAI = true;
    }

    public class AIGameData
    {
        public float IdleTimeMS = 0;
        public bool IsIdleAIRunning = false;
    }

    public class Game : BaseEngineComponent
    {
        static Game m_instance;
        public static Game Instance {
            get { return Game.m_instance; }
        }

        Asset<ConfigParameters> m_configParameters;
        public static ConfigParameters ConfigParameters {
            get { return m_instance.m_configParameters.Content; }
        }

        StatManager m_statManager;
        public static StatManager Stats {
            get { return m_instance.m_statManager; }
            set { m_instance.m_statManager = value; }
        }

        GameManager m_gameManager;
        public static GameManager GameManager {
            get { return m_instance.m_gameManager; }
            set { m_instance.m_gameManager = value; }
        }

        Arena m_arena;
        public static Arena Arena {
            get { return m_instance.m_arena; }
            set { m_instance.m_arena = value; }
        }

        MenuManager m_menuManager;
        public static MenuManager MenuManager {
            get { return m_instance.m_menuManager; }
        }

        GameStartInfo m_startInfo;
        public GameStartInfo StartInfo {
            get { return m_startInfo; }
        }

        GameSession m_gameSession;
        public static GameSession GameSession {
            get { return m_instance.m_gameSession; }
        }

        GameProfile m_gameProfil;
        public static GameProfile GameProfile {
            get { return m_instance.m_gameProfil; }
        }

        OptionManager m_optionMgr;
        public static OptionManager OptionMgr {
            get { return m_instance.m_optionMgr; }
        }

        GameMusic m_gameMusic;
        public static GameMusic GameMusic {
            get { return m_instance.m_gameMusic; }
        }

        AIGameData m_AIGameData = new AIGameData();
        public static AIGameData AIGameData {
            get { return m_instance.m_AIGameData; }
        }

        ScreenShake m_screenShake;
        ScreenFocus m_screenFocus;

        GameObject m_rootObject;
        KeyControl m_resetCtrl;

        public Game()
        {
            //Add this assemby for reflection
            Engine.AssetManager.TypeDatabase.AddAssembly(typeof(Game).Assembly);

            m_rootObject = new GameObject("Root");
            m_rootObject.Tag = "Root";
            m_instance = this;

            m_screenShake = new ScreenShake();
            m_screenFocus = new ScreenFocus();
        }

        public override void Startup()
        {
            //Registering assemblies needed for scripts
            ScriptAssembly.Assemblies.Add(Assembly.GetExecutingAssembly());
            ScriptAssembly.Assemblies.Add(typeof(Microsoft.Xna.Framework.Vector2).Assembly);
            ScriptAssembly.Assemblies.Add(typeof(FarseerPhysics.Dynamics.Body).Assembly);
            ScriptAssembly.Assemblies.Add(typeof(LBE.Gameplay.GameObject).Assembly);

            //Loading configuration
            m_configParameters = Engine.AssetManager.GetAsset<ConfigParameters>("Game/Config.lua::Config");
            Engine.Renderer.SetResolution(
                m_configParameters.Content.ResolutionX,
                m_configParameters.Content.ResolutionY);

            //Loading starting infos
            m_startInfo = Engine.AssetManager.Get<GameStartInfo>("Game/GameStart.lua::GameStart");
            if (m_startInfo.Release)
                m_startInfo = new GameStartInfo() { Release = true };

            Engine.Debug.Flags.EnableAI = m_startInfo.EnableAI;
            Engine.Debug.Flags.RenderDebug = m_startInfo.RenderDebug;
            Engine.Debug.Flags.EnableCommands = !m_startInfo.Release;

            //Loading player profil
            m_gameProfil = GameProfile.Load();
            if (m_gameProfil == null) m_gameProfil = GameProfile.NewProfile();
            GameProfile.Save(m_gameProfil);

            //Create game session
            m_gameSession = new Ball.GameSession();

            //Creating managers
            m_statManager = new StatManager();
            m_gameManager = new GameManager();
            m_optionMgr = new OptionManager();
            m_menuManager = new MenuManager();
            m_gameMusic = new GameMusic();

            //Reset control
            m_resetCtrl = new KeyControl(Keys.Home);

            //Start the game!!!
            StartGame();
        }

        private void StartGame()
        {
            if (m_startInfo.NoMenu)
            {
                //Start directly to gameplay
                MatchStartInfo info = new MatchStartInfo();
                info.Arena.Name = m_startInfo.DefaultArena;
                info.Arena.ColorScheme = new ColorScheme() { Color1 = Color.White, Color2 = Color.LightGray };
                info.Players[0].InputType = Gameplay.InputType.Gamepad;
                info.Players[0].InputIndex = PlayerIndex.One;
                info.Players[1].InputType = Gameplay.InputType.Keyboard;
                info.Players[1].InputIndex = PlayerIndex.Two;
                info.Teams[0].ColorScheme = new ColorScheme() { Color1 = Color.Crimson, Color2 = Color.Orange };
                info.Teams[1].ColorScheme = new ColorScheme() { Color1 = Color.MediumBlue, Color2 = Color.LightSkyBlue };
                info.Teams[0].ConsecutiveWins = 0;
                info.Teams[1].ConsecutiveWins = 0;

                Game.GameSession.CurrentMatchInfo = info;
                Game.GameManager.StartMatch(info);
            }
            else if (m_startInfo.NoLoading)
            {
                //Start to loading screen
                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/LoadingScreenMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
            }
            else
            {
                //Start to main menu
                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/MainMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
            }
        }

        public override void StartFrame()
        {
            m_optionMgr.Update();

            if (!m_gameManager.Paused)
            {
                m_screenShake.Update();
                m_screenFocus.Update();
            }

            if (!m_startInfo.Release && m_resetCtrl.KeyPressed())
                Reset();

            m_AIGameData.IdleTimeMS += Engine.GameTime.ElapsedMS;

            for (int i = 0; i < 4; i++)
            {
                foreach (var button in Enum.GetValues(typeof(Buttons)))
                {
                    if (Engine.Input.GamePadState((PlayerIndex)i).IsButtonDown((Buttons)button))
                        ResetAiTimerOrQuit();
                }
            }

            if (Engine.Input.KeyboardState().IsKeyDown(Keys.Enter))
                ResetAiTimerOrQuit();
            if (Engine.Input.KeyboardState().IsKeyDown(Keys.Escape))
                ResetAiTimerOrQuit();

            var iaCtrl = new KeyControl(Keys.End);
            if (iaCtrl.KeyPressed())
            {
                if (m_AIGameData.IsIdleAIRunning == false)
                {
                    StartAIDemo();
                }
                else
                {
                    QuitAIDemo();
                }
            }

            float timeDemoS = 60;
            if (m_AIGameData.IdleTimeMS > timeDemoS * 1000 && m_AIGameData.IsIdleAIRunning == false)
            {
                if (Game.MenuManager.CurrentMenu != null && Game.MenuManager.CurrentMenu.Script.GetType() == typeof(AltMainMenuScript))
                    StartAIDemo();
                else
                    Reset();
            }
        }

        private void StartAIDemo()
        {
            if (m_AIGameData.IsIdleAIRunning)
                return;

            m_AIGameData.IsIdleAIRunning = true;
            m_AIGameData.IdleTimeMS = 0;

            Engine.Log.Write("StartAIDemo");

            var colorSchemeDataAsset = Engine.AssetManager.GetAsset<ColorSchemeData>("Game/Colors.lua::ColorShemeArenas");
            var colorSchemeTeamAsset = Engine.AssetManager.GetAsset<ColorSchemeData>("Game/Colors.lua::ColorShemeTeams");

            var matchStartInfo = new MatchStartInfo();
            matchStartInfo.Arena.Name = "ArenaLarge";
            matchStartInfo.Arena.ColorScheme = colorSchemeDataAsset.Content.ColorSchemes[0];

            var iColorIdx1 = Engine.Random.Next(0, 2);
            var iColorIdx2 = Engine.Random.Next(3, 5);
            matchStartInfo.Teams[0].ColorScheme = colorSchemeTeamAsset.Content.ColorSchemes[iColorIdx1];
            matchStartInfo.Teams[1].ColorScheme = colorSchemeTeamAsset.Content.ColorSchemes[iColorIdx2];

            if (Game.MenuManager.CurrentMenu != null)
                Game.MenuManager.QuitMenu();

            Game.GameManager.StartMatch(matchStartInfo);
        }

        private void QuitAIDemo()
        {
            m_AIGameData.IsIdleAIRunning = false;
            m_AIGameData.IdleTimeMS = 0;

            Reset();
        }

        private void ResetAiTimerOrQuit()
        {
            m_AIGameData.IdleTimeMS = 0;
            if (m_AIGameData.IsIdleAIRunning)
                QuitAIDemo();
        }

        public override void Shutdown()
        {
            m_gameMusic.StopAll();
        }

        public void Reset()
        {
            Game.GameMusic.StopAll();

            if (Game.GameManager.Arena != null)
                Game.GameManager.EndMatch();

            foreach (var go in Engine.World.GameObjects)
                go.Kill();

            m_instance.m_rootObject = new GameObject("Root");

            var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/MainMenu.lua::Menu");
            Game.MenuManager.StartMenu(menuDef);

            m_AIGameData.IdleTimeMS = 0;
            m_AIGameData.IsIdleAIRunning = false;
        }
    }
}
