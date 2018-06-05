using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using LBE;
using Microsoft.Xna.Framework;
using LBE.Assets;
using LBE.Core;
using LBE.Graphics.Sprites;
using LBE.Audio;
using Ball.Gameplay.BallEffects;
using LBE.Graphics;
using Microsoft.Xna.Framework.Graphics;
using LBE.Graphics.Effects;
using LBE.Physics;
using FarseerPhysics.Dynamics;
using Ball.Physics;

namespace Ball.Gameplay.Arenas.Objects
{
    public class CentralBallLauncherComponent : LauncherComponent, IRenderable
    {

        AudioComponent m_audioCmpBallLaunch;
        public AudioComponent AudioCmpBallLaunch
        {
            get { return m_audioCmpBallLaunch; }
        }

        Player m_playerToAim;
        public Player PlayerToAim
        {
            get { return m_playerToAim; }
        }

        Team m_team;
        public Team Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        // Charge
        DynamicMesh<VertexPositionColor> m_lineMesh;
        DynamicMesh<VertexPositionColor> m_triangleMesh;

        EffectWrapper m_effect;

        float m_angle;
        public float Angle
        {
            get { return m_angle; }
        }

        Timer m_chargeTimerMS;
        Timer m_maxChargeTimerMS;

        bool m_chargedMax;
        public CentralBallLauncherComponent()
        {
        }

        List<Player> m_launcherTeam;

        SpriteComponent m_spriteCmp1;
        public SpriteComponent SpriteCmp1
        {
            get { return m_spriteCmp1; }
        }

        SpriteComponent m_spriteCmp2;
        public SpriteComponent SpriteCmp2
        {
            get { return m_spriteCmp2; }
        }

        LauncherShield m_shield;

        float m_scale = 3.2f;

        public override void Start()
        {
            base.Start();

            m_audioCmpBallLaunch = new AudioComponent("Audio/Sounds.lua::BallShot");
            Owner.Attach(m_audioCmpBallLaunch);

            Sprite sprite1 = Sprite.Create("Graphics/LauncherCentralShield.lua::Sprite");
            sprite1.SetAnimation("Normal");
            sprite1.Alpha = 0;
            m_spriteCmp1 = new SpriteComponent(sprite1, "ArenaOverlay5");
            m_spriteCmp1.Visible = false;
            sprite1.Playing = true;
            m_spriteCmp1.Sprite.Scale = new Vector2(m_scale, m_scale);

            Owner.Attach(m_spriteCmp1);
            
            Sprite sprite2 = Sprite.Create("Graphics/LauncherCentralShield.lua::Sprite2");
            sprite2.SetAnimation("Normal");
            sprite2.Alpha = 0;
            m_spriteCmp2 = new SpriteComponent(sprite2, "ArenaOverlay5");
            m_spriteCmp2.Visible = false;
            sprite2.Playing = true;

            Owner.Attach(m_spriteCmp2);
            
            var basicEffect = new BasicEffect(Engine.Renderer.Device);

            basicEffect.LightingEnabled = false;
            basicEffect.VertexColorEnabled = true;
            basicEffect.FogEnabled = false;
            m_effect = new EffectWrapper(basicEffect);

            int capacity = 512 * 512 * 4;
            m_lineMesh = new DynamicMesh<VertexPositionColor>(Engine.Renderer.Device, VertexPositionColor.VertexDeclaration, PrimitiveType.LineList, capacity);
            m_triangleMesh = new DynamicMesh<VertexPositionColor>(Engine.Renderer.Device, VertexPositionColor.VertexDeclaration, PrimitiveType.TriangleList, capacity);

            Engine.Renderer.RenderLayers["ArenaOverlay7"].Renderables.Add(this);

            m_chargeTimerMS = new Timer(Engine.GameTime.Source, 2000);
            m_chargeTimerMS.OnTime += m_chargeTimerMS_OnTime;

            m_maxChargeTimerMS = new Timer(Engine.GameTime.Source, 300);
            m_maxChargeTimerMS.OnTime += m_maxChargeTimerMS_OnTime;

            m_chargedMax = false;

            m_launcherTeam = new List<Player>();

            m_shield = new LauncherShield(this);
            Owner.Attach(m_shield);
        }

        void m_chargeTimerMS_OnTime(Timer source)
        {
            m_chargedMax = true;
            m_maxChargeTimerMS.Start();
        }

        void m_maxChargeTimerMS_OnTime(Timer source)
        {
            StopCharge();
            
            //disalbe effect
        }
        public override void Update()
        {
            if (!(m_chargeTimerMS.Active || m_maxChargeTimerMS.Active))
                return;
            
            base.Update();


            // Update angle
            float chargeCompletion = LBE.MathHelper.LinearStep(0, m_chargeTimerMS.TargetTime, m_chargeTimerMS.TimeMS);
            if (m_chargedMax)
                chargeCompletion = 1;

            m_angle = (float)Math.PI * chargeCompletion;

            // Create gfx feedback
            m_lineMesh.Reset();
            m_triangleMesh.Reset();

            int nbrSide = 32;
            float radius = 30 * m_scale;

            for (int i = 0; i < nbrSide; i++)
            {
                float angleStep = m_angle / nbrSide;

                Vector2 circlePoint1 = new Vector2((float)Math.Cos(angleStep * i), (float)Math.Sin(angleStep * i));
                circlePoint1 = circlePoint1.Rotate(GetWorldTransform().Orientation - 0.5f * m_angle);
                Vector2 p1 = Owner.Position + radius * circlePoint1;

                Vector2 circlePoint2 = new Vector2((float)Math.Cos(angleStep * (i + 1)), (float)Math.Sin(angleStep * (i + 1)));
                circlePoint2 = circlePoint2.Rotate(GetWorldTransform().Orientation - 0.5f * m_angle);
                Vector2 p2 = Owner.Position + radius * circlePoint2;

                // Circle
                var vertex = new VertexPositionColor();
                vertex.Color = m_team.ColorScheme.Color1;

                vertex.Position = new Vector3(p1, 0);
                int idx1 = m_lineMesh.Vertex(vertex);

                vertex.Position = new Vector3(p2, 0);
                int idx2 = m_lineMesh.Vertex(vertex);

                m_lineMesh.Index(idx1);
                m_lineMesh.Index(idx2);

                // Disk
                var vertexT = new VertexPositionColor();
                vertexT.Color = new Color(m_team.ColorScheme.Color1, 0.2f);

                vertexT.Position = new Vector3(Owner.Position, 0);
                int idxT1 = m_triangleMesh.Vertex(vertexT);

                vertexT.Position = new Vector3(p1, 0);
                int idxT2 = m_triangleMesh.Vertex(vertexT);

                vertexT.Position = new Vector3(p2, 0);
                int idxT3 = m_triangleMesh.Vertex(vertexT);

                m_triangleMesh.Index(idxT1);
                m_triangleMesh.Index(idxT2);
                m_triangleMesh.Index(idxT3);
            }

            //
            UpdateScan();

            if (m_playerToAim != null)
            {
                PassBallToTeam();
                StopCharge();
            }
        }

        public void StopCharge()
        {
            m_lineMesh.Reset();
            m_triangleMesh.Reset();
            m_chargedMax = false;
            m_chargeTimerMS.Stop();
            m_maxChargeTimerMS.Stop();
            SpriteCmp1.Sprite.Alpha = 0;
            m_spriteCmp1.Visible = false;
            m_spriteCmp2.Visible = false;
            m_spriteCmp1.Sprite.Alpha = 0;
            m_spriteCmp2.Sprite.Alpha = 0;
            m_shield.Body.Enabled = false;
        }

        public override void End()
        {
            base.End();
            StopCharge();

            m_chargeTimerMS.Stop();
            m_maxChargeTimerMS.Stop();

            m_chargeTimerMS.OnTime -= m_chargeTimerMS_OnTime;
            m_maxChargeTimerMS.OnTime -= m_maxChargeTimerMS_OnTime;

            m_triangleMesh.Dispose();
            m_lineMesh.Dispose();
        }

        public override GameObjectComponent Clone()
        {
            var launcher = new CentralBallLauncherComponent();
            launcher.Transform = m_transform;
            return launcher;
        }

        public override void Select()
        {
            base.Select();
            SpriteComponent spriteCmpParent = Owner.FindComponent<SpriteComponent>("LauncherSprite");
            if (spriteCmpParent != null)
            {
                spriteCmpParent.Sprite.Color = m_team.ColorScheme.Color1;
                spriteCmpParent.Sprite.Alpha = 0.5f;
            }

            m_spriteCmp1.Visible = true;
            m_spriteCmp1.Sprite.Alpha = 1f;
            m_shield.Body.Enabled = true;
        }

        public override void Unselect()
        {
            base.Unselect();
        }

        public override void Launch()
        {
            if (!Enabled)
                return;

            m_spriteCmp2.Visible = true;
            m_spriteCmp2.Sprite.Color = m_team.ColorScheme.Color1;
            m_spriteCmp2.Sprite.Alpha = 0.5f;

            Ball ball = Game.GameManager.Ball;
            Transform world = GetWorldTransform();
            ball.BodyCmp.SetPosition(world.Position + m_ballSpawnOffset.Rotate(world.Orientation));

            Game.GameManager.Ball.BodyCmp.Body.Enabled = true;
            Game.GameManager.Ball.BallSprite.Alpha = 255;

            m_chargeTimerMS.Start();
        }

        public override void Scan()
        {
            m_launcherTeam.Clear();

            foreach (var player in Game.GameManager.Players)
            {
                if (player.Team == m_team)
                    m_launcherTeam.Add(player);
            }

            UpdateScan();
        }


        void UpdateScan()
        {
            Transform parent = new Transform(Owner.Position, Owner.Orientation);
            Transform world = parent.Compose(m_transform);

            m_playerToAim = null;
            float shortestPlayerDist = float.MaxValue;
            foreach (var player in m_launcherTeam)
            {
                if (player.Team == m_team)
                {
                    Vector2 launcherToBallDir = Vector2.UnitX.Rotate(world.Orientation);

                    Vector2 launcherToPlayerDir = player.Position - world.Position;
                    launcherToPlayerDir.Normalize();

                    float launcherDirToPlayerAngle = (float)Math.Acos(Vector2.Dot(launcherToBallDir, launcherToPlayerDir));

                    if (launcherDirToPlayerAngle < (0.5f * m_angle))
                    {
                        // player is in the view zone
                        float dist = Vector2.Distance(player.Position, world.Position);

                        // check obstruction
                        bool lineClear = true;
                        Engine.PhysicsManager.World.RayCast(
                            (Fixture fixture, Vector2 point, Vector2 normal, float fraction) =>
                            {
                                if (fixture.CollidesWith.HasFlag((Category)CollisionType.Ball)
                                    && !fixture.CollisionCategories.HasFlag((Category)CollisionType.Ball)
                                    && !fixture.IsSensor
                                    && !((fixture.CollisionCategories.HasFlag((Category)CollisionType.Player) && fixture.Body.BodyId == player.Owner.RigidBodyCmp.Body.BodyId)) )
                                {
                                     lineClear = false;
                                    return 0;
                                }

                                return 1;
                            },
                            ConvertUnits.ToSimUnits(Owner.Position),
                            ConvertUnits.ToSimUnits(player.Position));
                    
                        // TODO test si player ds les but
                        if (lineClear && dist < shortestPlayerDist)
                        {
                            shortestPlayerDist = dist;
                            m_playerToAim = player;
                        }
                    }
                }
            }
        }

        void PassBallToTeam()
        {
            var ball = Game.GameManager.Ball;

            ball.AddEffect(new PassAssistEffect(m_playerToAim, null, m_team.ColorScheme.Color1));

            Vector2 impulseDirection = m_playerToAim.Position - GetWorldTransform().Position;
            impulseDirection.Normalize();

            float launchImpulse = 280;
            ball.BodyCmp.Body.ApplyLinearImpulse(impulseDirection * launchImpulse);

            m_audioCmpBallLaunch.Play();

            Engine.World.EventManager.ThrowEvent((int)EventId.LauncherShot, this);
        }


        public void Draw()
        {
            m_lineMesh.PrepareDraw();
            m_triangleMesh.PrepareDraw();

            Engine.Renderer.Device.BlendState = BlendState.NonPremultiplied;
            Engine.Renderer.Device.RasterizerState = RasterizerState.CullNone;
            Engine.Renderer.Device.DepthStencilState = DepthStencilState.None;

            Engine.Renderer.DrawMesh(m_lineMesh, m_effect);
            Engine.Renderer.DrawMesh(m_triangleMesh, m_effect);
        }

        public override void Enable(bool value)
        {
            if (value == false)
            {
                StopCharge();
                SpriteComponent spriteCmpParent = Owner.FindComponent<SpriteComponent>("LauncherSprite");
                if (spriteCmpParent != null)
                {
                    spriteCmpParent.Sprite.Alpha = 0;
                    spriteCmpParent.Visible = false;
                }
            }
        }
    }
}
