using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using LBE;
using LBE.Assets;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using LBE.Physics;
using LBE.Script;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ball.Gameplay.Arenas.Objects;
using System.Collections.Generic;
using Ball.Stats;
using Ball.Physics;
using Ball.Graphics;
using LBE.Input;
using Microsoft.Xna.Framework.Input;
using LBE.Debug;
using LBE.Utils;

namespace Ball.Gameplay.Arenas
{
    public partial class Arena : GameObjectComponent
    {
        String m_arenaId;

        Asset<ArenaPreview> m_preview;
        public ArenaPreview Preview
        {
            get { return m_preview.Content; }
        }

        Asset<ArenaDescription> m_description;
        public ArenaDescription Description
        {
            get { return m_description.Content; }
        }

        ArenaScript m_script;

        Goal m_leftGoal;
        public Goal LeftGoal
        {
            get { return m_leftGoal; }
            protected set { m_leftGoal = value; }
        }

        Goal m_rightGoal;
        public Goal RightGoal
        {
            get { return m_rightGoal; }
            protected set { m_rightGoal = value; }
        }

        Vector2 m_arenaHalfSize;
        public Vector2 ArenaHalfSize
        {
            get { return m_arenaHalfSize; }
            protected set { m_arenaHalfSize = value; }
        }

        List<IBallLauncher> m_launchers;
        public List<IBallLauncher> Launchers
        {
            get { return m_launchers; }
        }

        IBallLauncher m_selectedLauncher;
        public IBallLauncher SelectedLauncher
        {
            get { return m_selectedLauncher; }
            set { m_selectedLauncher = value; }
        }

        LDSettings m_lDSettings;
        public LDSettings LDSettings
        {
            set { m_lDSettings = value; }
            get { return m_lDSettings; }
        }


        SpriteFont m_font;

        SpriteComponent m_overlay;
        public SpriteComponent Overlay
        {
            get { return m_overlay; }
        }

        SpriteComponent m_overlayAlt;
        public SpriteComponent OverlayAlt
        {
            get { return m_overlayAlt; }
        }

        ColorScheme m_colorScheme;
        public ColorScheme ColorScheme
        {
            get { return m_colorScheme; }
        }
        
        public Arena(String arenaId) 
            : this(arenaId, new ColorScheme() {Color1 = Color.White, Color2 = Color.White})
        {
        }

        public Arena(ArenaInfo arenaInfo) 
            : this(arenaInfo.Name, arenaInfo.ColorScheme)
        {
        }

        public Arena(String arenaId, ColorScheme colorSheme)
        {
            m_arenaId = arenaId;
            m_colorScheme = colorSheme;
            m_launchers = new List<IBallLauncher>();
            m_lDSettings = new LDSettings { LaunchersSelection = LaunchersSelectionType.Random };
        }

        public override void Start()
        {
            Game.Arena = this;

            Engine.World.EventManager.AddListener((int)EventId.HalfTimeTransition, OnHalfTimeTransition);
            Engine.World.EventManager.AddListener((int)EventId.SecondPeriodBegin, OnSecondPeriodBegin);

            m_preview = Engine.AssetManager.GetAsset<ArenaPreview>("Arenas/" + m_arenaId + "/Arena.lua::Preview");
            m_description = Engine.AssetManager.GetAsset<ArenaDescription>("Arenas/" + m_arenaId + "/Arena.lua::Description");

            Owner.Name = m_arenaId;

            if (m_description.Content.LD != null)
            {
                Asset<LDDefinition> lDObjDef = Engine.AssetManager.GetAsset<LDDefinition>("Arenas/" + m_arenaId + "/Arena.lua::LD");

                for (int i = 0; i < lDObjDef.Content.LDObjects.Length; i++)
                {
                    Asset<GameObjectInstance> GOInstance = new Asset<GameObjectInstance>(lDObjDef.Content.LDObjects[i]);
                    Asset<GameObjectDefinition> GODefinition = new Asset<GameObjectDefinition>(GOInstance.Content.GameObjectDefinition);
                    GameObject go = new GameObject(GODefinition);
                    go.Position = GOInstance.Content.Position;
                    go.Orientation = GOInstance.Content.Orientation;
                    go.Name = GOInstance.Content.Name;
                }
            }

            
            if (m_description.Content.LDSettings != null)
            {
                Asset<LDSettings> lDSettings = Engine.AssetManager.GetAsset<LDSettings>("Arenas/" + m_arenaId + "/Arena.lua::LDSettings");
                m_lDSettings.LaunchersSelection = lDSettings.Content.LaunchersSelection;
            }
            

            m_preview.OnAssetChanged += new OnChange(Reset);
            //m_description.OnAssetChanged += new OnChange(Reset);


            Reset();

            //Init();
        }

        public void Reset()
        {
            var scriptRef = m_description.Content.Script;
            m_script = scriptRef.Clone();
            m_script.Arena = this;
        }

        public override void End()
        {
            m_script.OnEnd();
        }

        public void Init()
        {
            Owner.Attach(new RigidBodyComponent());

            m_font = Engine.AssetManager.Get<SpriteFont>("Graphics/DisplayFont");

            if (m_preview.Content.Size == ArenaSize.Large)
            {
                m_arenaHalfSize = new Vector2(800, 320);
            }
            else if (m_preview.Content.Size == ArenaSize.Normal)
            {
                m_arenaHalfSize = new Vector2(480, 320);
            }

            InitGoals();
            InitGeometry();
            InitPhysics();
            InitLaunchers();

            if (m_description.Content.Material != null && m_description.Content.AO != null)
            {
                ColorMaskedSprite arenaMask = new ColorMaskedSprite(Sprite.CreateFromTexture(m_description.Content.AO), "Background");
                arenaMask.Sprite.Scale = Vector2.One * m_description.Content.Scale;
                arenaMask.Mask = m_description.Content.Material;
                arenaMask.Color1 = m_colorScheme.Color2;
                arenaMask.Color2 = Game.GameManager.Teams[0].ColorScheme.Color1.SetSaturation(0.5f);
                arenaMask.Color3 = Game.GameManager.Teams[1].ColorScheme.Color1.SetSaturation(0.5f);
                arenaMask.Color4 = m_colorScheme.Color1;

                Owner.Attach(arenaMask);           
            }
            
            // TODO del
            else 
            {
                if (m_description.Content.Background != null)
                    Owner.Attach(new SpriteComponent(Sprite.CreateFromTexture(m_description.Content.Background)));

                if (m_description.Content.Overlay != null)
                 {
                    m_overlay = new SpriteComponent(Sprite.CreateFromTexture(m_description.Content.Overlay), "GroundOverlay0");
                    Owner.Attach(m_overlay);
                }

                if (m_description.Content.OverlayAlt != null)
                {
                    m_overlayAlt = new SpriteComponent(Sprite.CreateFromTexture(m_description.Content.OverlayAlt), "GroundOverlay0");
                    Owner.Attach(m_overlayAlt);
                    m_overlayAlt.Visible = false;
                }
            }

            InitTimerAndScore();

            //InitCup();
        }


        void InitPhysics()
        {
            Owner.RigidBodyCmp.Body.IgnoreGravity = true;
            Owner.RigidBodyCmp.Body.BodyType = BodyType.Static;
            Owner.RigidBodyCmp.Body.Restitution = 0.5f;
            Owner.RigidBodyCmp.Body.Friction = 0.0f;
        }

        void InitGeometry()
        {
            var rbCmp = Owner.RigidBodyCmp;
            rbCmp.Body.CollisionCategories = (Category)CollisionType.Wall;

			if (m_description.Content.Geometry != null)
			{
				foreach (var colDef in m_description.Content.Geometry.Entries) 
				{
					if (colDef.Vertices.Length <= 2)
						continue;

					rbCmp.AddCollisionPolygon(colDef);
				}
			}
				
            m_script.OnInitGeometry();                
        }

        void InitGoals()
        {
            float width = ConvertUnits.ToSimUnits(m_arenaHalfSize.X);
            float height = ConvertUnits.ToSimUnits(m_arenaHalfSize.Y);
            float goalWidth = ConvertUnits.ToSimUnits(100);
            float goalHeight = ConvertUnits.ToSimUnits(80);

            float goalOffset = ConvertUnits.ToSimUnits(30);

            GameObject goalObj = new GameObject("Left Goal");
            goalObj.Position = new Vector2(-ConvertUnits.ToDisplayUnits(width) - ConvertUnits.ToDisplayUnits(goalWidth) * 0.5f - goalOffset * 2, 0);
            m_leftGoal = new Goal(Game.GameManager.Teams[(int)TeamId.TeamOne]);
            m_leftGoal.Size = new Vector2(goalWidth - goalOffset, goalHeight * 2);
            m_leftGoal.Index = 1;
            goalObj.Attach(m_leftGoal);

            goalObj = new GameObject("Right Goal");
            goalObj.Position = new Vector2(ConvertUnits.ToDisplayUnits(width) + ConvertUnits.ToDisplayUnits(goalWidth) * 0.5f + goalOffset * 2, 0);
            m_rightGoal = new Goal(Game.GameManager.Teams[(int)TeamId.TeamTwo]);
            m_rightGoal.Size = new Vector2(goalWidth - goalOffset, goalHeight * 2);
            m_rightGoal.Index = 0;
            goalObj.Attach(m_rightGoal);
        }

        void InitLaunchers()
        {
            foreach (IBallLauncher launcher in m_launchers)
            {
                if (launcher.GetType() == typeof(AssistBallLauncherComponent))
                {
                    AssistBallLauncherComponent teamLauncher = (AssistBallLauncherComponent) launcher;
                    if (Vector2.DistanceSquared(teamLauncher.GetWorldTransform().Position, m_leftGoal.Position)
                        < Vector2.DistanceSquared(teamLauncher.GetWorldTransform().Position, m_rightGoal.Position))
                    {
                        teamLauncher.Team = m_leftGoal.Team;
                    }
                    else
                    {
                        teamLauncher.Team = m_rightGoal.Team;
                    }

                }
            }
        }

        public override void Update()
        {
            m_script.OnUpdate();

            UpdateTimerAndScore();

            if (Engine.Debug.Flags.ColorEdit == true)
                DebugColorEdit();

        }
        
        public void OnHalfTimeTransition(object eventParameter)
        {
            SetTextDisplayVisible(false);
            SwitchLaunchers();
            SwitchTeamSides();
            SwitchGoals();
            //SwitchCups();
        }


        public void OnSecondPeriodBegin(object eventParameter)
        {
            SetTextDisplayVisible(true);
        }


        private void SetTextDisplayVisible(bool visible)
        {
            m_leftTimerTextCmp.Visible = visible;
            m_rightTimerTextCmp.Visible = visible;
            m_dashTimerTextCmp.Visible = visible;

            m_leftScoreTextCmp.Visible = visible;
            m_rightScoreTextCmp.Visible = visible;
            m_scoreDashTextCmp.Visible = visible;
        }


        public void SelectALauncher(Team team)
        {
            m_selectedLauncher = null;
            
            
            if (m_lDSettings.LaunchersSelection == LaunchersSelectionType.Random)
            {

                int randomIndex = Engine.Random.Next(0, m_launchers.Count);
                m_selectedLauncher = m_launchers[randomIndex];

                m_selectedLauncher.Scan();

            }

            else if (m_lDSettings.LaunchersSelection == LaunchersSelectionType.KickOffTeam)
            {
                m_selectedLauncher = Game.Arena.Launchers[0];
                float shortestPlayerDist = float.MaxValue;

                for (int i = 0; i < Game.Arena.Launchers.Count; i++)
                {
                    AssistBallLauncherComponent launcher = (AssistBallLauncherComponent)Game.Arena.Launchers[i];

                    if (launcher.Team != team)
                        continue;

                    launcher.Scan();
                    if (launcher.PlayerToAim != null)
                    {
                        LBE.Core.Transform launcherParent = new LBE.Core.Transform(launcher.Owner.Position, launcher.Owner.Orientation);
                        LBE.Core.Transform launcherWorld = launcherParent.Compose(launcher.Transform);

                        float launcherToPlayerDist = Vector2.Distance(launcherWorld.Position, launcher.PlayerToAim.Position);

                        if (launcherToPlayerDist < shortestPlayerDist)
                        {
                            shortestPlayerDist = launcherToPlayerDist;
                            m_selectedLauncher = launcher;
                        }
                    }
                }
            }

            else if (m_lDSettings.LaunchersSelection == LaunchersSelectionType.Central)
            {
                m_selectedLauncher = Game.Arena.Launchers[0];

                int randomIndex = Engine.Random.Next(0, m_launchers.Count);
                m_selectedLauncher = m_launchers[randomIndex];

                for (int i = 0; i < Game.Arena.Launchers.Count; i++)
                {
                    CentralBallLauncherComponent launcher = (CentralBallLauncherComponent)Game.Arena.Launchers[i];

                    launcher.Team = team;
                    Vector2 goalPosition;
                    if (launcher.Team == m_leftGoal.Team)
                    {
                        goalPosition = m_leftGoal.Position;
                    }
                    else
                    {
                        goalPosition = m_rightGoal.Position;
                    }

                    Vector2 launcherToGoalDir = goalPosition - launcher.GetWorldTransform().Position;
                    launcherToGoalDir.Normalize();
                    launcher.Owner.Orientation = launcherToGoalDir.Angle();

                    launcher.Scan();
                }
            }
        }


        public void SwitchLaunchers()
        {
            foreach (IBallLauncher launcher in m_launchers)
            {
                if (launcher.GetType() == typeof(AssistBallLauncherComponent))
                {
                    AssistBallLauncherComponent teamLauncher = (AssistBallLauncherComponent)launcher;
                    if (teamLauncher.Team == m_leftGoal.Team)
                        teamLauncher.Team = m_rightGoal.Team;
                    else
                        teamLauncher.Team = m_leftGoal.Team;
                }
            }
        }

        public void SwitchTeamSides()
        {
            ColorMaskedSprite arenaMaskedSprite = Owner.FindComponent<ColorMaskedSprite>();
            Color tmp = arenaMaskedSprite.Color2;
            arenaMaskedSprite.Color2 = arenaMaskedSprite.Color3;
            arenaMaskedSprite.Color3 = tmp;
        }

        public void SwitchGoals()
        {
            Team team = LeftGoal.Team;
            LeftGoal.Team = RightGoal.Team;
            RightGoal.Team = team;

            int score = LeftGoal.Score;
            LeftGoal.Score = RightGoal.Score;
            RightGoal.Score = score;

            int scoreDisplay = LeftGoal.ScoreDisplay;
            LeftGoal.ScoreDisplay = RightGoal.ScoreDisplay;
            RightGoal.ScoreDisplay = scoreDisplay;
        }


        private void DebugColorEdit()
        {
            KeyControl keyArenaWall = new KeyControl(Keys.NumPad0);
            KeyControl keyArenaGround = new KeyControl(Keys.NumPad1);
            KeyControl keyTeam1 = new KeyControl(Keys.NumPad2);
            KeyControl keyTeam2 = new KeyControl(Keys.NumPad3);
            KeyControl keyShift1 = new KeyControl(Keys.LeftControl);
            KeyControl keyShift2 = new KeyControl(Keys.RightControl);
            if (keyArenaWall.KeyPressed())
            {
                ColorMaskedSprite arenaMaskedSprite = Owner.FindComponent<ColorMaskedSprite>();

                if (keyShift1.KeyDown() || keyShift2.KeyDown())
                    arenaMaskedSprite.Color1 = Colors.Previous();
                else
                   arenaMaskedSprite.Color1 = Colors.Next();
            }
            else if (keyArenaGround.KeyPressed())
            {
                ColorMaskedSprite arenaMaskedSprite = Owner.FindComponent<ColorMaskedSprite>();

                if (keyShift1.KeyDown() || keyShift2.KeyDown())
                    arenaMaskedSprite.Color4 = Colors.Previous();
                else
                    arenaMaskedSprite.Color4 = Colors.Next();
            }
            else 
            {
                bool keyTeamPressed = false;
                Team team = null;

                if (keyTeam1.KeyPressed())
                {
                    keyTeamPressed = true;
                    team = Game.GameManager.Teams[0];
                }
                else if (keyTeam2.KeyPressed())
                {
                    keyTeamPressed = true;
                    team = Game.GameManager.Teams[1];
                }

                if (keyTeamPressed)
                {
                    if (keyShift1.KeyDown() || keyShift2.KeyDown())
                        Colors.Previous();
                    else
                        Colors.Next();

                    team.ColorScheme.Color1 = Colors.Current();

                    ColorMaskedSprite arenaMaskedSprite = Owner.FindComponent<ColorMaskedSprite>();
                    if (m_leftGoal.Team.TeamID == team.TeamID)
                        arenaMaskedSprite.Color2 = Colors.Current();
                    else
                        arenaMaskedSprite.Color3 = Colors.Current();
                }
            }
        }
    }
}
