using LBE.Graphics.Camera;
using Microsoft.Xna.Framework.Graphics;

namespace LBE.Graphics
{
    public class SpriteRenderLayer : RenderLayer
    {
        public SpriteRenderLayer()
        {
        }

        public SpriteRenderLayer(ICamera camera)
        {
            m_camera = camera;
        }


        public override void Start()
        {
            Engine.Renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
        }

        public override void End()
        {
            Engine.Renderer.SpriteBatch.End();
        }
    }
}