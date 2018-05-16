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
        public PlayerAIController PlayerAI
        {
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

                if(PlayerAI.Player.IsShotCharged)
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
            var ballMap = Game.GameManager.Navigation.PotentialMaps["BallMap"];
            float ballValue = PlayerAI.MapValue(ballMap, PlayerAI.Player.Position);
            float teamMateBallValue = PlayerAI.MapValue(ballMap, PlayerAI.Player.TeamMate.Position);
            bool canTakeBall = PlayerAI.Player.BallTrigger.Enabled && !Game.GameManager.Ball.Properties.Untakable && !Game.GameManager.Ball.Properties.PassThroughPlayer;
            bool otherTeamHasBall = canTakeBall && Game.GameManager.Ball != null && Game.GameManager.Ball.Player != null;
            float defenseValue = DefenseValue(PlayerAI.Player.Team, PlayerAI.Player.Position);
            float attackValue = ShootValue(PlayerAI.Player.Team, PlayerAI.Player.Position);
            float relativePosition = attackValue / defenseValue;
            float teamMateDefenseValue = DefenseValue(PlayerAI.Player.Team, PlayerAI.Player.TeamMate.Position);
            float teamMateAttackValue = ShootValue(PlayerAI.Player.Team, PlayerAI.Player.TeamMate.Position);
            float teamMateRelativePosition = teamMateAttackValue / teamMateDefenseValue;
            float otherTeamAttackValue = float.NegativeInfinity;
            if (otherTeamHasBall)
                otherTeamAttackValue = DefenseValue(PlayerAI.Player.Team, Game.GameManager.Ball.Position);

            float ballDefensePosition = DefenseValue(PlayerAI.Player.Team, Game.GameManager.Ball.Position);
            float ballAttackPosition = ShootValue(PlayerAI.Player.Team, Game.GameManager.Ball.Position);
            float ballRelativePosition = ballAttackPosition / ballDefensePosition; //ballRelativePosition > 1 if in attack

            //var sb = new StringBuilder();
            //sb.AppendLine("ballValue: " + ballValue);
            //sb.AppendLine("ballDefensePosition: " + ballDefensePosition);
            //sb.AppendLine("teamMateDefenseValue: " + teamMateDefenseValue);
            //sb.AppendLine();
            //sb.AppendLine();
            //m_debugTxtCmp.Text = sb.ToString();
            //m_debugTxtCmp.AlignementVertical = TextAlignementVertical.Down;
            //m_debugTxtCmp.Alignement = TextAlignementHorizontal.Center;
            //m_debugTxtCmp.Style.Color = Color.Black;

            if (PlayerAI.Player.Ball == null)
            {
                float smoothRand = (float)Math.Cos( 2 * Engine.GameTime.TimeMS / 1000);
                if (smoothRand > 0)
                {
                    PlayerAI.TackleIfInRange();
                }
            }

            if (Game.GameManager.Ball != null)
                Engine.Log.Debug("Ball", "Ok");
            else
                Engine.Log.Debug("Ball", "Null");

            if (Game.Arena.SelectedLauncher != null)
                Engine.Log.Debug("Launcher", "Ok");
            else
                Engine.Log.Debug("Launcher", "Null");

            //Assist teamate
            if (PlayerAI.Player.TeamMate.Ball != null)
            {
                Assist();
                return;
            }

            //Player has the ball
            if (PlayerAI.Player.Ball != null)
            {
                //Navigate towards the goal
                AttackWithBall();

                //Set a delay before shooting the ball
                float minTimeBeforeShoot = Engine.Debug.EditSingle("PlayerShootDelay", 1200);
                if (Engine.GameTime.TimeMS - PlayerAI.Info.BallTakenTime < minTimeBeforeShoot)
                    return;

                HandleBall();
            }
            else
            {
                //Vector2 goalPos = Vector2.Zero;
                //if (Game.Arena.LeftGoal.Team == PlayerAI.Player.Team)
                //    goalPos = Game.Arena.LeftGoal.Position;
                //if (Game.Arena.LeftGoal.Team == PlayerAI.Player.Team)
                //    goalPos = Game.Arena.LeftGoal.Position;
                float ballThreshold = 0.9f;
                float defThreshold = 0.65f;
                float mateDefThreshold = 0.85f;
                Engine.Debug.Screen.AddCircle(PlayerAI.Position, 180);

				if ((PlayerAI.Info.BallInGoal || Game.Arena.SelectedLauncher != null) && (PlayerAI.Info.GoalTeam != null && PlayerAI.Player.Team != PlayerAI.Info.GoalTeam))
				{
						Defend ();
				}
			
				else if (canTakeBall && ballValue >= teamMateBallValue)
                {
                    PlayerAI.NavigateToBall();
                    Engine.Log.Debug("State " + PlayerAI.Player.PlayerIndex, "Move to ball");
                }
                else if (defenseValue < teamMateDefenseValue && ballDefensePosition > ballThreshold && defenseValue < defThreshold && teamMateDefenseValue < mateDefThreshold)
                {
                    if (!PlayerAI.Player.TeamMate.Properties.Blink)
                        PlayerAI.Blink(Vector2.Zero);
                    else
                        Defend();
                }
                else if (!PlayerAI.Player.BallTrigger.Enabled)
                {
                    if (Game.Arena.SelectedLauncher != null)
                    {
                        //Get launcher direction
                        var launcher = Game.Arena.SelectedLauncher;
                        Defend();
                    }
                    else
                    {
                        Defend();
                    }

                    Engine.Log.Debug("State " + PlayerAI.Player.PlayerIndex, "Defend");
                }
                else
                {
                    PlayerAI.Assist();
                    Engine.Log.Debug("State " + PlayerAI.Player.PlayerIndex, "Assist");
                }
            }
        }

        private void HandleBall()
        {
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

            float shootValue = ShootValue(PlayerAI.Player.Team, PlayerAI.Player.Position);
            float mateShootValue = ShootValue(PlayerAI.Player.Team, PlayerAI.Player.TeamMate.Position);

            float passTacticalValue = 0.75f;
            float dangerTacticalValue = 0.40f;
            if (!canShoot && mateCanShoot && mateTacticalValue > passTacticalValue)
            {
                PlayerAI.AimAtPosition(PlayerAI.Player.TeamMate.Position);
                if (PlayerAI.IsAimingAtPosition(PlayerAI.Player.TeamMate.Position))
                    PlayerAI.Pass(PlayerAI.GetAim());

                Engine.Log.Debug("State " + PlayerAI.Player.PlayerIndex, "Pass for shoot");
            }
            else if (!canShoot && tacticalValue < dangerTacticalValue && mateTacticalValue > tacticalValue)
            {
                PlayerAI.AimAtPosition(PlayerAI.Player.TeamMate.Position);
                if (PlayerAI.IsAimingAtPosition(PlayerAI.Player.TeamMate.Position))
                    PlayerAI.Pass(PlayerAI.GetAim());

                Engine.Log.Debug("State " + PlayerAI.Player.PlayerIndex, "Pass for panic");
            }
            else if (!canShoot && mateTacticalValue > tacticalValue && mateShootValue > shootValue)
            {
                PlayerAI.AimAtPosition(PlayerAI.Player.TeamMate.Position);
                if (PlayerAI.IsAimingAtPosition(PlayerAI.Player.TeamMate.Position))
                    PlayerAI.Pass(PlayerAI.GetAim());

                Engine.Log.Debug("State " + PlayerAI.Player.PlayerIndex, "Pass");
            }
            else
            {
                PlayerAI.ShootIfPossible();
                Engine.Log.Debug("State " + PlayerAI.Player.PlayerIndex, "Attack");
            }
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

        private void Assist()
        {
            //If a player in close to the teamate, tackle it
            foreach (var player in PlayerAI.Player.Opponents)
            {
                bool tackle = false;
                float assistTackleDistance = Engine.Debug.EditSingle("AssistTackleRange", 180);
                if (Vector2.Distance(player.Position, PlayerAI.Player.Position) < assistTackleDistance)
                    tackle = PlayerAI.TackleIfInRange();
            }

            //Assist the teammate
            PlayerAI.Assist();
        }

        private void Defend()
        {
            PotentialMap shootMap = null;
            if (Game.Arena.LeftGoal.Team == PlayerAI.Player.Team)
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootLeft"];
            else
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootRight"];
            PlayerAI.NavigateToBestValue(shootMap);
        }

		private void GoToInitPosition()
		{
			PlayerAI.MoveToPosition (PlayerAI.Player.InitialPosition);
		}
			

        public void AttackWithBall()
        {
            PotentialMap shootMap = null;
            if (Game.Arena.LeftGoal.Team == PlayerAI.Player.Team)
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootRight"];
            else
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootLeft"];
            PlayerAI.NavigateToBestValue(shootMap);
        }

        public float ShootValue(Team team, Vector2 pos)
        {
            PotentialMap shootMap = null;
            if (Game.Arena.LeftGoal.Team == team)
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootRight"];
            else
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootLeft"];

            Point index = Game.GameManager.Navigation.IndexFromPosition(pos);
            return shootMap.Grid[index].Value;
        }

        public float DefenseValue(Team team, Vector2 pos)
        {
            PotentialMap shootMap = null;
            if (Game.Arena.LeftGoal.Team == team)
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootLeft"];
            else
                shootMap = Game.GameManager.Navigation.PotentialMaps["ShootRight"];

            Point index = Game.GameManager.Navigation.IndexFromPosition(pos);
            return shootMap.Grid[index].Value;
        }
    }
}
