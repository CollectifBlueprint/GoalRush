using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Menus;
using LBE;
using LBE.Graphics.Sprites;
using Ball.Gameplay;
using Ball.Graphics;

namespace Ball.MainMenu.Scripts
{
    public class PauseMenuScript : MenuScript
    {
        MatchStartInfo m_newMatchInfo;
        Timer m_matchRestartTimer;
        TimerEvent m_matchRestartTimerEvent;

        ScreenFade m_screenFade;

        public override void Start()
        {
            var lastMatchSession = Game.GameSession.MatchSessions.Last();

            m_newMatchInfo = new MatchStartInfo();
            m_newMatchInfo.Arena.Name = lastMatchSession.MatchInfo.Arena.Name;
            m_newMatchInfo.Arena.ColorScheme = lastMatchSession.MatchInfo.Arena.ColorScheme;
            m_newMatchInfo.Players = new PlayerInfo[4];
            m_newMatchInfo.Players[0] = lastMatchSession.MatchInfo.Players[0];
            m_newMatchInfo.Players[1] = lastMatchSession.MatchInfo.Players[1];
            m_newMatchInfo.Players[2] = lastMatchSession.MatchInfo.Players[2];
            m_newMatchInfo.Players[3] = lastMatchSession.MatchInfo.Players[3];
            m_newMatchInfo.Teams = new TeamInfo[2];
            m_newMatchInfo.Teams[0] = lastMatchSession.MatchInfo.Teams[0];
            m_newMatchInfo.Teams[1] = lastMatchSession.MatchInfo.Teams[1];

            m_screenFade = new ScreenFade();
            Menu.Owner.Attach(m_screenFade);

            m_screenFade.Opacity = 0.7f;
            m_screenFade.StartFade(ScreenFade.FadeType.FadeIn, 0, false);

            m_matchRestartTimer = new Timer(Engine.GameTime.Source, 200);
            m_matchRestartTimerEvent = new TimerEvent(m_matchRestartTimer_OnTime);
            m_matchRestartTimer.OnTime += m_matchRestartTimerEvent;

            //Game.GameMusic.Tracks["MatchStream"].Pause();
        }

        public override void OnItemValid(string name, MenuController controller)
        {
            base.OnItemValid(name, controller);

            if (name == "Continue")
            {
                //Game.GameMusic.Tracks["MatchStream"].Resume();
                Game.MenuManager.QuitMenu();
                Game.GameManager.UnpauseMatch();
            }

            if (name == "Restart")
            {
                //Game.GameMusic.Tracks["MatchStream"].Stop();
                Game.GameManager.EndMatch();
                m_matchRestartTimer.Start();
            }

            if (name == "Controls")
            {
                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/ControlsPauseMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
            }

            if (name == "MainMenu")
            {
                //Game.GameMusic.Tracks["MatchStream"].Stop();
                Game.GameManager.EndMatch();
                Game.GameSession.CurrentMatchInfo = new MatchStartInfo();
                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/MainMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
            }
        }

        public override void OnCancel(string name, MenuController controller)
        {
            base.OnCancel(name, controller);

            //Game.GameMusic.Tracks["MatchStream"].Resume();
            Game.MenuManager.QuitMenu();
            Game.GameManager.UnpauseMatch();
        }

        void m_matchRestartTimer_OnTime(Timer source)
        {
            Game.GameManager.StartMatch(m_newMatchInfo);
            Game.MenuManager.QuitMenu();
        }

        public override void End()
        {
            Menu.Owner.Remove(m_screenFade);

            m_matchRestartTimer.Stop();
            m_matchRestartTimer.OnTime -= m_matchRestartTimerEvent;
        }
    }
}
