using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Log;
using System.IO;
using System.Diagnostics;

namespace LBE.Assets
{
    public class Asset<T> : IAsset, IAssetDependency
    {
        public event OnChange OnAssetChanged;

        String m_key;
        public String Key
        {
            get { return m_key; }
        }

        String m_name;
        public String Name
        {
            get { return m_name; }
        }

        String m_path;
        public String Path
        {
            get { return m_path; }
        }

        T m_content;
        public T Content
        {
            get { return m_content; }
            set { SetContent(value); }
        }

        public Type Type
        {
            get { return typeof(T); }
        }

        private List<IAssetDependency> m_dependencies;

        IAssetLoader<T> m_loader;
        public IAssetLoader<T> Loader
        {
            get { return m_loader; }
        }

        bool m_bReloadingDependencies;

        bool m_bReady;
        public bool Ready
        {
            get { return m_bReady; }
        }

        public Asset(String path, String key, IAssetLoader<T> assetLoader)
        {
            m_key = key;
            m_loader = assetLoader;
            m_path = path;
            m_dependencies = new List<IAssetDependency>();

            m_bReloadingDependencies = false;
            m_bReady = false;

            m_name = PathToName(path);
        }

        public Asset(IAssetLoader<T> assetLoader)
        {
            m_name = "";
            m_key = "";
            m_loader = assetLoader;
            m_path = "";
            m_dependencies = new List<IAssetDependency>();

            m_bReloadingDependencies = false;
            m_bReady = false;

            Load();
        }

        public Asset(T content)
        {
            m_content = content;
            m_dependencies = new List<IAssetDependency>();

            m_bReloadingDependencies = false;
            m_bReady = false;
        }

        public static String PathToName(String path)
        {
            if (path.Contains("::"))
            {
                String[] subStrings = path.Split(new String[] { "::" }, 2, StringSplitOptions.None);
                String assetListPath = subStrings[0];
                String assetDefName = subStrings[1];
                return assetDefName;
            }
            else
            {
                return System.IO.Path.GetFileName(path);
            }
        }

        public void AddDependency(IAssetDependency dependency)
        {
            m_dependencies.Add(dependency);
        }

        public Object Get()
        {
            return m_content;
        }

        public bool Load()
        {
            lock (this)
            {
                //Are we reloading the asset
                bool bReload = m_bReady;

                T oldContent = default(T);

                //If reloading, store the old content so we can unload it later
                if (bReload)
                    oldContent = m_content;

                AssetLoadResult<T> result = default(AssetLoadResult<T>);
                bool retry = true;
                while (retry)
                {
                    //Try to load the asset
                    ErrorResult error = ErrorResult.Succes;
                    try
                    {
                        result = m_loader.Load(m_path);
                    }
                    catch (Exception e)
                    {
                        StringBuilder errorString = new StringBuilder();
                        errorString.AppendLine("Error while loading " + m_path);
                        errorString.AppendLine("");
                        errorString.AppendLine("Informations:");
                        errorString.AppendLine(e.Message);
                        error = Engine.Log.Exception(errorString.ToString());
                    }

                    if (result.Instance != null && error == ErrorResult.Succes)
                    {
                        //If successful, update content
                        m_content = result.Instance;
                        retry = false;
                    }
                    else
                    {
                        //Else handle error as the user has requested
                        if(error == ErrorResult.Retry)
                        {
                            //Need to duplicate the list, because we may have depencies change during the reload
                            var cloneList = m_dependencies.ToArray();
                            //Reload all dependencies
                            foreach (IAssetDependency dependency in cloneList)
                            {
                                dependency.Reload();
                            }
                            retry = true;
                        }
                        else if (error == ErrorResult.Abort)
                        {
                            retry = false;
                            Environment.FailFast(null);
                            return false;
                        }
                        else if (error == ErrorResult.Ignore)
                        {
                            retry = false;
                            return false;
                        }
                    }
                }   

                //Clear old dependencies
                foreach (IAssetDependency dependency in m_dependencies)
                {
                    dependency.OnAssetChanged -= dependency_OnAssetChanged;
                }
                m_dependencies.Clear();

                //Add new dependencies
                foreach (IAssetDependency dependency in result.Dependencies)
                {
                    dependency.OnAssetChanged += new OnChange(dependency_OnAssetChanged);
                    m_dependencies.Add(dependency);
                }                

                if (bReload)
                {
                    //If content was already loaded before, trigger OnAssetChange
                    if (OnAssetChanged != null) OnAssetChanged();

                    //Unload the old content
                    m_loader.Unload(oldContent);
                }

                //The asset has now been loaded
                m_bReady = true;
            }

            return true;
        }

        public void SetContent(T content)
        {
            m_content = content;
            if (OnAssetChanged != null) OnAssetChanged();
        }

        public void Reload()
        {
            lock (this)
            {
                m_bReloadingDependencies = true;
                m_bReady = false;
            }

            //Reload dependencies
            foreach (IAssetDependency dependency in m_dependencies)
            {
                dependency.Reload();
            }

            lock (this)
            {
                m_bReloadingDependencies = false;
            }

            //Reload content
            if(m_loader != null)
                Load();
            else
                if (OnAssetChanged != null) OnAssetChanged();
        }

        void dependency_OnAssetChanged()
        {
            lock (this)
            {
                if (m_bReloadingDependencies)
                {
                    //Do Nothing, wait for reload to end
                }
                else
                {
                    Engine.Log.Write(String.Format("Reloading asset \"{0}\"", m_path));
                    Engine.Log.IndentMore();
                    Load();
                    Engine.Log.IndentLess();
                }
            }
        }
    }
}
