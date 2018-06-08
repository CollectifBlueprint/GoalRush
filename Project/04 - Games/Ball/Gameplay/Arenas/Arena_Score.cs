using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using LBE;
using LBE.Assets;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using LBE.Physics;
using LBE.Script;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ball.Gameplay.Arenas
{
    public partial class Arena : GameObjectComponent
    {
        TextComponent m_leftScoreTextCmp;
        public TextComponent LeftScoreTxtCmp
        {
            get { return m_leftScoreTextCmp; }
        }

        TextComponent m_rightScoreTextCmp;
        public TextComponent RightScoreTextCmp
        {
            get { return m_rightScoreTextCmp; }
        }

        TextComponent m_scoreDashTextCmp;
        public TextComponent ScoreDashTextCmp
        {
            get { return m_scoreDashTextCmp; }
        }


        TextComponent m_leftTimerTextCmp;
        public TextComponent LeftTimerTextCmp
        {
            get { return m_leftTimerTextCmp; }
        }

        TextComponent m_rightTimerTextCmp;
        public TextComponent RightTimerTextCmp
        {
            get { return m_rightTimerTextCmp; }
        }
        
        TextComponent m_dashTimerTextCmp;
        public TextComponent DashTimerTextCmp
        {
            get { return m_dashTimerTextCmp; }
        }


        private void InitTimerAndScore()
        {
            m_leftScoreTextCmp = new TextComponent("GroundOverlay1");
            m_rightScoreTextCmp = new TextComponent("GroundOverlay1");
            m_scoreDashTextCmp = new TextComponent("GroundOverlay1");

            Owner.Attach(m_leftScoreTextCmp);
            Owner.Attach(m_rightScoreTextCmp);
            Owner.Attach(m_scoreDashTextCmp);

            m_leftTimerTextCmp = new TextComponent("GroundOverlay1");
            m_rightTimerTextCmp = new TextComponent("GroundOverlay1");
            m_dashTimerTextCmp = new TextComponent("GroundOverlay1");

            Owner.Attach(m_leftTimerTextCmp);
            Owner.Attach(m_rightTimerTextCmp);
            Owner.Attach(m_dashTimerTextCmp);
        }

        private void UpdateTimerAndScore()
        {
            float yScore = Engine.Debug.EditSingle("yScore", 300);
            float xScore = Engine.Debug.EditSingle("xScore", 10);


            Vector2 scoreTextPos = new Vector2(xScore, yScore);

            float fontScale = Engine.Debug.EditSingle("FontScale",1.0f);


            Color color = new Color(140, 140, 140);
            if (ColorScheme.Dark)
            {
                 color = new Color(240, 240, 240);
            }
          
            m_scoreDashTextCmp.Text = "-";
            m_scoreDashTextCmp.Alignement = TextAlignementHorizontal.Center;
            m_scoreDashTextCmp.Style = new TextStyle();
            m_scoreDashTextCmp.Style.Font = m_font;
            m_scoreDashTextCmp.Style.Scale = fontScale;
            m_scoreDashTextCmp.Style.Color = color;
            m_scoreDashTextCmp.Position = scoreTextPos;

            float extraOffset = Engine.Debug.EditSingle("ScoreLeftOffset", 10);
            Vector2 leftScoreOffset = -m_scoreDashTextCmp.Style.Font.MeasureString("-").ProjectX() * 0.5f * fontScale - Vector2.UnitX * extraOffset;
            String leftScore = "" + m_rightGoal.ScoreDisplay;
            m_leftScoreTextCmp.Text = leftScore.PadLeft(1, '0');
            m_leftScoreTextCmp.Alignement = TextAlignementHorizontal.Right;
            m_leftScoreTextCmp.Style = new TextStyle();
            m_leftScoreTextCmp.Style.Font = m_font;
            m_leftScoreTextCmp.Style.Color = m_leftGoal.Team.ColorScheme.Color1;
            m_leftScoreTextCmp.Style.Scale = fontScale;
            m_leftScoreTextCmp.Position = scoreTextPos + leftScoreOffset;

            String rightScore = "" + m_leftGoal.ScoreDisplay;
            m_rightScoreTextCmp.Text = rightScore.PadLeft(1, '0');
            m_rightScoreTextCmp.Alignement = TextAlignementHorizontal.Left;
            m_rightScoreTextCmp.Style = new TextStyle();
            m_rightScoreTextCmp.Style.Font = m_font;
            m_rightScoreTextCmp.Style.Scale = fontScale;
            m_rightScoreTextCmp.Style.Color = m_rightGoal.Team.ColorScheme.Color1;
            m_rightScoreTextCmp.Position = scoreTextPos - leftScoreOffset;

            float xTime = Engine.Debug.EditSingle("xTime", 0);
            float yTime = Engine.Debug.EditSingle("yTime", -300);

            Vector2 timeTextPos = new Vector2(xTime, yTime);

            int second = 0;
            int minutes = 0;

            if (Game.GameManager.Match.MatchState == MatchState.Begin
             || Game.GameManager.Match.MatchState == MatchState.SecondPeriodBegin)
            {
                int timer = (int)(Game.GameManager.Match.TimeLeft() / 1000) + 1;
                second = timer % 60;
                minutes = timer / 60;
            }

            if (Game.GameManager.Match.MatchState == MatchState.FirstPeriod
             || Game.GameManager.Match.MatchState == MatchState.SecondPeriod)
            {
                int timer = (int)(Game.GameManager.Match.TimeLeft() / 1000) + 1;
                second = timer % 60;
                minutes = timer / 60;
            }
           
            String secondStr = "" + second;
            secondStr = secondStr.PadLeft(2, '0');
            String minuteStr = "" + minutes;
            minuteStr = minuteStr.PadLeft(2, '0');


            m_dashTimerTextCmp.Text = ":";
            m_dashTimerTextCmp.Alignement = TextAlignementHorizontal.Center;
            m_dashTimerTextCmp.Style = new TextStyle();
            m_dashTimerTextCmp.Style.Font = m_font;
            m_dashTimerTextCmp.Style.Scale = fontScale;
			m_dashTimerTextCmp.Style.Color = color;
            m_dashTimerTextCmp.Position = timeTextPos;

            extraOffset = Engine.Debug.EditSingle("TimerLeftOffset", 0);
            Vector2 leftTimerOffset = -m_dashTimerTextCmp.Style.Font.MeasureString("-").ProjectX() * 0.5f * fontScale - Vector2.UnitX * extraOffset;
            m_leftTimerTextCmp.Text = minuteStr;
            m_leftTimerTextCmp.Alignement = TextAlignementHorizontal.Right;
            m_leftTimerTextCmp.Style = new TextStyle();
            m_leftTimerTextCmp.Style.Font = m_font;
            m_leftTimerTextCmp.Style.Scale = fontScale;
			m_leftTimerTextCmp.Style.Color = color;
            m_leftTimerTextCmp.Position = timeTextPos + leftTimerOffset;

            m_rightTimerTextCmp.Text = secondStr;
            m_rightTimerTextCmp.Alignement = TextAlignementHorizontal.Left;
            m_rightTimerTextCmp.Style = new TextStyle();
            m_rightTimerTextCmp.Style.Font = m_font;
            m_rightTimerTextCmp.Style.Scale = fontScale;
			m_rightTimerTextCmp.Style.Color = color;
			m_rightTimerTextCmp.Position = timeTextPos - leftTimerOffset;
        }


        private void SwitchTextScore()
        {
           // string textTmp = m_leftScoreTextCmp.Text;
           // m_leftScoreTextCmp.Text = m_rightScoreTextCmp.Text;
           // m_rightScoreTextCmp.Text = textTmp;


            //String leftScore = "" + m_leftGoal.ScoreDisplay;
            //m_leftScoreTextCmp.Text = leftScore.PadLeft(1, '0');

            //String rightScore = "" + m_rightGoal.ScoreDisplay;
            //m_rightScoreTextCmp.Text = rightScore.PadLeft(1, '0');
        }
    }
}
