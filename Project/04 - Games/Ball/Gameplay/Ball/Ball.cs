using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using LBE.Physics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using LBE.Assets;
using LBE;
using LBE.Graphics.Sprites;
using Ball.Physics;
using Microsoft.Xna.Framework;
using Ball.Gameplay.BallEffects;
using LBE.Audio;
using LBE.Graphics.Particles;
using LBE.Debug;

namespace Ball.Gameplay
{
    public class BallParameters
    {
        public float Radius;
        public BallPhysicParameters Physic;
    }

    public class BallPhysicParameters
    {
        public float LinearDamping;
        public float AngularDamping;
        public float Restitution;
        public float Mass;
    }

    public class BallProperties
    {
        public PropertyStack PassThroughPlayer = new PropertyStack();
        public PropertyStack Untakable = new PropertyStack();
    }    

    public class Ball : GameObjectComponent
    {
        Asset<BallParameters> m_params;
        public BallParameters Parameters
        {
            get { return m_params.Content; }
        }

        Player m_player;
        public Player Player
        {
            get { return m_player; }
            set { m_player = value; }
        }

        Player m_lastPlayer;
        public Player LastPlayer
        {
            get { return m_lastPlayer; }
            set { m_lastPlayer = value; }
        }

        RigidBodyComponent m_bodyCmp;
        public RigidBodyComponent BodyCmp
        {
            get { return m_bodyCmp; }
        }

        List<BallEffect> m_effects;
        public List<BallEffect> Effects
        {
            get { return m_effects; }
        }

        BallTrailFx m_ballTrail;
        public BallTrailFx BallTrail
        {
            get { return m_ballTrail; }
        }

        BallProperties m_properties;
        public BallProperties Properties
        {
            get { return m_properties; }
        }

        Sprite m_ballSprite;
        public Sprite BallSprite
        {
            get { return m_ballSprite; }
        }

        Sprite m_ballBashSprite;
        public Sprite BallBashSprite
        {
            get { return m_ballBashSprite; }
        }

        AudioComponent m_audioCmpBallBounce;
        AudioComponent m_audioCmpBallPlayerSnap;

        public override void Start()
        {
            m_params = Engine.AssetManager.GetAsset<BallParameters>("Game/Ball.lua::Ball");

            m_properties = new BallProperties();

            m_effects = new List<BallEffect>();

            m_bodyCmp = new RigidBodyComponent();
            FarseerPhysics.Collision.Shapes.CircleShape bodyShape = new FarseerPhysics.Collision.Shapes.CircleShape(ConvertUnits.ToSimUnits(m_params.Content.Radius), 1.0f);
            Fixture fix = new Fixture(m_bodyCmp.Body, bodyShape);
            m_bodyCmp.Body.AngularDamping = m_params.Content.Physic.AngularDamping;
            m_bodyCmp.Body.Mass = m_params.Content.Physic.Mass;
            m_bodyCmp.Body.LinearDamping = m_params.Content.Physic.LinearDamping;
            m_bodyCmp.Body.Restitution = m_params.Content.Physic.Restitution;
            m_bodyCmp.Body.IsBullet = true;

            m_bodyCmp.Body.CollisionCategories = (Category)CollisionType.Gameplay | (Category)CollisionType.Ball;

            Owner.Attach(m_bodyCmp);

            m_ballSprite = Sprite.Create("Graphics/ballSprite.lua::Sprite");
            Owner.Attach(new SpriteComponent(m_ballSprite, "ArenaOverlay9"));

            m_ballBashSprite = Sprite.Create("Graphics/ballSprite.lua::SpriteBash");
            m_ballBashSprite.Alpha = 0;
            m_ballBashSprite.Playing = true;
            Owner.Attach(new SpriteComponent(m_ballBashSprite, "ArenaOverlay9"));


            m_ballTrail = new BallTrailFx(this);
            Owner.Attach(m_ballTrail);

            m_bodyCmp.Body.FixedRotation = true;

            m_bodyCmp.UserData.Add("Tag", "Ball");

            m_bodyCmp.OnCollision += new CollisionEvent(m_bodyCmp_OnCollision);

            m_player = null;
            m_lastPlayer = null;

            m_audioCmpBallBounce = new AudioComponent("Audio/Sounds.lua::BallBounce");
            Owner.Attach(m_audioCmpBallBounce);
            m_audioCmpBallPlayerSnap = new AudioComponent("Audio/Sounds.lua::BallPlayerSnap");
            Owner.Attach(m_audioCmpBallPlayerSnap);
        }

        public void Reset()
        {
            m_properties = new BallProperties();

            foreach (var e in m_effects) { e.End(); }                
            m_effects.Clear();

            m_player = null;
            m_lastPlayer = null;

            m_bodyCmp.Body.AngularDamping = m_params.Content.Physic.AngularDamping;
            m_bodyCmp.Body.Mass = m_params.Content.Physic.Mass;
            m_bodyCmp.Body.LinearDamping = m_params.Content.Physic.LinearDamping;
            m_bodyCmp.Body.Restitution = m_params.Content.Physic.Restitution;
        }

        bool m_bodyCmp_OnCollision(FarseerPhysics.Dynamics.Contacts.Contact contact, RigidBodyComponent self, RigidBodyComponent other)
        {
            if (m_player != null && other.Owner == m_player.Owner)
                return false;

            var player = other.Owner.FindComponent<Player>();
            if (player != null)
            {
                if (m_properties.PassThroughPlayer && player.Properties.Tackling && !player.Properties.Shield)
                {
                    Vector2 shieldDir = player.BodyCmp.Body.LinearVelocity;
                    Vector2 ballDir = m_bodyCmp.Body.LinearVelocity;
                    if (shieldDir.LengthSquared() > 0.001f && ballDir.LengthSquared() > 0.001f && Vector2.Dot(shieldDir, ballDir) < 0)
                    {
                        player.Properties.Shield.Set();
                        Engine.Log.Write("Shield up!!!");

                        PlayerShieldUpEffect shieldUpEffect = new PlayerShieldUpEffect();
                        shieldUpEffect.SetDuration(3000);
                        player.AddEffect(shieldUpEffect);

                        var emitterDef = Engine.AssetManager.Get<ParticleEmitterDefinition>("Graphics/Particles/Shield.lua::Emitter", false);
                        var shieldParticleCmp = new ParticleComponent();
                        shieldParticleCmp.Emitter = new ParticleEmitter(emitterDef);
                        shieldParticleCmp.Emitter.Definition.Color = player.PlayerColors[2];
                        shieldParticleCmp.DestroyOnEnd = true;
                        var shieldParticleOject = new GameObject("ShieldParticleOject");
                        shieldParticleOject.Position = player.Position;
                        shieldParticleOject.Orientation = player.Owner.Orientation;
                        shieldParticleOject.Attach(shieldParticleCmp);
                        shieldParticleOject.Attach(new DebugObjectComponent(
                            obj => obj.Position = player.Position,
                            2000.0f));
                    }
                }

                foreach (var effect in m_effects.ToArray())
                {
                    effect.OnPlayerTouchBall(player);
                }

                if (m_properties.PassThroughPlayer && !player.Properties.Shield)
                    return false;
            }
            else if(!contact.FixtureB.IsSensor)
            {                
                foreach (var effect in m_effects.ToArray())
                {
                    effect.OnWallCollision(contact.FixtureB);
                }
            }

            if (  contact.FixtureA.CollisionCategories.HasFlag((Category)CollisionType.Wall)
              || contact.FixtureB.CollisionCategories.HasFlag((Category)CollisionType.Wall))
            {
                m_audioCmpBallBounce.Play();
            }
            else if (contact.FixtureA.CollisionCategories.HasFlag((Category)CollisionType.Player)
              || contact.FixtureB.CollisionCategories.HasFlag((Category)CollisionType.Player))
            {
                m_audioCmpBallPlayerSnap.Play();
            }

            return true;
        }

        public void AttachToPlayer(Player player)
        {
            m_lastPlayer = player;
            m_player = player;
            
            foreach (var effect in m_effects.ToArray())
            {
                effect.OnPlayerTakeBall(player);
            }
        }

        public void Dettach()
        {
            m_player = null;
            m_bodyCmp.Body.CollisionCategories = (Category)CollisionType.Gameplay | (Category)CollisionType.Ball;
        }

        public void SetSlowMode(bool value)
        {
            if (value)
            {
                m_bodyCmp.Body.LinearDamping = 6.0f;
                m_bodyCmp.Body.Restitution = 0.05f;
            }
            else
            {
                m_bodyCmp.Body.LinearDamping = m_params.Content.Physic.LinearDamping;
                m_bodyCmp.Body.Restitution = m_params.Content.Physic.Restitution;
            }
        }

        public void Free()
        {
            m_player = null;
            m_lastPlayer = null;
        }

        public override void Update()
        {
            UpdateEffects();
        }

        public void AddEffect(BallEffect effect)
        {
            effect.Init(this);
            effect.Start();
            m_effects.Add(effect);
        }

        void UpdateEffects()
        {
            foreach (var effect in m_effects.ToArray())
            {
                effect.Update();

                if (!effect.Active)
                    RemoveEffect(effect);
            }
        }

        void RemoveEffect(BallEffect effect)
        {
            effect.End();
            m_effects.Remove(effect);
        }
    }
}
