using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using Microsoft.Xna.Framework;
using LBE;
using LBE.Graphics.Particles;

namespace Ball.Gameplay.BallEffects
{
    public class PassAssistParamters
    {
        public float Angle;
        public float Strength;
        public float Radius;
        public float AngleSpeed;
        public float Speed;
        public float Acceleration;
        public float MaxRotation;
    }

    public class PassAssistEffect : BallEffect
    {
        Asset<PassAssistParamters> m_params;
        public PassAssistParamters Parameters
        {
            get { return m_params.Content; }
        }

        float m_strength = 1.0f;
        public float Strength
        {
            get { return m_strength; }
            set { m_strength = value; }
        }

        Player m_ballOwner;
        Player m_target;

        float m_startTimeMs;

        Vector2 m_startDirection;
        Vector2 m_startPosition;
        float m_startVelocity;
        float m_rotationSign;

        Color m_color;
        Vector2 m_previousBallSpeedDir;
        float m_totalBallRotation;

        ParticleComponent m_particleCmpHead;
        ParticleComponent m_particleCmpTrail;
        ParticleComponent m_particleCmpTrailHighlight;

        public PassAssistEffect(Player target, Player ballOwner)
        {
            m_target = target;
            m_ballOwner = ballOwner;
            m_color = ballOwner.Team.ColorScheme.Color1;
        }

        public PassAssistEffect(Player target, Player ballOwner, Color color)
        {
            m_target = target;
            m_ballOwner = ballOwner;
            m_color = color;
        }
        public override void Start()
        {
            m_params = new Asset<PassAssistParamters>(new PassAssistParamters());
            Parameters.Angle = Engine.Debug.EditSingle("PassAssistAngle", 0.4f);
            Parameters.Strength = Engine.Debug.EditSingle("PassAssistStrength", 20);
            Parameters.Radius = Engine.Debug.EditSingle("PassAssistRadius", 80);
            Parameters.AngleSpeed = Engine.Debug.EditSingle("PassAssistAngleSpeed", 0.002f);
            Parameters.Speed = Engine.Debug.EditSingle("PassAssistSpeed", 10);
            Parameters.Acceleration = Engine.Debug.EditSingle("PassAssistAcceleration", 100);
            Parameters.MaxRotation = Engine.Debug.EditSingle("PassAssistMaxRotation", 3.2f);

            m_startPosition = Ball.Position;
            m_startTimeMs = Engine.GameTime.TimeMS;

            m_previousBallSpeedDir = Ball.BodyCmp.Body.LinearVelocity;

            Color effectColor;
            if (m_ballOwner != null) effectColor = m_ballOwner.PlayerColors[2];
            else effectColor = m_target.Team.ColorScheme.Color1;

            var headEmitterDefAsset = Engine.AssetManager.GetAsset<ParticleEmitterDefinition>("Graphics/Particles/PassShot.lua::Ball");
            var headEmitterDef = headEmitterDefAsset.Content;
            headEmitterDef.Color = effectColor;

            m_particleCmpHead = new ParticleComponent();
            m_particleCmpHead.Emitter = new ParticleEmitter(headEmitterDef);
            Ball.Owner.Attach(m_particleCmpHead);

            var trailEmitterDefAsset = Engine.AssetManager.GetAsset<ParticleEmitterDefinition>("Graphics/Particles/PassShot.lua::Trail");
            var trailEmitterDef = trailEmitterDefAsset.Content;
            trailEmitterDef.Color = effectColor;

            m_particleCmpTrail = new ParticleComponent();
            m_particleCmpTrail.Emitter = new ParticleEmitter(trailEmitterDef);
            Ball.Owner.Attach(m_particleCmpTrail);

            var trailHlEmitterDefAsset = Engine.AssetManager.GetAsset<ParticleEmitterDefinition>("Graphics/Particles/PassShot.lua::TrailHighlight");
            var trailHlEmitterDef = trailHlEmitterDefAsset.Content;
            trailHlEmitterDef.Color = effectColor;

            m_particleCmpTrailHighlight = new ParticleComponent();
            m_particleCmpTrailHighlight.Emitter = new ParticleEmitter(trailHlEmitterDef);
            Ball.Owner.Attach(m_particleCmpTrailHighlight);


            Ball.BallSprite.Alpha = 0.5f;
        }

        public override void End()
        {
            Ball.BallTrail.Desactivate(false);

            m_particleCmpHead.Emitter.Active = false;
            m_particleCmpHead.DestroyOnEnd = true;

            m_particleCmpTrail.Emitter.Active = false;
            m_particleCmpTrail.DestroyOnEnd = true;

            m_particleCmpTrailHighlight.Emitter.Active = false;
            m_particleCmpTrailHighlight.DestroyOnEnd = true;


            Ball.BallSprite.Alpha = 1;
        }

        bool m_processEffect = false;
        public override void Update()
        {
            m_params = new Asset<PassAssistParamters>(new PassAssistParamters());
            Parameters.Angle = Engine.Debug.EditSingle("PassAssistAngle", 0.4f);
            Parameters.Strength = Engine.Debug.EditSingle("PassAssistStrength", 20);
            Parameters.Radius = Engine.Debug.EditSingle("PassAssistRadius", 80);
            Parameters.AngleSpeed = Engine.Debug.EditSingle("PassAssistAngleSpeed", 0.002f);
            Parameters.Speed = Engine.Debug.EditSingle("PassAssistSpeed", 10);
            Parameters.Acceleration = Engine.Debug.EditSingle("PassAssistAcceleration", 100);
            Parameters.MaxRotation = Engine.Debug.EditSingle("PassAssistMaxRotation", 3.2f);

            Vector2 playerDir = m_target.Position - Ball.Position;
            Vector2 ballSpeedDir = Ball.BodyCmp.Body.LinearVelocity;

            //Cancel the assist if after the ball have turn sufficently
            float angleUpdate = 0;
            if (m_previousBallSpeedDir != Vector2.Zero && ballSpeedDir != Vector2.Zero)
            {
                angleUpdate = LBE.MathHelper.Angle(ballSpeedDir, m_previousBallSpeedDir);
                m_totalBallRotation += Math.Abs(angleUpdate);
            }
            m_previousBallSpeedDir = ballSpeedDir;

            if (m_totalBallRotation > Math.PI)
                Cancel();

            playerDir.Normalize();
            ballSpeedDir.Normalize();

            Engine.Log.Debug("PassAssistAngle", m_totalBallRotation);

            if (!m_processEffect)
            {
                m_processEffect = true;

                m_rotationSign = Math.Sign(LBE.MathHelper.CrossProductDet(Ball.BodyCmp.Body.LinearVelocity, playerDir));
                m_startVelocity = Ball.BodyCmp.Body.LinearVelocity.Length();
                m_startDirection = Ball.BodyCmp.Body.LinearVelocity; m_startDirection.Normalize();
                if (Vector2.Dot(playerDir, ballSpeedDir) < 0.5f)
                {
                   Ball.BodyCmp.Body.LinearVelocity *= 0.0f;
                }
            }

            if (m_processEffect)
            {
                if (!Ball.BallTrail.Active)
                {
                    Ball.BallTrail.Activate(m_color);
                }

                if (Vector2.Dot(playerDir, ballSpeedDir) < 0.5f)
                {
                    float startAngle = m_startDirection.Angle();
                    float maxAngle = Math.Abs(LBE.MathHelper.Angle(m_target.Position - m_startPosition, m_startDirection));
                    float angle = m_rotationSign * Parameters.AngleSpeed * (Engine.GameTime.TimeMS - m_startTimeMs) * m_strength;
                    angle = LBE.MathHelper.Clamp(-maxAngle, maxAngle, angle);
                    Vector2 startToPlayerDir = m_target.Position - m_startPosition; startToPlayerDir.Normalize();
                    Vector2 offset = startToPlayerDir * (Engine.GameTime.TimeMS - m_startTimeMs) * Parameters.Speed * m_strength;
                    Vector2 desiredPosition = m_startPosition + offset + Vector2.UnitX.Rotate(startAngle + angle) * Parameters.Radius / m_strength;

                    Engine.Debug.Screen.AddCross(desiredPosition, 16);

                    Vector2 impulseDir = desiredPosition - Ball.Position; impulseDir.Normalize();
                    Ball.BodyCmp.Body.ApplyForce(impulseDir * Parameters.Strength);
                }
                else
                {
                    Vector2 impulseDir = playerDir - Vector2.Dot(playerDir, ballSpeedDir) * ballSpeedDir;
                    if (impulseDir != Vector2.Zero && !float.IsNaN(impulseDir.X) && !float.IsNaN(impulseDir.Y))
                    {
                        float currentVelocity = Ball.BodyCmp.Body.LinearVelocity.Length();
                        float velocityDelta = m_startVelocity - currentVelocity;
                        Ball.BodyCmp.Body.LinearVelocity = LBE.MathHelper.Lerp(Ball.BodyCmp.Body.LinearVelocity, m_startVelocity * playerDir, Parameters.Acceleration);
                    }
                }
            }

            Ball.Owner.Orientation = (float)Math.Atan2(Ball.BodyCmp.Body.LinearVelocity.Y, Ball.BodyCmp.Body.LinearVelocity.X);
        }

        public override void OnPlayerTouchBall(Player player)
        {
            if (player != m_ballOwner)
                Cancel();
        }

        public override void OnPlayerTakeBall(Player player)
        {
            Cancel();
        }

        public override void OnWallCollision(FarseerPhysics.Dynamics.Fixture wall)
        {
            Cancel();
        }
    }
}
