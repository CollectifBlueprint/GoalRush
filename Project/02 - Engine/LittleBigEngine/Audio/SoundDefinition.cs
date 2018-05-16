using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace LBE.Audio
{
    public class SoundDefinition
    {
        public SoundEffectWrapper[] SoundEffects;
        public float Volume;
        public float VolumeMod;
        public float Pitch;
        public float PitchMod;
        public bool Positional;
    }

    public class SoundEffectWrapper
    {
        public SoundEffect SoundEffect;
    }
}
