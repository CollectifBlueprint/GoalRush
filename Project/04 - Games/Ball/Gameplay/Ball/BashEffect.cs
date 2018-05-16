using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using Microsoft.Xna.Framework;
using LBE;
using LBE.Gameplay;
using LBE.Graphics.Particles;

namespace Ball.Gameplay.BallEffects
{
    public class BashEffect : BallEffect
    {
        ParticleComponent m_particleCmpHead;
        ParticleComponent m_particleCmpTrail;
        ParticleComponent m_particleCmpTrailHighlight;

        public override void Start()
        {
            if (Ball.LastPlayer == null)
            {
                Cancel();
                return;
            }

            Ball.Properties.Untakable.Set();
            Ball.Properties.PassThroughPlayer.Set();
            Ball.BallSprite.Alpha = 0;

            StartFX();
        }

        public override void Update()
        {
            Ball.Owner.Orientation = (float)Math.Atan2(Ball.BodyCmp.Body.LinearVelocity.Y, Ball.BodyCmp.Body.LinearVelocity.X);

            CheckShield();
        }

        public override void End()
        {
            Ball.BallTrail.Desactivate(true);

            Ball.BallTrail.Desactivate(true);
            Ball.Properties.Untakable.Unset();
            Ball.Properties.PassThroughPlayer.Unset();

            Ball.BallSprite.Alpha = 1;

            EndFX();
        }

        public override void OnPlayerTouchBall(Player player)
        {
            if (player.Properties.Bashed)
                return;

            if (player.Properties.Shield)
            {
                if(player.BodyCmp.Body.LinearVelocity != null)
                {
                    EndFX();
                    Ball.LastPlayer = player;
                    StartFX();

                    var dir = player.BodyCmp.Body.LinearVelocity;
                    dir.Normalize();
                    Ball.BodyCmp.Body.LinearVelocity = dir * Ball.BodyCmp.Body.LinearVelocity.Length();

                    ScreenShake.Add(3, dir, 150);
                }
                return;
            }

            Vector2 ballToPlayerDir = player.Position - Ball.Position;
            Vector2 ballDir = Ball.BodyCmp.Body.LinearVelocity;
            ballDir.Normalize();

            float side = LBE.MathHelper.CrossProductSign(ballDir, ballToPlayerDir);
            float angleMin = 0.3f * (float)Math.PI;
            float angleMax = 0.5f * (float)Math.PI;
            float angleRandom = Engine.Random.NextFloat(angleMin, angleMax);
            float finalAngle = angleRandom * side;
            Vector2 bashDir = ballDir.Rotate(finalAngle);

            player.Bash(bashDir);
        }

        public override void OnPlayerTakeBall(Player player)
        {
            Cancel();
        }

        private void StartFX()
        {
            var headEmitterDefAsset = Engine.AssetManager.GetAsset<ParticleEmitterDefinition>("Graphics/Particles/ChargedShot.lua::Ball");
            var headEmitterDef = headEmitterDefAsset.Content;
            headEmitterDef.Color = Ball.LastPlayer.PlayerColors[2];

            m_particleCmpHead = new ParticleComponent();
            m_particleCmpHead.Emitter = new ParticleEmitter(headEmitterDef);
            Ball.Owner.Attach(m_particleCmpHead);

            var trailEmitterDefAsset = Engine.AssetManager.GetAsset<ParticleEmitterDefinition>("Graphics/Particles/ChargedShot.lua::Trail");
            var trailEmitterDef = trailEmitterDefAsset.Content;
            trailEmitterDef.Color = Ball.LastPlayer.PlayerColors[2];

            m_particleCmpTrail = new ParticleComponent();
            m_particleCmpTrail.Emitter = new ParticleEmitter(trailEmitterDef);
            Ball.Owner.Attach(m_particleCmpTrail);

            var trailHlEmitterDefAsset = Engine.AssetManager.GetAsset<ParticleEmitterDefinition>("Graphics/Particles/ChargedShot.lua::TrailHighlight");
            var trailHlEmitterDef = trailHlEmitterDefAsset.Content;
            trailHlEmitterDef.Color = Ball.LastPlayer.PlayerColors[2];

            m_particleCmpTrailHighlight = new ParticleComponent();
            m_particleCmpTrailHighlight.Emitter = new ParticleEmitter(trailHlEmitterDef);
            Ball.Owner.Attach(m_particleCmpTrailHighlight);
        }

        private void EndFX()
        {
            m_particleCmpHead.Emitter.Active = false;
            m_particleCmpHead.DestroyOnEnd = true;

            m_particleCmpTrail.Emitter.Active = false;
            m_particleCmpTrail.DestroyOnEnd = true;

            m_particleCmpTrailHighlight.Emitter.Active = false;
            m_particleCmpTrailHighlight.DestroyOnEnd = true;
        }

        private void CheckShield()
        {
            var maxDist = 300;
            var minDist = 50.0f;
            var player = Game.GameManager.Players[0];
            if (Ball.LastPlayer != player && player.Properties.Tackling)
            {
                var playerToBall = Ball.Position - player.Position;
                if (Vector2.Dot(playerToBall, player.BodyCmp.Body.LinearVelocity) > 0 && Vector2.Dot(-playerToBall, Ball.BodyCmp.Body.LinearVelocity) > 0)
                {
                    var dist = Vector2.Distance(Ball.Position, player.Position);
                    var distRel = (dist - minDist) / (maxDist - minDist);
                    if (distRel < 1)
                    {
                        distRel = LBE.MathHelper.Clamp(0, 1, distRel);
                        ScreenFocus.Instance.Focus((Ball.Position + player.Position) * 0.5f, 1 - distRel);
                    }
                }
            }
        }
    }
}
