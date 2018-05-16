using System;
using System.Collections.Generic;
using LBE.Graphics.Camera;
using LBE.Graphics.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LBE.Graphics;
using LBE;
using System.Linq;

//
namespace MonoApp
{
    public class Renderer : BaseEngineComponent
    {
        private GraphicsDevice m_device;
        public GraphicsDevice Device
        {
            get { return m_device; }
            set { m_device = value; }
        }

        Camera2D m_currentCamera;
        public Camera2D CurrentCamera
        {
            get { return m_currentCamera; }
            set { m_currentCamera = value; }
        }

        Camera2D[] m_cameras;
        public Camera2D[] Cameras
        {
            get { return m_cameras; }
        }

        public Camera2D GameCamera
        {
            get { return m_cameras[0]; }
        }

        public Camera2D InterfaceCamera
        {
            get { return m_cameras[1]; }
        }

        Vector2 m_cameraPosition;
        public Vector2 CameraPosition
        {
            get { return m_cameraPosition; }
            set { m_cameraPosition = value; }
        }

        float m_cameraZoom;
        public float CameraZoom
        {
            get { return m_cameraZoom; }
            set { m_cameraZoom = value; }
        }

        private SpriteBatch m_spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return m_spriteBatch; }
        }

        Dictionary<string, RenderLayer> m_renderLayers;
        public Dictionary<string, RenderLayer> RenderLayers
        {
            get { return m_renderLayers; }
        }

        SpriteFont m_spriteFont;
        public SpriteFont SpriteFont
        {
            get { return m_spriteFont; }
        }

        RenderTarget2D m_mainBuffer;
        public RenderTarget2D MainBuffer
        {
            get { return m_mainBuffer; }
        }

        Sprite m_mainBufferSprite;

        public override void Startup()
        {
            Engine.AssetManager.RegisterAssetType<SpriteFont>(new SpriteFontLoader());
            Engine.AssetManager.RegisterAssetType<Texture2D>(new Texture2DLoader());
            Engine.AssetManager.RegisterAssetType<Sprite>(new SpriteLoader());
            Engine.AssetManager.RegisterAssetType<Effect>(new EffectLoader());

            m_mainBuffer = new RenderTarget2D(m_device, 1920, 1200);
            m_mainBufferSprite = Sprite.CreateFromTexture(m_mainBuffer);

            m_cameraZoom = 1;
            m_cameras = new Camera2D[]
            {
                new Camera2D(Vector2.Zero, m_device.PresentationParameters.BackBufferWidth, m_device.PresentationParameters.BackBufferHeight), // game camera
                new Camera2D(Vector2.Zero, m_device.PresentationParameters.BackBufferWidth, m_device.PresentationParameters.BackBufferHeight) // gui camera
            };

            m_currentCamera = m_cameras[0];

            m_renderLayers = new Dictionary<String, RenderLayer>();
            m_renderLayers.Add("Background", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("Default", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("GroundOverlay0", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("GroundOverlay1", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("GroundOverlay2", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("GroundOverlay3", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaOverlay0", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaOverlay1", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaOverlay2", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaOverlay3", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaOverlay4", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaOverlay5", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaOverlay6", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaOverlay7", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaOverlay8", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaOverlay9", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaOverlay10", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaOverlay11", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaUIOverlay0", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("ArenaUIOverlay1", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("UIOverlay0", new SpriteRenderLayer(m_cameras[1]));
            m_renderLayers.Add("UIOverlay1", new SpriteRenderLayer(m_cameras[1]));
            m_renderLayers.Add("BlackScreenOverlay", new SpriteRenderLayer(m_cameras[0]));
            m_renderLayers.Add("MenuBackground", new SpriteRenderLayer(m_cameras[1]));
            m_renderLayers.Add("Menu", new SpriteRenderLayer(m_cameras[1]));
            m_renderLayers.Add("Menu2", new SpriteRenderLayer(m_cameras[1]));
            m_renderLayers.Add("DebugOverlay", new RenderLayer(m_cameras[0]));
            m_renderLayers.Add("DebugPhysicsOverlay", new RenderLayer(m_cameras[0]));
            m_renderLayers.Add("HeatMapOverlay", new RenderLayer(m_cameras[0]));

            m_spriteBatch = new SpriteBatch(m_device);
            m_spriteFont = Engine.AssetManager.Get<SpriteFont>("System/SystemFont");

            Engine.Log.Write("Backbuffer size is:" + m_device.PresentationParameters.BackBufferWidth + ":" + m_device.PresentationParameters.BackBufferHeight);
        }

        public override void Shutdown()
        {
        }

        public void BeginScene()
        {
            m_cameraZoom = Engine.Debug.EditSingle("Zoom", 1.0f);
            float width = m_mainBuffer.Width / m_cameraZoom;
            float height = m_mainBuffer.Height / m_cameraZoom;

            foreach (var camera in m_cameras)
            {
                camera.SetProjection((int)width, (int)height);
                camera.SetView();
            }

            Device.Clear(Color.White);
        }

        public void Render()
        {
            m_device.SetRenderTarget(m_mainBuffer);
            Device.Clear(Color.Black);

            foreach (var renderLayer in m_renderLayers.Values)
            {
                m_currentCamera = (Camera2D)renderLayer.Camera;
                if (renderLayer.Enabled) renderLayer.Draw();
            }
        }

        public void EndScene()
        {
            int windowWidth = Engine.Application.Window.ClientBounds.Width;
            int windowHeight = Engine.Application.Window.ClientBounds.Height;
            int desiredSourceWidth = 1920;
            int desiredSourceHeight = 1080;

            Camera2D cam = new Camera2D(Vector2.Zero, windowWidth, windowHeight);
            CurrentCamera = cam; 

            m_device.SetRenderTarget(null);
            Device.Clear(Color.Black);

            float ratio = Math.Min((float)windowWidth / desiredSourceWidth, (float)windowHeight / desiredSourceHeight);
            m_mainBufferSprite.Scale = new Vector2(ratio, ratio);

            m_spriteBatch.Begin();
            Draw(m_mainBufferSprite, Vector2.Zero);
            m_spriteBatch.End();
        }

        public void DrawStringUnbatched(Point screenPos, string msg, Color color)
        {
            SpriteBatch.Begin();
            SpriteBatch.DrawString(m_spriteFont, msg, screenPos, color);
            SpriteBatch.End();
        }

        public void DrawString(String text, Vector2 position)
        {
            DrawString(text, position, 1.0f, new TextStyle(), TextAlignementHorizontal.Left, TextAlignementVertical.Up);
        }

        public void DrawString(String text, Vector2 position, TextStyle style)
        {
            DrawString(text, position, 1.0f, style, TextAlignementHorizontal.Left, TextAlignementVertical.Up);
        }

        public void DrawString(String text, Vector2 position, float scale, TextStyle style, TextAlignementHorizontal alignement, TextAlignementVertical alignementVertical)
        {
            Vector2 worldPos = position;
            SpriteFont font = style.Font != null ? style.Font : m_spriteFont;

            Vector2 stringSize = font.MeasureString(text);
            if (alignement == TextAlignementHorizontal.Center)
                worldPos.X -= stringSize.X * 0.5f * style.Scale * scale;
            if (alignement == TextAlignementHorizontal.Right)
                worldPos.X -= stringSize.X * 1.0f * style.Scale * scale;

            if (alignementVertical == TextAlignementVertical.Center)
                worldPos.Y += stringSize.Y * 0.5f * style.Scale * scale;
            if (alignementVertical == TextAlignementVertical.Down)
                worldPos.Y += stringSize.Y * 1.0f * style.Scale * scale;

            Point screenPos = m_currentCamera.WorldToScreen(worldPos);

            Vector2 halfSize = stringSize * style.Scale * 0.5f;
            SpriteBatch.DrawString(font, text, screenPos, style.Color, 0, Vector2.Zero, scale * style.Scale * m_cameraZoom * m_currentCamera.Zoom, SpriteEffects.None, 0);
        }

        public void DrawMesh(Mesh mesh, LBE.Graphics.Effects.EffectWrapper effectWrapper)
        {
            Effect effect = effectWrapper.Effect;

            //This is a hack, while effects get sorted out
            if (effect is BasicEffect)
            {
                var be = effect as BasicEffect;
                be.View = m_currentCamera.View;
                be.Projection = m_currentCamera.Projection;
            }
            else
            {
                try
                {
                    //Set the shader parameters
                    effect.Parameters["View"].SetValue(m_currentCamera.View);
                    effect.Parameters["Projection"].SetValue(m_currentCamera.Projection);
                }
                catch { }
            }

            m_device.SetVertexBuffer(mesh.VertexBuffer);
            m_device.Indices = mesh.IndexBuffer;

            //Draw the mesh
            effect.CurrentTechnique.Passes[0].Apply();

            if(mesh.PrimitiveCount > 0)
                m_device.DrawIndexedPrimitives(mesh.PrimitiveType, 0, 0, mesh.VerticesCount, 0, mesh.PrimitiveCount);
        }

        public void Draw(Sprite sprite, Vector2 position, float orientation = 0, SpriteBatch spriteBatch = null)
        {
            if (spriteBatch == null)
                spriteBatch = m_spriteBatch;

            var texture = sprite.Texture;
            var pos = m_currentCamera.WorldToScreen(position);
            var sourceRectangle = new Rectangle(sprite.Source.X, sprite.Source.Y, sprite.Size.X, sprite.Size.Y);
            var color = new Color(sprite.Color.ToVector3().X, sprite.Color.ToVector3().Y, sprite.Color.ToVector3().Z, sprite.Alpha);
            float rotation = -orientation;
            var origin = new Point((int)(0.5f * sprite.Size.X), (int)(0.5f * sprite.Size.Y));
            var scale = sprite.Scale * m_cameraZoom * m_currentCamera.Zoom;
            var effects = SpriteEffects.None;

            if (sprite.Mirror == true)
                effects = SpriteEffects.FlipHorizontally;

            //m_device.SamplerStates[0].Filter = TextureFilter.Linear;
            m_spriteBatch.Draw(texture, pos, sourceRectangle, color, rotation, origin, scale, effects, 0.0f);
        }

        public void Draw(Texture2D texture, int width, int height)
        {
            Draw(texture, width, height, m_currentCamera);
        }

        public void Draw(Texture2D texture, int width, int height, ICamera camera)
        {
            Draw(texture, width, height, Vector2.Zero, camera);
        }

        public void Draw(Texture2D texture, int width, int height, Vector2 pos, ICamera camera)
        {
            Vector2 screenPos = camera.WorldToScreen(pos);
            var sourceRec = new Rectangle(0, 0, texture.Width, texture.Height);
            var destRec = new Rectangle((int)screenPos.X - width / 2, (int)screenPos.Y - height / 2, width, height);

            m_spriteBatch.Draw(
                texture,
                destRec, sourceRec,
                Color.White);
        }

        public void SetFullscreen()
        {
            var modes  = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes;
            var bestMode = modes.Last();
            var width = bestMode.Width;
            var height = bestMode.Height;

            m_device.PresentationParameters.BackBufferWidth = width;
            m_device.PresentationParameters.BackBufferHeight = height;

            Engine.Application.GraphicsDeviceManager.IsFullScreen = true;

            Engine.Application.GraphicsDeviceManager.PreferredBackBufferWidth = width;
            Engine.Application.GraphicsDeviceManager.PreferredBackBufferHeight = height;

            Engine.Renderer.Device.Viewport = new Viewport(0, 0, width, height);
            Engine.Application.GraphicsDeviceManager.ApplyChanges();
        }

        public void SetWindowed(int width, int height)
        {
            m_device.PresentationParameters.BackBufferWidth = width;
            m_device.PresentationParameters.BackBufferHeight = height;

            Engine.Application.GraphicsDeviceManager.IsFullScreen = false;

            Engine.Application.GraphicsDeviceManager.PreferredBackBufferWidth = width;
            Engine.Application.GraphicsDeviceManager.PreferredBackBufferHeight = height;

            Engine.Renderer.Device.Viewport = new Viewport(0, 0, width, height);
            Engine.Application.GraphicsDeviceManager.ApplyChanges();
        }

        public void SetResolution(int width, int height)
        {
            m_device.PresentationParameters.BackBufferWidth = width;
            m_device.PresentationParameters.BackBufferHeight = height;

            Engine.Application.GraphicsDeviceManager.PreferredBackBufferWidth = width;
            Engine.Application.GraphicsDeviceManager.PreferredBackBufferHeight = height;

            Engine.Application.GraphicsDeviceManager.ApplyChanges();
        }

        public Vector2 ScreenToWorld(Vector2 screen)
        {
            return m_cameras[0].ScreenToWorld(screen);
        }
    }
}