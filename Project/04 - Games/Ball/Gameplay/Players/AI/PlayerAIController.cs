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

    public class AIParameters
    {
        public float Agressiveness;
        public float Precision;
        public float TacticalSkills;
    }

    public class AIState
    {
        public float AgressivenessCoef;
        public float ShootingPrecision;
        public float TacticalSkills;

        public bool ShootCharged;
        public bool TeleportToDefense;
        public bool Pass;

        public float SkillFluctuationPhase;
    }

    public class PlayerAIController : PlayerController
    {
        public PlayerAIController(Player player)
            : base(player)
        {
        }

        KeyControl m_mouseCtrl;

        PotentialMap m_initialPositionMap;
        public PotentialMap MouseMap {
            get { return m_initialPositionMap; }
        }

        PotentialMap m_assistMap;
        public PotentialMap AssistMap {
            get { return m_assistMap; }
        }

        GameInfo m_gameInfos;
        public GameInfo Info {
            get { return m_gameInfos; }
            set { m_gameInfos = value; }
        }

        public AIState AiState {
            get { return m_aiState; }
        }

        PotentialMapInfluencePoint m_initialPositionMapSource;
        Script m_script;

        AIParameters m_aiParams;
        AIState m_aiState;

        public override void Start()
        {
            m_mouseCtrl = new KeyControl(MouseButtons.Left);

            m_initialPositionMap = new PotentialMap(Game.GameManager.Navigation, PotentialMapType.Linear);
            m_initialPositionMapSource = new PotentialMapInfluencePoint();
            m_initialPositionMapSource.Radius = 80;
            m_initialPositionMapSource.Value = 1;
            m_initialPositionMapSource.Position = Player.InitialPosition;
            m_initialPositionMap.Sources.Add(m_initialPositionMapSource);
            var launcherObstacle = new PotentialMapInfluencePoint();
            launcherObstacle.Radius = 60;
            launcherObstacle.Value = 5.0f;
            launcherObstacle.Position = Vector2.Zero;
            m_initialPositionMap.Costs.Add(launcherObstacle);
            Game.GameManager.Navigation.PotentialMaps.Add("IntialPositionMap" + (int)Player.PlayerIndex, m_initialPositionMap);

            m_assistMap = new PotentialMap(Game.GameManager.Navigation, PotentialMapType.Linear);
            m_assistMap.Sources.Add(new AssistPlayerPotentialSource() { Assistee = Player.TeamMate, Assistant = Player });
            m_assistMap.LinearParameters.SpatialDecay = 0.03f;
            Game.GameManager.Navigation.PotentialMaps.Add("AssistMap" + (int)Player.PlayerIndex, m_assistMap);

            m_aiParams = new AIParameters();
            m_aiParams.Agressiveness = Engine.Debug.EditSingle("AIAgressiveness", 0.5f);
            m_aiParams.Precision = Engine.Debug.EditSingle("AIPrecision", 0.5f);
            m_aiParams.TacticalSkills = Engine.Debug.EditSingle("AiTactics", 0.5f);

            m_aiState = new AIState();
            m_aiState.SkillFluctuationPhase = Engine.Random.NextFloat(0, 1000);

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
            UpdateSkillCoeficients();

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

            m_initialPositionMapSource.Position = Player.InitialPosition;
            m_initialPositionMap.UpdateFrequency = PotentialMapUpdateFrequency.Slow;
            m_assistMap.UpdateFrequency = PotentialMapUpdateFrequency.Slow;
            if (Player.TeamMate.Ball != null)
                m_assistMap.UpdateFrequency = PotentialMapUpdateFrequency.Fast;

            if (Engine.Debug.Flags.EnableAI == false)
                return;

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

        private void UpdateSkillCoeficients()
        {
            //Update AI coefficients
            m_aiParams.Agressiveness = Engine.Debug.EditSingle("AIAgressiveness", 0.5f);
            m_aiParams.Precision = Engine.Debug.EditSingle("AIPrecision", 0.5f);
            m_aiParams.TacticalSkills = Engine.Debug.EditSingle("AiTactics", 0.5f);

            m_aiParams.Agressiveness = 1.0f;
            m_aiParams.Precision = 1.0f;
            m_aiParams.TacticalSkills = 1.0f;

            //if (Player.Team.TeamID == TeamId.TeamOne)
            //{
            //    m_aiParams.Agressiveness = 1.0f;
            //    m_aiParams.Precision = 1.0f;
            //    m_aiParams.TacticalSkills = 1.0f;
            //}
            //else
            //{
            //    m_aiParams.Agressiveness = 0.3f;
            //    m_aiParams.Precision = 0.3f;
            //    m_aiParams.TacticalSkills = 0.3f;
            //}

            float period = 12.0f;
            float agressiveCoefRaw = 0;
            float precisionCoefRaw = 0;
            float tacticalCoefRaw = 0;
            agressiveCoefRaw = UpdateSkillCoeficient(period, m_aiParams.Agressiveness);
            precisionCoefRaw = UpdateSkillCoeficient(period * 0.87f, m_aiParams.Precision);
            tacticalCoefRaw = UpdateSkillCoeficient(period * 1.17f, m_aiParams.TacticalSkills);

            m_aiState.AgressivenessCoef = LBE.MathHelper.Lerp(m_aiParams.Agressiveness, 1.0f, agressiveCoefRaw * agressiveCoefRaw);
            m_aiState.ShootingPrecision = LBE.MathHelper.Lerp(m_aiParams.Precision, 1.0f, precisionCoefRaw * precisionCoefRaw);
            m_aiState.TacticalSkills = LBE.MathHelper.Lerp(m_aiParams.Precision, 1.0f, tacticalCoefRaw * tacticalCoefRaw);

            m_aiState.ShootCharged = m_aiState.TacticalSkills > 0.8f;
            m_aiState.Pass = m_aiState.TacticalSkills > 0.8f;
            m_aiState.TeleportToDefense = m_aiState.TacticalSkills > 0.8f;

            //if (Player.PlayerIndex != PlayerIndex.Two)
            //    return;
            //if (Player.Team.TeamID == TeamId.TeamOne)
            //    return;

            Engine.Log.Debug("AIAgressiveness", AiState.AgressivenessCoef.ToString("0.0"));
            Engine.Log.Debug("AIPrecision", AiState.ShootingPrecision.ToString("0.0"));
            Engine.Log.Debug("AITactical", AiState.TacticalSkills.ToString("0.0"));
        }

        private float UpdateSkillCoeficient(float period, float baseSkillCoef)
        {
            return baseSkillCoef;

            var loopingTime = (Engine.GameTime.TimeMS / 1000 + AiState.SkillFluctuationPhase) % period;

            float movingCoef = 0;
            if (loopingTime > period * (1 - baseSkillCoef))
                movingCoef = 1;
            else
                movingCoef = 0;

            return movingCoef;
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

        public void NavigateToInitialPosition()
        {
            float stoppingPos = 40;
            float distSq = Vector2.DistanceSquared(Player.Position, m_initialPositionMapSource.Position);
            if (distSq < stoppingPos * stoppingPos)
                AimAtPosition(Vector2.Zero);
            NavigateWithMapAndRaycast(m_initialPositionMap, m_initialPositionMapSource.Position);
        }

        public void NavigateToAssistPosition()
        {
            NavigateToBestValue(m_assistMap);
        }

        public void NavigateToBall()
        {
            var ballMap = Game.GameManager.Navigation.PotentialMaps["BallMap"];
            var position = Game.GameManager.Ball.Position;
            NavigateWithMapAndRaycast(ballMap, position);
        }

        public void NavigateWithMapAndRaycast(PotentialMap map, Vector2 position)
        {
            float rayCastDist = 250;
            float distSq = Vector2.DistanceSquared(Player.Position, position);
            if (distSq < rayCastDist * rayCastDist && NavigationManager.RayCastVisibility(Player.Position, position))
            {
                Engine.Debug.Screen.ResetBrush(); Engine.Debug.Screen.Brush.DrawSurface = false;
                Engine.Debug.Screen.AddLine(Player.Position, position);
                MoveToPosition(position);
            }
            else
            {
                NavigateToBestValue(map);
            }
        }

        public void ShootIfPossible()
        {
            Goal goal = Game.Arena.LeftGoal.Team == Player.Team ? Game.Arena.RightGoal : Game.Arena.LeftGoal;
            Team otherTeam = Player.Team.TeamID == TeamId.TeamOne ? Game.GameManager.Teams[1] : Game.GameManager.Teams[0];

            bool canShoot = NavigationManager.DoubleRayCast(Player.Position, goal.Position, 20);
            bool noInterception = NavigationManager.TestInterception(otherTeam, Player.Position, goal.Position);

            //Compute distance charge based on agressiveness
            float distChargeMin = 350;
            float distChargeMax = 550;
            float distCharge = LBE.MathHelper.Lerp(distChargeMax, distChargeMin, AiState.AgressivenessCoef);

            float opponentDistance = Math.Min(
                Vector2.Distance(otherTeam.Players[0].Position, Player.Position),
                Vector2.Distance(otherTeam.Players[1].Position, Player.Position));

            var goalDirection = goal.Position - Player.Position;
            goalDirection.Normalize();

            float maxRandomAngle = 3.14f * 0.3f;

            Aim(goalDirection);
            Move(goalDirection);

            if (canShoot && Player.IsShotCharging && Player.IsShotCharged)
            {
                if (IsAiming(goalDirection))
                {
                    var shootDirection = RandomShootingDirection(goalDirection, maxRandomAngle, AiState.ShootingPrecision);
                    ShootCharged(shootDirection);
                }
            }
            else if (canShoot && noInterception)
            {
                if (IsAiming(goalDirection))
                {
                    var shootDirection = RandomShootingDirection(goalDirection, maxRandomAngle, AiState.ShootingPrecision);
                    Shoot(shootDirection);
                }
            }
            else if (AiState.ShootCharged && canShoot && opponentDistance > distCharge)
            {
                if (!Player.IsShotCharging)
                    StartShotCharge();
            }
        }

        Vector2 RandomShootingDirection(Vector2 baseDirection, float maxAngle, float precision)
        {
            //Skew deviation towards max value
            float deviation = Engine.Random.NextFloat();
            deviation = (float)Math.Pow(deviation, 0.25f);

            float side = Engine.Random.NextSign();

            float angle = maxAngle * deviation * side * (1 - precision);
            Engine.Log.Debug("OffsetAngle", angle);

            return baseDirection.Rotate(angle);
        }

        public bool TackleIfInRange()
        {
            //Compute tackle max distance based on agressivness
            float tackleDist = Engine.Debug.EditSingle("TackleDistance", 75);
            tackleDist = LBE.MathHelper.Lerp(tackleDist * 0.5f, tackleDist, AiState.AgressivenessCoef);

            foreach (var player in Player.Opponents)
            {
                if (Vector2.Distance(player.Position, Player.Position) < tackleDist)
                {
                    float maxRandomAngle = 3.14f * 1.4f;

                    var tackleDirection = player.Position - player.Position;
                    tackleDirection = RandomShootingDirection(tackleDirection, maxRandomAngle, AiState.TacticalSkills);
                    Tackle(tackleDirection);
                    return true;
                }
            }
            return false;
        }
    }
}
