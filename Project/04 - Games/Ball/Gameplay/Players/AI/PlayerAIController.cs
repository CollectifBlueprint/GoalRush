using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Physics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Ball.Physics;
using LBE;
using Ball.Gameplay.Navigation;
using LBE.Input;
using Ball.Gameplay.Navigation.PotentialMaps;

namespace Ball.Gameplay.Players.AI
{
    public class GameInfo
    {
        public float BallShotTime;
        public float BallTakenTime;

        public bool BallShotByTeam;
        public bool BallInGoal;
		public Team GoalTeam; 
    }

    public class PlayerAIController : PlayerController
    {
        public PlayerAIController(Player player)
            : base(player)
        {
        }

        KeyControl m_mouseCtrl;

        PotentialMap m_mouseMap;
        public PotentialMap MouseMap
        {
            get { return m_mouseMap; }
        }

        PotentialMap m_assistMap;
        public PotentialMap AssistMap
        {
            get { return m_assistMap; }
        }

        GameInfo m_gameInfos;
        public GameInfo Info
        {
            get { return m_gameInfos; }
            set { m_gameInfos = value; }
        }

        PotentialMapInfluencePoint m_mouseSource;
        Script m_script;

        public override void Start()
        {
            m_mouseCtrl = new KeyControl(MouseButtons.Left);
            
            m_mouseMap = new PotentialMap(Game.GameManager.Navigation, PotentialMapType.Linear);
            m_mouseSource = new PotentialMapInfluencePoint();
            m_mouseSource.Radius = 60;
            m_mouseSource.Value = 1;
            m_mouseMap.Sources.Add(m_mouseSource);
            Game.GameManager.Navigation.PotentialMaps.Add("MouseMap" + (int)Player.PlayerIndex, m_mouseMap);

            m_assistMap = new PotentialMap(Game.GameManager.Navigation, PotentialMapType.Linear);
            m_assistMap.Sources.Add(new AssistPlayerPotentialSource() { Assistee = Player.TeamMate, Assistant = Player });
            m_assistMap.LinearParameters.SpatialDecay = 0.03f;
            Game.GameManager.Navigation.PotentialMaps.Add("AssistMap" + (int)Player.PlayerIndex, m_assistMap);

            m_script = new AssistAndDefendScript();
            m_script.PlayerAI = this;
            m_script.Start();

            m_gameInfos = new GameInfo();
            m_gameInfos.BallInGoal = false;
            m_gameInfos.BallShotByTeam = false;
            m_gameInfos.BallShotTime = float.NegativeInfinity;
            m_gameInfos.BallTakenTime = float.NegativeInfinity;

            Engine.World.EventManager.AddListener((int)EventId.Goal, OnGoal);
            Engine.World.EventManager.AddListener((int)EventId.KickOff, OnKickOff);
            Engine.World.EventManager.AddListener((int)EventId.PlayerShootBall, OnPlayerShootBall);
            Engine.World.EventManager.AddListener((int)EventId.PlayerReceiveBall, OnPlayerReceiveBall);
        }

        public override void Update()
        {
            if (Player.Properties.ControlDisabled)
            {
                if (Player.IsShotCharging)
                    CancelShotCharge();

                if (Player.IsPassCharging)
                    CancelPassCharge();

                Player.MoveDirection = Vector2.Zero;
                Player.AimDirection = Vector2.Zero;
                return;
            }

            m_mouseMap.UpdateFrequency = PotentialMapUpdateFrequency.None;
            m_assistMap.UpdateFrequency = PotentialMapUpdateFrequency.Slow;
            if (Player.TeamMate.Ball != null)
                m_assistMap.UpdateFrequency = PotentialMapUpdateFrequency.Fast;

            if (Engine.Debug.Flags.EnableAI == false)
                return;

            if (m_mouseCtrl.KeyPressed())
            {
                Point mousePos = new Point(Engine.Input.MouseState().X, Engine.Input.MouseState().Y);
                m_mouseSource.Position = Engine.Renderer.Cameras[(int)CameraId.Game].ScreenToWorld(mousePos);
            }

            Move(Vector2.Zero);
            Aim(Vector2.Zero);

            if (Player.Ball == null)
            {
                CancelShotCharge();
                CancelPassCharge();
            }

            if (Game.GameManager.Match.MatchState == MatchState.FirstPeriod || Game.GameManager.Match.MatchState == MatchState.SecondPeriod)
                m_script.Update();
        }

        public override void End()
        {
            Game.GameManager.Navigation.PotentialMaps.Remove("MouseMap" + (int)Player.PlayerIndex);
            Game.GameManager.Navigation.PotentialMaps.Remove("AssistMap" + (int)Player.PlayerIndex);

            Engine.World.EventManager.RemoveListener((int)EventId.Goal, OnGoal);
            Engine.World.EventManager.RemoveListener((int)EventId.KickOff, OnKickOff);
            Engine.World.EventManager.RemoveListener((int)EventId.PlayerShootBall, OnPlayerShootBall);
            Engine.World.EventManager.RemoveListener((int)EventId.PlayerReceiveBall, OnPlayerReceiveBall);
        }

        public void OnGoal(object eventParameter)
        {
			m_gameInfos.BallInGoal = true;
			m_gameInfos.GoalTeam = (Team)((object[])eventParameter)[0];
        }

        public void OnKickOff(object eventParameter)
        {
            m_gameInfos.BallInGoal = false;
        }

        public void OnPlayerShootBall(object eventParameter)
        {
            Player player = (Player)((object[])eventParameter)[0];   
            m_gameInfos.BallShotTime = Engine.GameTime.TimeMS;

            if (player == Player || player == Player.TeamMate)
                m_gameInfos.BallShotByTeam = true;
            else
                m_gameInfos.BallShotByTeam = false;
        }

        public void OnPlayerReceiveBall(object eventParameter)
        {
            m_gameInfos.BallTakenTime = Engine.GameTime.TimeMS;
        }

        public void Assist()
        {
            NavigateToBestValue(m_assistMap);            
        }

        public float MapValue(PotentialMap map, Vector2 position)
        {
            Point index = Game.GameManager.Navigation.IndexFromPosition(position);
            if (map.Grid.TestBounds(index))
                return map.Grid[index].Value;
            return map.LinearParameters.MinValue;
        }

        public void NavigateToBestValue(PotentialMap map)
        {            
            Point index = Game.GameManager.Navigation.IndexFromPosition(Player.Position);

            if (Game.GameManager.Navigation.Parameters.Debug)
            {
                Engine.Debug.Screen.ResetBrush(); Engine.Debug.Screen.Brush.DrawSurface = false;
                Engine.Debug.Screen.AddSquare(Game.GameManager.Navigation.NavigationGrid[index].Position, Game.GameManager.Navigation.Parameters.GridSpacing / 2);
            }

            if (map.Grid.TestBounds(index))
            {
                Vector2 grad = map.Gradient(index);

                Engine.Log.Debug("Player " + Player.PlayerIndex + " panic", "no");
                if (grad != Vector2.Zero)
                {
                    Move(grad);
                }
                else
                {
                    NavigationCell bestCell = null;
                    NavigationCell navCell = Game.GameManager.Navigation.NavigationGrid[index];
                    foreach (var nextCell in navCell.Neighbours)
                    {
                        if (nextCell == null)
                            continue;

                        if (map.Grid[nextCell.Index].Value < map.Grid[index].Value)
                            continue;

                        if (bestCell != null && map.Grid[nextCell.Index].Value < map.Grid[bestCell.Index].Value)
                            continue;

                        Engine.Log.Debug("Player " + Player.PlayerIndex + " panic", "yes");

                        if (NavigationManager.RayCastVisibility(Player.Position, nextCell.Position))
                            bestCell = nextCell;
                    }

                    if (bestCell != null)
                        MoveToPosition(bestCell.Position);
                }
            }
        }

        public void NavigateTo(Vector2 position)
        {            
            m_mouseSource.Position = position;
            NavigateToBestValue(m_mouseMap);
        }

        public void NavigateToBall()
        {
            float rayCastDist = 250;
            float distSq = Vector2.DistanceSquared(Player.Position, Game.GameManager.Ball.Position);
            if (distSq < rayCastDist * rayCastDist && NavigationManager.RayCastVisibility(Player.Position, Game.GameManager.Ball.Position))
            {
                Engine.Debug.Screen.ResetBrush(); Engine.Debug.Screen.Brush.DrawSurface = false;
                Engine.Debug.Screen.AddLine(Player.Position, Game.GameManager.Ball.Position);
                MoveToPosition(Game.GameManager.Ball.Position);
            }
            else
            {
                var ballMap = Game.GameManager.Navigation.PotentialMaps["BallMap"];
                NavigateToBestValue(ballMap);
            }
        }

        public bool TackleIfInRange()
        {
            float tackleDist = Engine.Debug.EditSingle("TackleDistance", 75);
            foreach (var player in Player.Opponents)
            {
                if (Vector2.Distance(player.Position, Player.Position) < tackleDist)
                {
                    Tackle(player.Position - player.Position);
                    return true;
                }
            } 
            return false;
        }

        public void ShootIfPossible()
        {
            Goal goal = Game.Arena.LeftGoal.Team == Player.Team ? Game.Arena.RightGoal : Game.Arena.LeftGoal;
            Team otherTeam = Player.Team.TeamID == TeamId.TeamOne ? Game.GameManager.Teams[1] : Game.GameManager.Teams[0];

            bool canShoot = NavigationManager.DoubleRayCast(Player.Position, goal.Position, 20);
            bool intercept = NavigationManager.TestInterception(otherTeam, Player.Position, goal.Position);

            float distCharge = 350;
            float opponentDistance = Math.Min(
                Vector2.Distance(otherTeam.Players[0].Position, Player.Position),
                Vector2.Distance(otherTeam.Players[1].Position, Player.Position));

            var goalDirection = goal.Position - Player.Position;
            goalDirection.Normalize();

            Aim(goalDirection);

            if (canShoot && Player.IsShotCharging && Player.IsShotCharged)
            {
                if(IsAiming(goalDirection))
                    ShootCharged(goalDirection);
            }
            else if (canShoot && intercept)
            {
                if (IsAiming(goalDirection))
                    Shoot(goalDirection);
            }
            else if (canShoot && opponentDistance > distCharge)
            {
                if (!Player.IsShotCharging)
                    StartShotCharge();
            }
        }
    }
}
