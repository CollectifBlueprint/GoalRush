using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using FarseerPhysics.Dynamics;

using World = FarseerPhysics.Dynamics.World;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Common;
using FarseerPhysics.Factories;
using LBE.Assets;

namespace LBE.Physics
{
    public delegate bool CollisionEvent(Contact contact, RigidBodyComponent self, RigidBodyComponent other);
    public delegate void SeparationEvent(RigidBodyComponent self, RigidBodyComponent other);

    public class RigidBodyComponentParameters
    {
        public CollisionDefinition Collision;
        public CollisionDefinition[] ExtraCollision;

        public BodyType Type = BodyType.Dynamic;
    }

    public class RigidBodyComponent : GameObjectComponent
    {
        public event CollisionEvent OnCollision;
        public event SeparationEvent OnSperatation;

        Asset<RigidBodyComponentParameters> m_parameters;
        public Asset<RigidBodyComponentParameters> ParametersAsset
        {
            get { return m_parameters; }
            set { SetParameters(value); }
        }

        public RigidBodyComponentParameters Parameters
        {
            get { return m_parameters.Content; }
            set { SetParameters(value); }
        }

        Body m_body;
        public Body Body
        {
            get { return m_body; }
        }

        Dictionary<String, Object> m_userData;
        public Dictionary<String, Object> UserData
        {
            get { return m_userData; }
        }

        public RigidBodyComponent()
        {
            m_userData = new Dictionary<string, object>();

            m_body = new Body(Engine.PhysicsManager.World, this);
            m_body.BodyType = BodyType.Dynamic;
        }


        public RigidBodyComponent(Asset<RigidBodyComponentParameters> parameters)
            : this()
        {
            m_parameters = parameters;
            m_parameters.OnAssetChanged += new OnChange(Reset);

            //Reset();
        }

        private void SetParameters(RigidBodyComponentParameters value)
        {
            m_parameters = new Asset<RigidBodyComponentParameters>(value);
        }

        private void SetParameters(Asset<RigidBodyComponentParameters> value)
        {
            m_parameters = value;
            m_parameters.OnAssetChanged += new OnChange(Reset);
        }

        void Reset()
        {
            m_body.BodyType = BodyType.Static;

            if (m_body.FixtureList != null)
            {
                foreach (var fix in m_body.FixtureList.ToArray())
                {
                    m_body.FixtureList.Remove(fix);
                }
            }

            if (Parameters.Collision != null)
            {
                foreach(var colDef in Parameters.Collision.Entries)
                    AddCollisionPolygon(colDef);
            }

            if (Parameters.ExtraCollision != null)
            {
                foreach (var extraCols in Parameters.ExtraCollision)
                {
                    foreach (var colDef in extraCols.Entries)
						AddCollisionPolygon(colDef);
                }
            }
        }

        public override void Start()
        {
            Engine.Log.Assert(Owner.RigidBodyCmp == null);
            Owner.RigidBodyCmp = this;

            if (m_parameters != null)
                Reset();

            m_body.SetTransform(ConvertUnits.ToSimUnits(Owner.Position), Owner.Orientation);
        }

        public void FixtureAdded(Fixture target)
        {
            target.OnCollision += new OnCollisionEventHandler(m_body_OnCollision);
            target.OnSeparation += new OnSeparationEventHandler(m_body_OnSeparation);
        }

        bool m_body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (OnCollision != null && fixtureB.Body.UserData is RigidBodyComponent)
                return OnCollision(contact, fixtureA.Body.UserData as RigidBodyComponent, fixtureB.Body.UserData as RigidBodyComponent);
            return true;
        }

        void m_body_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (OnSperatation != null && fixtureB.Body.UserData is RigidBodyComponent)
                OnSperatation(fixtureA.Body.UserData as RigidBodyComponent, fixtureB.Body.UserData as RigidBodyComponent);
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

        public override void  UpdatePrePhysics()
        {
            if (ConvertUnits.ToSimUnits(Owner.Position) != m_body.Position || Owner.Orientation != m_body.Rotation)
            {
                m_body.Awake = true;
                m_body.SetTransform(ConvertUnits.ToSimUnits(Owner.Position), Owner.Orientation);
            }
        }

        public void UpdateGameObjectPosition()
        {
            Owner.Position = ConvertUnits.ToDisplayUnits(m_body.Position);
            Owner.Orientation = m_body.Rotation;
        }

        public override void UpdatePostPhysics()
        {
            
        }

        public override void End()
        {
            Owner.RigidBodyCmp = null;
            m_body.Dispose();
        }

        public override GameObjectComponent Clone()
        {
            return new RigidBodyComponent(m_parameters);
        }

        public IEnumerable<Fixture> AddCollision(CollisionDefinition collisionDef)
        {
            foreach (var entry in collisionDef.Entries)
            {
				yield return AddCollisionLoopShape(entry);
            }
        }

        public Fixture AddCollisionLoopShape(CollisionDefinitionEntry entry)
        {
            Vertices verts = new Vertices();
            foreach (var point in entry.Vertices)
            {
                verts.Add(ConvertUnits.ToSimUnits(point));
            }
			return FixtureFactory.AttachLoopShape(verts, m_body);
		}

		public void AddCollisionPolygon(CollisionDefinitionEntry entry)
		{
			var path = new Path ();

			foreach (var point in entry.Vertices)
			{
				path.Add (ConvertUnits.ToSimUnits (point));
			}
			path.Closed = true;

			PathManager.ConvertPathToPolygon (path, m_body, 0, entry.Vertices.Length);
		}
    }
}
