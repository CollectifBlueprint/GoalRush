using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LBE;
using LBE.Gameplay;
using LBE.Physics;
using LBE.Assets;

using Microsoft.Xna.Framework;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using LBE.Graphics.Sprites;

namespace Ball.Gameplay
{
    public class ProjectileParameters
    {
        public float Radius;
        public float Speed;
        public float Power;
        public float StunTimeMS;
        public float StunSpeedMod;
        public ProjectilePhysicParameters Physic;
    }

    public class ProjectilePhysicParameters
    {
        public float LinearDamping;
        public float Mass;
    }

    class Projectile : GameObjectComponent
    {

        RigidBodyComponent m_bodyCmp;

        Asset<ProjectileParameters> m_params;

        Player m_player;
        public Player Player
        {
            get { return m_player; }
            set { m_player = value; }
        }


        public ProjectileParameters Parameters
        {
            get { return m_params.Content; }
        }

        
        public Projectile()
        {
            m_params = Engine.AssetManager.GetAsset<ProjectileParameters>("Game/Projectile.lua::Projectile");

        }


        //
        public override void Start()
        {
            m_bodyCmp = new RigidBodyComponent();
            CircleShape bodyShape = new CircleShape(ConvertUnits.ToSimUnits(m_params.Content.Radius), 1.0f);
            Fixture fix = new Fixture(m_bodyCmp.Body, bodyShape);

            Owner.Attach(m_bodyCmp);

            Sprite projSprite = Sprite.Create("Graphics/ballSprite.lua::Sprite");
            projSprite.Scale = 0.3f * Vector2.One;
            projSprite.Color = Color.Red;
            Owner.Attach(new SpriteComponent(projSprite));

            m_bodyCmp.Body.LinearDamping = m_params.Content.Physic.LinearDamping;
            m_bodyCmp.Body.Mass = m_params.Content.Physic.Mass;

            m_bodyCmp.UserData.Add("Tag", "Projectile");

            m_bodyCmp.OnCollision += new CollisionEvent(OnCollision);
        }


        public override void Update()
        {

        }


        bool OnCollision(FarseerPhysics.Dynamics.Contacts.Contact contact, RigidBodyComponent self, RigidBodyComponent other)
        {
            object otherTag = "";
            other.UserData.TryGetValue("Tag", out otherTag);
            if ((string)otherTag == "Player")
            {
                //Projectile projectile = (Projectile)self.Owner.FindComponent<Projectile>();
                Player player = (Player)other.Owner.FindComponent<Player>();
                if (this.Player.Owner == player.Owner)
                {
                    return true;
                }
                else
                {
                    Owner.Kill();
                    return false;
                }
              
            }
            else
            {
                Owner.Kill();
            }
            return false;
            ;
        }

    }
}
