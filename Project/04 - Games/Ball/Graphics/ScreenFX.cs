using LBE.Gameplay;
using LBE.Graphics.Sprites;
using Microsoft.Xna.Framework.Graphics;
using LBE;
using Microsoft.Xna.Framework;
using LBE.Graphics;

namespace Ball.Graphics
{
    public class ScreenFade : GameObjectComponent, IRenderable
    {
        float m_opacity = 1;
        public float Opacity
        {
            get { return m_opacity; }
            set { m_opacity = value; }
        }

        int m_fadeSign;

        Texture2D m_pixel;
      
        float m_fadeOpacity;

        Timer m_fadeTimerMS;

        //
        public enum FadeType
        {
            FadeIn,
            FadeOut
        }


        //
        public override void Start()
        {
            m_pixel = new Texture2D(Engine.Renderer.Device, 1, 1);
            m_pixel.SetData<Color>(new Color[] { Color.White });

            Engine.Renderer.RenderLayers["BlackScreenOverlay"].Renderables.Add(this);

            m_fadeOpacity = 0;
        }


        //
        public override void Update()
        {
            if (m_fadeTimerMS.Active)
            {
                float maxOpacity = LBE.MathHelper.Clamp(0, 1, m_opacity);
                m_fadeOpacity += (1 - maxOpacity) + maxOpacity * m_fadeSign * Engine.RealTime.ElapsedMS / m_fadeTimerMS.TargetTime;
                LBE.MathHelper.Clamp(0, 1, m_fadeOpacity);
            }
        }


        //
        public void Draw()
        {
            float screenWidth = Engine.Renderer.MainBuffer.Width;
            float screenHeight = Engine.Renderer.MainBuffer.Height;
            Engine.Renderer.SpriteBatch.Draw(m_pixel, new Vector2(0, 0), null, new Color(0, 0, 0, m_fadeOpacity), 0f, Vector2.Zero, new Vector2(screenWidth, screenHeight), SpriteEffects.None, 0);
        }


        //
        public override void End()
        {
            Engine.Renderer.RenderLayers["BlackScreenOverlay"].Renderables.Remove(this); 
        }


        //
        public void StartFade(FadeType fadeType, float timeMS , bool destroyAtEnd)
        {
            if (fadeType == FadeType.FadeIn)
            {
                m_fadeOpacity = m_opacity;
                m_fadeSign = -1;

            }
            else
            {
                m_fadeOpacity = 0;
                m_fadeSign = 1;
            }

            m_fadeTimerMS = new Timer(Engine.RealTime.Source, timeMS);
           
            if (destroyAtEnd)
            {
                m_fadeTimerMS.OnTime += m_fadeTimer_OnTime;
            }
            
            m_fadeTimerMS.Start();
        }


        //
        private void m_fadeTimer_OnTime(Timer Source)
        {
            Owner.Remove(this);
        }
    }
}