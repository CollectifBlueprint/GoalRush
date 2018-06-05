using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Gameplay.Navigation;
using Microsoft.Xna.Framework;
using LBE;
using LBE.Graphics.Sprites;

namespace Ball.Gameplay.Players.AI
{
    public class Script
    {
        PlayerAIController m_playerAI;
        public PlayerAIController PlayerAI {
            get { return m_playerAI; }
            set { m_playerAI = value; }
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }
    }

    public class TestShieldScript : Script
    {
        TextComponent m_debugTxtCmp;
        public override void Start()
        {
            m_debugTxtCmp = new TextComponent();
            PlayerAI.Owner.Attach(m_debugTxtCmp);
        }

        public override void Update()
        {
            var ballMap = Game.GameManager.Navigation.PotentialMaps["BallMap"];
            if (PlayerAI.Player.Ball != null)
            {
                var target = Game.GameManager.Players[0];
                var dir = target.Position - PlayerAI.Position;

                PlayerAI.AimAtPosition(target.Position);

                if (!PlayerAI.Player.IsShotCharging)
                    PlayerAI.StartShotCharge();

                if (PlayerAI.Player.IsShotCharged)
                    PlayerAI.ShootCharged(dir);
            }
            else if (Game.GameManager.Ball != null && Game.GameManager.Ball.Player == null)
            {
                PlayerAI.NavigateToBall();
            }
        }
    }

    public class AssistAndDefendScript : Script
    {
        TextComponent m_debugTxtCmp;
        public override void Start()
        {
            m_debugTxtCmp = new TextComponent();
            PlayerAI.Owner.Attach(m_debugTxtCmp);
        }

        public override void Update()
        {
            if (PlayerAI.Player.TeamMate.Ball != null)
                AssistMode();
            else if (PlayerAI.Player.Ball != null)
                AttackMode();
            else
                DefenseMode();
        }

        private void AssistMode()
        {
            foreach (var opponents in PlayerAI.Player.Opponents)
            {
                //If a ennemy player in close to the teamate, tackle it
                float assistTackleDistance = Engine.Debug.EditSingle("AssistTackleRange", 180);
                if (Vector2.Distance(opponents.Position, PlayerAI.Player.TeamMate.Position) < assistTackleDistance)
                    PlayerAI.TackleIfInRange();
            }

            //Assist the teammate
            PlayerAI.NavigateToAssistPosition();
        }

        private void DefenseMode()
        {
            var ballMap = Game.GameManager.Navigation.PotentialMaps["BallMap"];
            float ballValue = PlayerAI.MapValue(ballMap, PlayerAI.Player.Position);
            float teamMateBallValue = PlayerAI.MapValue(ballMap, PlayerAI.Player.TeamMate.Position);
            bool canTakeBall = PlayerAI.Player.BallTrigger.Enabled && !Game.GameManager.Ball.Properties.Untakable && !Game.GameManager.Ball.Properties.PassThroughPlayer;
            bool otherTeamHasBall = canTakeBall && Game.GameManager.Ball != null && Game.GameManager.Ball.Player != null;
            float defenseValue = GetDefenseValue(PlayerAI.Player.Team, PlayerAI.Player.Position);
            float attackValue = GetShootValue(PlayerAI.Player.Team, PlayerAI.Player.Position);
            float relativePosition = attackValue / defenseValue;
            float teamMateDefenseValue = GetDefenseValue(PlayerAI.Player.Team, PlayerAI.Player.TeamMate.Position);
            float teamMateAttackValue = GetShootValue(PlayerAI.Player.Team, PlayerAI.Player.TeamMate.Position);
            float teamMateRelativePosition = teamMateAttackValue / teamMateDefenseValue;
            float otherTeamAttackValue = float.NegativeInfinity;
            if (otherTeamHasBall)
                otherTeamAttackValue = GetDefenseValue(PlayerAI.Player.Team, Game.GameManager.Ball.Position);

            float ballDefensePosition = GetDefenseValue(PlayerAI.Player.Team, Game.GameManager.Ball.Position);
            float ballAttackPosition = GetShootValue(PlayerAI.Player.Team, Game.GameManager.Ball.Position);
            float ballRelativePosition = ballAttackPosition / ballDefensePosition; //ballRelativePosition > 1 if in attack

            float ballThreshold = 0.9f;
            float defThreshold = 0.65f;
            float mateDefThreshold = 0.85f;
            Engine.Debug.Screen.AddCircle(PlayerAI.Position, 180);

            if ((PlayerAI.Info.BallInGoal || Game.Arena.SelectedLauncher != null) && (PlayerAI.Info.GoalTeam != null && PlayerAI.Player.Team != PlayerAI.Info.GoalTeam))
            {
                MoveToInitialPosition();
                //MoveToDefense();
            }
            else if (canTakeBall && ballValue >= teamMateBallValue)
            {
                PlayerAI.TackleIfInRange();
                PlayerAI.NavigateToBall();
            }
            else if (defenseValue < teamMateDefenseValue && ballDefensePosition > ballThreshold && defenseValue < defThreshold && teamMateDefenseValue < mateDefThreshold)
            {
                if (PlayerAI.AiState.TeleportToDefense && !PlayerAI.Player.TeamMate.Properties.Blink)
                    PlayerAI.Blink(Vector2.Zero);
                else
                    MoveToDefense();
            }
            else if (!PlayerAI.Player.BallTrigger.Enabled)
            {
                //MoveToInitialPosition();
                MoveToDefense();
            }
            else
            {
                PlayerAI.TackleIfInRange();
                PlayerAI.NavigateToAssistPosition();
            }
        }

        private void AttackMode()
        {
            Engine.Log.Debug("State " + PlayerAI.Player.PlayerIndex, "Attack");

            //Compute a tactical value based on the distance to opponents
            float tacticalDistance = 300;
            float tacticalValue = Math.Min(
                1 - LBE.MathHelper.WeightDistance(PlayerAI.Player.Position, tacticalDistance, PlayerAI.Player.Opponents[0].Position),
                1 - LBE.MathHelper.WeightDistance(PlayerAI.Player.Position, tacticalDistance, PlayerAI.Player.Opponents[1].Position));

            float mateTacticalValue = Math.Min(
                1 - LBE.MathHelper.WeightDistance(PlayerAI.Player.TeamMate.Position, tacticalDistance, PlayerAI.Player.Opponents[0].Position),
                1 - LBE.MathHelper.WeightDistance(PlayerAI.Player.TeamMate.Position, tacticalDistance, PlayerAI.Player.Opponents[1].Position));

            //Check if the player can shoot from its current position
            bool canShoot;
            bool mateCanShoot;
            CheckShootPossible(out canShoot, out mateCanShoot);

            float shootValue = GetShootValue(PlayerAI.Player.Team, PlayerAI.Player.Position);
            float mateShootValue = GetShootValue(PlayerAI.Player.Team, PlayerAI.Player.TeamMate.Position);

            float passTacticalValue = 0.75f;
            float dangerTacticalValue = 0.40f;

            //Get distance to goal
            Goal goal = Game.Arena.LeftGoal.Team == PlayerAI.Player.Team ? Game.Arena.RightGoal : Game.Arena.LeftGoal;
            float distFromGoal = Vector2.Distance(goal.Position, PlayerAI.Position);

            MoveToShootPosition();
            if (distFromGoal < 250 && canShoot)
                PlayerAI.AimAtPosition(goal.Position);

            //Set a delay before shooting or passing the ball when we just got it
            float shootDelayMin = Engine.Debug.EditSingle("AIShootDelayMin", 80);
            float shootDelayDistanceBase = Engine.Debug.EditSingle("AIShootDelayDistanceBase", 400);
            float shootDelayDistanceCoefMax = Engine.Debug.EditSingle("AIShootDelayDistanceCoefMax", 5);
            float shootDelaySkillBase = Engine.Debug.EditSingle("AIShootDelaySkillBase", 400);
            float shootDelaySkillCoefMin = Engine.Debug.EditSingle("AIShootDelaySkillCoefMin", 0.5f);

            //Shoot faster if closer to goal
            float distanceCoef = LBE.MathHelper.LinearStep(100, 600, distFromGoal);

            //More agressive AI will shoot faster
            float agresssivnessCoef = LBE.MathHelper.Lerp(shootDelaySkillCoefMin, 1.0f, (1 - PlayerAI.AiState.AgressivenessCoef));
            float shootDelay = shootDelayMin + shootDelayDistanceBase * distanceCoef + shootDelaySkillBase * agresssivnessCoef;

            if (Engine.GameTime.TimeMS - PlayerAI.Info.BallTakenTime < shootDelay)
                return;

            //If possible, shoot
            if (canShoot)
            {
                PlayerAI.ShootIfPossible();
            }
            //If mate can shoot has good tactical value, pass the ball
            else if (PlayerAI.AiState.Pass && mateCanShoot && mateTacticalValue > passTacticalValue)
            {
                AimAndPass(PlayerAI.Player.TeamMate.Position);
            }
            //If personal tactical value is bad, do an emergency pass
            else if (PlayerAI.AiState.Pass && tacticalValue < dangerTacticalValue && mateTacticalValue > tacticalValue)
            {
                AimAndPass(PlayerAI.Player.TeamMate.Position);
            }
            //If mate has better tactical value, pass the ball
            else if (PlayerAI.AiState.Pass && mateTacticalValue > tacticalValue && mateShootValue > shootValue)
            {
                AimAndPass(PlayerAI.Player.TeamMate.Position);
            }
        }

        private void MoveToDefense()
        {
            Engine.Log.Debug("State " + PlayerAI.Player.PlayerIndex, "Defend");
            PotentialMap defenseMap = null;
            if (Game.Arena.LeftGoal.Team == PlayerAI.Player.Team)
                defenseMap = Game.GameManager.Navigation.PotentialMaps["ShootLeft"];
            else
                defenseMap = Game.GameManager.Navigation.PotentialMaps["ShootRight"];
            PlayerAI.NavigateToBestValue(defenseMap);
        }

        private void MoveToInitialPosition()
        {
            PlayerAI.NavigateToInitialPosition();
        }

        public void MoveToShootPosition()
        {
            PotentialMap shootMap = null;
            if (Game.Arena.LeftGoal.Team == PlayerAI.Player.Team)
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootRight"];
            else
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootLeft"];
            PlayerAI.NavigateToBestValue(shootMap);
        }

        private void CheckShootPossible(out bool canShoot, out bool mateCanShoot)
        {
            Goal goal = Game.Arena.LeftGoal.Team == PlayerAI.Player.Team ? Game.Arena.RightGoal : Game.Arena.LeftGoal;
            Team otherTeam = PlayerAI.Player.Team.TeamID == TeamId.TeamOne ? Game.GameManager.Teams[1] : Game.GameManager.Teams[0];
            mateCanShoot = NavigationManager.DoubleRayCast(PlayerAI.Player.TeamMate.Position, goal.Position, 20);
            mateCanShoot = mateCanShoot && NavigationManager.TestInterception(otherTeam, PlayerAI.Player.TeamMate.Position, goal.Position);
            canShoot = NavigationManager.DoubleRayCast(PlayerAI.Player.Position, goal.Position, 20);
            canShoot = canShoot && NavigationManager.TestInterception(otherTeam, PlayerAI.Player.Position, goal.Position);
        }

        public float GetShootValue(Team team, Vector2 pos)
        {
            PotentialMap shootMap = null;
            if (Game.Arena.LeftGoal.Team == team)
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootRight"];
            else
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootLeft"];

            Point index = Game.GameManager.Navigation.IndexFromPosition(pos);
            return shootMap.Grid[index].Value;
        }

        public float GetDefenseValue(Team team, Vector2 pos)
        {
            PotentialMap shootMap = null;
            if (Game.Arena.LeftGoal.Team == team)
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootLeft"];
            else
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootRight"];

            Point index = Game.GameManager.Navigation.IndexFromPosition(pos);
            return shootMap.Grid[index].Value;
        }

        private void AimAndPass(Vector2 position)
        {
            PlayerAI.AimAtPosition(position);
            if (PlayerAI.IsAimingAtPosition(position))
                PlayerAI.Pass(PlayerAI.GetAim());
        }
    }
}
