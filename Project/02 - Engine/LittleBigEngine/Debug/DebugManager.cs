using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using LBE.Debug;
using Microsoft.Xna.Framework.Input;
using LBE.Assets;
using LBE;
using System.IO;
using Microsoft.Xna.Framework;
using LBE.Gameplay;

namespace LBE.Debug
{
    public class DebugFlags
    {
        public bool EnableCommands = true;

        public bool ShowMenu = true;

        public bool ShowGameObjectPositions = false;
        public bool ShowGameObjectNames = false;

        public bool RenderDebug = false;
        public bool RenderSprites = true;

        public bool ShowPhysics = false;
        public bool SlowPhysics = false;

        public bool EnableAI = true;

        public bool ColorEdit = true;
    }

    public class DebugParameters
    {
        public DebugCommandDefinition[] Commands;
    }

    public class DebugManager : BaseEngineComponent
    {
        private DebugScreen m_debugScreen;
        public DebugScreen Screen
        {
            get { return m_debugScreen; }
        }

        DebugFlags m_flags;
        public DebugFlags Flags
        {
            get { return m_flags; }
            set { m_flags = value; }
        }

        Asset<AssetDefinition> m_editAsset;

        Asset<DebugParameters> m_paramAsset;

        List<DebugCommand> m_commands;

        public DebugManager()
        {
            m_flags = new DebugFlags();
        }

        public override void Startup()
        {
            m_paramAsset = Engine.AssetManager.GetAsset<DebugParameters>("System/Debug.lua::Debug");
            m_paramAsset.OnAssetChanged += new OnChange(m_paramAsset_OnAssetChanged);
            m_paramAsset_OnAssetChanged();

            m_editAsset = Engine.AssetManager.GetAsset<AssetDefinition>("Game/Edit.lua::Edit");

            m_debugScreen = new DebugScreen();
        }

        public override void StartFrame()
        {
            Engine.PhysicsManager.ShowDebug = m_flags.ShowPhysics;
            Engine.Log.DebugEnabled = m_flags.ShowMenu;

            foreach (var dc in m_commands)
            {
                if ( m_flags.EnableCommands && dc.Control.KeyPressed())
                {
                    dc.Command.Execute();
                }
            }

            if (m_flags.ShowGameObjectPositions)
            {
                foreach (var go in Engine.World.GameObjects)
                {
                    Engine.Debug.Screen.ResetBrush();
                    Engine.Debug.Screen.Brush.LineColor = Color.Red;
                    Engine.Debug.Screen.AddCross(go.Position, 7.0f);
                    if (m_flags.ShowGameObjectNames)
                        Engine.Debug.Screen.AddString(go.Name, go.Position + new Vector2(0, 18.0f), true);
                }
            }
        }

        public override void EndFrame()
        {
            m_debugScreen.Reset();
        }

        void m_paramAsset_OnAssetChanged()
        {
            //TODO: scripts take too long to build
            m_commands = new List<DebugCommand>();
            foreach (var cd in m_paramAsset.Content.Commands)
            {
                DebugCommand command = new DebugCommand();
                command.Definition = cd;
                command.Control = new Input.KeyControl(cd.Key);
                command.Command = cd.Script;

                m_commands.Add(command);
            }
        }

        public T Edit<T>(String name, T defaultValue = default(T))
        {
            try
            {
                AssetDefinition assetDef = m_editAsset.Content;
                if (assetDef.Fields.ContainsKey(name))
                    return (T)assetDef.Fields[name];
            }
            catch { }

            return defaultValue;
        }

        public T EditEnum<T>(String name, T defaultValue = default(T))
        {
            try
            {
                AssetDefinition assetDef = m_editAsset.Content;
                if (assetDef.Fields.ContainsKey(name))
                    return (T)Enum.Parse(typeof(T), assetDef.Fields[name] as String, true);
            }
            catch { }

            return defaultValue;
        }

        public int EditInt(String name, int defaultValue = 0)
        {
            try
            {
                AssetDefinition assetDef = m_editAsset.Content;
                if (assetDef.Fields.ContainsKey(name))
                    return assetDef.AsInt(name);
            }
            catch { }

            return defaultValue;
        }

        public float EditSingle(String name, float defaultValue = 0)
        {
            try
            {
                AssetDefinition assetDef = m_editAsset.Content;
                if (assetDef.Fields.ContainsKey(name))
                    return assetDef.AsFloat(name);
            }
            catch { }

            return defaultValue;
        }

        public bool EditBoolean(String name, bool defaultValue = false)
        {
            try
            {
                AssetDefinition assetDef = m_editAsset.Content;
                if (assetDef.Fields.ContainsKey(name))
                    return assetDef.AsBool(name);
            }
            catch { }

            return defaultValue;
        }

        public Vector2 EditVector2(String name)
        {
            return EditVector2(name, Vector2.Zero);
        }

        public Vector2 EditVector2(String name, Vector2 defaultValue)
        {
            try
            {
                AssetDefinition assetDef = m_editAsset.Content;
                if (assetDef.Fields.ContainsKey(name) && assetDef.Fields[name] is AssetDefinition)
                {
                    AssetDefinition vecAssetDef = assetDef.Fields[name] as AssetDefinition;
                    return new Vector2(vecAssetDef.AsFloat("X"), vecAssetDef.AsFloat("Y"));
                }
            }
            catch { }

            return defaultValue;
        }

        public void Do(Action<GameObject> action)
        {
            GameObject obj = new GameObject("ActionObject - " + action);
            obj.Attach(new DebugObjectComponent(action));
        }

        public void Do(float duration, Action<GameObject> action)
        {
            GameObject obj = new GameObject("ActionObject - " + action);
            obj.Attach(new DebugObjectComponent(action, duration));
        }
    }
}
