using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace LBE.Physics
{
    public class TriggerComponent : GameObjectComponent
    {
        public event Action<GameObject> OnTrigger;
        public event Action<GameObject> OnLeave;

        Body m_body;
        public Body Body
        {
            get { return m_body; }
        }


        private List<RigidBodyComponent> m_activeObjects;
        public List<RigidBodyComponent> ActiveObjects
        {
            get { return m_activeObjects; }
            set { m_activeObjects = value; }
        }

        public bool Active
        {
            get { return Enabled && (m_activeObjects.Count != 0); }
        }

        public TriggerComponent()
        {
            m_activeObjects = new List<RigidBodyComponent>();

            m_body = new Body(Engine.PhysicsManager.World, this);
            m_body.BodyType = BodyType.Static;
        }

        public override void Start()
        {
            m_body.SetTransform(ConvertUnits.ToSimUnits(Owner.Position), Owner.Orientation);

            m_body.OnCollision += new OnCollisionEventHandler(m_body_OnCollision);
            m_body.OnSeparation += new OnSeparationEventHandler(m_body_OnSeparation);
        }

        public override void Update()
        {
            //m_body.SetTransform(ConvertUnits.ToSimUnits(Owner.Position), Owner.Orientation);
        }

        public void SetPosition(Vector2 position)
        {
            Owner.Position = position;
            m_body.SetTransform(ConvertUnits.ToSimUnits(position), Owner.Orientation);
        }

        public void SetOrientation(float orientation)
        {
            Owner.Orientation = orientation;
            m_body.Rotation = orientation;
        }

        public override void UpdatePrePhysics()
        {
            if (ConvertUnits.ToSimUnits(Owner.Position) != m_body.Position || Owner.Orientation != m_body.Rotation)
                m_body.SetTransformIgnoreContacts(ConvertUnits.ToSimUnits(Owner.Position), Owner.Orientation);
        }

        public override void UpdatePostPhysics()
        {
            foreach (var rb in m_activeObjects.ToArray())
            {
                if (!rb.Body.Enabled || rb.Body.IsDisposed)
                    m_activeObjects.Remove(rb);
            }
        }

        public override void End()
        {
            m_body.Dispose();
        }

        bool m_body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (!Enabled)
                return false;
            
            if (fixtureB.Body.UserData is RigidBodyComponent)
            {
                var otherRB = fixtureB.Body.UserData as RigidBodyComponent;
                if (m_activeObjects.Contains(otherRB) == false)
                {
                    m_activeObjects.Add(otherRB);
                    if (OnTrigger != null)
                        OnTrigger(otherRB.Owner);
                }
            }
            return true;
        }

        void m_body_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (!Enabled)
                return;
            
            if (fixtureB.Body.UserData is RigidBodyComponent)
            {
                var otherRB = fixtureB.Body.UserData as RigidBodyComponent;
                if (m_activeObjects.Contains(otherRB))
                {
                    m_activeObjects.Remove(otherRB);
                    if (OnLeave != null)
                        OnLeave(otherRB.Owner);
                }
            }
        }


        public override void OnPositionChange(Vector2 position)
        {
          //  m_body.Position = ConvertUnits.ToSimUnits(position);
        }
    }
}
