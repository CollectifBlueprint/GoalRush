using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using Microsoft.Xna.Framework.Graphics;
using LBE;
using Microsoft.Xna.Framework;
using Ball.Gameplay.Players.AI;
using Ball.Gameplay.Players;

namespace Ball.Gameplay.Fx
{
    public class PlayerControllerTextFx : GameObjectComponent
    {
        TextComponent m_controllerTxtCmp;
        public TextComponent ControllerTxtCmp
        {
            get { return m_controllerTxtCmp; }
        }

        Timer m_displayControllerTimerMS;
        TimerEvent  m_displayControllerTimerEvent;
        bool m_active;

        Player m_player;

        public PlayerControllerTextFx(Player player)
        {
            m_player = player;
            m_active = false;
        }

        public override void Start()
        {
            m_controllerTxtCmp = new TextComponent("ArenaOverlay11");

            if (m_player.Owner.FindComponent<PlayerAIController>() != null)
                m_controllerTxtCmp.Text = "CPU";
            else
            {
                var ctrl = m_player.Owner.FindComponent<PlayerHumanController>();
                if (ctrl != null)
                {
                    if (ctrl.Input.InputType == InputType.Keyboard || ctrl.Input.InputType == InputType.MouseKeyboard)
                        m_controllerTxtCmp.Text = "Keyboard";
                    else
                        m_controllerTxtCmp.Text = "Gamepad " + (1 + (int)ctrl.Input.InputIndex);
                }
            }

            m_controllerTxtCmp.Alignement = TextAlignementHorizontal.Center;
            m_controllerTxtCmp.Style = new TextStyle();
            m_controllerTxtCmp.Style.Font = Engine.AssetManager.Get<SpriteFont>("Graphics/GameplayFont");
            m_controllerTxtCmp.Style.Scale = 0.62f;
            m_controllerTxtCmp.Style.Color = m_player.PlayerColors[(int)Player.ColorElement.MakeUp];
            m_controllerTxtCmp.Position = new Vector2(0, 55);
            m_controllerTxtCmp.Visible = false;
            Owner.Attach(m_controllerTxtCmp);

            m_displayControllerTimerMS = new Timer(Engine.GameTime.Source, 3000);
            m_displayControllerTimerEvent = delegate(Timer source) { m_active = false; };
            m_displayControllerTimerMS.OnTime += m_displayControllerTimerEvent;
        }

        public override void Update()
        {
            if (!m_active)
            {
                m_controllerTxtCmp.Visible = false;
                return;
            }

            if (((int)(m_displayControllerTimerMS.TimeMS + 1000) / 650) % 2 == 0)
            {
                m_controllerTxtCmp.Visible = false;
            }
            else
            {
                m_controllerTxtCmp.Visible = true;
            }
        }

        public void Begin()
        {
            m_displayControllerTimerMS.Start();
            m_active = true;
        }

        public override void End()
        {
            base.End();
            m_displayControllerTimerMS.Stop();
            m_displayControllerTimerMS.OnTime -= m_displayControllerTimerEvent;
        }

        public void Stop()
        {
            m_active = false;
            m_displayControllerTimerMS.Stop();

        }
    }
}
