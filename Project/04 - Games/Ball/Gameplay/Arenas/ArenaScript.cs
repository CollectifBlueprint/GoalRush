using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;

namespace Ball.Gameplay.Arenas
{
    public abstract class ArenaScript
    {
        Arena m_arena;
        public Arena Arena
        {
            get { return m_arena; }
            set { m_arena = value; }
        }

        public virtual void OnInitGeometry() { }
        public virtual void OnUpdate() { }
        public virtual void OnEnd() { }

        public ArenaScript Clone()
        {
            var type = this.GetType();
            return (ArenaScript)Activator.CreateInstance(type);
        }
    }
}
