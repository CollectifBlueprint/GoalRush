using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ball.Career
{
    public class PlayerSkillParameters
    {
        public float SpeedBase = 1.0f;
        public float SpeedCoef = 1.0f;

        public float ShotPowerBase = 1.0f;
        public float ShotPowerCoef = 1.0f;
        public float ChargedShotPowerBase = 1.0f;
        public float ChargedShotPowerCoef = 1.0f;
        public float ChargedShotTimeBase = 1.0f;
        public float ChargedShotTimeCoef = 1.0f;
        public float ChargedShotCurveBase = 1.0f;
        public float ChargedShotCurveCoef = 1.0f;

        public float PassCurveBase = 1.0f;
        public float PassCurveCoef = 1.0f;
    }
}
