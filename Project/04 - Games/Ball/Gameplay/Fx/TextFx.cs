using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LBE.Graphics.Sprites;
using LBE;
using LBE.Gameplay;

namespace Ball.Gameplay.Fx
{
    public class TextFxParameters
    {
        public float TextEffectTimeMS;
        public float ScaleStart;
        public float ScaleEnd;
        public float ScaleTimeMS;
        public float ScaleDelayMS;

        public float FadeStart;
        public float FadeEnd;
        public float FadeTimeMS;
        public float FadeDelayMS;

        public float MoveTimeMS;
        public float MoveDelayMS;
        public Vector2 MoveValue;
    }    
    
    public class TextFx : GameObjectComponent
    {
        TextComponent m_text;

        Timer m_textEffectTimer;
        public Timer TextEffectTimer
        {
            get { return m_textEffectTimer;  }
        }

        float m_textEffectTimeMS;
        public float TextEffectTimeMS
        {
            set { m_textEffectTimeMS = value; }
            get { return m_textEffectTimeMS; }
        }

        float m_scaleStart = 1;
        public float ScaleStart
        {
            set { m_scaleStart = value; }
            get { return m_scaleStart;}
        }

        float m_scaleEnd = 1;
        public float ScaleEnd
        {
            set { m_scaleEnd = value; }
            get { return m_scaleEnd; }
        }

        float m_scaleTimeMS = 0;
        public float ScaleTimeMS
        {
            set { m_scaleTimeMS = value; }
            get { return m_scaleTimeMS; }
        }

        float m_scaleDelayMS = 0;
        public float ScaleDelayMS
        {
            set { m_scaleDelayMS = value; }
            get { return m_scaleDelayMS; }
        }

        float m_fadeStart = 255;
        public float FadeStart
        {
            set { m_fadeStart = value; }
            get { return m_fadeStart; }
        }

        float m_fadeEnd = 0;
        public float FadeEnd
        {
            set { m_fadeEnd = value; }
            get { return m_fadeEnd; }
        }

        float m_fadeTimeMS = 0;
        public float FadeTimeMS
        {
            set { m_fadeTimeMS = value; }
            get { return m_fadeTimeMS; }
        }

        float m_fadeDelayMS = 0;
        public float FadeDelayMS
        {
            set { m_fadeDelayMS = value; }
            get { return m_fadeDelayMS; }
        }

        Vector2 m_moveStart;
        public Vector2 MoveStart
        {
            set { m_moveStart = value; }
            get { return m_moveStart; }
        }

        Vector2 m_moveEnd;
        public Vector2 MoveEnd
        {
            set { m_moveEnd = value; }
            get { return m_moveEnd; }
        }

        float m_moveTimeMS = 0;
        public float MoveTimeMS
        {
            set { m_moveTimeMS = value; }
            get { return m_moveTimeMS; }
        }

        float m_moveDelayMS = 0;
        public float MoveDelayMS
        {
            set { m_moveDelayMS = value; }
            get { return m_moveDelayMS; }
        }

        Vector2 m_moveValue;
        public Vector2 MoveValue
        {
            set { m_moveValue = value; }
            get { return m_moveValue; }
        }

        public TextFx(TextComponent textComponent)
        {
            m_text = textComponent;
        }


        public override void Start()
        {
             //parent.Attach(m_text);
             m_textEffectTimer = new Timer(Engine.GameTime.Source, m_textEffectTimeMS);
             m_textEffectTimer.OnTime += m_textEffectTimer_OnTime;
             m_fadeStart =  Convert.ToSingle(m_text.Style.Color.A);
             m_fadeEnd = 0;
        }

        void m_textEffectTimer_OnTime(Timer source)
        {
            Owner.Remove(this);
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
                }
                else
                {
                    m_text.Position = m_moveStart;
                }

            }
        }


        public void StartFx()
        {
            m_textEffectTimer.TargetTime = m_textEffectTimeMS;
            m_textEffectTimer.Start();

            m_moveStart = m_text.Position ;
            m_moveEnd = m_text.Position + m_moveValue;
        }

        public void SetParameters(TextFxParameters parameters)
        {
             m_textEffectTimeMS = parameters.TextEffectTimeMS;
             m_scaleStart = parameters.ScaleStart;
             m_scaleEnd = parameters.ScaleEnd;
             m_scaleTimeMS = parameters.ScaleTimeMS;
             m_scaleDelayMS = parameters.ScaleDelayMS;
             
             m_fadeStart = parameters.FadeStart;
             m_fadeEnd = parameters.FadeEnd;
             m_fadeTimeMS = parameters.FadeTimeMS;
             m_fadeDelayMS = parameters.FadeDelayMS;
             
             m_moveTimeMS = parameters.MoveTimeMS;
             m_moveDelayMS = parameters.MoveDelayMS;
             m_moveValue = parameters.MoveValue;
        }

        public override void End()
        {
            base.End();
            m_textEffectTimer.Stop();
            m_textEffectTimer.OnTime -= m_textEffectTimer_OnTime;
        }

    }
}
