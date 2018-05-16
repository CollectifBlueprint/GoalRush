using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LBE.Assets
{
    public class FileDependency : IAssetDependency
    {
        FileSystemWatcher m_watcher;
        bool m_bChanged;

        String m_path;
        public String FilePath
        {
            get { return m_path; }
        }

        public FileDependency(String path)
        {
            m_path = path;

            String fullPath = Path.GetFullPath(path);
            m_watcher = new FileSystemWatcher(Path.GetDirectoryName(fullPath), Path.GetFileName(fullPath));
            m_watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime | NotifyFilters.FileName; 
            m_watcher.Changed += new FileSystemEventHandler(FileChanged);

            m_bChanged = false;
            m_watcher.EnableRaisingEvents = true;
        }

        public event OnChange OnAssetChanged;

        public void Reload()
        {
            if (OnAssetChanged != null) OnAssetChanged();
        }

        void FileChanged(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                m_bChanged = true;
            }
        }

        public bool PollChange()
        {
            lock (this)
            {
                bool changed = m_bChanged;
                m_bChanged = false;
                return changed;
            }
        }
    }
}
