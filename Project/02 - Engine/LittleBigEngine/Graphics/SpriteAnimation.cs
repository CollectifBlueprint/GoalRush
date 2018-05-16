using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;

namespace LBE.Graphics.Sprites
{
    public class SpriteAnimation
    {
        String m_name;
        public String Name
        {
            get { return m_name; }
        }

        Texture2D m_texture;
        public Texture2D Texture
        {
            get { return m_texture; }
        }

        float m_frameTime;
        public float FrameTime
        {
            get { return m_frameTime; }
        }

        int m_startIndex;
        public int StartIndex
        {
            get { return m_startIndex; }
        }

        int m_endIndex;
        public int EndIndex
        {
            get { return m_endIndex; }
        }

        bool m_loop;
        public bool Loop
        {
            get { return m_loop; }
        }

        Point m_size;
        public Point Size
        {
            get { return m_size; }
        }

        Point m_frameCount;
        public Point FrameCount
        {
            get { return m_frameCount; }
        }

        public SpriteAnimation(SpriteAnimationDefinition spriteAnimDefinition)
        {
            m_name = spriteAnimDefinition.Name;

            m_texture = spriteAnimDefinition.Texture;

            m_startIndex = spriteAnimDefinition.StartIndex;
            m_endIndex = spriteAnimDefinition.EndIndex;

            m_frameCount = spriteAnimDefinition.FrameCount;

            m_frameTime = spriteAnimDefinition.FrameTime;
            m_loop = spriteAnimDefinition.Loop;

            if (m_frameCount.X == 0) m_frameCount.X = 1;
            if (m_frameCount.Y == 0) m_frameCount.Y = 1;

            m_size.X = m_texture.Width / m_frameCount.X;
            m_size.Y = m_texture.Height / m_frameCount.Y;

            int nFrame = m_frameCount.X * m_frameCount.Y;
            if(m_endIndex == -1)
            {
                //If endIndex is unspecified, assume its the last frame in the animation
                m_endIndex = nFrame - 1;
            }
        }
    }
}
