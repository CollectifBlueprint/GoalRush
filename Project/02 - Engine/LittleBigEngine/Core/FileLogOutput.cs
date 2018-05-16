using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LBE.Log
{
    public class FileLogOutput : ILogOutput
    {
        FileStream m_logFile;
        StreamWriter m_logFileWriter;

        public FileLogOutput(String path)
        {
            m_logFile = File.Open(path, FileMode.Create);

            m_logFileWriter = new StreamWriter(m_logFile);
            m_logFileWriter.AutoFlush = true;
        }

        public void Write(string msg)
        {
            String indent = "".PadLeft(Engine.Log.IndentLevel * 2);
            m_logFileWriter.WriteLine(indent + msg);
        }

        public void Error(string msg)
        {
            String indent = "".PadLeft(Engine.Log.IndentLevel * 2);
            m_logFileWriter.WriteLine(indent + msg);
        }
    }
}
