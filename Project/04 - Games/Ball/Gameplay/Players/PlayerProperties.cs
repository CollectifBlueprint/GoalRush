using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ball.Gameplay
{
    public class PlayerProperties
    {
        public PropertyStack ControlDisabled = new PropertyStack();
        public PropertyStack MagnetDisabled = new PropertyStack();

        public PropertyStack Invincible = new PropertyStack();
        public PropertyStack Stunned = new PropertyStack();
        public PropertyStack Bashed = new PropertyStack();
        public PropertyStack Charging = new PropertyStack();
        public PropertyStack Tackling = new PropertyStack();
        public PropertyStack Shield = new PropertyStack();
        public PropertyStack Blink = new PropertyStack();

        public PropertyStack Shooting = new PropertyStack();

        public float LastTackleTimeMS = 0;
        public float LastBallGrabTimeMS = 0;
    }
}
