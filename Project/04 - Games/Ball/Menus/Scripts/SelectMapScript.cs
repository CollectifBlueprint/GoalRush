using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Menus;
using Ball.Gameplay;
using Microsoft.Xna.Framework;
using LBE;
using LBE.Graphics.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Ball.Gameplay.Arenas;
using LBE.Assets;

namespace Ball.MainMenu.Scripts
{
    public class ArenaItem
    {
        public SpriteComponent Preview;
        public SpriteComponent Selection;

        public TextComponent Name;
        public TextComponent Description;
    }

    public class SelectMapScript : MenuScript
    {
        Asset<ArenaList> m_arenas;
        ArenaItem[] m_items;
        ArenaPreview[] m_previews;

        float m_previewOffset;
        Vector2 m_previewSize;

        float[] m_previewScales;
        int m_itemPerScreen;
        int m_selectionIndex;

        public float m_yMap;
        public float m_mapOffset1 = 290;
        public float m_mapOffset2 = 185;

        float m_nameOffset = 140;
        float m_descOffset = 120;

        TextStyle m_style;

        public override void Start()
        {
            if (Game.GameSession.CurrentMatchInfo == null)
                Game.GameSession.CurrentMatchInfo = new MatchStartInfo();

            var backgroundCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/Background.png"), "MenuBackground");
            Menu.Owner.Attach(backgroundCmp);

            m_style = new TextStyle();
            m_style.Font = Engine.AssetManager.Get<SpriteFont>("Graphics/GameplayFont");
            m_style.Scale = 0.6f;

            m_arenas = Engine.AssetManager.GetAsset<ArenaList>("Arenas/Arenas.lua::Arenas"); m_itemPerScreen = 3;

            m_selectionIndex = 0;
            m_previews = new ArenaPreview[m_arenas.Content.Arenas.Length];
            for (int i = 0; i < m_previews.Length; i++)
            {
                String arenaId = m_arenas.Content.Arenas[i];
                m_previews[i] = Engine.AssetManager.Get<ArenaPreview>("Arenas/" + arenaId + "/Arena.lua::Preview");

                //Set the arena to last selected/played
                if (Game.GameSession.CurrentMatchInfo.Arena.Name == arenaId)
                    m_selectionIndex = i;
            }

            m_yMap = 30;

            m_previewOffset = 75;
            m_previewSize = new Vector2(320, 180);

            m_itemPerScreen = 5;


            m_items = new ArenaItem[m_itemPerScreen];
            for (int i = 0; i < m_itemPerScreen; i++)
            {
                m_items[i] = new ArenaItem();
                m_items[i].Preview = new SpriteComponent("Menu");
                Menu.Owner.Attach(m_items[i].Preview);
            }
            m_items[0].Preview.Position = new Vector2(-m_mapOffset1 - m_mapOffset2, m_yMap);
            m_items[1].Preview.Position = new Vector2(-m_mapOffset1, m_yMap);
            m_items[2].Preview.Position = new Vector2(0, m_yMap);
            m_items[3].Preview.Position = new Vector2(m_mapOffset1, m_yMap);
            m_items[4].Preview.Position = new Vector2(m_mapOffset1 + m_mapOffset2, m_yMap);

            for (int i = 0; i < m_itemPerScreen; i++)
            {
                m_items[i].Selection = new SpriteComponent(Sprite.CreateFromTexture("Graphics/arenaPreviewBackground.png"), "Menu");
                m_items[i].Selection.Position = m_items[i].Preview.Position;
                Menu.Owner.Attach(m_items[i].Selection);

                m_items[i].Name = new TextComponent("Menu");
                m_items[i].Name.Alignement = TextAlignementHorizontal.Center;
                m_items[i].Name.Style = m_style;
                m_items[i].Name.Position = m_items[i].Preview.Position + new Vector2(0 , m_nameOffset);
                Menu.Owner.Attach(m_items[i].Name);

                m_items[i].Description = new TextComponent("Menu");
                m_items[i].Description.Alignement = TextAlignementHorizontal.Center;
                m_items[i].Description.Style = m_style;
                m_items[i].Description.Position = m_items[i].Preview.Position + new Vector2(0, -m_descOffset);
                Menu.Owner.Attach(m_items[i].Description);
            }

            m_previewScales = new float[] { 0.33f, 0.5f, 1.0f, 0.5f, 0.33f };

            Game.GameMusic.PlayMenuMusic();

            UpdateMenu();
        }

        public override void Update()
        {
            foreach (var ctrl in Game.MenuManager.Controllers)
            {
                if (ctrl.RightCtrl.KeyPressed())
                {
                    m_selectionIndex++;
                    UpdateMenu();
                    break;
                } 
                
                if (ctrl.LeftCtrl.KeyPressed())
                {
                    m_selectionIndex--;
                    UpdateMenu();
                    break;
                }

                if (ctrl.ValidCtrl.KeyPressed())
                {
                    if (m_arenas.Content.Arenas.Length > 0 && m_selectionIndex < m_arenas.Content.Arenas.Length)
                    {
                        Game.MenuManager.QuitMenu();
                        Game.GameSession.CurrentMatchInfo.Arena.Name = m_arenas.Content.Arenas[m_selectionIndex]; 
                        Game.GameManager.StartMatch(Game.GameSession.CurrentMatchInfo);
                        break;
                    }
                }
            }
        }

        public void UpdateMenu()
        {
            if (m_selectionIndex < 0)
                m_selectionIndex = 0;
            if (m_selectionIndex >= m_previews.Count())
                m_selectionIndex = m_previews.Count() - 1;

            for (int i = 0; i < m_itemPerScreen; i++)
            {
                int iMap = m_selectionIndex + i - 2;
                if (iMap >= 0 && iMap < m_previews.Count())
                {
                    ArenaPreview desc = m_previews[iMap];
                    m_items[i].Preview.Sprite = Sprite.CreateFromTexture(desc.Preview);
                    m_items[i].Preview.Sprite.ScaleToSizeFixedRatio(m_previewSize * m_previewScales[i]);
                    m_items[i].Selection.Sprite.Scale = new Vector2(m_previewScales[i], m_previewScales[i]);
                    m_items[i].Selection.Visible = true;
                    m_items[i].Preview.Visible = true;
                }
                else
                {
                    m_items[i].Selection.Visible = false;
                    m_items[i].Preview.Visible = false;
                }

                if (i == 0 || i == 4)
                    if (m_items[i].Preview.Sprite != null) m_items[i].Preview.Sprite.Color = new Color(80, 80, 80);
                if (i == 1 || i == 3)
                    if (m_items[i].Preview.Sprite != null) m_items[i].Preview.Sprite.Color = Color.DarkGray;
                
                m_items[i].Name.Visible = false;
                if (i == 2)
                {
                    ArenaPreview desc = m_previews[iMap];
                    m_items[i].Name.Visible = true;
                    m_items[i].Name.Text = desc.Name;
                }

                m_items[i].Description.Visible = false;
                if (i == 2)
                {
                    ArenaPreview desc = m_previews[iMap];
                    m_items[i].Description.Visible = true;
                    m_items[i].Description.Text = desc.Description;
                }
            }
        }

        public override void OnCancel(string name, MenuController controller)
        {
            base.OnCancel(name, controller);

            var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/SelectTeamMenu.lua::Menu");
            Game.MenuManager.StartMenu(menuDef);
        }
    }
}
