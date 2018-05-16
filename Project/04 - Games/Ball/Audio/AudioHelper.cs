using LBE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ball.Audio
{
    public static class AudioHelper
    {
        public static float LinearVolumeTodBVolume(float linearVol)
        {
            if (linearVol == 0)
                return 0;
            
            //linear inter : 0 --> -60   1--> 0
            float dBVol = MathHelper.Lerp(-60, 0, linearVol);
            //Engine.Log.Write("linearVol " + linearVol + " | dBVol " + dBVol);

            return (float)Math.Pow(10, (dBVol) / 20.0f);
        }
    }
}
