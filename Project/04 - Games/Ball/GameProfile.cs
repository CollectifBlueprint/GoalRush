using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Ball.Career;
using System.IO.IsolatedStorage;
using System.Reflection;

namespace Ball
{
    public class GameOptions
    {
        public bool FullScreen;
        public bool VSync;

        public float MusicVolume;
        public float SfxVolume;
    }

    public class GameStats
    {
        public int GamePlayed;
        public float GameTimeMinutes;
    }

    public class GameProfile
    {
        public CareerProfile Career;
        public GameOptions Options;
        public GameStats Stats;

        public void CommitChanges()
        {
            Save(this);
        }

        public void ResetAll()
        {
            ResetProgression();
            ResetOptions();
            ResetStats();
        }

        public void ResetProgression()
        {
            Career = new CareerProfile();
        }

        public void ResetStats()
        {
            Stats = new GameStats()
            {
                GamePlayed = 0,
                GameTimeMinutes = 0,
            };
        }

        public void ResetOptions()
        {
            Options = new GameOptions()
            {
                FullScreen = true,
                VSync = true,
                SfxVolume = 1.0f,
                MusicVolume = 1.0f,
            };
        }

        public static GameProfile NewProfile()
        {
            GameProfile profile = new GameProfile();
            profile.ResetAll();
            return profile;
        }

        public static String GetSavePath()
        {
#if WINDOWS
            var myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(myDocsPath, "GoalRush/profile.xml");
#else
            var path = "./07 - Output/profile.xml";
            return path;
#endif
        }

        public static GameProfile Load()
        {
            String path = GetSavePath();

            if (File.Exists(path))
            {
                FileStream f = File.Open(path, FileMode.Open);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameProfile));
                GameProfile profile = (GameProfile)xmlSerializer.Deserialize(f);
                f.Close();

                return profile;
            }
            else
            {
                return null;
            }
        }

        public static void Save(GameProfile profile)
        {
            String path = GetSavePath();

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            FileStream f = File.Create(path);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameProfile));
            xmlSerializer.Serialize(f, profile);
            f.Close();
        }
    }
}
