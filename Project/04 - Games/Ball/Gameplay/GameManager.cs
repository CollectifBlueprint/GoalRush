using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using Ball.Gameplay.Arenas;
using Microsoft.Xna.Framework;
using LBE;
using LBE.Physics;
using LBE.Graphics.Sprites;
using Ball.Gameplay.Navigation;
using Ball.Gameplay.Arenas.Objects;
using Ball.Gameplay.Players.AI;
using Ball.Gameplay.Players;
using Ball.Gameplay.BallEffects;
using LBE.Audio;
using LBE.Assets;

namespace Ball.Gameplay
{
    public class GameManager
    {
        MatchStartInfo m_matchStartInfo;
        public MatchStartInfo MatchStartInfo
        {
            get { return m_matchStartInfo; }
        }

        Arena m_arena;
        public Arena Arena
        {
            get { return m_arena; }
        }

        NavigationManager m_navigation;
        public NavigationManager Navigation
        {
            get { return m_navigation; }
        }

        Player[] m_players;
        public Player[] Players
        {
            get { return m_players; }
        }

        Team[] m_teams;
        public Team[] Teams
        {
            get { return m_teams; }
        }

        Ball m_ball;
        public Ball Ball
        {
            get { return m_ball; }
            set { m_ball = value; }
        }

        GameCamera m_camera;
        public GameCamera Camera
        {
            get { return m_camera; }
        }

        Tutorial m_tutorial;
        public Tutorial Tutorial
        {
            get { return m_tutorial; }
        }
        
        MatchFeedback m_matchFeedBack;
        public MatchFeedback MatchFeedBack
        {
            get { return m_matchFeedBack; }
        }

        int[] m_score;
        public int[] Score
        {
            get { return m_score; }
        }

        Match m_match;
        public Match Match
        {
            get { return m_match; }
        }

        bool m_paused;
        public bool Paused
        {
            get { return m_paused; }
        }

        Timer m_launcherSelectionDelayTimer;
        Timer m_launcherSelectionFeedbackTimer;

        GameObject m_matchObject;

        public GameManager()
        {
            m_launcherSelectionFeedbackTimer = new Timer(Engine.GameTime.Source, 1500);
            m_launcherSelectionFeedbackTimer.OnTime += new TimerEvent(m_launcherSelectionFeedbackTimer_OnTime);

            m_launcherSelectionDelayTimer = new Timer(Engine.GameTime.Source, 700);
            m_launcherSelectionDelayTimer.OnTime += new TimerEvent(m_launcherSelectionDelayTimer_OnTime);
        }

        public void StartMatch(MatchStartInfo info)
        {
            if (m_matchObject != null)
                EndMatch();

            m_matchObject = new GameObject("Match");

            Engine.World.EventManager.AddListener((int)EventId.HalfTime, OnHalfTime);
            Engine.World.EventManager.AddListener((int)EventId.HalfTimeTransition, OnHalfTimeTransition);
            Engine.World.EventManager.AddListener((int)EventId.SecondPeriod, OnSecondPeriod);
            Engine.World.EventManager.AddListener((int)EventId.MatchEnd, OnMatchEnd);
            Engine.World.EventManager.AddListener((int)EventId.Victory, OnMatchVictory);
            Engine.World.EventManager.AddListener((int)EventId.MatchFinalize, OnMatchFinalize);
            Engine.World.EventManager.AddListener((int)EventId.Goal, OnGoal);
            Engine.World.EventManager.AddListener((int)EventId.KickOff, OnKickOff);
            Engine.World.EventManager.AddListener((int)EventId.LauncherShot, OnLauncherShot);

            Game.GameSession.StartMatch(info);

            m_matchStartInfo = info;
            m_arena = new Arena(m_matchStartInfo.Arena);
            m_matchObject.Attach(m_arena);

            m_score = new int[2];

            m_navigation = new NavigationManager();
            m_navigation.Init();
            m_matchObject.Attach(m_navigation);

            CreateTeams();
            CreatePlayers();
            CreateBall();

            SetColors();

            Arena.Init();
            InitPlayers();

            CreateCamera();

            CreateMatch();

            m_tutorial = new Tutorial();
            m_matchObject.Attach(m_tutorial);
            m_tutorial.Enabled = false;

            m_matchFeedBack = new MatchFeedback();
            m_matchObject.Attach(m_matchFeedBack);

            Game.GameMusic.StopAll();
			Game.GameMusic.PlayMatchMusic();
        }

        public bool IsAIGame()
        {
            bool AIGame = true;
            foreach (var player in m_players)
            {
                if (player.PlayerInfo.InputType != InputType.AI)
                {
                    AIGame = false;
                    break;
                }
            }
            return AIGame;
        }

        public void EndMatch()
        {

            if (!IsAIGame())
            {
                Game.GameProfile.Stats.GamePlayed++;
                Game.GameProfile.Stats.GameTimeMinutes += m_match.Time() / (60 * 1000.0f);
                Game.GameProfile.CommitChanges();
            }


            foreach (var obj in Engine.World.GameObjects)
            {
                if (obj.Tag == "Gameplay")
                    obj.Kill();
            }
            m_matchObject = null;

            m_arena = null;

          
            if (m_paused)
                UnpauseMatch();

            Engine.World.EventManager.RemoveAllListeners();

            //m_matchFeedBack.EndAllFeedback
        }

        public void PauseMatch()
        {
            if (!m_paused)
            {
                m_paused = true;
                Engine.GameTime.Source.Paused = true;
                Engine.GameTime.Pause();

                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/PauseMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
            }
        }

        public void UnpauseMatch()
        {
            if (m_paused)
            {
                m_paused = false;
                Engine.GameTime.Start();
                Engine.GameTime.Source.Paused = false;
            }
        }

        void CreateCamera()
        {
            GameObject cameraObj = new GameObject("Camera");
            m_camera = new Gameplay.GameCamera();
            m_camera.SetCamZoneBoundaries(m_arena.Description.Size.X * 0.5f, m_arena.Description.Size.Y * 0.5f);
            cameraObj.Attach(m_camera);

            if (Game.ConfigParameters.AutoFocus)
                m_camera.FocusAuto();
            else 
                m_camera.Focus(m_ball.Owner);

            m_camera.DisableZoomUpdate = !(Game.ConfigParameters.AutoZoom);

            cameraObj.Attach(new AudioListener());
        }

        void CreateBall()
        {
            GameObject ballObj = new GameObject("Ball");
            if (Arena.Description.FirstHalfStartInfo != null && !Arena.Description.FirstHalfStartInfo.UseBallLauncher)
                ballObj.Position = Arena.Description.FirstHalfStartInfo.BallPosition;

            m_ball = new Gameplay.Ball();
            ballObj.Attach(m_ball); 
        }


        private void SetColors()
        {

            Asset<ColorSchemeData> colorSchemeBodiesAsset = Engine.AssetManager.GetAsset<ColorSchemeData>("Game/Colors.lua::ColorShemePlayerBodies");

            Color[] bodyColors = new Color[] { colorSchemeBodiesAsset.Content.ColorSchemes[0].Color1, colorSchemeBodiesAsset.Content.ColorSchemes[1].Color1, colorSchemeBodiesAsset.Content.ColorSchemes[2].Color1 };
            Color[] playerColors = new Color[] { m_teams[0].ColorScheme.Color1, m_teams[0].ColorScheme.Color2, m_teams[1].ColorScheme.Color1, m_teams[1].ColorScheme.Color2 };
            Color[] teamColors = new Color[] { m_teams[0].ColorScheme.Color1, m_teams[0].ColorScheme.Color1, m_teams[1].ColorScheme.Color1, m_teams[1].ColorScheme.Color1 };
            Color[] body2Colors = new Color[] { colorSchemeBodiesAsset.Content.ColorSchemes[0].Color2, colorSchemeBodiesAsset.Content.ColorSchemes[1].Color2, colorSchemeBodiesAsset.Content.ColorSchemes[2].Color2 };

            for (int i = 0; i < m_players.Length; i++)
            {
                m_players[i].SetColor(Player.ColorElement.MakeUp, playerColors[i]);
                m_players[i].SetColor(Player.ColorElement.Team, teamColors[i]);
                if (m_players[i].Team.ConsecutiveWins > 0)
                {
                    m_players[i].SetColor(Player.ColorElement.Body, bodyColors[2]);
                    m_players[i].SetColor(Player.ColorElement.Body2, bodyColors[2]);
                }
                else
                {
                    m_players[i].SetColor(Player.ColorElement.Body, bodyColors[(int)m_players[i].Team.TeamID]);
                    m_players[i].SetColor(Player.ColorElement.Body2, bodyColors[(int)m_players[i].Team.TeamID]);
                }
                //m_players[i].ResetColors();
            }
        }

        private void SetPlayersInitialPos(bool invert = false)
        {        
            ArenaStartInfo arenaStartInfo =  Arena.Description.FirstHalfStartInfo;
            if (Match != null && Match.MatchState >= MatchState.HalfTime)
                arenaStartInfo =  Arena.Description.SecondHalfStartInfo;

            if (arenaStartInfo != null && arenaStartInfo.PlayerPosition != null)
            {
				for (int i = 0; i < 4; i++) 
				{
					m_players [i].InitialPosition = arenaStartInfo.PlayerPosition[i];
					m_players[i].Owner.RigidBodyCmp.SetPosition (arenaStartInfo.PlayerPosition[i]);
				}

            }
            else
            {
                float sign = invert ? -1 : 1;
                float playerX = 120 * sign;
                float playerY = 80 * sign;  
                Vector2[] defaultPlayerPos = new Vector2[4] { 
                new Vector2(-playerX, playerY),
                new Vector2(-playerX, -playerY),
                new Vector2(playerX, playerY),
                new Vector2(playerX, -playerY)
                }; 
            
				for (int i = 0; i < 4; i++) 
				{
					m_players [i].InitialPosition = defaultPlayerPos[i];
					m_players [i].Owner.RigidBodyCmp.SetPosition (defaultPlayerPos[i]);
				}

            }
        }

        private void CreateTeams()
        {
            m_teams = new Team[2];
            m_teams[(int)TeamId.TeamOne] = new Team() { TeamID = TeamId.TeamOne, ColorScheme = m_matchStartInfo.Teams[0].ColorScheme, ConsecutiveWins = m_matchStartInfo.Teams[0].ConsecutiveWins };
            m_teams[(int)TeamId.TeamTwo] = new Team() { TeamID = TeamId.TeamTwo, ColorScheme = m_matchStartInfo.Teams[1].ColorScheme, ConsecutiveWins = m_matchStartInfo.Teams[1].ConsecutiveWins };
        }

        private void CreatePlayers()
        {

            m_players = new Player[4];

            m_players[0] = new Player(m_matchStartInfo.Players[0], m_teams[(int)TeamId.TeamOne], "Game/Player.lua::PlayerHuman1");
            m_players[1] = new Player(m_matchStartInfo.Players[1], m_teams[(int)TeamId.TeamOne], "Game/Player.lua::PlayerHuman2");
            m_players[2] = new Player(m_matchStartInfo.Players[2], m_teams[(int)TeamId.TeamTwo], "Game/Player.lua::PlayerAI1");
            m_players[3] = new Player(m_matchStartInfo.Players[3], m_teams[(int)TeamId.TeamTwo], "Game/Player.lua::PlayerAI2");
        
            GameObject playerObj = new GameObject("Player 1");
            playerObj.Attach(m_players[0]);

            playerObj = new GameObject("Player 2");
            playerObj.Attach(m_players[1]);

            playerObj = new GameObject("Player 3");
            playerObj.Attach(m_players[2]);

            playerObj = new GameObject("Player 4");
            playerObj.Attach(m_players[3]);

            m_teams[0].Players = new Player[] { m_players[0], m_players[1] };
            m_teams[1].Players = new Player[] { m_players[2], m_players[3] };

            m_players[0].TeamMate = m_players[1];
            m_players[1].TeamMate = m_players[0];
            m_players[2].TeamMate = m_players[3];
            m_players[3].TeamMate = m_players[2];

            m_players[0].Opponents = new Player[] { m_players[2], m_players[3] };
            m_players[1].Opponents = new Player[] { m_players[2], m_players[3] };
            m_players[2].Opponents = new Player[] { m_players[0], m_players[1] };
            m_players[3].Opponents = new Player[] { m_players[0], m_players[1] };
        }

        void InitPlayers()
        {
            for (int i = 0; i < 4; i++)
            {
                m_players[i].Init();
                if (m_matchStartInfo.Players[i].InputType == InputType.AI || Game.Instance.StartInfo.NoHuman)
                {
                    var controller = new PlayerAIController(m_players[i]);
                    m_players[i].SetController(controller);
                }
                else
                {
                    var input = PlayerInput.CreateHuman(m_matchStartInfo.Players[i].InputType, m_matchStartInfo.Players[i].InputIndex);
                    var controller = new PlayerHumanController(m_players[i], input);
                    m_players[i].SetController(controller);
                }
            }
            SetPlayersInitialPos();
        }

        void CreateMatch()
        {
            GameObject matchObj = new GameObject("Match");
            m_match = new Gameplay.Match();
            matchObj.Attach(m_match);
        }

        public void OnHalfTime(object eventParamater)
        {
            foreach (Player player in m_players)
            {
                player.Owner.RigidBodyCmp.Body.LinearVelocity = Vector2.Zero;
            }

            foreach (LauncherComponent launcher in Arena.Launchers)
            {
                launcher.Enabled = false;
            }
               
            m_launcherSelectionFeedbackTimer.Stop();
            m_launcherSelectionDelayTimer.Stop ();

            if (Game.ConfigParameters.AutoFocus)
                Camera.FocusAuto();
            else 
                Camera.Focus(m_ball.Owner);            
        }

        public void OnHalfTimeTransition(object eventParamater)
        {
            m_ball.Owner.RigidBodyCmp.Body.Enabled = true;
            m_ball.Owner.RigidBodyCmp.Body.LinearVelocity = Vector2.Zero;
            m_ball.BallSprite.Alpha = 255;

            if (Arena.Description.SecondHalfStartInfo != null && !Arena.Description.SecondHalfStartInfo.UseBallLauncher)
                m_ball.Owner.Position = Arena.Description.SecondHalfStartInfo.BallPosition;
            else
                m_ball.Owner.Position = Vector2.Zero;
            
            m_ball.SetSlowMode(false);
            m_ball.Free();
            m_ball.Reset();

            SpriteComponent ballSprite = m_ball.Owner.FindComponent<SpriteComponent>();
            ballSprite.Visible = true;

            SetPlayersInitialPos(true);
            foreach (Player player in m_players)
            {
                player.BallTrigger.Enabled = true;
                player.MagnetSensor.Enabled = true;
            }

            foreach (LauncherComponent launcher in Arena.Launchers)
            {
                launcher.Enabled = true;
            }

        }

        public void OnSecondPeriod(object eventParamater)
        {
            
        }


        public void OnMatchEnd(object eventParamater)
        {
            foreach (Player player in m_players)
            {
                player.Owner.RigidBodyCmp.Body.LinearVelocity = Vector2.Zero;
            }

            foreach (LauncherComponent launcher in Arena.Launchers)
            {
                launcher.Enabled = false;
            }
        }


        public void OnMatchVictory(object eventParamater)
        {
            m_ball.Owner.RigidBodyCmp.SetPosition(Vector2.Zero);
            m_ball.Owner.RigidBodyCmp.Body.Enabled = false;
            SpriteComponent ballSprite = m_ball.Owner.FindComponent<SpriteComponent>();
            ballSprite.Visible = false;

            foreach (Player player in m_players)
            {
                player.ResetState();
            }

            if (m_arena.LeftGoal.Score == m_arena.RightGoal.Score)
            {
                Vector2[] playerPos = new Vector2[] {
                    new Vector2(-120, 100), new Vector2(-60, 100),
                    new Vector2(60, 100), new Vector2(120, 100) };

                foreach (Player player in m_players)
                {
                    player.Owner.RigidBodyCmp.SetPosition(playerPos[(int)player.PlayerIndex]);
                }
            }
            else
            {
                Team defeatedTeam;
                if (m_arena.LeftGoal.Score > m_arena.RightGoal.Score)
                {
                    defeatedTeam = m_arena.LeftGoal.Team;
                }
                else
                {
                    defeatedTeam = m_arena.RightGoal.Team;
                }

                Vector2[] playerPos = new Vector2[] {
                    new Vector2(120, 100), new Vector2(-120, 100),
                    new Vector2(200, 200), new Vector2(-200, 200) };

                foreach (Player player in m_players)
                {
                    if (player.Team == defeatedTeam)
                    {
                        player.Owner.RigidBodyCmp.Body.Enabled = false;
                        player.Properties.ControlDisabled.Set();
                        SpriteComponent playerSprite = player.Owner.FindComponent<SpriteComponent>();
                        playerSprite.Visible = false;
                        player.ShadowCmp.Visible = false;

                    }

                    player.Owner.RigidBodyCmp.SetPosition(playerPos[(int)player.PlayerIndex]);
                }
            }

            //Camera.Focus(Arena.CupCamFocus);
            Camera.FocusAuto();            
        }

        public void OnMatchFinalize(object eventParamater)
        {
            Game.GameMusic.StopAll();

            var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/MatchEndMenu.lua::Menu");
            Game.MenuManager.StartMenu(menuDef);
        }

        public void OnGoal(object eventParamater)
        {
            if (m_ball.Player != null)
            {
                m_ball.Player.DettachBall();
            }

            m_ball.Properties.Untakable.Set();
            
            foreach (Player player in m_players)
            {
                player.BallTrigger.Enabled = false;
                player.MagnetSensor.Enabled = false;
            }
        }

        public void OnKickOff(object eventParameter)
        {
            if (!(Game.GameManager.Match.MatchState == MatchState.FirstPeriod
                || Game.GameManager.Match.MatchState == MatchState.SecondPeriod))
                return;

            m_ball.Reset();
            m_ball.Free();
            m_ball.BodyCmp.Body.LinearVelocity = Vector2.Zero;
            m_ball.SetSlowMode(false);
            m_ball.BodyCmp.Body.Enabled = true;
            SpriteComponent ballSprite = m_ball.Owner.FindComponent<SpriteComponent>();
            ballSprite.Visible = true;

            Vector2 ballSpawnPos = Vector2.Zero;
            Vector2 launcherDir = Vector2.Zero;

            if (Arena.Launchers.Count != 0)
            {
                m_ball.Reset();
                m_ball.BodyCmp.Body.Enabled = false;
                BallFadeEffect ballFadeEffect = new BallFadeEffect();
                ballFadeEffect.SetDuration(500);
                m_ball.AddEffect(ballFadeEffect);

                Team team = (Team)((object[])eventParameter)[0];
                Arena.SelectALauncher(team);

                m_launcherSelectionDelayTimer.Start();

                m_ball.Properties.Untakable.Set();
            }
          

            foreach (Player player in m_players)
            {
                player.BallTrigger.Enabled = true;
                player.MagnetSensor.Enabled = true;
            }
        }

        public void OnLauncherShot(object eventParameter)
        {
            if (Game.ConfigParameters.AutoFocus)
                Camera.FocusAuto();
            else
                Camera.Focus(m_ball.Owner);            
        }
        

        void m_launcherSelectionFeedbackTimer_OnTime(Timer source)
        {
            if (Arena.SelectedLauncher == null)
            {
                Engine.Log.Write("No laucnher selected!");
                return;
            }

            Arena.SelectedLauncher.Unselect();
            Arena.SelectedLauncher.Launch();

            Arena.SelectedLauncher = null;
            m_ball.Properties.Untakable.Unset();
        }

        void m_launcherSelectionDelayTimer_OnTime(Timer source)
        {
            Arena.SelectedLauncher.Select();
            GameObjectComponent launcher = (GameObjectComponent)Arena.SelectedLauncher;
            Camera.Focus(launcher.Owner);
            m_launcherSelectionFeedbackTimer.Start();
        }
    }
}
