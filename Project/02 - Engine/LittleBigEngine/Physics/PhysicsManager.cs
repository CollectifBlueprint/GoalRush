using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using LBE.Core;
using FarseerPhysics.DebugViews;
using LBE.Graphics;
using LBE.Graphics.Sprites;

namespace LBE.Physics
{
    public class PhysicsManager : BaseEngineComponent, IRenderable
    {
        World m_world;
        public World World
        {
            get { return m_world; }
        }

        bool m_showDebug = false;
        public bool ShowDebug
        {
            get { return m_showDebug; }
            set { m_showDebug = value; }
        }

        DebugViewXNA m_debugView;

        public const float Scale = 0.12f;

        public override void Startup()
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(1.0f / Scale);

            m_world = new World(new Vector2(0, 0));
            m_world.FixtureAdded = OnFixtureAdded;

            m_debugView = new DebugViewXNA(m_world);

            Engine.AssetManager.RegisterAssetType<CollisionDefinition>(new CollisionDefinitionLoader());
        }

        public override void StartFrame()
        {
            if (!m_debugView.Loaded && Engine.Renderer != null)
            {
                m_debugView.LoadContent(Engine.Renderer.Device, Engine.AssetManager.ContentManager);
                m_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.CenterOfMass);
                m_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.PolygonPoints);
                m_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.ContactPoints);
                m_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.Shape);

                Engine.Renderer.RenderLayers["DebugPhysicsOverlay"].Renderables.Add(this);
            }

            float timeCoef = Engine.Debug.EditSingle("TimeCoef", 0.85f);
            
            Engine.World.UpdatePrePhysics();
            m_world.Step(Engine.GameTime.ElapsedMS * 0.001f * timeCoef);
          
            //fix rigidBody
            foreach (var go in Engine.World.GameObjects)
            {
                if (go.RigidBodyCmp != null)
                  go.RigidBodyCmp.UpdateGameObjectPosition();
            }

            Engine.World.UpdatePostPhysics();
        }

        public void Draw()
        {



            Matrix proj = Engine.Renderer.RenderLayers["DebugPhysicsOverlay"].Camera.Projection;
            Matrix view = Engine.Renderer.RenderLayers["DebugPhysicsOverlay"].Camera.View;
            Matrix scale = Matrix.CreateScale(1.0f / PhysicsManager.Scale);
            view = scale * view;

            if(m_showDebug)
                m_debugView.RenderDebugData(ref proj, ref view);
        }

        void OnFixtureAdded(Fixture target)
        {
            var rbCmp = target.Body.UserData as RigidBodyComponent;
            if (rbCmp !=  null)
            {
                rbCmp.FixtureAdded(target);
            }
        }
    }
}
