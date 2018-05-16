using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ball.Gameplay;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;

namespace Ball.Stats
{
    public class EventDataEntry
    {
        [XmlAttribute]
        public String Key;
        public Object Value;
    }

    public class EventEntry
    {
        [XmlAttribute]
        public EventType Type;

        [XmlElement("Data")]
        public List<EventDataEntry> Values;
    }

    public class StatManager
    {
        bool m_active;

        List<Event> m_events;
        public List<Event> Events
        {
            get { return m_events; }
        }

        String m_fileName;
        String m_folder;

        BackgroundWorker m_worker;

        public StatManager()
        {
            m_events = new List<Event>();

            m_worker = new BackgroundWorker();
            m_worker.WorkerSupportsCancellation = true;
            m_worker.DoWork += new DoWorkEventHandler(m_worker_DoWork);
        }

        void m_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            SaveLog();
            e.Cancel = true;
            m_worker.Dispose();

            m_worker = new BackgroundWorker();
            m_worker.WorkerSupportsCancellation = true;
            m_worker.DoWork += new DoWorkEventHandler(m_worker_DoWork);
        }

        public void LogEvent(Event e)
        {
            if (m_active)
            {
                e.Data["Time"] = Game.GameManager.Match.Time();
                m_events.Add(e);

                if (!m_worker.IsBusy)
                    m_worker.RunWorkerAsync();
            }
        }

        void StartLog(String filename, String folder)
        {
            m_active = true;
            m_fileName = filename;
            m_folder = folder;
        }

        void SaveLog()
        {
            List<EventEntry> m_entries = new List<EventEntry>();
            foreach (var e in m_events)
            {
                EventEntry entry = new EventEntry();
                entry.Type = e.Type;
                entry.Values = new List<EventDataEntry>();
                foreach (var key in e.Data.Keys)
                {
                    entry.Values.Add(new EventDataEntry() { Key = key, Value = e.Data[key] });
                }
                m_entries.Add(entry);
            }

            String basePath = "07 - Output/Stats/";
            String folderPath = basePath + "/" + m_folder;
            Directory.CreateDirectory(folderPath);

            FileStream fs = new FileStream(folderPath + "/" + m_fileName, FileMode.Create);
            XmlSerializer serializer =
                new XmlSerializer
                    (typeof(List<EventEntry>),
                    new Type[] { typeof(Vector2) });
            serializer.Serialize(fs, m_entries);
            fs.Flush();
            fs.Close();
        }

        void EndLog()
        {
            SaveLog();
            m_active = false;
        }

        public void MatchStart()
        {
            String arenaName = Game.Arena.Preview.Name;
            String date = DateTime.Now.ToString("yyyy-MM-dd");
            String time = DateTime.Now.ToString("HH.mm.ss");

            String fileName = String.Format("{0} - {1}.xml", time, arenaName);
            String folder = String.Format("{0}", date);

            StartLog(fileName, folder);

            Event e = new Event(EventType.MatchStart);
            e.Data["Arena"] = Game.Arena.Preview.Name;
            e.Data["Size"] = Game.Arena.Preview.Size;
            LogEvent(e);
        }

        public void MatchEnd()
        {
            Event e = new Event(EventType.MatchEnd);
            e.Data["Score - Red"] = Game.Arena.LeftGoal.Score;
            e.Data["Score - Blue"] = Game.Arena.RightGoal.Score;
            LogEvent(e);
        }

        public void Shoot(Player player, Vector2 direction, int chargeLevel)
        {
            Event e = new Event(EventType.Shoot);
            e.Data["Position"] = player.Position;
            e.Data["PlayerIndex"] = player.PlayerIndex;
            e.Data["Direction"] = direction;
            e.Data["Charge"] = chargeLevel;
            LogEvent(e);
        }
    }
}