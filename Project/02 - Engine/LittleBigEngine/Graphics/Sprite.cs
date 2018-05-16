using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace LBE.Graphics.Sprites
{
    public class Sprite
    {
        Asset<SpriteDefinition> m_definition;

        Dictionary<String, SpriteAnimation> m_animations;
        public Dictionary<String, SpriteAnimation> Animations
        {
            get { return m_animations; }
        }

        SpriteAnimation m_currentAnimation;
        public SpriteAnimation CurrentAnimation
        {
            get { return m_currentAnimation; }
        }

        public Texture2D Texture
        {
            get { return m_currentAnimation.Texture; }
        }

        public Point Size
        {
            get { return m_currentAnimation.Size; }
        }

        Point m_source;
        public Point Source
        {
            get { return m_source; }
        }

        Color m_color;
        public Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        float m_alpha;
        public float Alpha
        {
            get { return m_alpha; }
            set { m_alpha = value; }
        }

        Vector2 m_scale;
        public Vector2 Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        float m_orientation;
        public float Orientation
        {
            get { return m_orientation; }
            set { m_orientation = value; }
        }

        bool m_mirror;
        public bool Mirror
        {
            get { return m_mirror; }
            set { m_mirror = value; }
        }

        bool m_playing;
        public bool Playing
        {
            get { return m_playing; }
            set { m_playing = value; }
        }

        float m_animationTime;
        public float AnimationTime
        {
            get { return m_animationTime; }
            set { SetAnimationTime(value); }
        }

        int m_animationIndex;
        public int AnimationIndex
        {
            get { return m_animationIndex; }
            set { SetAnimationIndex(value); }
        }

        float m_animationSpeed;
        public float AnimationSpeed
        {
            get { return m_animationSpeed; }
            set { m_animationSpeed = value; }
        }

        public static Sprite Create(String path)
        {
            Asset<SpriteDefinition> asset = Engine.AssetManager.GetAsset<SpriteDefinition>(path);
            return new Sprite(asset);
        }

        public static Sprite Create(SpriteDefinition def)
        {
            return new Sprite(def);
        }

        public static Sprite Create(Asset<SpriteDefinition> asset)
        {
            return new Sprite(asset);
        }

        public static Sprite CreateFromTexture(String path)
        {
            var texAsset = Engine.AssetManager.GetAsset<Texture2D>(path);
            var animDef = new SpriteAnimationDefinition();
            animDef.Texture = texAsset.Content;
            var spriteDef = new SpriteDefinition();
            var SpriteDefAsset = new Asset<SpriteDefinition>(spriteDef);
            spriteDef.Animations = new SpriteAnimationDefinition[1] { animDef };
            texAsset.OnAssetChanged +=
                () =>
                {
                    var newAnimDef = new SpriteAnimationDefinition();
                    newAnimDef.Texture = texAsset.Content;
                    SpriteDefAsset.Content.Animations = new SpriteAnimationDefinition[1] { newAnimDef };
                    SpriteDefAsset.Reload();
                };

            return new Sprite(SpriteDefAsset);
        }

        public static Sprite CreateFromTexture(Texture2D texture)
        {
            var animDef = new SpriteAnimationDefinition();
            animDef.Texture = texture;

            var spriteDef = new SpriteDefinition();
            spriteDef.Animations = new SpriteAnimationDefinition[1] { animDef };
            return new Sprite(spriteDef);
        }

        public Sprite(Asset<SpriteDefinition> spriteAsset)
        {
            m_definition = spriteAsset;
            m_animations = new Dictionary<string, SpriteAnimation>();

            spriteAsset.OnAssetChanged += new OnChange(spriteAsset_OnAssetChanged);

            Reset(m_definition.Content);
        }

        public Sprite(SpriteDefinition sprite)
        {
            m_definition = new Asset<SpriteDefinition>(sprite);
            m_animations = new Dictionary<string, SpriteAnimation>();

            m_definition.OnAssetChanged += new OnChange(spriteAsset_OnAssetChanged);

            Reset(m_definition.Content);
        }

        void spriteAsset_OnAssetChanged()
        {
            Reset(m_definition.Content);
        }

        void Reset(SpriteDefinition spriteDefinition)
        {
            m_animations.Clear();
            if (spriteDefinition.Animations != null)
            {
                foreach (SpriteAnimationDefinition def in spriteDefinition.Animations)
                {
                    m_animations.Add(def.Name, new SpriteAnimation(def));
                }
            }
            else if (spriteDefinition.Texture != null)
            {
                SpriteAnimationDefinition def = new SpriteAnimationDefinition();
                def.Texture = spriteDefinition.Texture;
                m_animations.Add(spriteDefinition.DefaultAnimation, new SpriteAnimation(def));
            }
            SetAnimation(spriteDefinition.DefaultAnimation);
            SetAnimationIndex(m_currentAnimation.StartIndex);      

            m_animationSpeed = spriteDefinition.AnimationSpeed;

            m_alpha = spriteDefinition.Alpha;
            m_scale = new Vector2(spriteDefinition.Scale, spriteDefinition.Scale);
            m_orientation = spriteDefinition.Orientation;
            m_color = new Color(spriteDefinition.Color);
            m_mirror = false;
        }

        public void SetAnimation(String name)
        {
            m_currentAnimation = m_animations[name];
            m_animationTime = 0.0f;
            m_animationIndex = m_currentAnimation.StartIndex;
        }

        void SetAnimationTime(float time)
        {
            m_animationTime = time;

            int nIndex = m_currentAnimation.EndIndex - m_currentAnimation.StartIndex + 1;
            m_animationIndex = m_currentAnimation.StartIndex
                + (int)(m_animationTime / m_currentAnimation.FrameTime) % nIndex;

            UpdateSource();
        }

        void SetAnimationIndex(int index)
        {
            m_animationIndex = m_currentAnimation.StartIndex + index;

            if (index > m_currentAnimation.EndIndex)
                index = m_currentAnimation.EndIndex;
            if (index < m_currentAnimation.StartIndex)
                index = m_currentAnimation.EndIndex;

            m_animationTime = (index - m_currentAnimation.StartIndex) * m_currentAnimation.FrameTime;

            UpdateSource();
        }

        public void ScaleToSize(Vector2 size)
        {
            m_scale = new Vector2(
                size.X / Texture.Width,
                size.Y / Texture.Height);
        }

        public void ScaleToSizeFixedRatio(Vector2 size)
        {
            float ratio = Math.Min(size.X / Texture.Width, size.Y / Texture.Height);
            m_scale = new Vector2(ratio, ratio);
        }

        void UpdateSource()
        {
            int row = m_animationIndex / m_currentAnimation.FrameCount.X;
            int column = m_animationIndex % m_currentAnimation.FrameCount.X;

            m_source = new Point(
                column * m_currentAnimation.Size.X,
                row * m_currentAnimation.Size.Y);
        }

        public void Update(float timeMS)
        {
            if (m_playing)
            {
                //If not looping the animation, stop it when reaching the end
                int nIndex = m_currentAnimation.EndIndex - m_currentAnimation.StartIndex + 1;
                if (!m_currentAnimation.Loop && m_animationTime + m_animationSpeed * timeMS > m_currentAnimation.FrameTime * nIndex)
                {
                    m_playing = false;
                    SetAnimationIndex(m_currentAnimation.EndIndex);
                }
                else
                {
                    SetAnimationTime(m_animationTime + m_animationSpeed * timeMS);
                }
            }
        }
    }
}
