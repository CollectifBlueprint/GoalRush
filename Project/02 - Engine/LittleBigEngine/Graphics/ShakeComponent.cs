using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using LBE;
using Microsoft.Xna.Framework;

namespace Ball.Gameplay
{
    public class ShakeComponent : GameObjectComponent
    {
        Vector2 m_initialSpritePos;
        SpriteComponent m_sprite;

        float m_shakeAmount;
        public float ShakeAmount
        {
            set { m_shakeAmount = value; }
            get { return m_shakeAmount; }
        }        

        bool doShake;

        public ShakeComponent(SpriteComponent sprite)
        {
            m_sprite = sprite;
        }

        public override void Start()
        {
            m_initialSpritePos = m_sprite.Position;
            doShake = false;
        }

        public override void Update()
        {
            if (doShake && Engine.GameTime.Source.Paused == false)
            {
                Random random = new Random();
                float shift = random.NextFloat(-0.5f * m_shakeAmount, 0.5f * m_shakeAmount);
     
                m_sprite.Position = new Vector2(m_initialSpritePos.X + shift, m_initialSpritePos.Y + shift); 
            }
        }

        public void StartShake()
        {
            doShake = true;
        }

        public void StopShake()
        {
            doShake = false;
            m_sprite.Position = m_initialSpritePos;
        }
    }
}
