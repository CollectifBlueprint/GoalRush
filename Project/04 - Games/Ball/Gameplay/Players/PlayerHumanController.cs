using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Input;
using LBE;
using Microsoft.Xna.Framework;
using Ball.Gameplay.BallEffects;
using System.Diagnostics;

namespace Ball.Gameplay.Players
{
    public class PlayerHumanController : PlayerController
    {
        PlayerInput m_input;
        public PlayerInput Input
        {
            get { return m_input; }
        }

        bool m_fireCharging;
        bool m_passCharging;

        public PlayerHumanController(Player player, PlayerInput input)
            : base(player)
        {
            m_input = input;
            m_fireCharging = false;
            m_passCharging = false;
        }

        public override void Update()
        {
            if (Game.GameManager.Paused)
                return;

            if (Player.Properties.ControlDisabled)
            {
                if (m_fireCharging)
                {
                    m_fireCharging = false;
                    CancelShotCharge();
                }
                if (m_passCharging)
                {
                    m_passCharging = false;
                    CancelPassCharge();
                }

                Player.MoveDirection = Vector2.Zero;
                Player.AimDirection = Vector2.Zero;
                return;
            }

            if (Player.Properties.Shooting)
            {
                Player.MoveDirection = Vector2.Zero;
                Player.AimDirection = Vector2.Zero;
                return;
            }

            //Pause the game
            if (m_input.PauseCtrl.KeyPressed())
            {
                Game.GameManager.PauseMatch();
            }

            //Movement
            Vector2 moveInput = m_input.MoveCtrl.Get();
            float minInput = Engine.Debug.EditSingle("MinPlayerInput", 0.1f);
            if (moveInput.Length() < minInput)
                Move(Vector2.Zero);
            else
                Move(moveInput);

            //Aiming
            Vector2 aimInput = m_input.BallCtrl.Get();
            float minAimInput = Engine.Debug.EditSingle("MinAimPlayerInput", 0.5f);
            if (aimInput.Length() < minAimInput)
                Aim(Vector2.Zero);
            else
                Aim(aimInput);

            Vector2 aimDir = GetAim();

            if (Player.Ball == null) //Defensive controls
            {
                m_fireCharging = false;
                CancelShotCharge();

                if (m_input.BallLaunchCtrl.KeyPressed())
                    Tackle(moveInput);
                else if (m_input.SpecialCtrl.KeyPressed())
                    Blink(moveInput);
            }
            else //Offensive controls
            {
                if (Player.IsShotCharged)
                {
                    ShootCharged(aimDir);
                }

                if (m_input.SpecialCtrl.KeyPressed())
                {
                    Pass(aimDir);
                }
                else
                {
                    //Start charging
                    if (m_input.BallLaunchCtrl.KeyPressed() && !m_fireCharging)
                    {
                        StartShotCharge();
                        m_fireCharging = true;
                    }

                    //Shoot
                    if (m_input.BallLaunchCtrl.KeyUp() && m_fireCharging)
                    {
                        if (Player.IsShotCharged)
                        {
                            ShootCharged(aimDir);
                            m_fireCharging = false;
                        }
                        else
                        {
                            Shoot(aimDir);
                        }
                    }
                }
            }

            return;
        }
    }
}
