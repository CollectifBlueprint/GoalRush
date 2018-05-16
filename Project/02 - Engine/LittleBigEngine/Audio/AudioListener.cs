using LBE.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LBE.Audio
{
    public class AudioListener : GameObjectComponent
    {

        public AudioListener()
        {
        }

        public override void Start()
        {
            Engine.Log.Assert(Engine.Audio.AudioListener == null, "An AudioListener is already set");
            
            Engine.Audio.AudioListener = this;
        }

        public override void End()
        {
            if (Engine.Audio.AudioListener == this)
            {
                Engine.Audio.AudioListener = null;
            }
        }
    }
}
