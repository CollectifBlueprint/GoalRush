using LBE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Ball.Gameplay.BallEffects
{
    public enum BallFadeSprite
    {
        AllSprites = 0,
        BallSprite,
        BallBashSprite
    }
    
    public class BallFadeEffect : BallEffect
    {
        float m_fadeStart = 1;
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

        BallFadeSprite m_ballFadeSprite = BallFadeSprite.AllSprites;
        public BallFadeSprite BallFadeSprite
        {
            set { m_ballFadeSprite = value; }
            get { return m_ballFadeSprite; }
        }

        public override void Start()
        {
            m_fadeTimeMS = Timer.TargetTime;
        }

        public override void Update()
        {
            if (Timer.TimeMS > m_fadeDelayMS)
            {
                float fadeVariation = m_fadeEnd - m_fadeStart;

                float fadeCoef = LBE.MathHelper.LinearStep(m_fadeDelayMS, m_fadeTimeMS + m_fadeDelayMS, Timer.TimeMS);
                float currentFade = m_fadeStart + fadeVariation * fadeCoef;

                if (m_ballFadeSprite == BallFadeSprite.AllSprites)
                {
                    Ball.BallSprite.Alpha = currentFade;
                    Ball.BallBashSprite.Alpha = currentFade;
                }
                else if (m_ballFadeSprite == BallFadeSprite.BallSprite)
                {
                    Ball.BallSprite.Alpha = currentFade;
                }
                else if (m_ballFadeSprite == BallFadeSprite.BallBashSprite)
                {
                    Ball.BallBashSprite.Alpha = currentFade;
                }
            }
        }


        public override void End()
        {
            float currentFade = m_fadeEnd;

            if (m_ballFadeSprite == BallFadeSprite.AllSprites)
            {
                Ball.BallSprite.Alpha = currentFade;
                Ball.BallBashSprite.Alpha = currentFade;
            }
            else if (m_ballFadeSprite == BallFadeSprite.BallSprite)
            {
                Ball.BallSprite.Alpha = currentFade;
            }
            else if (m_ballFadeSprite == BallFadeSprite.BallBashSprite)
            {
                Ball.BallBashSprite.Alpha = currentFade;
            }
        }
    }
}

