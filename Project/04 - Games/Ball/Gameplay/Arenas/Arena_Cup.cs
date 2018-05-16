using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using LBE;

namespace Ball.Gameplay.Arenas
{

   


    public partial class Arena : GameObjectComponent
    {

        SpriteComponent m_cupSpriteLeftCmp;
        public SpriteComponent CupSpriteLeftCmp
        {
            get { return m_cupSpriteLeftCmp; }
        }

        SpriteComponent m_cupSpriteRightCmp;
        public SpriteComponent CupSpriteRightCmp
        {
            get { return m_cupSpriteRightCmp; }
        }

        GameObject m_cupCamFocus;
        public GameObject CupCamFocus
        {
            get { return m_cupCamFocus; }
        }

        private void InitCup()
        {
            m_cupCamFocus = new GameObject("CupCam Focus");
            m_cupCamFocus.Position = new Vector2(0, 200);   

            Sprite spriteCupLeft = Sprite.Create("Graphics/cupSprite.lua::Sprite");
            m_cupSpriteLeftCmp = new SpriteComponent(spriteCupLeft, "GroundOverlay1");
            m_cupSpriteLeftCmp.Sprite.Scale = new Vector2(0.10f, 0.10f);
            Owner.Attach(m_cupSpriteLeftCmp);
            int cupId = Math.Min(LeftGoal.Team.ConsecutiveWins, 7);
            string animationNameLeft ="Cup" + cupId.ToString();
            m_cupSpriteLeftCmp.Sprite.SetAnimation(animationNameLeft);
            m_cupSpriteLeftCmp.Sprite.Playing = true;
            m_cupSpriteLeftCmp.Position = new Vector2(-Engine.Debug.EditSingle("CupXOffset"), Engine.Debug.EditSingle("CupYOffset"));

            Sprite spriteCupRight = Sprite.Create("Graphics/cupSprite.lua::Sprite");
            m_cupSpriteRightCmp = new SpriteComponent(spriteCupRight, "GroundOverlay1");
            m_cupSpriteRightCmp.Sprite.Scale = new Vector2(0.10f, 0.10f);
            Owner.Attach(m_cupSpriteRightCmp);
            cupId = Math.Min(RightGoal.Team.ConsecutiveWins, 7);
            string animationNameRight = "Cup" + cupId.ToString();
            m_cupSpriteRightCmp.Sprite.SetAnimation(animationNameRight);
            m_cupSpriteRightCmp.Sprite.Playing = true;
            m_cupSpriteRightCmp.Position = new Vector2(Engine.Debug.EditSingle("CupXOffset"), Engine.Debug.EditSingle("CupYOffset"));

            Engine.World.EventManager.AddListener((int)EventId.Victory, OnMatchVictory);

        }

        public void SwitchCups()
        {
            SpriteAnimation AnimationTmp = m_cupSpriteLeftCmp.Sprite.CurrentAnimation;
            m_cupSpriteLeftCmp.Sprite.SetAnimation(m_cupSpriteRightCmp.Sprite.CurrentAnimation.Name);
            m_cupSpriteLeftCmp.Sprite.Playing = true;
            m_cupSpriteRightCmp.Sprite.SetAnimation(AnimationTmp.Name);
            m_cupSpriteRightCmp.Sprite.Playing = true;
        }

        public void OnMatchVictory(object eventParameter)
        {


            if (LeftGoal.Score == RightGoal.Score)
            {
               
            }
            else
            {
                if (LeftGoal.Score > RightGoal.Score)
                {
                    RightGoal.Team.ConsecutiveWins++;
                    LeftGoal.Team.ConsecutiveWins = 0;
                }
                else
                {
                    LeftGoal.Team.ConsecutiveWins++;
                    RightGoal.Team.ConsecutiveWins = 0;
                }
            }
        }
    }
}
