using System;
using LBE.Graphics.Sprites;
using LBE;

using LBE.Assets;
using Ball.Menus;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ball
{
    public class LoadingScreenMenuScript : MenuScript
    {
        volatile bool m_done;
        public bool Done
        {
            get { return m_done; }
        }
        TextComponent m_debugTextCmp;
        TextComponent m_loadingTextCmp;
        int m_frame;

        int m_assetIndex;
        AssetDatabaseEntry[] m_assetToLoad;

        DateTime m_startTime;

        LBE.Timer m_textUpdateTimerMS;
        LBE.TimerEvent m_textUpdateTimerEvent;
        int m_timerCount = 0;

        public override void Start()
        {
            m_frame = Engine.FrameCount;

            m_assetToLoad = Engine.AssetManager.AssetDb.Assets.ToArray();
            m_assetIndex = 0;

            m_startTime = DateTime.Now;

            var backgroundCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/Background.png"), "MenuBackground");
            Menu.Owner.Attach(backgroundCmp);

            SpriteFont font = Engine.AssetManager.Get<SpriteFont>("Graphics/Menu/BigMacUnselectedPen");
            SpriteFont fontSelected = Engine.AssetManager.Get<SpriteFont>("Graphics/Menu/BigMacSelected");
            TextStyle textStyle = new TextStyle() { Scale = 1, Color = Color.White, Font = fontSelected };

            m_loadingTextCmp = new TextComponent("Loading...", "Menu");

            m_loadingTextCmp.Style = textStyle;
            m_loadingTextCmp.AlignementVertical = TextAlignementVertical.Center;
            m_loadingTextCmp.Alignement = TextAlignementHorizontal.Left;
            Menu.Owner.Attach(m_loadingTextCmp);

            m_textUpdateTimerMS = new Timer(Engine.RealTime.Source, 200, TimerBehaviour.Restart);
            m_textUpdateTimerEvent = new TimerEvent(m_textUpdateTimerMS_OnTime);
            m_textUpdateTimerMS.OnTime += m_textUpdateTimerEvent;
            m_textUpdateTimerMS.Start();

#if DEBUG
            m_debugTextCmp = new TextComponent("Loading...", "Menu");
            m_debugTextCmp.Style.Scale = 3;
            m_debugTextCmp.Alignement = TextAlignementHorizontal.Center;
            m_debugTextCmp.AlignementVertical = TextAlignementVertical.Down;
            m_debugTextCmp.Position += textStyle.Font.MeasureString(m_loadingTextCmp.Text).Y * textStyle.Scale * Vector2.UnitY;
            Menu.Owner.Attach(m_debugTextCmp);
#endif


        }

        public override void Update()
        {
            if (Engine.FrameCount < m_frame + 5)
            {
                return;
            }

#if DEBUG
            foreach (var ctrl in Game.MenuManager.Controllers)
            {
                if (ctrl.StartCtrl.KeyPressed() || ctrl.ValidCtrl.KeyPressed())
                {
                    m_done = true;
                }
            }
#endif

            int nAssetPerFrame = 3;
            for (int i = 0; i < nAssetPerFrame; i++)
            {
                if (m_assetIndex < m_assetToLoad.Length)
                {
                    Engine.AssetManager.AssetDb.Load(m_assetToLoad[m_assetIndex]);
                    m_assetIndex++;
                }
                else
                {
                    var duration = DateTime.Now - m_startTime;
                    Engine.Log.Write("Load time: " + duration.TotalSeconds.ToString("0.000"));

                    m_done = true;
                    break;
                }
            }

#if DEBUG
            m_debugTextCmp.Text = "Loading: " + m_assetIndex + "/" + m_assetToLoad.Length + "\nPress start to skip";
#endif

            if (m_done)
            {
                var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/MainMenu.lua::Menu");
                Game.MenuManager.StartMenu(menuDef);
            }
        }

        void m_textUpdateTimerMS_OnTime(Timer source)
        {
            m_timerCount++;

            if (m_timerCount % 4 == 0)
                m_loadingTextCmp.Text = "Loading";
            else if (m_timerCount % 4 == 1)
                m_loadingTextCmp.Text = "Loading.";
            else if (m_timerCount % 4 == 2)
                m_loadingTextCmp.Text = "Loading..";
            else if (m_timerCount % 4 == 3)
                m_loadingTextCmp.Text = "Loading...";
 
        }

        public override void End()
        {
            base.End();
            m_textUpdateTimerMS.Stop();
            m_textUpdateTimerMS.OnTime -= m_textUpdateTimerEvent;
        }
    }
}
