using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;
using LBE.Assets;
using LBE.Core;
using LBE.Gameplay;
using LBE.Graphics;
using LBE.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ball.Gameplay;

namespace Ball
{
    public class MatchSession
    {
        public MatchStartInfo MatchInfo;
    }

    public class GameSession
    {
        public MatchStartInfo CurrentMatchInfo;
        public List<MatchSession> MatchSessions;
        Color[] TeamColors;

        public GameSession()
        {
            TeamColors = new Color[2];
            CurrentMatchInfo = new MatchStartInfo();
            MatchSessions = new List<MatchSession>(); 
        }

        public MatchSession StartMatch(MatchStartInfo matchInfo)
        {
            MatchSession session = new MatchSession();
            session.MatchInfo = matchInfo;

            CurrentMatchInfo = matchInfo;

            MatchSessions.Add(session);

            return session;
        }
    }
}
