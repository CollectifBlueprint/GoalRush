using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Menus;
using LBE;
using LBE.Graphics.Sprites;
using Ball.Gameplay;

namespace Ball.MainMenu.Scripts
{
    public class MatchEndScript : MenuScript
    {
        MatchStartInfo m_newMatchInfo;
        Timer m_matchRestartTimer;
        TimerEvent m_matchRestartTimerEvent;

        public override void Start()
        {
            var backgroundCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/Background.png"), "MenuBackground");
            Menu.Owner.Attach(backgroundCmp);

            Game.GameManager.EndMatch();

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
            m_newMatchInfo.Teams[0].ConsecutiveWins = Game.GameManager.Teams[0].ConsecutiveWins;
            m_newMatchInfo.Teams[1].ConsecutiveWins = Game.GameManager.Teams[1].ConsecutiveWins;

            m_matchRestartTimer = new Timer(Engine.GameTime.Source, 200);
            m_matchRestartTimerEvent = new TimerEvent(m_matchRestartTimer_OnTime);
            m_matchRestartTimer.OnTime += m_matchRestartTimerEvent;
        }

        public override void OnItemValid(string name, MenuController controller)
        {
            base.OnItemValid(name, controller);

            if (name == "Restart")
            {
                m_matchRestartTimer.Start();
            }

            if (name == "SelectArena")
            {
                Game.GameSession.CurrentMatchInfo = m_newMatchInfo;
                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/SelectMapMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
            }

            if (name == "SelectTeam")
            {
                Game.GameSession.CurrentMatchInfo = m_newMatchInfo;
                Game.GameSession.CurrentMatchInfo.Teams[0].ConsecutiveWins = 0;
                Game.GameSession.CurrentMatchInfo.Teams[1].ConsecutiveWins = 0;

                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/SelectTeamMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);

            }

            if (name == "MainMenu")
            {
                Game.GameSession.CurrentMatchInfo = new MatchStartInfo();
                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/MainMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
            }
        }

        void m_matchRestartTimer_OnTime(Timer source)
        {
            Game.GameManager.StartMatch(m_newMatchInfo);
            Game.MenuManager.QuitMenu();
        }

        public override void End()
        {
            base.End();

            m_matchRestartTimer.Stop();
            m_matchRestartTimer.OnTime -= m_matchRestartTimerEvent;

        }
    }
}
