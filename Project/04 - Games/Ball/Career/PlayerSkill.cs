using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE;

namespace Ball.Career
{
    class Skill
    {
        public int Value = 0;
        public int Max = 1;

        public Skill(int max = 1)
        {
            Max = max;
        }
    }

    public class PlayerSkill
    {
        Dictionary<String, Skill> Skills;

        public PlayerSkill()
        {
            Skills = new Dictionary<String, Skill>();

            //Agility
            Skills["Tackle"] =              new Skill();
            Skills["TackleStun"] =          new Skill();
            Skills["Blink"] =               new Skill();
            Skills["Speed"] =               new Skill(3);//

            //Power
            Skills["ChargedShot"] =         new Skill();
            Skills["ChargedShotStun"] =     new Skill();
            Skills["ChargedShotInstant"] =  new Skill();
            Skills["ShotPower"] =           new Skill(3);//
            Skills["ChargedShotPower"] =    new Skill(3);//
            Skills["ChargedShotTime"] =     new Skill(3);//
            Skills["ChargedShotCurve"] =    new Skill(3);//

            //Pass
            Skills["Pass"] =                new Skill();
            Skills["PassCurve"] =           new Skill(3);//
        }

        public void Set(String key)
        {
            if (Skills.Keys.Contains(key))
                Skills[key].Value += 1;
            else
                Engine.Log.Error("Couldn't find skill '" + key + "'");
        }

        public int Get(String key)
        {
            if (Skills.Keys.Contains(key))
                return Skills[key].Value;

            Engine.Log.Error("Couldn't find skill '" + key + "'");
            return 0;
        }

        public bool Has(String key)
        {
            return Get(key) > 0;
        }
    }
}