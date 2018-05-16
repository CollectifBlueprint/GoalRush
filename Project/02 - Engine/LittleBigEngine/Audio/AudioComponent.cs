using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using LBE.Assets;

namespace LBE.Audio
{
    public class AudioComponent : GameObjectComponent
    {
        Sound m_sound;

        float m_lastPlayTimeMS;

        public Sound Sound
        {
            get { return m_sound; }
        }

        SoundEffectInstance m_soundInstance;

        public AudioComponent(string soundDefinitionPath)
        {
            m_sound = Sound.Create(soundDefinitionPath);
        }

        public AudioComponent(SoundDefinition soundDefinition)
        {
            m_sound = Sound.Create(soundDefinition);
        }

        public AudioComponent(Asset<SoundDefinition> soundAsset)
        {
            m_sound = Sound.Create(soundAsset);
        }

        public override void Start()
        {
            m_lastPlayTimeMS = 0;
        }


        public override void Update()
        {
        }

        public void Play()
        {
            if (Engine.RealTime.TimeMS - m_lastPlayTimeMS < 100)
                return;
            
            int sfxIndex = Engine.Random.Next(m_sound.Definition.SoundEffects.Length);
            float sfxVolume = m_sound.Definition.Volume
                + Engine.Random.NextFloat(-0.5f * m_sound.Definition.VolumeMod, 0.5f * m_sound.Definition.VolumeMod);
            float pitch = m_sound.Definition.Pitch
                + Engine.Random.NextFloat(-0.5f * m_sound.Definition.PitchMod, 0.5f * m_sound.Definition.PitchMod);

            float pan = 0;
                        
            if (m_sound.Definition.Positional)
            {
                Engine.Log.Assert(Engine.Audio.AudioListener != null, "Can't play a positional Sound if no AudioListener is set");
                
                if (Engine.Audio.AudioListener != null)
                {
                    float thisToAudioListenerDistX = Owner.Position.X - Engine.Audio.AudioListener.Position.X;
                    pan = thisToAudioListenerDistX / Engine.Audio.StereoWidth * 2 - 1;
                }
            }

            m_soundInstance = m_sound.Definition.SoundEffects[sfxIndex].SoundEffect.CreateInstance();
			m_soundInstance.Volume = MathHelper.Clamp(0, 1.0f, sfxVolume * Engine.Audio.MasterVolume);
            m_soundInstance.Pitch = pitch;
			m_soundInstance.Pan = MathHelper.Clamp(-1.0f, 1.0f, pan);
            m_soundInstance.Play();

            m_lastPlayTimeMS = Engine.GameTime.TimeMS;

            //m_sound.Definition.SoundEffects[sfxIndex].SoundEffect.Play(volume, pitch, pan);
        }


        public void Stop()
        {
            if (m_soundInstance != null)
            {
                m_soundInstance.Stop();
            }
        }
    }


}
