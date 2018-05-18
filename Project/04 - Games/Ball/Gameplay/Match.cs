using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Assets;
using LBE.Core;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using LBE;
using Microsoft.Xna.Framework;
using Ball.Graphics;
using LBE.Audio;

namespace Ball.Gameplay
{
    public class MatchParameters
    {
        public float TimeS;
    }

    public enum MatchState
    {
        Init,
        Begin,
        FirstPeriod,
        HalfTime,
        SecondPeriodBegin,
        SecondPeriod,
        End,
    }

    public class Match : GameObjectComponent
    {
        Asset<MatchParameters> m_params;

        float m_halfTimeDuration;
        public float HalfTimeDuration {
            get { return m_halfTimeDuration; }
        }

        float m_matchBeginTimer;
        float[] m_halfTimePauseDuration;
        float[] m_endGamePauseDuration;

        float m_elapsedTime;
        public float ElapsedTime {
            get { return m_elapsedTime; }
        }

        float m_timerMS;
        public float TimerMS {
            get { return m_timerMS; }
        }

        float m_timerMSPrevious;
        float m_elapsedTimeMSPrevious;

        MatchState m_matchState;
        public MatchState MatchState {
            get { return m_matchState; }
        }

        int m_matchStateStep = 0;

        Timer m_goTextTimerMS;

        float m_tutorialDisplayDuration = 8000;
        SpriteComponent m_tutorialSprite;

        AudioComponent m_audioCmpTimerPeriodEnd;
        AudioComponent m_audioCmpTimerPeriodLastSeconds;


        public Match()
        {
        }

        public override void Start()
        {
            m_params = Engine.AssetManager.GetAsset<MatchParameters>("Game/Match.lua::Match");
            m_params.OnAssetChanged += new OnChange(m_params_OnAssetChanged);

            m_params_OnAssetChanged();

            m_elapsedTime = 0;
            m_matchBeginTimer = 3000;

            m_matchStateStep = 0;
            m_timerMS = m_params.Content.TimeS * 1000;
            m_timerMSPrevious = m_timerMS;

            //m_tutorialSprite = new SpriteComponent(Sprite.CreateFromTexture("Graphics/TutoPassAssist.png"), "ArenaUIOverlay0");
            //m_tutorialSprite.AttachedToOwner = false;
            //m_tutorialSprite.Visible = false;
            //Owner.Attach(m_tutorialSprite);

            m_audioCmpTimerPeriodLastSeconds = new AudioComponent("Audio/Sounds.lua::TimerPeriodLastSeconds");
            Owner.Attach(m_audioCmpTimerPeriodLastSeconds);
            m_audioCmpTimerPeriodEnd = new AudioComponent("Audio/Sounds.lua::TimerPeriodEnd");
            Owner.Attach(m_audioCmpTimerPeriodEnd);

            m_goTextTimerMS = new Timer(Engine.GameTime.Source, 1000);
            m_goTextTimerMS.OnTime += m_goTextTimerMS_OnTime;

            ChangeState(MatchState.Init);

        }

        void m_params_OnAssetChanged()
        {
            m_halfTimeDuration = m_params.Content.TimeS * 1000;
            m_halfTimePauseDuration = new float[] { 2000, 1000, 1000 };
            m_endGamePauseDuration = new float[] { 2000, 1000, 5000 };
        }


        void UpdateMatchSoundEvents()
        {
            if (m_matchState == MatchState.FirstPeriod || m_matchState == MatchState.SecondPeriod)
            {
                if ((m_elapsedTime >= (m_halfTimeDuration - 4000) && m_elapsedTimeMSPrevious < (m_halfTimeDuration - 4000))
                  || (m_elapsedTime >= (m_halfTimeDuration - 3000) && m_elapsedTimeMSPrevious < (m_halfTimeDuration - 3000))
                  || (m_elapsedTime >= (m_halfTimeDuration - 2000) && m_elapsedTimeMSPrevious < (m_halfTimeDuration - 2000))
                  || (m_elapsedTime >= (m_halfTimeDuration - 1000) && m_elapsedTimeMSPrevious < (m_halfTimeDuration - 1000)))
                {
                    m_audioCmpTimerPeriodLastSeconds.Play();

                    int remainingTimeS = (int)(m_halfTimeDuration - m_elapsedTimeMSPrevious) / 1000;
                    Engine.World.EventManager.ThrowEvent((int)EventId.LastSeconds, remainingTimeS);
                }
                else if (m_elapsedTime >= m_halfTimeDuration && m_elapsedTimeMSPrevious < m_halfTimeDuration)
                {
                    m_audioCmpTimerPeriodEnd.Play();
                }
            }
        }



        private void CheckForNewPlayers()
        {
            foreach (var ctrl in Game.MenuManager.Controllers)
            {
                if (ctrl.StartCtrl.KeyPressed())
                {
                    bool controllerIsAssigned = false;
                    foreach (var player in Game.GameManager.Players)
                    {
                        if (ctrl.InputIndex == player.PlayerInfo.InputIndex && ctrl.Type == player.PlayerInfo.InputType)
                        {
                            controllerIsAssigned = true;
                            break;
                        }
                    }


                    if (!controllerIsAssigned)
                    {
                        foreach (var player in Game.GameManager.Players)
                        {
                            if (player.Input == null)
                            {
                                var input = PlayerInput.CreateHuman(ctrl.Type, ctrl.InputIndex);
                                var controller = new Players.PlayerHumanController(player, input);
                                player.SetController(controller);
                                var newPlayerInfo = player.PlayerInfo;
                                newPlayerInfo.InputType = ctrl.Type;
                                player.PlayerInfo = newPlayerInfo;
                                Game.GameSession.CurrentMatchInfo.Players[(int)player.PlayerIndex] = newPlayerInfo;
                                player.ControllerTxtCmp.Begin();
                                break;
                            }
                        }
                    }
                }
            }

        }

        public override void Update()
        {
            CheckForNewPlayers();

            m_timerMSPrevious = m_timerMS;
            m_elapsedTimeMSPrevious = m_elapsedTime;

            m_elapsedTime += Engine.GameTime.ElapsedMS;

            if (Game.StunfestData.IsIdleAIRunning == false)
                m_timerMS -= Engine.GameTime.ElapsedMS;

            UpdateMatchSoundEvents();

            if (m_matchState == MatchState.Init)
            {
                if (m_elapsedTime > 0)
                {
                    m_elapsedTime = 0;
                    ChangeState(MatchState.Begin);
                }
            }

            if (m_matchState == MatchState.Begin)
            {
                if (m_elapsedTime >= m_matchBeginTimer)
                {
                    m_elapsedTime = 0;
                    ChangeState(MatchState.FirstPeriod);
                    Game.Stats.MatchStart();
                }

            }

            if (m_matchState == MatchState.FirstPeriod)
            {
                if (m_elapsedTime >= m_halfTimeDuration)
                {
                    m_elapsedTime = 0;
                    ChangeState(MatchState.HalfTime);
                }
            }
            else if (m_matchState == MatchState.HalfTime)
            {
                Ball ball = Game.GameManager.Ball;

                if (m_matchStateStep == 0 && m_elapsedTime >= m_halfTimePauseDuration[0])
                {
                    ball.BodyCmp.Body.LinearDamping += 0.005f * Engine.GameTime.ElapsedMS;

                    if (ball.Player != null || !ball.BodyCmp.Body.Awake || ball.BodyCmp.Body.LinearVelocity.LengthSquared() < 0.01f)
                    {
                        m_elapsedTime = 0;
                        m_matchStateStep = 1;

                        ball.BodyCmp.Body.LinearDamping = ball.Parameters.Physic.LinearDamping;

                        ScreenFade screenFade = new ScreenFade();
                        Owner.Attach(screenFade);
                        screenFade.StartFade(ScreenFade.FadeType.FadeOut, 500, false);
                    }
                }
                else if (m_matchStateStep == 1 && m_elapsedTime >= m_halfTimePauseDuration[1])
                {
                    m_elapsedTime = 0;
                    m_matchStateStep = 4; ;

                    Engine.World.EventManager.ThrowEvent((int)EventId.HalfTimeTransition);

                    if (Game.GameManager.Tutorial.Enabled)
                    {
                        m_matchStateStep = 2;
                        m_tutorialSprite.Visible = true;
                    }
                    else
                    {
                        m_matchStateStep = 4;
                        ScreenFade screenFade = Owner.FindComponent<ScreenFade>();
                        screenFade.StartFade(ScreenFade.FadeType.FadeIn, 500, true);
                    }
                }

                else if (m_matchStateStep == 2 && m_elapsedTime >= m_tutorialDisplayDuration)
                {
                    m_elapsedTime = 0;
                    m_matchStateStep = 4;

                    m_tutorialSprite.Visible = false;
                    ScreenFade screenFade = Owner.FindComponent<ScreenFade>();
                    screenFade.StartFade(ScreenFade.FadeType.FadeIn, 1000, true);
                }

                else if (m_matchStateStep == 4 && m_elapsedTime >= m_halfTimePauseDuration[2])
                {
                    ChangeState(MatchState.SecondPeriodBegin);

                }
            }
            else if (m_matchState == MatchState.SecondPeriodBegin)
            {
                if (m_elapsedTime >= m_matchBeginTimer)
                {
                    m_elapsedTime = 0;
                    ChangeState(MatchState.SecondPeriod);
                }
            }
            else if (m_matchState == MatchState.SecondPeriod)
            {
                if (m_elapsedTime >= m_halfTimeDuration)
                {
                    m_elapsedTime = 0;
                    ChangeState(MatchState.End);
                }
            }
            else if (m_matchState == MatchState.End)
            {
                Ball ball = Game.GameManager.Ball;

                if (m_matchStateStep == 0 && m_elapsedTime >= m_endGamePauseDuration[0])
                {
                    ball.BodyCmp.Body.LinearDamping += 0.005f * Engine.GameTime.ElapsedMS;

                    if (ball.Player != null || !ball.BodyCmp.Body.Awake || ball.BodyCmp.Body.LinearVelocity.LengthSquared() < 0.01f)
                    {
                        m_elapsedTime = 0;
                        m_matchStateStep++;
                        ball.BodyCmp.Body.LinearDamping = ball.Parameters.Physic.LinearDamping;

                        ScreenFade screenFade = new ScreenFade();
                        Owner.Attach(screenFade);
                        screenFade.StartFade(ScreenFade.FadeType.FadeOut, 500, false);
                    }
                }
                else if (m_matchStateStep == 1 && m_elapsedTime >= m_endGamePauseDuration[1])
                {
                    m_elapsedTime = 0;
                    m_matchStateStep++;
                    Engine.World.EventManager.ThrowEvent((int)EventId.Victory);

                    ScreenFade screenFade = Owner.FindComponent<ScreenFade>();
                    screenFade.StartFade(ScreenFade.FadeType.FadeIn, 500, true);
                }
                else if (m_matchStateStep == 2 && m_elapsedTime >= m_endGamePauseDuration[2])
                {
                    m_elapsedTime = 0;
                    m_matchStateStep++;
                    Engine.World.EventManager.ThrowEvent((int)EventId.MatchFinalize);

                    Game.Stats.MatchEnd();
                }
            }
        }

        public void ChangeState(MatchState matchState)
        {
            m_matchState = matchState;
            m_matchStateStep = 0;

            m_elapsedTime = 0;

            switch (m_matchState)
            {
                case MatchState.Begin:
                    Engine.World.EventManager.ThrowEvent((int)EventId.MatchBegin);
                    break;
                case MatchState.FirstPeriod:
                    m_goTextTimerMS.Start();
                    Engine.World.EventManager.ThrowEvent((int)EventId.FirstPeriod);
                    break;
                case MatchState.HalfTime:
                    ;
                    m_timerMS = m_halfTimePauseDuration[0];
                    Engine.World.EventManager.ThrowEvent((int)EventId.HalfTime);
                    break;
                case MatchState.SecondPeriodBegin:
                    Engine.World.EventManager.ThrowEvent((int)EventId.SecondPeriodBegin);
                    break;
                case MatchState.SecondPeriod:
                    m_goTextTimerMS.Start();
                    m_timerMS = m_params.Content.TimeS * 1000;
                    Engine.World.EventManager.ThrowEvent((int)EventId.SecondPeriod);
                    break;
                case MatchState.End:
                    m_timerMS = m_endGamePauseDuration[0];
                    Engine.World.EventManager.ThrowEvent((int)EventId.MatchEnd);
                    break;
            }
        }

        public float Time()
        {
            if (m_matchState == Gameplay.MatchState.Begin)
                return 0;

            if (m_matchState == Gameplay.MatchState.HalfTime)
                return m_halfTimeDuration;

            if (m_matchState == Gameplay.MatchState.SecondPeriodBegin)
                return m_halfTimeDuration;

            if (m_matchState == Gameplay.MatchState.FirstPeriod)
                return m_elapsedTime;

            if (m_matchState == Gameplay.MatchState.SecondPeriod)
                return m_halfTimeDuration + m_elapsedTime;

            return m_halfTimeDuration * 2;
        }

        public float TimeLeft()
        {
            if (m_matchState == Gameplay.MatchState.Begin)
                return m_halfTimeDuration;

            if (m_matchState == Gameplay.MatchState.HalfTime)
                return m_halfTimeDuration;

            if (m_matchState == Gameplay.MatchState.SecondPeriodBegin)
                return m_halfTimeDuration;

            if (m_matchState == Gameplay.MatchState.FirstPeriod)
                return m_halfTimeDuration - m_elapsedTime;

            if (m_matchState == Gameplay.MatchState.SecondPeriod)
                return m_halfTimeDuration - m_elapsedTime;

            return 0;
        }


        //
        void m_goTextTimerMS_OnTime(Timer source)
        {
        }
    }
}
