using Ball.Graphics;
using LBE;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Ball.Gameplay
{


    public class PlayerOffScreenMarker : GameObjectComponent
    {
        float m_playerSpriteOffset = 10;

        Player m_player;
        ColorMaskedSprite m_markerSprite;

        bool m_hide = false;
        public bool Hide
        {
            get { return m_hide; }
            set { m_hide = value; }
        }

        public override void Start()
        {
        }

        public void Init()
        {
            m_player = (Player)Owner.FindComponent<Player>();
            m_markerSprite = Game.GameManager.MatchFeedBack.PlayerOffScreenMarkerSprites[(int)m_player.PlayerIndex];     
        }

        public override void Update()
        {
            if (m_hide)
                return;

            int isPlayerOffScreenX = 0;

            Vector2 camborderXMaxScreen = new Vector2(Engine.Renderer.Cameras[(int)CameraId.Game].Width, 0);
            Vector2 camborderXMaxWorld = Engine.Renderer.Cameras[(int)CameraId.Game].ScreenToWorld(camborderXMaxScreen);

            Vector2 camborderXMinScreen = Vector2.Zero;
            Vector2 camborderXMinWorld = Engine.Renderer.Cameras[(int)CameraId.Game].ScreenToWorld(camborderXMinScreen);

            if (m_player.Position.X >  camborderXMaxWorld.X + m_playerSpriteOffset)
            {
                isPlayerOffScreenX = 1;
            }
            else if (m_player.Position.X < camborderXMinWorld.X - m_playerSpriteOffset)
            {
                isPlayerOffScreenX = -1;
            }

            if (isPlayerOffScreenX != 0)
            {
                //m_markerSprite.Position = new Vector2(Game.GameManager.Camera.ViewZoomBoundingBoxWidth.Max.X, m_player.Position.Y);
                int padding = 30;
                float markerSpriteOffset = 0;
                Vector2 markerPositionWorld = Vector2.Zero;
                if (isPlayerOffScreenX == 1)
                {
                    Vector2 markerPositionScreen = new Vector2(Engine.Renderer.Cameras[(int)CameraId.Game].Width - padding, 0);
                    markerPositionWorld = Engine.Renderer.Cameras[(int)CameraId.Game].ScreenToWorld(markerPositionScreen);
                    m_markerSprite.Orientation = 0;
                
                }
                if (isPlayerOffScreenX == -1)
                {
                    Vector2 markerPositionScreen = new Vector2(padding, 0);
                    markerPositionWorld = Engine.Renderer.Cameras[(int)CameraId.Game].ScreenToWorld(markerPositionScreen);
                    m_markerSprite.Orientation = (float) Math.PI;
                }
                
                new Vector2(Engine.Renderer.Cameras[(int)CameraId.Game].Position.X + markerSpriteOffset, m_player.Position.Y);
                markerPositionWorld.Y = m_player.Position.Y;
                m_markerSprite.Position = markerPositionWorld;

                m_markerSprite.Visible = true;
            }
            else
            {
                m_markerSprite.Visible = false;
            }

        }
      


    }
}