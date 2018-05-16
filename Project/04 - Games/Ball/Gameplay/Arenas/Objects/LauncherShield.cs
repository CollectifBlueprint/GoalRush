using Ball.Physics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using LBE;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using LBE.Physics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ball.Gameplay.Arenas.Objects
{
    public class LauncherShield : GameObjectComponent
    {

        float m_shieldRadius = 90;

        Body m_body;
        public Body Body
        {
            get { return m_body; }
        }

        LauncherComponent m_launcher;
        public LauncherComponent Launchers
        {
            get { return m_launcher; }
        }

        public LauncherShield(LauncherComponent launcher)
        {
            m_launcher = launcher;
        }

        public override void Start()
        {
            m_body = new Body(Engine.PhysicsManager.World, this);
            m_body.BodyType = BodyType.Static;

            var fixture = FixtureFactory.AttachCircle(
              ConvertUnits.ToSimUnits(m_shieldRadius),
              0,
              m_body);

            m_body.CollidesWith = (Category)CollisionType.Player;

            m_body.Enabled = false;
        }

        public override void End()
        {
            m_body.Dispose();
            base.End();
        }

        public override void Enable(bool value)
        {
            base.Enable(value);
            if (value == false)
                m_body.Enabled = false;
        }
    }
}
