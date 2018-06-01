using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Graphics.Sprites;
using LBE.Gameplay;
using LBE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ball.Gameplay.Fx;
using LBE.Core;
using Ball.Graphics;

namespace Ball.Gameplay 
{
    public class MatchFeedback : GameObjectComponent
    {
        TextComponent[] m_scoreTexts;
        Vector2[] m_scoreTextsSpacing;
        TextFx[] m_scoreTextFxs;
        TextFxParameters[] m_textFxParameters;

        Timer m_timerScoreTextMS;
        TimerEvent m_timerScoreTextEvent;

        TextComponent m_timeText;
        TextFx m_timeTextFx;

		TextFx m_MatchBeginTextFx;
		TextFx m_FirstPeriodTextFx;
		TextFx m_HalfTimeTextFx;
		TextFx m_MatchEndTextFx;
		TextFx m_VictoryTextFx;

        SpriteComponent m_cupSprite;
        SpriteFx m_cupSpriteFx;

        Timer m_timerCupSpriteMS;
        TimerEvent m_timerCupSpriteEvent;


        ColorMaskedSprite[] m_playerOffScreenMarkerSprites;
        public ColorMaskedSprite[] PlayerOffScreenMarkerSprites
        {
            get { return m_playerOffScreenMarkerSprites; }
        }

        
        public override void Start()
        {
            m_scoreTexts = new TextComponent[3];
            m_scoreTextFxs = new TextFx[3];
            m_textFxParameters = new TextFxParameters[3];
            for (int i = 0; i < m_textFxParameters.Length; i++)
            {
                m_textFxParameters[i] = new TextFxParameters();
            }

            m_scoreTextsSpacing = new Vector2[]
            {
                new Vector2(-150, 140),
                new Vector2(0, 140),
                new Vector2(150, 140)

            };

            Engine.World.EventManager.AddListener((int)EventId.Goal, OnGoal);
            Engine.World.EventManager.AddListener((int)EventId.LastSeconds, OnLastSeconds);
			Engine.World.EventManager.AddListener((int)EventId.Victory, OnVictory);
			Engine.World.EventManager.AddListener((int)EventId.MatchBegin, OnMatchBegin);
			Engine.World.EventManager.AddListener((int)EventId.FirstPeriod, OnFirstPeriod);
			Engine.World.EventManager.AddListener((int)EventId.SecondPeriodBegin, OnSecondPeriodBegin);
			Engine.World.EventManager.AddListener((int)EventId.SecondPeriod, OnSecondPeriod);
			Engine.World.EventManager.AddListener((int)EventId.HalfTime, OnHalfTime);
			Engine.World.EventManager.AddListener((int)EventId.MatchEnd, OnMatchEnd);
			Engine.World.EventManager.AddListener((int)EventId.Victory, OnVictory);

            m_timerScoreTextMS = new Timer(Engine.GameTime.Source, 0);
            m_timerScoreTextEvent = new TimerEvent(m_timerScoreTextMS_OnTime);
            m_timerScoreTextMS.OnTime += m_timerScoreTextEvent;

            m_timerCupSpriteMS = new Timer(Engine.GameTime.Source, 0);
            m_timerCupSpriteEvent = new TimerEvent(m_timerCupSpriteMS_OnTime);
            m_timerCupSpriteMS.OnTime += m_timerCupSpriteEvent;

            m_playerOffScreenMarkerSprites = new ColorMaskedSprite[4];
            for (int i = 0; i < m_playerOffScreenMarkerSprites.Length; i++)
            {
                Sprite sprite = Sprite.CreateFromTexture("Graphics/playerOffScreenMarkerAO.png");
                sprite.Scale = new Vector2(0.5f, 0.5f);
                m_playerOffScreenMarkerSprites[i] = new ColorMaskedSprite(sprite, "ArenaOverlay9");
                var texAsset = Engine.AssetManager.GetAsset<Texture2D>("Graphics/playerOffScreenMarkerAO.png");

                m_playerOffScreenMarkerSprites[i].Mask = texAsset.Content;
                m_playerOffScreenMarkerSprites[i].Color1 = Game.GameManager.Players[i].PlayerColors[2];
                m_playerOffScreenMarkerSprites[i].Color2 = Game.GameManager.Players[i].PlayerColors[2];
                m_playerOffScreenMarkerSprites[i].Color3 = Game.GameManager.Players[i].PlayerColors[2];
                m_playerOffScreenMarkerSprites[i].Visible = false;

                Owner.Attach(m_playerOffScreenMarkerSprites[i]);

                PlayerOffScreenMarker playerOffScreenMarker = Game.GameManager.Players[i].Owner.FindComponent<PlayerOffScreenMarker>();
                playerOffScreenMarker.Init();

            }
        }

        public override void Update()
        {
            for (int i = 0; i < m_scoreTexts.Length; i++)
            {
                if (m_scoreTextFxs[i] == null)
                    continue;

                m_scoreTextFxs[i].MoveStart = new Vector2(Game.GameManager.Camera.GetPosition().X + m_scoreTextsSpacing[i].X, Game.GameManager.Camera.GetPosition().Y + m_scoreTextsSpacing[i].Y);

                // tes memory leak?

            }

            if (m_timeTextFx != null)
            {
                m_timeTextFx.MoveStart = new Vector2(Game.GameManager.Camera.GetPosition().X, Game.GameManager.Camera.GetPosition().Y);
            }


            //Vector2 StartPos = new Vector2(Game.GameManager.Camera.GetPosition().X - 0, Game.GameManager.Camera.GetPosition().Y+ 0);
            //Engine.Debug.Screen.Brush.LineColor = Color.Turquoise;
            //Engine.Debug.Screen.AddLine(StartPos, Game.GameManager.Arena.RightScoreTextCmp.Position);

            //Engine.Debug.Screen.Brush.LineColor = Color.Red;
            //Engine.Debug.Screen.AddCross(Game.GameManager.Arena.RightScoreTextCmp.Position,10);
        }

        public void CreateFeedBackText(GameObject parent, ref TextComponent textCmp, ref TextFx textFx, string text, Color color, Vector2 offset, TextFxParameters textFxParams)
        {
            if (textCmp != null)
                textCmp.Owner.Remove(textCmp);

            textCmp = new TextComponent("ArenaUIOverlay0");

            SpriteFont font = Engine.AssetManager.Get<SpriteFont>("Graphics/GameplayFontLarge");
			float fontScale = 1.0f;

            textCmp.Text = text;
            textCmp.Alignement = TextAlignementHorizontal.Center;
            textCmp.Style = new TextStyle();
            textCmp.Style.Font = font;
			textCmp.Style.Scale = fontScale  * textFxParams.ScaleStart ;
            textCmp.Style.Color = color;

            textCmp.Position = offset;

            textFx = new TextFx(textCmp);

            textFx.SetParameters(textFxParams);
            parent.Attach(textCmp);

            textCmp.Owner.Attach(textFx);
            textFx.StartFx();
        }


        //
        public void CreateFeedBackSpriteCup(GameObject parent, ref SpriteComponent spriteCmp, ref SpriteFx spriteFx, Vector2 offset, SpriteFxParameters spriteFxParams)
        {
            if (spriteCmp != null)
                spriteCmp.Owner.Remove(spriteCmp);

            if (Game.Arena.LeftGoal.Score != Game.Arena.RightGoal.Score)
            {
                string animationName;
                
                if (Game.Arena.LeftGoal.Score > Game.Arena.RightGoal.Score)
                {
                    int cupId = Math.Min(Game.Arena.RightGoal.Team.ConsecutiveWins, 7);
                    animationName = "Cup" + cupId.ToString();
                }
                else
                {
                    int cupId = Math.Min(Game.Arena.LeftGoal.Team.ConsecutiveWins, 7);
                    animationName = "Cup" + cupId.ToString();
                }

                spriteCmp = new SpriteComponent(Sprite.Create("Graphics/cupSprite.lua::Sprite"), "ArenaUIOverlay0");
                spriteCmp.Sprite.Scale = new Vector2(0.7f, 0.7f);

                spriteCmp.Sprite.SetAnimation(animationName);
                spriteCmp.Sprite.Playing = true;

                spriteCmp.Position = offset;
                
                spriteFx = new SpriteFx(spriteCmp);

                spriteFx.SetParameters(spriteFxParams);
                parent.Attach(spriteCmp);

                spriteCmp.Owner.Attach(spriteFx);
                spriteFx.StartFx();
            }
        }

        //
        public void OnGoal(object arg)
        {
            if (!Enabled)
                return;

            for (int i = 0; i < m_textFxParameters.Length; i++)
            {
                m_textFxParameters[i] = new TextFxParameters
                {
                    TextEffectTimeMS = 1550,
                    ScaleStart = 1.00f,
                    ScaleEnd = 0.15f,
                    ScaleTimeMS = 300,
                    ScaleDelayMS = 1000,

                    FadeStart = 255,
                    FadeEnd = 0,
                    FadeTimeMS = 500,
                    FadeDelayMS = 1000,

                    MoveTimeMS = 500,
                    MoveDelayMS = 1000,
                };
            }

            m_textFxParameters[0].MoveValue = new Vector2(-Game.GameManager.Camera.GetPosition().X + Game.GameManager.Arena.RightScoreTextCmp.Position.X + 100,
                Game.GameManager.Camera.GetPosition().Y + Game.GameManager.Arena.RightScoreTextCmp.Position.Y - 144);

            m_textFxParameters[1].MoveValue = new Vector2(-Game.GameManager.Camera.GetPosition().X + Game.GameManager.Arena.ScoreDashTextCmp.Position.X,
                Game.GameManager.Camera.GetPosition().Y + Game.GameManager.Arena.ScoreDashTextCmp.Position.Y - 144);

            m_textFxParameters[2].MoveValue = new Vector2(-Game.GameManager.Camera.GetPosition().X + Game.GameManager.Arena.LeftScoreTxtCmp.Position.X - 100,
                Game.GameManager.Camera.GetPosition().Y + Game.GameManager.Arena.LeftScoreTxtCmp.Position.Y - 144);


            CreateFeedBackText(Owner, ref m_scoreTexts[0], ref m_scoreTextFxs[0],
                Game.GameManager.Arena.RightGoal.Score.ToString("00"),
                Game.GameManager.Arena.LeftGoal.Team.ColorScheme.Color1,
                new Vector2(Game.GameManager.Camera.GetPosition().X - 150, Game.GameManager.Camera.GetPosition().Y+ 140),
                m_textFxParameters[0]);

            CreateFeedBackText(Owner, ref m_scoreTexts[1], ref m_scoreTextFxs[1], " - ",
                Color.LightGray, new Vector2(Game.GameManager.Camera.GetPosition().X + 0, Game.GameManager.Camera.GetPosition().Y + 140),
                m_textFxParameters[1]);

            CreateFeedBackText(Owner, ref m_scoreTexts[2], ref m_scoreTextFxs[2],
                Game.GameManager.Arena.LeftGoal.Score.ToString("00"),
                Game.GameManager.Arena.RightGoal.Team.ColorScheme.Color1,
                new Vector2(Game.GameManager.Camera.GetPosition().X + 150, Game.GameManager.Camera.GetPosition().Y + 140),
                m_textFxParameters[2]);


            m_scoreTextFxs[0].MoveEnd = new Vector2(Game.GameManager.Arena.RightScoreTextCmp.Position.X, Game.GameManager.Arena.RightScoreTextCmp.Position.Y);
            m_scoreTextFxs[1].MoveEnd = new Vector2(Game.GameManager.Arena.ScoreDashTextCmp.Position.X, Game.GameManager.Arena.ScoreDashTextCmp.Position.Y);
            m_scoreTextFxs[2].MoveEnd = new Vector2(Game.GameManager.Arena.LeftScoreTxtCmp.Position.X, Game.GameManager.Arena.LeftScoreTxtCmp.Position.Y);

            m_timerScoreTextMS.TargetTime = m_textFxParameters[0].TextEffectTimeMS;
            m_timerScoreTextMS.Start();
                

        }

        public void OnLastSeconds(object arg)
        {
            if (!Enabled)
                return;

            int remainingTime = (int)((object[])arg)[0];

            TextFxParameters textFxParam = new TextFxParameters();
           
            textFxParam = new TextFxParameters
            {
                TextEffectTimeMS = 1000,
                ScaleStart = 1.0f,
                ScaleEnd = 0.25f,
                ScaleTimeMS = 300,
                ScaleDelayMS = 500,

                FadeStart = 255,
                FadeEnd = 0,
                FadeTimeMS = 300,
                FadeDelayMS = 500,

                MoveTimeMS = 300,
                MoveDelayMS = 500,
            };

            textFxParam.MoveValue = new Vector2(-Game.GameManager.Camera.GetPosition().X + Game.GameManager.Arena.RightTimerTextCmp.Position.X * 3,
                Game.GameManager.Camera.GetPosition().Y + Game.GameManager.Arena.RightTimerTextCmp.Position.Y);

            CreateFeedBackText(Owner, ref m_timeText, ref m_timeTextFx,
                remainingTime.ToString(),
                Color.LightGray,
                new Vector2(Game.GameManager.Camera.GetPosition().X, Game.GameManager.Camera.GetPosition().Y),
                textFxParam);

                m_timeTextFx.MoveEnd = new Vector2(Game.GameManager.Arena.RightTimerTextCmp.Position.X + 35, Game.GameManager.Arena.RightTimerTextCmp.Position.Y);
        }

		/*
        public void OnVictory(object arg)
        {
            if (!Enabled)
                return;

            SpriteFxParameters spriteFxParam = new SpriteFxParameters();

            spriteFxParam = new SpriteFxParameters
            {
                SpriteEffectTimeMS = 1600,
                ScaleStart = 0.7f,
                ScaleEnd = 0.15f,
                ScaleTimeMS = 400,
                ScaleDelayMS = 1000,

                FadeStart = 255,
                FadeEnd = 0,
                FadeTimeMS = 490,
                FadeDelayMS = 1000,

                MoveTimeMS = 500,
                MoveDelayMS = 1000,
            };

	


            if (Game.Arena.LeftGoal.Score != Game.Arena.RightGoal.Score)
            {
                //spriteFxParam.MoveValue = new Vector2(-Game.GameManager.Camera.GetPosition().X,
                //Game.GameManager.Camera.GetPosition().Y + Game.GameManager.Arena.CupSpriteRightCmp.Position.Y);
               
                //CreateFeedBackSpriteCup(Owner, ref m_cupSprite, ref m_cupSpriteFx,
                //    new Vector2(0, Game.GameManager.Camera.GetPosition().Y),
                //    spriteFxParam);


                if (m_cupSpriteFx != null)
                {
                    if (Game.Arena.LeftGoal.Score > Game.Arena.RightGoal.Score)
                    {
                        m_cupSpriteFx.MoveEnd = new Vector2(Game.GameManager.Arena.CupSpriteRightCmp.Position.X + 7, Game.GameManager.Arena.CupSpriteRightCmp.Position.Y);

                    }
                    else
                    {
                        m_cupSpriteFx.MoveEnd = new Vector2(Game.GameManager.Arena.CupSpriteLeftCmp.Position.X - 7, Game.GameManager.Arena.CupSpriteLeftCmp.Position.Y);
                    }
                }

                m_timerCupSpriteMS.TargetTime = spriteFxParam.SpriteEffectTimeMS;
                m_timerCupSpriteMS.Start();
            }

        }
		*/

		public void OnMatchBegin(object arg)
		{
			if (!Enabled)
				return;

			TextFxParameters textFxParam = new TextFxParameters();

			textFxParam = new TextFxParameters
			{
				TextEffectTimeMS = 3000,
				ScaleStart = 0.60f,
				ScaleEnd = 0.25f,
				ScaleTimeMS = 150,
				ScaleDelayMS = 0,

				FadeStart = 255,
				FadeEnd = 0,
				FadeTimeMS = 300,
				FadeDelayMS = 2000,

				MoveTimeMS = 500,
				MoveDelayMS = 2300,
			};

			textFxParam.MoveValue = new Vector2(Game.GameManager.Camera.GetPosition().X, Game.GameManager.Camera.GetPosition().Y);

			CreateFeedBackText(Owner, ref m_timeText, ref m_MatchBeginTextFx,
				"Goal...",
				Color.Orange,
				new Vector2(-50, 50),
				textFxParam);

			m_MatchBeginTextFx.MoveEnd = new Vector2(Game.GameManager.Camera.GetPosition().X,Game.GameManager.Camera.GetPosition().Y);
		}


		public void OnFirstPeriod(object arg)
		{
			if (!Enabled)
				return;

			TextFxParameters textFxParam = new TextFxParameters();

			textFxParam = new TextFxParameters
			{
				TextEffectTimeMS = 2000,
				ScaleStart = 0.60f,
				ScaleEnd = 0.35f,
				ScaleTimeMS = 100,
				ScaleDelayMS = 0,

				FadeStart = 255,
				FadeEnd = 0,
				FadeTimeMS = 300,
				FadeDelayMS = 1400,

				MoveTimeMS = 400,
				MoveDelayMS = 2150,
			};

			textFxParam.MoveValue = new Vector2(Game.GameManager.Camera.GetPosition().X, Game.GameManager.Camera.GetPosition().Y);
			//textFxParam.MoveValue = Vector2.Zero;

			CreateFeedBackText(Owner, ref m_timeText, ref m_FirstPeriodTextFx,
				"Rush!",
				Color.Orange,
				new Vector2(50,  40),
				textFxParam);

			m_FirstPeriodTextFx.MoveEnd = new Vector2(Game.GameManager.Camera.GetPosition().X,Game.GameManager.Camera.GetPosition().Y);
		}


		public void OnSecondPeriodBegin(object arg)
		{
			OnMatchBegin(arg);
		}

		public void OnSecondPeriod(object arg)
		{
			OnFirstPeriod(arg);
		}

		public void OnHalfTime(object arg)
		{
			if (!Enabled)
				return;

			TextFxParameters textFxParam = new TextFxParameters();

			textFxParam = new TextFxParameters
			{
				TextEffectTimeMS = 3500,
				ScaleStart = 0.60f,
				ScaleEnd = 0.35f,
				ScaleTimeMS = 100,
				ScaleDelayMS = 0,

				FadeStart = 255,
				FadeEnd = 0,
				FadeTimeMS = 300,
				FadeDelayMS = 1400,

				MoveTimeMS = 400,
				MoveDelayMS = 2950,
			};

			textFxParam.MoveValue = new Vector2(Game.GameManager.Camera.GetPosition().X, Game.GameManager.Camera.GetPosition().Y);

			CreateFeedBackText(Owner, ref m_timeText, ref m_HalfTimeTextFx,
				"HalfTime",
				Color.Orange,
				new Vector2(Game.GameManager.Camera.GetPosition().X,  Game.GameManager.Camera.GetPosition().Y),
				textFxParam);

			m_HalfTimeTextFx.MoveEnd = new Vector2(Game.GameManager.Camera.GetPosition().X,Game.GameManager.Camera.GetPosition().Y);
		}

		public void OnMatchEnd(object arg)
		{
			if (!Enabled)
				return;

			TextFxParameters textFxParam = new TextFxParameters();

			textFxParam = new TextFxParameters
			{
				TextEffectTimeMS = 3000,
				ScaleStart = 0.60f,
				ScaleEnd = 0.35f,
				ScaleTimeMS = 100,
				ScaleDelayMS = 0,

				FadeStart = 255,
				FadeEnd = 0,
				FadeTimeMS = 300,
				FadeDelayMS = 1900,

				MoveTimeMS = 400,
				MoveDelayMS = 2550,
			};

			textFxParam.MoveValue = new Vector2(Game.GameManager.Camera.GetPosition().X, Game.GameManager.Camera.GetPosition().Y);

			CreateFeedBackText(Owner, ref m_timeText, ref m_MatchEndTextFx,
				"Match Over",
				Color.Orange,
				new Vector2(Game.GameManager.Camera.GetPosition().X,  Game.GameManager.Camera.GetPosition().Y),
				textFxParam);

			m_MatchEndTextFx.MoveEnd = new Vector2(Game.GameManager.Camera.GetPosition().X,Game.GameManager.Camera.GetPosition().Y);
		}

		public void OnVictory(object arg)
		{
			if (!Enabled)
				return;

			string victoryText = "";

			if (Game.Arena.LeftGoal.Score != Game.Arena.RightGoal.Score)
			{
				victoryText = " Victory !";
			}
			else
			{
				victoryText = "Draw Game";
			}

			TextFxParameters textFxParam = new TextFxParameters();

			textFxParam = new TextFxParameters
			{
				TextEffectTimeMS = 4000,
				ScaleStart = 0.70f,
				ScaleEnd = 0.40f,
				ScaleTimeMS = 100,
				ScaleDelayMS = 0,

				FadeStart = 255,
				FadeEnd = 0,
				FadeTimeMS = 300,
				FadeDelayMS = 2900,

				MoveTimeMS = 400,
				MoveDelayMS = 3550,
			};

			textFxParam.MoveValue = new Vector2(Game.GameManager.Camera.GetPosition().X, Game.GameManager.Camera.GetPosition().Y);

			CreateFeedBackText(Owner, ref m_timeText, ref m_VictoryTextFx,
				victoryText,
				Color.Orange,
				new Vector2(50,  40),
				textFxParam);

			m_VictoryTextFx.MoveEnd = new Vector2(Game.GameManager.Camera.GetPosition().X,Game.GameManager.Camera.GetPosition().Y);
		}


        void m_timerScoreTextMS_OnTime(Timer source)
        {
            Game.GameManager.Arena.RightGoal.ScoreDisplay = Game.GameManager.Arena.RightGoal.Score;
            Game.GameManager.Arena.LeftGoal.ScoreDisplay = Game.GameManager.Arena.LeftGoal.Score;
        }

        void m_timerCupSpriteMS_OnTime(Timer source)
        {
            
            //if (Game.Arena.LeftGoal.Score != Game.Arena.RightGoal.Score)
            //{
            //    string animationName;

            //    if (Game.Arena.LeftGoal.Score > Game.Arena.RightGoal.Score)
            //    {
            //        int cupId = Math.Min(Game.Arena.RightGoal.Team.ConsecutiveWins, 7);
            //        animationName = "Cup" + cupId.ToString();

            //        Game.Arena.CupSpriteRightCmp.Sprite.SetAnimation(animationName);
            //        Game.Arena.CupSpriteRightCmp.Sprite.Playing = true;

            //        Game.Arena.CupSpriteLeftCmp.Sprite.SetAnimation("Cup0");
            //        Game.Arena.CupSpriteLeftCmp.Sprite.Playing = true;
            //    }
            //    else
            //    {
            //        int cupId = Math.Min(Game.Arena.LeftGoal.Team.ConsecutiveWins, 7);
            //        animationName = "Cup" + cupId.ToString();

            //        Game.Arena.CupSpriteLeftCmp.Sprite.SetAnimation(animationName);
            //        Game.Arena.CupSpriteLeftCmp.Sprite.Playing = true;

            //        Game.Arena.CupSpriteRightCmp.Sprite.SetAnimation("Cup0");
            //        Game.Arena.CupSpriteRightCmp.Sprite.Playing = true;
            //    }
            //}
        }

        public override void End()
        {
            base.End();
            m_timerScoreTextMS.Stop();
            m_timerScoreTextMS.OnTime -= m_timerScoreTextEvent;

            m_timerCupSpriteMS.Stop();
            m_timerCupSpriteMS.OnTime -= m_timerCupSpriteEvent;
        }
    }
}
