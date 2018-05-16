using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace LBE.Log
{
    public interface IEditor
    {
        void Edit(Object obj);
    }

    public enum ErrorResult
    {
        Succes,
        Abort,
        Retry,
        Ignore,
    }

    public class DebugCategory
    {
        public Dictionary<String, String> Values;
    }

    public delegate void DebugEntryEvent(String name);

    public class LogManager : BaseEngineComponent
    {
        public event DebugEntryEvent OnDebugAdded;

        int m_indentLevel;
        public int IndentLevel
        {
            get { return m_indentLevel; }
        }

        IEditor m_editor;
        public IEditor Editor
        {
            get { return m_editor; }
            set { m_editor = value; }
        }

        List<ILogOutput> m_outputs;
        public List<ILogOutput> Outputs
        {
            get { return m_outputs; }
        }

        Dictionary<String, DebugCategory> m_debugCategories;
        public Dictionary<String, DebugCategory> DebugCategories
        {
            get { return m_debugCategories; }
        }

        String m_currentDebugCategory;
        public String CurrentDebugCategory
        {
            get { return m_currentDebugCategory; }
            set { m_currentDebugCategory = value; }
        }
        
        Dictionary<String, String> m_debugValues;
        public Dictionary<String, String> DebugValues
        {
            get { return m_debugValues; }
        }

        bool m_debugEnabled;
        public bool DebugEnabled
        {
            get { return m_debugEnabled; }
            set { m_debugEnabled = value; }
        }

        bool m_ignoreExceptions;
        public bool IgnoreExceptions
        {
            get { return m_ignoreExceptions; }
            set { m_ignoreExceptions = value; }
        }

        public LogManager()
        {
            m_debugValues = new Dictionary<string, string>();
            m_debugCategories = new Dictionary<string, DebugCategory>();

            m_outputs = new List<ILogOutput>();
            m_indentLevel = 0;
            m_debugEnabled = true;
        }

        public override void Startup()
        {
            m_outputs.Add(new ConsoleLogOutput());
            m_outputs.Add(new FileLogOutput("log.txt"));
        }

        public void IndentMore()
        {
            m_indentLevel++;
        }

        public void IndentLess()
        {
            Assert(m_indentLevel > 0, "Indent level is already at 0");
            m_indentLevel--;

            if(m_indentLevel == 0)
            {
                Write("");
            }
        }

        public void Edit(Object obj)
        {
            if (m_editor != null)
            {
                m_editor.Edit(obj);
                }
        }

        public void Debug(String name, String msg)
        {
            if (m_debugValues.Keys.Contains(name))
            {
                m_debugValues[name] = msg;
            }
            else
            {
                m_debugValues[name] = msg;
                if (OnDebugAdded != null) OnDebugAdded(name);
            }
        }

        public void Debug(String name, Object msg)
        {
            Debug(name, msg.NullSafeToString());
        }

        public void Write(String msg)
        {
            foreach (var output in m_outputs)
                output.Write(msg);
        }

        public void Error(String msg)
        {
            foreach (var output in m_outputs)
                output.Error(msg);
        }

        public ErrorResult Exception(String msg)
        {
            Error("An exception has occurred\n: " + msg);

            DialogResult result = MessageBox.Show(
                "An exception has occurred:\n" + msg, 
                "Exception",
                MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button2);

            if (m_ignoreExceptions)
                return ErrorResult.Ignore;

            if (result == DialogResult.Abort)
            {
                return ErrorResult.Abort;
            }
            else if (result == DialogResult.Retry)
                return ErrorResult.Retry;
            else
                return ErrorResult.Ignore;
        }

        public ErrorResult Exception(Exception e)
        {
            return Exception(e.Message);
        }

        public ErrorResult Assert(bool expr, String msg = "")
        {
            if (!expr)
            {
                DialogResult result = MessageBox.Show(
                "An assert has failed:\n" + msg,
                "Assert",
                MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

                if (m_ignoreExceptions)
                    return ErrorResult.Ignore;

                if (result == DialogResult.Abort)
                {
                    System.Diagnostics.Debugger.Break();
                    throw new Exception("An assert has failed:\n" + msg);
                }
                else if (result == DialogResult.Retry)
                {
                    throw new Exception("An assert has failed:\n" + msg);
                }
                else
                {
                    return ErrorResult.Ignore;
                }
            }
            return ErrorResult.Succes;
        }
    }
}
