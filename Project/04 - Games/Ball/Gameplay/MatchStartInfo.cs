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
using Ball.Career;

namespace Ball.Gameplay
{
    public struct PlayerInfo
    {
        public PlayerIndex PlayerIndex;

        public InputType InputType;
        public PlayerIndex InputIndex;

        public PlayerSkill PlayerSkill;
    }

    public struct TeamInfo
    {
        public ColorScheme ColorScheme;
        public int ConsecutiveWins;
    }

    public struct ArenaInfo
    {
        public String Name;
        public ColorScheme ColorScheme;
    }


    public class MatchStartInfo
    {
        public ArenaInfo Arena;
        public TeamInfo[] Teams;
        public PlayerInfo[] Players;

        public MatchStartInfo()
        {
            Arena = new ArenaInfo();
            Arena.Name = "TutoArena";

            Teams = new TeamInfo[2];
            Teams[0] = new TeamInfo();
            Teams[1] = new TeamInfo();

            Players = new PlayerInfo[4];
            Players[0] = new PlayerInfo();
            Players[0].PlayerIndex = PlayerIndex.One;
            Players[0].InputType = InputType.AI;
            Players[0].PlayerSkill = CreateMultiplayerPlayerSkill();

            Players[1] = new PlayerInfo();
            Players[1].PlayerIndex = PlayerIndex.Two;
            Players[1].InputType = InputType.AI;
            Players[1].PlayerSkill = CreateMultiplayerPlayerSkill();

            Players[2] = new PlayerInfo();
            Players[2].PlayerIndex = PlayerIndex.Three;
            Players[2].InputType = InputType.AI;
            Players[2].PlayerSkill = CreateMultiplayerPlayerSkill();

            Players[3] = new PlayerInfo();
            Players[3].PlayerIndex = PlayerIndex.Four;
            Players[3].InputType = InputType.AI;
            Players[3].PlayerSkill = CreateMultiplayerPlayerSkill();
        }

        PlayerSkill CreateMultiplayerPlayerSkill()
        {
            PlayerSkill ps = new PlayerSkill();

            ps.Set("Tackle");
            ps.Set("TackleStun");
            ps.Set("Blink");

            ps.Set("Speed");
            ps.Set("Speed");

            ps.Set("ChargedShot");
            ps.Set("ChargedShotStun");
            ps.Set("ChargedShotInstant");

            ps.Set("ShotPower");
            ps.Set("ShotPower");
            ps.Set("ChargedShotPower");
            ps.Set("ChargedShotPower");
            ps.Set("ChargedShotTime");
            ps.Set("ChargedShotTime");
            ps.Set("ChargedShotCurve");
            ps.Set("ChargedShotCurve");

            ps.Set("Pass");

            ps.Set("PassCurve");
            ps.Set("PassCurve");

            return ps;
        }
    }
}
