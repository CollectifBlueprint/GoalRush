using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;

namespace LBE.Gameplay
{
    public class World : BaseEngineComponent
    {
        List<GameObject> m_gameObjects;
        public List<GameObject> GameObjects
        {
            get { return m_gameObjects; }
        }

        List<GameObject> m_goToAdd;
        List<GameObject> m_goToRemove;

        EventManager m_eventManager;
        public EventManager EventManager
        {
            get { return m_eventManager; }
        }


        public World()
        {
            m_gameObjects = new List<GameObject>();

            m_goToAdd = new List<GameObject>();
            m_goToRemove = new List<GameObject>();

            m_eventManager = new EventManager();
        }

        public override void StartFrame()
        {
            var temp = m_goToAdd.ToArray();
            m_goToAdd.Clear();
            foreach (var go in temp)
            {
                m_gameObjects.Add(go);
                go._Start();
            }

            foreach (var go in m_gameObjects)
            {
                go._Update();
            }

            temp = m_goToRemove.ToArray();
            m_goToRemove.Clear();
            foreach (var go in temp)
            {
                m_gameObjects.Remove(go);
                go._End();
            }
        }

        public void UpdatePrePhysics()
        {
            foreach (var go in m_gameObjects)
            {
                go._UpdatePrePhysics();
            }
        }

        public void UpdatePostPhysics()
        {
            foreach (var go in m_gameObjects)
            {
                go._UpdatePostPhysics();
            }
        }

        public void Add(GameObject go)
        {
            m_goToAdd.Add(go);
        }

        public void Remove(GameObject go)
        {
            m_goToRemove.Add(go);
        }
    }
}
