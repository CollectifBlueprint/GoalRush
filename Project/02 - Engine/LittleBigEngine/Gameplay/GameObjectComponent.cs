using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using Microsoft.Xna.Framework;

namespace LBE.Gameplay
{
    public class GameObjectComponent
    {
        GameObject m_owner;
        public GameObject Owner
        {
            get { return m_owner; }
        }

        String m_name;
        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public Vector2 Position
        {
            get { return Owner.Position; }
        }

        bool m_started;
        public bool Started
        {
            get { return m_started; }
        }

        bool m_enabled;
        public bool Enabled
        {
            set 
            { 
                m_enabled = value;
                Enable(value);
            }
            get { return m_enabled; }
        }

        public GameObjectComponent()
        {
            m_started = false;
            m_enabled = true;

            m_name = this.GetType().Name;
        }

        internal void AttachTo(GameObject owner)
        {
            Engine.Log.Assert(m_owner == null, "The GameObjectComponent already has a owner");
            m_owner = owner;
        }

        internal void DoStart()
        {
            m_started = true;
            Start();
        }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void UpdatePrePhysics() { }
        public virtual void UpdatePostPhysics() { }
        public virtual void End() { }
        public virtual void Enable(bool value) { }
        public virtual void OnPositionChange(Vector2 position) { }

        public virtual GameObjectComponent Clone()
        {
            Engine.Log.Error(
                String.Format("Could not clone component {0} of type {1} because it does not override the Clone() method", m_name, this.GetType().Name));
            return new GameObjectComponent();
        }
    }
}
