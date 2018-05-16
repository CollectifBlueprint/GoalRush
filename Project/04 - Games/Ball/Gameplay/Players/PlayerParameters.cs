using Ball.Gameplay.Players;
using Microsoft.Xna.Framework;
using Ball.Career;
using Microsoft.Xna.Framework.Graphics;
using LBE.Graphics.Sprites;

namespace Ball.Gameplay
{
    public class PlayerProfileParameters
    {
        public PlayerParameters Player;
    }

    public class PlayerControlParameters
    {
        public float Speed;
        public float SpeedWithBallMod;
        public float BallSpeed;
    }

    public class PlayerPhysicParameters
    {
        public float LinearDamping;
        public float AngularDamping;
        public float Restitution;
        public float Mass;
    }

    public class PlayerSkillsParameters
    {
        public bool Fire;
        public bool Tackle;
    }

    public class TackleParameters
    {
        public float Speed;
        public float Power;
        public float CooldownMS;
        public float DurationMS;
        public float StunTimeMS;
        public float StunSpeedMod;
    }

    public class ChargedShotParameters
    {
        public float ChargeTimeMS;
        public float ShotImpulse;
        public float PlayerSpeedMod;
        public float BallBashTimeMS;
        public float BallBashImpulse;
    }

    public class ChargedPassParameters
    {
        public float ChargeTimeMS;
        public float PlayerSpeedMod;
    }

    public class PlayerSpriteInfo
    {
        public Texture2D Material;
        public Texture2D Mask;
    }

    public class PlayerParameters
    {
        public Color Color;
        public PlayerSpriteInfo[] SpriteInfos;

        public InputType ControllerType;
        public float Radius;
        public float MagnetPower;
        public float MagnetRadiusMin;
        public float MagnetRadiusMax;
        public float MagnetCooldownMS;
        public float BallSnapRadius;
        public float ShotImpulse;
        public float StunTimeMSDefault = 500;
        public float StunSpeedModDefault = 0.1f;
        public float StunBallImpulseDefault = 0;
        public PlayerControlParameters Controls;
        public PlayerPhysicParameters Physic;
        public TackleParameters Tackle;
        public ChargedShotParameters ChargedShot;
        public ChargedPassParameters ChargedPass;
        public PlayerSkillParameters Skills;

    }
}