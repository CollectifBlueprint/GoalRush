using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using LBE;
using Ball.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ball.Gameplay
{
    public class PlayerTackleFx : GameObjectComponent
    {
        Player m_player;

        ColorMaskedSprite[] m_spriteCmps;

        float m_fadeTimeMS;
        float m_repeatTimeMS;
        float m_totalTimeMS;

        Timer m_repeatTimer;
        TimerEvent m_repeatTimerEvent;
        Timer m_lifeTimer;
        TimerEvent m_lifeTimerEvent;

        public override void Start()
        {
            m_player = Owner.FindComponent<Player>();

            int nSprite = 10;
            m_spriteCmps = new ColorMaskedSprite[nSprite];
            for (int i = 0; i < nSprite; i++)
            {

                var allPlayerSpriteInfos = Engine.AssetManager.Get<PlayerSpriteInfo[]>("Game/PlayerSprites.lua::PlayerSpritesTackle");
                var playerSpriteInfo = allPlayerSpriteInfos[(int)m_player.PlayerIndex];

                Sprite playerSprite = Sprite.CreateFromTexture(playerSpriteInfo.Material);

                playerSprite.Scale = new Vector2(0.48f, 0.48f);
                playerSprite.Orientation = m_player.Owner.Orientation;

                m_spriteCmps[i] = new ColorMaskedSprite(playerSprite, "ArenaOverlay5");
                var texAsset = playerSpriteInfo.Mask;
                m_spriteCmps[i].Mask = texAsset;

                m_spriteCmps[i].Color1 = m_player.PlayerColors[0];
                m_spriteCmps[i].Color2 = m_player.PlayerColors[1];
                m_spriteCmps[i].Color3 = m_player.PlayerColors[2];

                m_spriteCmps[i].AttachedToOwner = false;
                m_spriteCmps[i].Sprite.Alpha = 0;
                Owner.Attach(m_spriteCmps[i]);
            }

            m_fadeTimeMS = 0.12f * 1000;
            m_repeatTimeMS = 0.03f * 1000;
            m_totalTimeMS = 0.2f * 1000;

            m_repeatTimer = new Timer(Engine.GameTime.Source, m_repeatTimeMS, TimerBehaviour.Restart);
            m_repeatTimerEvent = new TimerEvent(m_repeatTimer_OnTime);
            m_repeatTimer.OnTime += m_repeatTimerEvent;
            m_repeatTimer.Start();
            m_repeatTimer_OnTime(null);

            m_lifeTimer = new Timer(Engine.GameTime.Source, m_totalTimeMS, TimerBehaviour.Stop);
            m_lifeTimerEvent = new TimerEvent(m_lifeTimer_OnTime);
            m_lifeTimer.OnTime += m_lifeTimerEvent;
            m_lifeTimer.Start();
        }

        void m_lifeTimer_OnTime(Timer source)
        {
            m_repeatTimer.Stop();
        }

        void m_repeatTimer_OnTime(Timer source)
        {
            for (int i = 0; i < m_spriteCmps.Length; i++)
            {
                if (m_spriteCmps[i].Sprite.Alpha <= 0)
                {
                    m_spriteCmps[i].Sprite.Alpha = 1;
                    m_spriteCmps[i].Position = Owner.Position;
                    break;
                }
            }
        }

        public override void Update()
        {
            bool alive = false;
            for (int i = 0; i < m_spriteCmps.Length; i++)
            {
                float alphaDelta = Engine.GameTime.ElapsedMS / m_fadeTimeMS * 1.0f;
                m_spriteCmps[i].Sprite.Alpha -= alphaDelta;

                if (m_spriteCmps[i].Sprite.Alpha > 0) 
                    alive = true;
            }

            if (alive == false && m_lifeTimer.Active == false)
            {
                Owner.Remove(this);
            }
        }

        public override void End()
        {
            foreach (var spriteCmp in m_spriteCmps.ToArray())
            {
                Owner.Remove(spriteCmp);
            }

            m_lifeTimer.Stop();
            m_lifeTimer.OnTime -= m_lifeTimerEvent;

            m_repeatTimer.Stop();
            m_repeatTimer.OnTime -= m_repeatTimerEvent;
        }
    }
}
