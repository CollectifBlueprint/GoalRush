using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Gameplay;
using LBE.Graphics.Particles.Modifiers;
using Microsoft.Xna.Framework;
using LBE.Assets;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace LBE.Graphics.Particles
{
    public class ParticleEmitter
    {
        Asset<ParticleEmitterDefinition> m_definitionAsset;
        public ParticleEmitterDefinition Definition
        {
            get { return m_definitionAsset.Content; }
        }
        
        List<IModifier> m_modifiers;
        public List<IModifier> Modifiers
        {
            get { return m_modifiers; }
        }

        TimeSource m_time;
        public TimeSource TimeMS
        {
            get { return m_time; }
        }

        Particle[] m_particles;
        public Particle[] Particles
        {
            get { return m_particles; }
        }

        Stack<int> m_freeParticles;
        public int ParticleCount
        {
            get { return m_particles.Length - m_freeParticles.Count; }
        }

        public Texture2D Texture
        {
            get { return Definition.Texture != null? Definition.Texture : m_defaultTexture.Content; }
        }

        bool m_active = true;
        public bool Active
        {
            get { return m_active; }
            set { m_active = value; }
        }

        Vector2 m_position;
        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        float m_orientation;
        public float Orientation
        {
            get { return m_orientation; }
            set { m_orientation = value; }
        }

        Asset<Texture2D> m_defaultTexture;

        Timer m_emissionTimer;

        float m_particleToEmit;

        float m_timeScale;

        int m_maxUsedIndex;

        public ParticleEmitter()
            : this(new Asset<ParticleEmitterDefinition>(new ParticleEmitterDefinition()))
        {
        }

        public ParticleEmitter(ParticleEmitterDefinition definition)
            : this(new Asset<ParticleEmitterDefinition>(definition))
        {
        }

        public ParticleEmitter(Asset<ParticleEmitterDefinition> definitionAsset)
        {
            m_definitionAsset = definitionAsset;
            m_definitionAsset.OnAssetChanged +=new OnChange(Reset);
            Reset();
        }

        public void Reset()
        {
            //Initialise the simulation time
            m_time = new TimeSource();

            m_particles = new Particle[Definition.MaxParticle];
            m_freeParticles = new Stack<int>(Definition.MaxParticle);

            //Initialise the particle array, and allocate all the particles in it
            for (int i = 0; i < Definition.MaxParticle; i++)
            {
                int index = Definition.MaxParticle - 1 - i;
                m_particles[index] = new Particle() { Owner = this, Alive = false, };
                m_freeParticles.Push(index);
            }

            m_maxUsedIndex = -1;

            m_modifiers = Definition.Modifiers.ToList();

            m_emissionTimer = new Timer(m_time, Definition.EmitDelay * 1000);
            m_emissionTimer.Start();
            m_particleToEmit = 0;

            m_defaultTexture = Engine.AssetManager.GetAsset<Texture2D>("System/DefaultParticle.png");
        }

        public bool Emit()
        {
            //Abort if the stack is empty
            if (m_freeParticles.Count == 0)
                return false;

            //Initialise particle
            int particleIndex = m_freeParticles.Pop();
            var particle = m_particles[particleIndex];

            m_maxUsedIndex = Math.Max(m_maxUsedIndex, particleIndex);

            //Compute local basis
            Vector2 shapeNormal = Definition.Shape.Get(Engine.Random.NextFloat());
            Vector2 velocity = Definition.Velocity.Get();
            Vector2 position = Definition.Position.Get();
            float orientation = Definition.Orientation.Get();
            if (Definition.Coordinates == CoordinatesMode.Local)
                shapeNormal = shapeNormal.Rotate(m_orientation);
            Vector2 shapeTangent = shapeNormal.Rotate((float)Math.PI / 2);
            //position.Rotate(m_orientation);

            particle.Reset();
            particle.Alive = true;
            particle.LifetimeMS = Definition.Lifetime.Get() * 1000;
            particle.Position = position.X * shapeNormal + position.Y * shapeTangent;
            if (Definition.Coordinates == CoordinatesMode.World)
                particle.Position += m_position;
            particle.Velocity = velocity.X * shapeNormal + velocity.Y * shapeTangent;
            particle.Orientation = shapeNormal.Angle() + orientation;
            if (Definition.Coordinates == CoordinatesMode.World)
                particle.Orientation += m_orientation;
            particle.AngularVelocity = Definition.AngularVelocity.Get();
            particle.Scale = Definition.Scale.Get();
            particle.ScaleModifier = Vector2.One;
            particle.Opacity = Definition.Opacity.Get();
            particle.Color = Definition.Color;
            particle.ColorModifier = Definition.Color;
            return true;
        }

        public void Simulate(float elapsedMS)
        {
            //Advance the time of the simulation
            m_time.TickMS(elapsedMS);

            //Emit particles as necessary
            if(m_active)
                m_particleToEmit += Definition.EmitRate * elapsedMS / 1000.0f;
            if (m_active && /*m_emissionTimer.Active == false && */m_particleToEmit >= 1)
            {
                m_emissionTimer.Start();
                for (int i = 0; i < m_particleToEmit; i++)
                {
                    Emit();
                    m_particleToEmit--;
                }
            }

            //ResetModifiers
            for (int i = 0; i <= m_maxUsedIndex; i++)
            {
                m_particles[i].ScaleModifier = Vector2.One;
                m_particles[i].ColorModifier = m_particles[i].Color;
            }

            //Run through all modifiers
            for (int i = 0; i <= m_maxUsedIndex; i++)
            {
                for (int iMod = 0; iMod < m_modifiers.Count; iMod++)
                {
                    m_modifiers[iMod].Modify(m_particles, i, i+1, elapsedMS);
                }
            }

            //Update age and position
            float velocityScale = 0.1f;
            for (int i = 0; i <= m_maxUsedIndex; i++)
            {
                m_particles[i].Age += elapsedMS;
                m_particles[i].Position += m_particles[i].Velocity * elapsedMS * velocityScale;
                m_particles[i].Orientation += m_particles[i].AngularVelocity * elapsedMS;
            }

            //Finally, remove all dead particle
            for (int i = 0; i <= m_maxUsedIndex; i++)
            {
                if (m_particles[i].Alive && m_particles[i].Age >= m_particles[i].LifetimeMS)
                {
                    m_particles[i].Alive = false;
                    m_freeParticles.Push(i);
                }
            }
        }

        public void Debug()
        {
            for (int i = 0; i < m_particles.Length; i++)
            {
                if (m_particles[i].Alive)
                {
                    Engine.Debug.Screen.ResetBrush();
                    Engine.Debug.Screen.Brush.DrawSurface = false;
                    Engine.Debug.Screen.AddRectangle(m_particles[i].Position, m_particles[i].Scale * 8);
                }
            }
        }
    }
}
