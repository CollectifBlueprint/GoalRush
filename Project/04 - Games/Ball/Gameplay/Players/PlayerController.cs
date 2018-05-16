using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LBE.Gameplay;
using LBE;
using Ball.Gameplay.BallEffects;
using LBE.Input;
using Ball.Gameplay.Fx;

namespace Ball.Gameplay
{
    public class PlayerController : GameObjectComponent
    {
        Player m_player;
        public Player Player
        {
            get { return m_player; }
        }

        public PlayerController(Player player)
        {
            m_player = player;
        }

        public Vector2 GetAim()
        {
            return new Vector2(
                (float)Math.Cos(m_player.BallAngle),
                (float)Math.Sin(m_player.BallAngle));
        }

        public void Aim(Vector2 direction)
        {
            m_player.AimDirection = direction;
        }

        public void AimAtPosition(Vector2 position)
        {
            Vector2 dir = position - m_player.Position;
            Aim(dir);
        }

        public bool IsAiming(Vector2 direction)
        {
            float maxAngleDelta = 0.08f;
            float angle = (float)Math.Atan2(direction.Y, direction.X);

            float size = 150;
            Engine.Debug.Screen.AddArrow(Player.Position, Player.Position + Vector2.UnitX.Rotate(angle) * size);
            Engine.Debug.Screen.AddArrow(Player.Position, Player.Position + Vector2.UnitX.Rotate(m_player.BallAngle) * size);

            if (Math.Abs(LBE.MathHelper.NormalizeAngle(angle - m_player.BallAngle)) < maxAngleDelta)
                return true;
            return false;
        }

        public bool IsAimingAtPosition(Vector2 position)
        {
            Vector2 dir = position - m_player.Position;
            return IsAiming(dir);
        }

        public void Shoot(Vector2 direction)
        {
            m_player.ShootBall(direction);
        }

        public void ShootCharged(Vector2 direction)
        {
            m_player.ShootBallCharged();
            m_player.StopChargingShot();
        }

        public void Pass(Vector2 direction)
        {
            m_player.Pass(Player.TeamMate, direction);
            m_player.StopChargingPass();
            return;           
        }

        public void StartShotCharge()
        {
            m_player.StartChargingShot();
        }

        public void CancelShotCharge()
        {
            if(m_player.IsShotCharging)
                m_player.StopChargingShot();
        }

        public void StartPassCharge()
        {
            m_player.StartChargingPass();
        }

        public void CancelPassCharge()
        {
            m_player.StopChargingPass();
        }

        public void Move(Vector2 direction)
        {
            Vector2 desiredDirection = direction;
            if (desiredDirection != Vector2.Zero)
                desiredDirection.Normalize();
            Player.MoveDirection = desiredDirection;
        }

        public void MoveToPosition(Vector2 position)
        {
            Vector2 dir = position - m_player.Position;
            Move(dir);
        }

        public void Tackle()
        {
            m_player.Tackle(m_player.MoveDirection);
        }

        public void Tackle(Vector2 direction)
        {
            Move(direction);
            m_player.Tackle(direction);
        }

        public void Blink(Vector2 direction)
        {
            m_player.StartBlink(direction);
        }
    }   
}
