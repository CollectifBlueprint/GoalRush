using System;
using Microsoft.Xna.Framework;

namespace Ball.Gameplay
{
    public enum TeamId
    {
        TeamOne = 0,
        TeamTwo
    }

    public class Team
    {
        TeamId m_teamID;
        public TeamId TeamID
        {
            get { return m_teamID; }
            set { m_teamID = value; }
        }

        ColorScheme m_colorScheme;
        public ColorScheme ColorScheme
        {
            set { m_colorScheme = value; }
            get { return m_colorScheme; }
        }

        Player[] m_players;
        public Player[] Players
        {
            get { return m_players; }
            set { m_players = value; }
        }

        public Player GetTeamate(Player player)
        {
            if (m_players[0] == player)
                return m_players[1];

            if (m_players[1] == player)
                return m_players[0];

            throw new ArgumentException("player was not found in the team");
        }

        int m_consecutiveWins = 0;
        public int ConsecutiveWins
        {
            set { m_consecutiveWins = value; }
            get { return m_consecutiveWins; }
        }
    }
}