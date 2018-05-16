using System;
using System.Collections.Generic;
using Ball.Gameplay.BallEffects;
using Ball.Gameplay.Fx;
using Ball.Physics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using LBE;
using LBE.Assets;
using LBE.Audio;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using LBE.Input;
using LBE.Physics;
using Microsoft.Xna.Framework;
using Ball.Gameplay.Players;
using Ball.Gameplay.Players.AI;
using Ball.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LBE.Debug;

namespace Ball.Gameplay
{
    public enum ShotType
    {
        None,
        Normal,
        Charged,
        Pass,
    }

    public class Player : GameObjectComponent
    {
        public enum ColorElement
        {
            Body = 0,
            Team,
            MakeUp,
            Body2
        }

        #region Members

        PlayerInfo m_playerInfo;
        public PlayerInfo PlayerInfo {
            get { return m_playerInfo; }
            set { m_playerInfo = value; }
        }

        PlayerIndex m_playerIndex;
        public PlayerIndex PlayerIndex {
            get { return m_playerIndex; }
        }

        PlayerController m_controller;
        public PlayerController Controller {
            get { return m_controller; }
        }

        PlayerInput m_input;
        public PlayerInput Input {
            get { return m_input; }
        }

        PlayerProperties m_properties;
        public PlayerProperties Properties {
            get { return m_properties; }
        }

        RigidBodyComponent m_bodyCmp;
        public RigidBodyComponent BodyCmp {
            get { return m_bodyCmp; }
        }
        Fixture m_fixture = null;

        TriggerComponent m_ballTrigger;
        public TriggerComponent BallTrigger {
            get { return m_ballTrigger; }
        }

        TriggerComponent m_magnetSensor;
        public TriggerComponent MagnetSensor {
            get { return m_magnetSensor; }
        }

        AudioComponent m_audioCmpBallShot;
        public AudioComponent AudioCmpBallShot {
            get { return m_audioCmpBallShot; }
        }

        AudioComponent m_audioCmpStun;
        public AudioComponent AudioCmpStun {
            get { return m_audioCmpStun; }
        }

        AudioComponent m_audioCmpCharge;
        public AudioComponent AudioCmpCharge {
            get { return m_audioCmpCharge; }
        }

        List<PlayerEffect> m_effects;
        public List<PlayerEffect> Effects {
            get { return m_effects; }
        }

        float m_ballAngle;
        public float BallAngle {
            get { return m_ballAngle; }
        }

		Vector2 m_initialPosition;
		public Vector2 InitialPosition {
			get { return m_initialPosition; }
			set { m_initialPosition = value; }
		}

        Vector2 m_moveDirection;
        public Vector2 MoveDirection {
            get { return m_moveDirection; }
            set { m_moveDirection = value; }
        }

        Vector2 m_aimDirection;
        public Vector2 AimDirection {
            get { return m_aimDirection; }
            set { m_aimDirection = value; }
        }

        Asset<PlayerParameters> m_paramAsset;
        public PlayerParameters Parameters {
            get { return m_paramAsset.Content; }
        }

        Color[] m_playerColors; //
        public Color[] PlayerColors {
            get { return m_playerColors; }
        }

        // Charged Shot
        Timer m_chargedShotTimer;
        public bool IsShotCharged {
            get { return (!m_chargedShotTimer.Active && m_chargingShot); }
        }

        Timer m_chargedShotDelayTimer;
        public Timer ChargedShotDelayTimer {
            get { return m_chargedShotDelayTimer; }
        }

        bool m_chargingShot;
        public bool IsShotCharging {
            get { return m_chargingShot; }
        }

        // Charged Pass
        bool m_chargingPass;
        public bool IsPassCharging {
            get { return m_chargingPass; }
        }

        Asset<PlayerProfileParameters> m_playerAsset;

        float m_magnetRayCastBallFraction;
        float m_magnetRayCastShortestBlockerFraction;

        Ball m_ball;
        public Ball Ball {
            get { return m_ball; }
        }

        Team m_team;
        public Team Team {
            get { return m_team; }
        }

        Player[] m_opponents;
        public Player[] Opponents {
            get { return m_opponents; }
            set { m_opponents = value; }
        }

        Player m_teamMate;
        public Player TeamMate {
            get { return m_teamMate; }
            set { m_teamMate = value; }
        }

        ColorMaskedSprite m_spritePlayerCmp;
        public ColorMaskedSprite SpritePlayerCmp {
            get { return m_spritePlayerCmp; }
        }

        SpriteComponent m_shadowCmp;
        public SpriteComponent ShadowCmp {
            get { return m_shadowCmp; }
        }
        SpriteComponent m_sparksCmp;
        public SpriteComponent SparksCmp {
            get { return m_sparksCmp; }
        }

        SpriteComponent m_goldFxSpriteCmp;
        public SpriteComponent GoldFxSpriteCmp {
            get { return m_goldFxSpriteCmp; }
        }

        SpriteComponent m_shieldUpFxSpriteCmp;
        public SpriteComponent ShieldUpFxSpriteCmp
        {
            get { return m_shieldUpFxSpriteCmp; }
        }

        PlayerBlink m_blink;
        public PlayerBlink Blink {
            get { return m_blink; }
        }

        PlayerControllerTextFx m_controllerTxtCmp;
        public PlayerControllerTextFx ControllerTxtCmp {
            get { return m_controllerTxtCmp; }
        }

        #endregion Members

        #region ctor/dtor

        public Player(PlayerInfo playerInfo, Team team, String profilePath = "Game/Player.lua::PlayerDefault")
        {
            m_playerInfo = playerInfo;
            m_playerIndex = playerInfo.PlayerIndex;
            m_team = team;

            m_playerColors = new Color[] { Color.Silver, Color.Silver, Color.Silver, Color.Silver };

            m_paramAsset = Engine.AssetManager.GetAsset<PlayerParameters>("Game/PlayerSolo.lua::Player");
            m_paramAsset.OnAssetChanged += new OnChange(ResetParams);

            m_properties = new PlayerProperties();
        }

        public override void End()
        {
            foreach (var effect in m_effects.ToArray())
            {
                effect.End();

                if (!effect.Active)
                    RemoveEffect(effect);
            }
            m_effects.Clear();

            base.End();
        }

        #endregion Members

        #region Initialisation

        public override void Start()
        {
        }

        public void SetController(PlayerController controller)
        {
            if (m_controller != null)
                Owner.Remove(m_controller);

            m_controller = controller;
            Owner.Attach(controller);

            var humanController = m_controller as PlayerHumanController;
            if (humanController != null)
                m_input = humanController.Input;

            if (m_controllerTxtCmp != null)
                m_controllerTxtCmp.Stop();

            m_controllerTxtCmp = new PlayerControllerTextFx(this);
            Owner.Attach(m_controllerTxtCmp);
        }


        public void SetColor(ColorElement element, Color color)
        {
            m_playerColors[(int)element] = color;
        }

        public void Init()
        {
            m_effects = new List<PlayerEffect>();

            var allPlayerSpriteInfos = Engine.AssetManager.Get<PlayerSpriteInfo[]>("Game/PlayerSprites.lua::PlayerSprites");
            var playerSpriteInfo = allPlayerSpriteInfos[(int)m_playerIndex];

            Sprite playerSprite = Sprite.CreateFromTexture(playerSpriteInfo.Material);
            playerSprite.Scale = new Vector2(0.48f, 0.48f);
            m_spritePlayerCmp = new ColorMaskedSprite(playerSprite, "ArenaOverlay9");
            var texAsset = playerSpriteInfo.Mask;

            m_spritePlayerCmp.Mask = texAsset;
            ResetColors();
            Owner.Attach(m_spritePlayerCmp);

            m_shadowCmp = new SpriteComponent(
                Sprite.CreateFromTexture("Graphics/playerShadow.png"),
                "ArenaOverlay8");
            m_shadowCmp.Sprite.Scale = playerSprite.Scale;
            Owner.Attach(m_shadowCmp);

            Sprite sparksSprite = Sprite.Create("Graphics/playerSprite.lua::Sprite");
            sparksSprite.SetAnimation("Sparks");
            sparksSprite.Playing = true;
            m_sparksCmp = new SpriteComponent(sparksSprite, "ArenaOverlay10");
            m_sparksCmp.Visible = false;
            Owner.Attach(m_sparksCmp);

            Sprite goldFxSprite = Sprite.Create("Graphics/playerSprite.lua::Sprite");
            goldFxSprite.SetAnimation("GoldFx");
            goldFxSprite.Playing = false;
            m_goldFxSpriteCmp = new SpriteComponent(goldFxSprite, "ArenaOverlay10");
            m_goldFxSpriteCmp.Visible = false;
            Owner.Attach(m_goldFxSpriteCmp);

            Sprite shieldUpFxSprite = Sprite.Create("Graphics/playerSprite.lua::Sprite");
            shieldUpFxSprite.SetAnimation("ShieldUp");
            shieldUpFxSprite.Playing = false;
            m_shieldUpFxSpriteCmp = new SpriteComponent(shieldUpFxSprite, "ArenaOverlay10");
            m_shieldUpFxSpriteCmp.Visible = false;
            Owner.Attach(m_shieldUpFxSpriteCmp);

            // Physic
            m_bodyCmp = new RigidBodyComponent();
            CircleShape bodyShape = new CircleShape(ConvertUnits.ToSimUnits(Parameters.Radius), 1.0f);
            Fixture fix = new Fixture(m_bodyCmp.Body, bodyShape);

            Owner.Attach(m_bodyCmp);

            m_bodyCmp.Body.FixedRotation = true;

            m_bodyCmp.Body.LinearDamping = Parameters.Physic.LinearDamping;
            m_bodyCmp.Body.AngularDamping = Parameters.Physic.AngularDamping;
            m_bodyCmp.Body.Restitution = Parameters.Physic.Restitution;
            m_bodyCmp.Body.Mass = Parameters.Physic.Mass;
            m_bodyCmp.Body.CollisionCategories = (Category)CollisionType.Player;
            m_bodyCmp.Body.SleepingAllowed = false;
            m_bodyCmp.Body.IsBullet = true;

            // Blink
            m_blink = new PlayerBlink(this);
            Owner.Attach(m_blink);

            if (m_playerIndex == PlayerIndex.One)
                m_bodyCmp.Body.CollisionCategories = (Category)CollisionType.Gameplay | (Category)CollisionType.Player | (Category)CollisionType.TeamA;
            if (m_playerIndex == PlayerIndex.Two)
                m_bodyCmp.Body.CollisionCategories = (Category)CollisionType.Gameplay | (Category)CollisionType.Player | (Category)CollisionType.TeamA;
            if (m_playerIndex == PlayerIndex.Three)
                m_bodyCmp.Body.CollisionCategories = (Category)CollisionType.Gameplay | (Category)CollisionType.Player | (Category)CollisionType.TeamB;
            if (m_playerIndex == PlayerIndex.Four)
                m_bodyCmp.Body.CollisionCategories = (Category)CollisionType.Gameplay | (Category)CollisionType.Player | (Category)CollisionType.TeamB;

            // Audio
            m_audioCmpBallShot = new AudioComponent("Audio/Sounds.lua::BallShot");
            Owner.Attach(m_audioCmpBallShot);
            m_audioCmpStun = new AudioComponent("Audio/Sounds.lua::Stun");
            Owner.Attach(m_audioCmpStun);
            m_audioCmpCharge = new AudioComponent("Audio/Sounds.lua::Charge");
            Owner.Attach(m_audioCmpCharge);

            //
            m_ballTrigger = new TriggerComponent();
            var triggerFixture = FixtureFactory.AttachCircle(
                ConvertUnits.ToSimUnits(Parameters.Radius + Parameters.BallSnapRadius),
                0,
                m_ballTrigger.Body);
            triggerFixture.IsSensor = true;
            m_ballTrigger.Body.CollidesWith = (Category)CollisionType.Ball;
            Owner.Attach(m_ballTrigger);

            m_magnetSensor = new TriggerComponent();
            triggerFixture = FixtureFactory.AttachCircle(
                ConvertUnits.ToSimUnits(Parameters.MagnetRadiusMax),
                0,
                m_magnetSensor.Body);
            triggerFixture.IsSensor = true;
            m_magnetSensor.Body.CollidesWith = (Category)CollisionType.Ball;
            Owner.Attach(m_magnetSensor);

            Engine.World.EventManager.AddListener((int)EventId.MatchBegin, OnMatchBegin);
            Engine.World.EventManager.AddListener((int)EventId.FirstPeriod, OnFirstPeriod);
            Engine.World.EventManager.AddListener((int)EventId.HalfTime, OnHalfTime);
            Engine.World.EventManager.AddListener((int)EventId.HalfTimeTransition, OnHalfTimeTransition);
            Engine.World.EventManager.AddListener((int)EventId.SecondPeriod, OnSecondPeriod);
            Engine.World.EventManager.AddListener((int)EventId.MatchEnd, OnMatchEnd);
            Engine.World.EventManager.AddListener((int)EventId.Victory, OnMatchVictory);
            Engine.World.EventManager.AddListener((int)EventId.KickOff, OnKickOff);

            m_ballAngle = 0;

            //Initialise ChargetShots Timer
            float chargeTimeMS = Parameters.ChargedShot.ChargeTimeMS;
            chargeTimeMS *= ComputeSkillCoef("ChargedShotTime", Parameters.Skills.ChargedShotTimeBase, Parameters.Skills.ChargedShotTimeCoef);
            m_chargedShotTimer = new Timer(Engine.GameTime.Source, chargeTimeMS, TimerBehaviour.Stop);

            float delay = Engine.Debug.EditSingle("ChargedShotDelay", 250);
            m_chargedShotDelayTimer = new Timer(Engine.GameTime.Source, delay);

            ShakeComponent shakeCmp = new ShakeComponent(m_spritePlayerCmp);
            shakeCmp.ShakeAmount = 4.0f;
            Owner.Attach(shakeCmp);

            PlayerOffScreenMarker offScreenMarker = new PlayerOffScreenMarker();
            Owner.Attach(offScreenMarker);

            if (m_playerInfo.InputType == InputType.AI && m_teamMate.PlayerInfo.InputType == InputType.AI)
                offScreenMarker.Hide = true;

            m_moveDirection = Vector2.Zero;

            m_bodyCmp.OnCollision += new CollisionEvent(OnCollision);

            m_bodyCmp.UserData.Add("Tag", "Player");

            //Antenna
            var playerAntenna = new PlayerAntenna();
            Owner.Attach(playerAntenna);

        }

        private void ResetMass()
        {
            m_bodyCmp.Body.Mass = Parameters.Physic.Mass;
        }

        public void ResetState()
        {
            foreach (var effect in m_effects.ToArray())
            {
                RemoveEffect(effect);
            }
            m_properties = new PlayerProperties();

            StopChargingShot();
            StopChargingPass();
            DettachBall();
        }

        public void ResetParams()
        {
            //TODO
            m_paramAsset.ToString();
        }

        public void ResetColors()
        {
            m_spritePlayerCmp.Color1 = m_playerColors[0];
            m_spritePlayerCmp.Color2 = m_playerColors[3];
            m_spritePlayerCmp.Color3 = m_playerColors[2];
        }

        #endregion Initialisation

        #region Update

        public override void Update()
        {
            if (Game.GameManager.Paused)
                return;

            UpdateEffects();

            // Check if the player can Grab the ball
            if (!Properties.MagnetDisabled && m_ballTrigger.Active && m_ball == null)
            {
                Ball ball = m_ballTrigger.ActiveObjects[0].Owner.FindComponent<Ball>();
                if (ball != null && ball.Player == null && !ball.Properties.Untakable)
                {
                    ball.AttachToPlayer(this);
                    AttachBall(ball);
                }
            }

            if (m_ball == null && m_magnetSensor.Active)
                ApplyMagnet();

            // Update ball control for mouse
            if (Parameters.ControllerType == InputType.MouseKeyboard)
            {
                CustomInput2D cInput = (CustomInput2D)m_input.BallCtrl.Inputs.Find(delegate (IInput2D i) { return i.GetType() == typeof(CustomInput2D); });
                if (cInput != null)
                {
                    Vector2 MousePos = new Vector2(Engine.Input.MouseState().X, Engine.Input.MouseState().Y);
                    MousePos = Engine.Renderer.Cameras[(int)CameraId.Game].ScreenToWorld(MousePos);
                    Vector2 MousePlayerAxe = MousePos - Owner.Position;
                    MousePlayerAxe.Normalize();
                    cInput.Set(MousePlayerAxe);
                    cInput.Update();
                }
            }

            // Update object rotation
            Owner.Orientation = m_ballAngle;

            // Debug ball axis
            Vector2 ballAxis = new Vector2((float)Math.Cos(Owner.Orientation), (float)Math.Sin(Owner.Orientation));
            if (m_input != null && m_input.BallCtrl.Get() != Vector2.Zero)
                ballAxis = m_input.BallCtrl.Get();
            ballAxis.Normalize();
            Engine.Debug.Screen.AddLine(Owner.Position, Owner.Position + ballAxis * Parameters.Radius * 1.5f * 3);

            // Debug player direction Axis
            Engine.Debug.Screen.Brush.LineColor = Color.Blue;
            Engine.Debug.Screen.AddLine(Owner.Position, Owner.Position + m_moveDirection * Parameters.Radius * 2);

            if (Engine.Debug.Flags.ColorEdit == true)
                DebugColorEdit();

            float shakeScale = 0.2f;
            if (m_chargingShot && m_chargedShotTimer.Active)
            {
                float completion = m_chargedShotTimer.TimeMS / m_chargedShotTimer.TargetTime;
                ScreenShake.Add(shakeScale * LBE.MathHelper.Clamp(0, 1, completion));
            }
            else if (m_chargingShot && m_chargedShotTimer.Active == false)
            {
                ScreenShake.Add(shakeScale);
            }

            if (m_chargedShotDelayTimer.Active)
            {
                float time = 1 - m_chargedShotDelayTimer.Completion;
                //time = (float)Math.Pow(time, 0.2f);

                float slowCoef = 0.1f;
                Engine.TimeCoef = LBE.MathHelper.Lerp(slowCoef, 1, time);

                Engine.Log.Write("Time: " + time);
                Engine.Log.Write("Slow: " + Engine.TimeCoef);
            }

            if (m_properties.Shooting && m_ball == null)
            {
                m_properties.Shooting.Unset();
                Engine.TimeCoef = 1.0f;
            }

            if (m_properties.Shooting && !m_chargedShotDelayTimer.Active)
            {
                Engine.TimeCoef = 1.0f;
                m_properties.Shooting.Unset();
                ShootBallChargedAfterDelay();
            }
        }

        private void ApplyMagnet()
        {
            if (Properties.MagnetDisabled)
                return;

            Ball ball = m_magnetSensor.ActiveObjects[0].Owner.FindComponent<Ball>();
            if (ball != null && !ball.Properties.Untakable)
            {
                m_magnetRayCastBallFraction = 0;
                m_magnetRayCastShortestBlockerFraction = -1;

                Engine.PhysicsManager.World.RayCast(OnMagnetRayCast,
                    ConvertUnits.ToSimUnits(Owner.Position),
                    ConvertUnits.ToSimUnits(ball.Owner.Position));

                if (m_magnetRayCastShortestBlockerFraction == -1
                    || m_magnetRayCastBallFraction < m_magnetRayCastShortestBlockerFraction)
                {
                    // no object between the ball and the player, magnet can be applied
                    float distBallToPlayer = Vector2.Distance(ball.Position, Owner.Position);

                    float magnetMinRadius = Parameters.MagnetRadiusMin;
                    float magnetMaxRadius = Parameters.MagnetRadiusMax;

                    if (distBallToPlayer < magnetMaxRadius)
                    {
                        Vector2 magnetDir = -(ball.Position - Owner.Position);
                        magnetDir.Normalize();
                        float magnetForce = 100;

                        ball.BodyCmp.Body.LinearVelocity = magnetDir * magnetForce;
                        ball.BodyCmp.Body.ApplyForce(magnetForce * magnetDir);
                    }
                }
            }
        }

        private void UpdateBallPosition(Ball ball, float desiredAngle)
        {
            if (m_ball == null)
                return;

            int nbrPoint = 16;
            float angleDistance = LBE.MathHelper.AngleDistanceSigned(m_ballAngle, desiredAngle);
            float angleStep = 2 * (float)Math.PI / nbrPoint * Math.Sign(angleDistance);
            float angleAcc = 0;
            while (Math.Abs(angleAcc) < Math.Abs(angleDistance))
            {
                float angle = m_ballAngle + angleAcc;
                float angleNext = angle + angleStep;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 dirNext = new Vector2((float)Math.Cos(angleNext), (float)Math.Sin(angleNext));
                Vector2 pos = Owner.Position + dir * (Parameters.Radius + ball.Parameters.Radius + 1);
                Vector2 posNext = Owner.Position + dirNext * (Parameters.Radius + ball.Parameters.Radius + 1);

                bool rayTest = true;
                Engine.PhysicsManager.World.RayCast(
                    (fixture, point, normal, fraction) => {
                        if (fixture.CollisionCategories.HasFlag((Category)CollisionType.Wall))
                        {
                            Engine.Debug.Screen.AddCross((pos + posNext) * 0.5f, 10);
                            Engine.Debug.Screen.AddCross(ConvertUnits.ToDisplayUnits(fixture.Body.Position), 20);
                            rayTest = false;
                            return 0;
                        }
                        return -1;
                    },
                    ConvertUnits.ToSimUnits(pos),
                    ConvertUnits.ToSimUnits(posNext));

                if (!rayTest)
                    break;

                angleAcc += angleStep;
            }

            if (Math.Abs(angleAcc) > Math.Abs(angleDistance))
                angleAcc = angleDistance;

            m_ballAngle += angleAcc;
            ball.Owner.Position = Owner.Position + new Vector2((float)Math.Cos(m_ballAngle), (float)Math.Sin(m_ballAngle)) * (Parameters.Radius + ball.Parameters.Radius + 1);
        }


        private void UpdatePlayerOrientation(float desiredAngle)
        {
            float angleDistance = LBE.MathHelper.AngleDistanceSigned(m_ballAngle, desiredAngle);

            m_ballAngle += angleDistance;
            //ball.Owner.Position = Owner.Position + new Vector2((float)Math.Cos(m_ballAngle), (float)Math.Sin(m_ballAngle)) * (Parameters.Radius + ball.Parameters.Radius + 1);
        }



        public override void UpdatePrePhysics()
        {
            Move(m_moveDirection);

            float minVelocity = 2.0f;
            if (m_bodyCmp.Body.LinearVelocity.Length() < minVelocity)
                m_bodyCmp.Body.LinearVelocity = Vector2.Zero;

            float minAimVelocity = 6.0f;
            if (m_aimDirection != Vector2.Zero)
            {
                float aimAngle = m_aimDirection.Angle();
                if (m_ball != null)
                    UpdateBallAngle(aimAngle);
                else
                    UpdatePlayerAngle(aimAngle);
            }
            else if (m_bodyCmp.Body.LinearVelocity.Length() > minAimVelocity)
            {
                float aimAngle = m_bodyCmp.Body.LinearVelocity.Angle();
                if (m_ball != null)
                    UpdateBallAngle(aimAngle);
                else
                    UpdatePlayerAngle(aimAngle);
            }
        }

        public override void UpdatePostPhysics()
        {
            if (m_ball != null)
            {
                UpdateBallPosition(m_ball, m_ballAngle);
            }
            else
            {
                UpdatePlayerOrientation(m_ballAngle);
            }
        }

        #endregion Update

        #region Actions

        float ComputeSkillCoef(String skill, float baseCoef, float increment)
        {
            int skillLevel = m_playerInfo.PlayerSkill.Get(skill);
            return baseCoef + skillLevel * increment;
        }

        void Move(Vector2 direction)
        {
            float speedMod = 1.0f;
            if (m_ball != null)
                speedMod *= Parameters.Controls.SpeedWithBallMod;
            if (Properties.Stunned)
                speedMod *= 0;
            if (m_chargingShot)
                speedMod *= Parameters.ChargedShot.PlayerSpeedMod;
            if (m_chargingPass)
                speedMod *= Parameters.ChargedPass.PlayerSpeedMod;

            int speedLevel = m_playerInfo.PlayerSkill.Get("Speed"); speedLevel = 3;
            float skillMod = Parameters.Skills.SpeedBase + speedLevel * Parameters.Skills.SpeedCoef;
            speedMod *= skillMod;

            m_bodyCmp.Body.ApplyForce(Parameters.Controls.Speed * speedMod * direction);
        }

        public void Tackle(Vector2 direction)
        {
            if (Engine.GameTime.TimeMS < (Properties.LastTackleTimeMS + Parameters.Tackle.CooldownMS))
                return;

            Vector2 tackleDir = m_bodyCmp.Body.LinearVelocity;
            if (direction != Vector2.Zero)
                tackleDir = direction;

            if (tackleDir != Vector2.Zero)
                tackleDir.Normalize();

            PlayerTackleEffect tackleEffect = new PlayerTackleEffect();
            tackleEffect.SetDuration(Parameters.Tackle.DurationMS);
            tackleEffect.Direction = tackleDir;
            tackleEffect.Strength = Parameters.Tackle.Speed;
            AddEffect(tackleEffect);
        }

        public void StartBlink(Vector2 direction)
        {
            if (m_playerInfo.PlayerSkill.Has("Blink"))
                m_blink.Do();
        }

        public void StartChargingShot()
        {
            ShakeComponent playerShake = Owner.FindComponent<ShakeComponent>();
            playerShake.StartShake();

            Color color = Ball.LastPlayer.Team.ColorScheme.Color1;
            PlayerChargedShotFx playerChargedShotCmp = new PlayerChargedShotFx();
            Owner.Attach(playerChargedShotCmp);
            playerChargedShotCmp.Color = color;

            m_chargedShotTimer.Reset();
            m_chargedShotTimer.Start();
            m_chargingShot = true;

            m_audioCmpCharge.Play();
        }

        public void StopChargingShot()
        {
            ShakeComponent playerShake = Owner.FindComponent<ShakeComponent>();
            if (playerShake != null) playerShake.StopShake();

            PlayerChargedShotFx playerChargedShotCmp = Owner.FindComponent<PlayerChargedShotFx>();
            if (playerChargedShotCmp != null)
                playerChargedShotCmp.Stop();

            m_chargedShotTimer.Stop();
            m_chargingShot = false;

            m_audioCmpCharge.Stop();
        }

        public void StartChargingPass()
        {
            PlayerChargedPassFx playerChargedPassCmp = new PlayerChargedPassFx();
            Owner.Attach(playerChargedPassCmp);

            m_chargingPass = true;
        }

        public void StopChargingPass()
        {
            PlayerChargedPassFx playerChargedPassCmp = Owner.FindComponent<PlayerChargedPassFx>();
            if (playerChargedPassCmp != null)
                playerChargedPassCmp.Stop();

            m_chargingPass = false;
        }

        public void Pass(Player target)
        {
            Pass(target, new Vector2((float)Math.Cos(m_ballAngle), (float)Math.Sin(m_ballAngle)));
        }

        public void Pass(Player target, Vector2 direction)
        {
            Ball ball = m_ball;

            float impulse = Parameters.ShotImpulse;
            //Use base shot impulse for pass? //impulse *= ComputeSkillCoef("ChargedShotPower", Parameters.Skills.ChargedShotPowerBase, Parameters.Skills.ChargedShotPowerCoef);
            DettachBall(impulse * direction);

            PassAssistEffect passEffect = new PassAssistEffect(target, this);
            passEffect.Strength = ComputeSkillCoef("PassCurve", Parameters.Skills.PassCurveBase, Parameters.Skills.PassCurveCoef);
            ball.AddEffect(passEffect);

            m_audioCmpBallShot.Play();

            Engine.World.EventManager.ThrowEvent((int)EventId.PlayerPassAssist, this);
        }

        public void ShootBallCharged()
        {
            ShootBallCharged(new Vector2((float)Math.Cos(m_ballAngle), (float)Math.Sin(m_ballAngle)));
        }

        public void ShootBallCharged(Vector2 direction)
        {
            ShootBallChargedAfterDelay();
            return;
        }

        public void ShootBallChargedAfterDelay()
        {
            Ball ball = m_ball;
            if (ball == null)
                return;

            var direction = new Vector2((float)Math.Cos(m_ballAngle), (float)Math.Sin(m_ballAngle));

            float impulse = Parameters.ChargedShot.ShotImpulse;
            impulse *= ComputeSkillCoef("ChargedShotPower", Parameters.Skills.ChargedShotPowerBase, Parameters.Skills.ChargedShotPowerCoef);
            DettachBall(impulse * direction);

            BashEffect bashEffect = new BashEffect();
            bashEffect.SetDuration(Parameters.ChargedShot.BallBashTimeMS);
            ball.AddEffect(bashEffect);

            TargetEffect targetEffect = new TargetEffect(this);
            targetEffect.SetDuration(Parameters.ChargedShot.BallBashTimeMS);
            targetEffect.Strength = ComputeSkillCoef("ChargedShotCurve", Parameters.Skills.ChargedShotCurveBase, Parameters.Skills.ChargedShotCurveCoef);
            ball.AddEffect(targetEffect);

            ScreenShake.Add(3, direction, 150);
            m_audioCmpBallShot.Play();

            Engine.World.EventManager.ThrowEvent((int)EventId.PlayerShootBallCharged, this);
        }

        public void ShootBall()
        {
            ShootBall(new Vector2((float)Math.Cos(m_ballAngle), (float)Math.Sin(m_ballAngle)));
        }

        public void ShootBall(Vector2 direction)
        {
            Ball ball = m_ball;

            float impulse = Parameters.ShotImpulse;
            impulse *= ComputeSkillCoef("ShotPower", Parameters.Skills.ShotPowerBase, Parameters.Skills.ShotPowerCoef);
            DettachBall(impulse * direction);

            m_audioCmpBallShot.Play();

            Engine.World.EventManager.ThrowEvent((int)EventId.PlayerShootBall, this);
        }

        void UpdateBallAngle(float ballTargetAngle)
        {
            UpdateBallAngle(ballTargetAngle, Parameters.Controls.BallSpeed);
        }

        public void UpdateBallAngleInstant(float ballTargetAngle)
        {
            UpdateBallAngle(ballTargetAngle, 10000f);
        }

        void UpdateBallAngle(float ballTargetAngle, float ballSpeed)
        {
            float angleError = LBE.MathHelper.NormalizeAngle(ballTargetAngle - m_ballAngle);
            float angleUpdateMax = Engine.GameTime.ElapsedMS * ballSpeed * 0.001f;
            float angleUpdate = Math.Sign(angleError) * Math.Min(angleUpdateMax, Math.Abs(angleError));
            UpdateBallPosition(m_ball, m_ballAngle + angleUpdate);
        }

        void UpdatePlayerAngle(float playerTargetAngle)
        {
            UpdatePlayerAngle(playerTargetAngle, Parameters.Controls.BallSpeed);
        }

        void UpdatePlayerAngle(float playerTargetAngle, float playerRotationSpeed)
        {
            float angleError = LBE.MathHelper.NormalizeAngle(playerTargetAngle - m_ballAngle);
            float angleUpdateMax = Engine.GameTime.ElapsedMS * playerRotationSpeed * 0.001f;
            float angleUpdate = Math.Sign(angleError) * Math.Min(angleUpdateMax, Math.Abs(angleError));
            UpdatePlayerOrientation(m_ballAngle + angleUpdate);
        }

        public void Bash(Vector2 dir)
        {
            PlayerBashEffect bashEffect = new PlayerBashEffect();
            bashEffect.SetDuration(80);
            bashEffect.Direction = dir;
            bashEffect.Strength = Parameters.ChargedShot.BallBashImpulse;
            AddEffect(bashEffect);
        }

        public void Stun()
        {
            Stun(Parameters.StunTimeMSDefault,
               Parameters.StunSpeedModDefault,
               Parameters.StunBallImpulseDefault);
        }

        public void Stun(float stunTimeMS, float stunSpeedMod, float ballImpulse)
        {
            if (m_properties.Invincible)
                return;

            if (m_properties.Stunned)
                return;

            PlayerStunEffect stunEffect = new PlayerStunEffect();
            stunEffect.SetDuration(stunTimeMS);
            stunEffect.Parameters = new StunParamters() { BallImpulse = ballImpulse };

            if (m_ball != null)
            {
                SlowEffect slowEffect = new SlowEffect();
                slowEffect.SetDuration(80);
                m_ball.AddEffect(slowEffect);
            }

            AddEffect(stunEffect);
        }

        public void AttachBall(Ball ball)
        {
            if (Game.ConfigParameters.AutoFocus)
                Game.GameManager.Camera.FocusAuto();
            else
                Game.GameManager.Camera.Focus(this.Owner);

            Vector2 ballPosOld = ball.Position;

            Vector2 ballDir = ball.BodyCmp.Body.LinearVelocity;
            float ballAngle = 0;
            if (ballDir.LengthSquared() < 0.01f)
            {
                ballDir = ball.Position - Owner.Position;
                ballDir.Normalize();
                ballAngle = (float)Math.Atan2(ballDir.Y, ballDir.X);
            }
            else
            {
                ballDir.Normalize();

                //Find the position where the player cirlce intercept the ball line
                float playerRadius = Parameters.Radius + Parameters.BallSnapRadius;
                Vector2 relBallPos = ball.Position - Owner.Position; //ball position relative to player
                Vector2 dirToLine = new Vector2(-ballDir.Y, ballDir.X); //The direction from player to ball line is orthogonal to ball line
                if (Vector2.Dot(ball.Position - Owner.Position, dirToLine) < 0) dirToLine = -dirToLine; //correct sign if necessary
                float distToLine = Vector2.Dot(ball.Position - Owner.Position, dirToLine); // The distance from player to ball line

                //The intersection of the ball and the line
                Vector2 intersection = ball.Position;
                if (distToLine < 0)
                {
                    Engine.Log.Error("Math error in Player.AttachBall");
                }
                else if (distToLine > playerRadius)
                {
                    intersection = Owner.Position + playerRadius * dirToLine;
                }
                else
                {
                    float offset = (float)Math.Sqrt(playerRadius * playerRadius - distToLine * distToLine);
                    intersection = Owner.Position + distToLine * dirToLine - offset * ballDir;
                }

                Vector2 ballInterceptDir = intersection - Owner.Position;
                ballInterceptDir.Normalize();

                ballAngle = (float)Math.Atan2(ballInterceptDir.Y, ballInterceptDir.X);

            }

            CircleShape bodyShape = new CircleShape(ConvertUnits.ToSimUnits(ball.Parameters.Radius), 0.0f);
            bodyShape.Position = ConvertUnits.ToSimUnits((Parameters.Radius + ball.Parameters.Radius) * Vector2.UnitX);

            m_bodyCmp.OnCollision -= OnCollision;

            m_fixture = new Fixture(m_bodyCmp.Body, bodyShape);
            m_fixture.CollisionCategories = (Category)CollisionType.Ball;

            m_bodyCmp.Body.Rotation = m_ballAngle;
            Owner.Orientation = m_ballAngle;

            m_bodyCmp.Body.LocalCenter = Vector2.Zero;

            m_bodyCmp.OnCollision += new CollisionEvent(OnCollision);

            ResetMass();


            m_ballAngle = ballAngle;
            m_ball = ball;
            UpdateBallAngleInstant(ballAngle);

            ball.BodyCmp.Body.LinearVelocity = Vector2.Zero;
            ball.BodyCmp.Body.Enabled = false;

            m_properties.LastBallGrabTimeMS = Engine.GameTime.TimeMS;

            Engine.World.EventManager.ThrowEvent((int)EventId.PlayerReceiveBall, this);

            Vector2 ballPosNew = ball.Position;

            bool rayTest = true;
            var rayFrom = ConvertUnits.ToSimUnits(ballPosOld);
            var rayTo = ConvertUnits.ToSimUnits(ballPosNew);
            if (rayFrom != rayTo)
                Engine.PhysicsManager.World.RayCast(
                    (fixture, point, normal, fraction) => {
                        if (fixture.CollisionCategories.HasFlag((Category)CollisionType.Wall))
                        {
                            Engine.Debug.Screen.AddCross((ballPosOld + ballPosNew) * 0.5f, 10);
                            Engine.Debug.Screen.AddCross(ConvertUnits.ToDisplayUnits(fixture.Body.Position), 20);
                            rayTest = false;
                            return 0;
                        }
                        return -1;
                    }, rayFrom, rayTo);

            if (rayTest == false)
            {
                Engine.Log.Write("Ayayaya");
                Engine.Debug.Do(5000, (GameObject obj) => {
                    Engine.Debug.Screen.ResetBrush();
                    Engine.Debug.Screen.AddCross(ballPosOld, 6);
                    Engine.Debug.Screen.AddCross(ballPosNew, 6);
                    Engine.Debug.Screen.AddLine(ballPosOld, ballPosNew);
                });

                Owner.Position = Position - (ballPosNew - ballPosOld);
            }
        }

        public void DettachBall()
        {
            DettachBall(Vector2.Zero);
        }

        public void DettachBall(float impulse)
        {
            Vector2 dir = new Vector2((float)Math.Cos(m_ballAngle), (float)Math.Sin(m_ballAngle));
            DettachBall(impulse * dir);
        }

        public void DettachBall(Vector2 impulseVector)
        {
            if (m_ball == null)
                return;

            if (Game.ConfigParameters.AutoFocus)
                Game.GameManager.Camera.FocusAuto();
            else
                Game.GameManager.Camera.Focus(m_ball.Owner);

            if (m_fixture != null)
            {
                m_bodyCmp.Body.DestroyFixture(m_fixture);
                m_fixture = null;
            }

            m_ball.BodyCmp.Body.Enabled = true;
            ResetMass();

            m_ball.Dettach();
            m_ball.BodyCmp.Body.ApplyLinearImpulse(impulseVector);
            m_ball = null;

            PlayerDisableMagnetEffect effect = new PlayerDisableMagnetEffect();
            effect.SetDuration(Parameters.MagnetCooldownMS);
            AddEffect(effect);
        }

        #endregion Actions

        #region Collisions

        bool OnCollision(FarseerPhysics.Dynamics.Contacts.Contact contact, RigidBodyComponent self, RigidBodyComponent other)
        {
            object otherTag = "";
            other.UserData.TryGetValue("Tag", out otherTag);
            if ((string)otherTag == "Projectile")
            {
                Projectile projectile = (Projectile)other.Owner.FindComponent<Projectile>();
                return CollideWithProjectile(projectile);
            }
            else if ((string)otherTag == "Player")
            {
                Player otherPlayer = (Player)other.Owner.FindComponent<Player>();
                return CollideWithPlayer(otherPlayer);
            }
            else if ((string)otherTag == "Ball")
            {
                Ball ball = (Ball)other.Owner.FindComponent<Ball>();
                return CollideWithBall(ball);
            }

            return true;
        }

        private bool CollideWithBall(Ball ball)
        {
            if (Properties.MagnetDisabled || Properties.Shield)
                return true;

            return false;
        }

        private bool CollideWithPlayer(Player otherPlayer)
        {
            if (Properties.Tackling)
            {
                ScreenShake.Add(1.0f);
            }

            if (otherPlayer.Properties.Tackling && !Properties.Tackling)
            {
                Stun(otherPlayer.Parameters.Tackle.StunTimeMS,
                    otherPlayer.Parameters.Tackle.StunSpeedMod,
                    otherPlayer.Parameters.Tackle.Power);

                if (m_properties.Bashed)
                    return true;

                Vector2 otherPlayerToPlayerDir = Position - otherPlayer.Position;
                Vector2 otherPlayerDir = otherPlayer.m_bodyCmp.Body.LinearVelocity;

                if (otherPlayerDir.LengthSquared() < 0.01f)
                    return true;

                Vector2 otherPlayerDirNormalized = otherPlayerDir;
                otherPlayerDirNormalized.Normalize();

                float side = LBE.MathHelper.CrossProductSign(otherPlayerDirNormalized, otherPlayerToPlayerDir);
                float angleMin = 0.3f * (float)Math.PI;
                float angleMax = 0.5f * (float)Math.PI;
                float angleRandom = Engine.Random.NextFloat(angleMin, angleMax);
                float finalAngle = angleRandom * side;
                Vector2 bashDir = otherPlayerDirNormalized.Rotate(finalAngle);

                Bash(bashDir);

                return false;
            }
            return true;
        }

        private bool CollideWithProjectile(Projectile projectile)
        {
            if (Owner == projectile.Player.Owner)
                return false;

            Stun();
            return true;
        }

        public float OnMagnetRayCast(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            if (fixture != null)
            {
                if ((fixture.CollisionCategories & ((Category)CollisionType.Ball)) == (Category)CollisionType.Ball)
                {
                    m_magnetRayCastBallFraction = fraction;
                    return fraction;
                }

                bool magnetBlocker = false;
                magnetBlocker = magnetBlocker ||
                    ((fixture.CollisionCategories & ((Category)CollisionType.Wall)) == (Category)CollisionType.Wall);
                magnetBlocker = magnetBlocker
                    || ((fixture.CollisionCategories & ((Category)CollisionType.Player)) == (Category)CollisionType.Player);

                if (magnetBlocker)
                {
                    if (m_magnetRayCastShortestBlockerFraction == -1 || fraction < m_magnetRayCastShortestBlockerFraction)
                    {
                        m_magnetRayCastShortestBlockerFraction = fraction;
                    }
                }

                return 1.0f;
            }

            return 0;
        }

        #endregion Collisions

        #region Match Events

        public void OnMatchBegin(object eventParameter)
        {
            Properties.ControlDisabled.Set();
            Properties.MagnetDisabled.Set();

            m_controllerTxtCmp.Begin();
        }

        public void OnFirstPeriod(object eventParameter)
        {
            Properties.ControlDisabled.Unset();
            Properties.MagnetDisabled.Unset();
        }

        public void OnHalfTime(object eventParameter)
        {
            DettachBall();
            Properties.ControlDisabled.Set();
            Properties.MagnetDisabled.Set();
        }

        public void OnHalfTimeTransition(object eventParameter)
        {
            ResetState();
            Properties.ControlDisabled.Set();
            Properties.MagnetDisabled.Set();
        }

        public void OnSecondPeriod(object eventParameter)
        {
            Properties.ControlDisabled.Unset();
            Properties.MagnetDisabled.Unset();
        }

        public void OnMatchEnd(object eventParameter)
        {
            DettachBall();
            Properties.ControlDisabled.Set();
        }

        public void OnMatchVictory(object eventParameter)
        {
            if (Game.Arena.LeftGoal.Score == Game.Arena.RightGoal.Score)
            {

            }
            else
            {
                TeamId winnerTeam;
                if (Game.Arena.LeftGoal.Score > Game.Arena.RightGoal.Score)
                    winnerTeam = Game.Arena.RightGoal.Team.TeamID;
                else
                    winnerTeam = Game.Arena.LeftGoal.Team.TeamID;

                if (m_team.TeamID == winnerTeam)
                {
                    Asset<ColorSchemeData> colorSchemeBodiesAsset = Engine.AssetManager.GetAsset<ColorSchemeData>("Game/Colors.lua::ColorShemePlayerBodies");
                    PlayerGoldEffect goldEffect = new PlayerGoldEffect();
                    goldEffect.Parameters = new PlayerGoldParameters() { ColorScheme = colorSchemeBodiesAsset.Content.ColorSchemes[2] };

                    goldEffect.SetDuration(3000);

                    Timer effectDelayTimerMS = new Timer(Engine.GameTime.Source, 1000);
                    effectDelayTimerMS.OnTime += delegate (Timer source) { AddEffect(goldEffect); };

                    effectDelayTimerMS.Start();
                }
            }
        }

        public void OnKickOff(object eventParameter)
        {
            Team kickOffTeam = (Team)((object[])eventParameter)[0];
        }

        #endregion Match Events

        #region Player Effects

        public void AddEffect(PlayerEffect effect)
        {
            effect.Init(this);
            effect.Start();
            m_effects.Add(effect);
        }

        void UpdateEffects()
        {
            foreach (var effect in m_effects.ToArray())
            {
                effect.Update();

                if (!effect.Active)
                    RemoveEffect(effect);
            }
        }

        public void RemoveEffect(PlayerEffect effect)
        {
            effect.End();
            m_effects.Remove(effect);
        }

        #endregion PlayerEffects

        #region Player Debug

        private void DebugColorEdit()
        {
            KeyControl keyPlayer = new KeyControl(Keys.NumPad4);
            KeyControl keyTeam1 = new KeyControl(Keys.NumPad2);
            KeyControl keyTeam2 = new KeyControl(Keys.NumPad3);
            KeyControl keyShift1 = new KeyControl(Keys.LeftControl);
            KeyControl keyShift2 = new KeyControl(Keys.RightControl);
            if (keyPlayer.KeyPressed())
            {
                ColorMaskedSprite playerMaskedSprite = Owner.FindComponent<ColorMaskedSprite>();
                if (PlayerIndex == PlayerIndex.One) // hack to avoid several Color incrementations
                {
                    if (keyShift1.KeyDown() || keyShift2.KeyDown())
                        Colors.Previous();
                    else
                        Colors.Next();
                }

                playerMaskedSprite.Color1 = Colors.Current();

            }
            else if (keyTeam1.KeyPressed() && m_team.TeamID == TeamId.TeamOne
                    || (keyTeam2.KeyPressed() && m_team.TeamID == TeamId.TeamTwo))
            {
                ColorMaskedSprite playerMaskedSprite = Owner.FindComponent<ColorMaskedSprite>();

                if (keyShift1.KeyDown() || keyShift2.KeyDown())
                    Colors.Current();
                else
                    Colors.Current();

                m_team.ColorScheme.Color1 = Colors.Current();

                playerMaskedSprite.Color3 = Colors.Current();
            }
        }

        #endregion PlayerDebug

    }
}