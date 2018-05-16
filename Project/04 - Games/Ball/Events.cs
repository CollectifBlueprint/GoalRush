using LBE.Gameplay;

namespace Ball
{
    public enum EventId
    {
        //Game flow
        MatchBegin,       
        FirstPeriod,
        HalfTime,
        HalfTimeTransition,
        SecondPeriodBegin,
        SecondPeriod,
        MatchEnd,
        Victory,
        MatchFinalize,


        //Gameplay events
        PlayerShootBall,
        PlayerReceiveBall,
        PlayerLoseBall,
        PlayerTackled,
        PlayerDash,
        PlayerPassAssist,
        PlayerShootBallCharged,
        Goal,
        KickOff,
        LastSeconds,
        LauncherShot
    }
}