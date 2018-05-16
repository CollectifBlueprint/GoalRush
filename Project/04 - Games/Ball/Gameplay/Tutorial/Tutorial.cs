using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Graphics.Sprites;
using LBE.Gameplay;
using LBE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ball.Gameplay
{
    public class Tutorial : GameObjectComponent
    {
        TextComponent m_text;
        Timer m_textEffectTimer;
        float m_textEffectTimeMS = 1550;
  
        // text fx
        float m_scaleStart = 1.2f;
        float m_scaleEnd = 1;
        float m_scaleTimeMS = 200;
        float m_scaleDelayMS = 0;

        float m_fadeStart;
        float m_fadeEnd;
        float m_fadeTimeMS = 500;
        float m_fadeDelayMS = 1000;
        public enum fadeType
        {
            fadeIn = -1,
            fadeOut = 1
        }

        Vector2 m_moveStart;
        Vector2 m_moveEnd;
        float m_moveTimeMS = 500;
        float m_moveDelayMS = 1000;
        Vector2 m_moveValue;



        public override void Start()
        {
            Engine.World.EventManager.AddListener((int)EventId.PlayerPassAssist, OnPlayerPassAssist);
            Engine.World.EventManager.AddListener((int)EventId.PlayerShootBallCharged, OnPlayerShootBallCharged);
            m_textEffectTimer = new Timer(Engine.GameTime.Source, m_textEffectTimeMS);

             m_scaleStart = 1f;
             m_scaleEnd = 0.7f;
             m_scaleTimeMS = 200;
             m_scaleDelayMS = 0;
            
             m_fadeTimeMS = 500;
             m_fadeDelayMS = 1000;

             m_moveTimeMS = 500;
             m_moveDelayMS = 1000;
             m_moveValue = new Vector2(0, 50);
           
        }


        public override void Update()
        {
            if (m_textEffectTimer.Active)
            {
                if (m_textEffectTimer.TimeMS > m_scaleDelayMS)
                {
                    float scaleVariation = m_scaleEnd - m_scaleStart;

                    float scaleCoef = LBE.MathHelper.LinearStep(m_scaleDelayMS, m_scaleTimeMS + m_scaleDelayMS, m_textEffectTimer.TimeMS);
                    float currentScale = m_scaleStart + scaleVariation * scaleCoef;

                    m_text.Style.Scale = currentScale;
                }


                if (m_textEffectTimer.TimeMS > m_fadeDelayMS)
                {
                    float fadeVariation = m_fadeEnd - m_fadeStart;

                    float fadeCoef = LBE.MathHelper.LinearStep(m_fadeDelayMS, m_fadeTimeMS + m_fadeDelayMS, m_textEffectTimer.TimeMS);
                    float currentFade = m_fadeStart + fadeVariation * fadeCoef;

                    m_text.Style.Color.A = Convert.ToByte(currentFade);
                }


                if (m_textEffectTimer.TimeMS > m_moveDelayMS)
                {
                    Vector2 moveVariation = m_moveEnd - m_moveStart;

                    float moveCoef = LBE.MathHelper.LinearStep(m_moveDelayMS, m_moveTimeMS + m_moveDelayMS, m_textEffectTimer.TimeMS);
                    Vector2 currentMove = m_moveStart + moveVariation * moveCoef;

                    m_text.Position = currentMove;

                    Engine.Log.Debug("currentMove", currentMove);
                }

            }

          
        }
       

        public void CreateFeedBackText(GameObject parent, string text, Color color)
        {
            if (m_text != null)
                m_text.Owner.Remove(m_text);

            m_text = new TextComponent("UIOverlay0");

            SpriteFont font = Engine.AssetManager.Get<SpriteFont>("Graphics/GameplayFont");
            float fontScale = 0.5f;
            Vector2 offset = new Vector2(10, 50);

            m_text.Text = text;
            m_text.Alignement = TextAlignementHorizontal.Center;
            m_text.Style = new TextStyle();
            m_text.Style.Font = font;
            m_text.Style.Scale = fontScale;
            m_text.Style.Color = color;
            m_text.Style.Scale = m_scaleStart;

            m_text.Position = offset;

            parent.Attach(m_text);

            m_textEffectTimer.TargetTime = m_textEffectTimeMS;
            m_textEffectTimer.Start();
            
            m_fadeStart =  Convert.ToSingle(m_text.Style.Color.A);
            m_fadeEnd = 0;

            m_moveStart = m_text.Position ;
            m_moveEnd = m_text.Position + m_moveValue;

        }


        //
        public void OnPlayerPassAssist(object arg)
        {
            if (!Enabled)
                return;
            
            Player player = (Player)((object[])arg)[0];
            string text = "Pass Assist";

            CreateFeedBackText(player.Owner, text, player.Parameters.Color);
        }


        //
        public void OnPlayerShootBallCharged(object arg)
        {
            if (!Enabled)
                return;
            
            Player player = (Player)((object[])arg)[0];
            string text = "Charged Shot";

            CreateFeedBackText(player.Owner, text, player.Parameters.Color);
        }
    }
}
