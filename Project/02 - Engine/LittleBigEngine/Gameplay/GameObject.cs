using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LBE.Physics;
using LBE.Assets;

namespace LBE.Gameplay
{
    public class GameObject
    {
        String m_name;
        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        String m_tag = "Gameplay";
        public String Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        bool m_positionIsDirty;
        Vector2 m_position;
        public Vector2 Position
        {
            get { return m_position; }
            set
            {
                m_position = value;
                m_positionIsDirty = true;
            }
        }

        float m_orientation;
        public float Orientation
        {
            get { return m_orientation; }
            set { m_orientation = value; }
        }

        RigidBodyComponent m_rigidBodyCmp;
        public RigidBodyComponent RigidBodyCmp
        {
            get { return m_rigidBodyCmp; }
            internal set { m_rigidBodyCmp = value; }
        }

        bool m_started;
        public bool Started
        {
            get { return m_started; }
        }

        List<GameObjectComponent> m_components;
        public IEnumerable<GameObjectComponent> Components
        {
            get { return m_components; }
        }

        Asset<GameObjectDefinition> m_definition;
        //Dictionary<String, Asset<GameObjectComponentParameters>> m_componentAssets;

        public GameObject(String name = "GameObject")
        {
            m_started = false;
            m_name = name;
            m_components = new List<GameObjectComponent>();

            Engine.World.Add(this);
        }

        public GameObject(Asset<GameObjectDefinition> def)
        {
            m_definition = def;
            m_definition.OnAssetChanged += new OnChange(m_definition_OnAssetChanged);

            m_started = false;
            m_name = def.Name;
            m_components = new List<GameObjectComponent>();

            Engine.World.Add(this);

            foreach (var cmp in m_definition.Content.Components)
            {
                GameObjectComponent clonedCmp = cmp.Clone();
                clonedCmp.Name = cmp.Name;
                Attach(clonedCmp);
            }
        }

        void m_definition_OnAssetChanged()
        {
            //List<String> existingCmpDef = (from cmpAsset in m_componentAssets
            //                               select cmpAsset.Key).ToList();

            //Engine.Log.Write("Reloading GameObject");

            //Engine.Log.Write("Existing components:");
            //foreach (var cmp in m_components)
            //{
            //    Engine.Log.Write("- " + cmp.Name);
            //}

            //Engine.Log.Write("Existing definition:");
            //foreach (var name in existingCmpDef)
            //{
            //    Engine.Log.Write("- " + name);
            //}

            //Engine.Log.Write("New Definition::");
            //foreach (var cmpDef in m_definition.Content.Components)
            //{
            //    Engine.Log.Write("- " + cmpDef.Name);
            //}

            //foreach (var cmpDef in m_definition.Content.Components)
            //{
            //    if (m_componentAssets.ContainsKey(cmpDef.Name))
            //    {
            //        Engine.Log.Write("Updating " + cmpDef.Name);
            //        m_componentAssets[cmpDef.Name].SetContent(cmpDef);
            //    }
            //    else
            //    {
            //        Engine.Log.Write("Creating " + cmpDef.Name);
            //        m_componentAssets[cmpDef.Name] = new Asset<GameObjectComponentParameters>(cmpDef);
            //        GameObjectComponent cmp = cmpDef.Instantiate(m_componentAssets[cmpDef.Name]);
            //        cmp.Name = cmpDef.Name;
            //        Attach(cmp);
            //    }
            //    existingCmpDef.Remove(cmpDef.Name);
            //}

            //foreach (var name in existingCmpDef)
            //{
            //    GameObjectComponent cmp = FindComponent<GameObjectComponent>(name);
            //    if (cmp != null)
            //    {
            //        Engine.Log.Write("Removing " + name);
            //        Remove(cmp);
            //        m_componentAssets.Remove(name);
            //    }
            //    else
            //    {
            //        Engine.Log.Write("Couldn't find " + name);
            //    }
            //}

            //Engine.Log.Write("New components:");
            //foreach (var cmp in m_components)
            //{
            //    Engine.Log.Write("- " + cmp.Name);
            //}
        }

        internal void _Start()
        {
            Start();
            m_started = true;
            foreach (var cmp in m_components.ToArray())
            {
                if (!cmp.Started) cmp.DoStart();
            }
        }

        internal void _Update()
        {
            Update();
            foreach (var cmp in m_components.ToArray())
            {
                if (cmp.Enabled)
                {
                    cmp.Update();
                }
            }
        }

        internal void _UpdatePrePhysics()
        {
            foreach (var cmp in m_components.ToArray())
            {
                if (cmp.Enabled)
                {
                    cmp.UpdatePrePhysics();
                }
            }
        }

        internal void _UpdatePostPhysics()
        {
            foreach (var cmp in m_components.ToArray())
            {
                if (cmp.Enabled)
                {
                    cmp.UpdatePostPhysics();
                }
            }
        }

        internal void _End()
        {
            foreach (var cmp in m_components.ToArray())
            {
                Remove(cmp);
            }
            End();
        }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void End() { }

        public void Kill()
        {
            foreach (var cmp in m_components)
            {
                cmp.Enabled = false;
            }
            Engine.World.Remove(this);
        }

        public void Attach(GameObjectComponent component)
        {
            component.AttachTo(this);
            m_components.Add(component);
            component.DoStart();
        }

        public void Remove(GameObjectComponent component)
        {
            m_components.Remove(component);
            component.End();
        }

        public T FindComponent<T>(String name) where T : class
        {
            foreach (var cmp in m_components)
            {
                if (typeof(T).IsAssignableFrom(cmp.GetType()) && cmp.Name == name)
                {
                    return cmp as T;
                }
            }
            return null;
        }

        public T FindComponent<T>() where T : class
        {
            foreach (var cmp in m_components)
            {
                if (typeof(T).IsAssignableFrom(cmp.GetType()))
                {
                    return cmp as T;
                }
            }
            return null;
        }
    }
}
