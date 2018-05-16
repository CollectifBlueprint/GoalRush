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
using Ball.Graphics;

namespace Ball.MainMenu.Scripts
{
    public class SelectArenaParameters
    {
        public float DescriptionPanelSize;
        public float SelectionPanelSize;

        public float SeparatorLineOffset;
        public float SeparatorMargin;

        public Vector2 DescriptionPreviewSize;
        public float DescriptionPreviewHeight;
        public float DescriptionPreviewMargin;

        public Vector2 SelectionPreviewSize;
        public float SelectionPreviewMargin;
    }

    class SelectionPanel
    {
        public SpriteComponent Arrows;
        public SpriteComponent[] Previews;
    }

    class DescriptionPanel
    {
        public TextComponent Title;
        public TextComponent Content;

        public SpriteComponent Preview;
    }

    public class AltArenaItem
    {
        public SpriteComponent Preview;
        public SpriteComponent Selection;

        public TextComponent Name;
        public TextComponent Description;
    }

    public class AltSelectMapScript : MenuScript
    {
        Asset<SelectArenaParameters> m_parameters;
        public SelectArenaParameters Parameters
        {
            get { return m_parameters.Content; }
        }

        Asset<ArenaList> m_arenas;
        ArenaPreview[] m_previews;

        int m_itemPerScreen;
        int m_selectionIndex;
        int m_previewStartIndex;

        TextStyle m_titleStyle;
        TextStyle m_contentStyle;
        TextStyle m_contentStyle2;

        DescriptionPanel m_descriptionPanel;
        SelectionPanel m_selectionPanel;
        SpriteComponent m_separatorCmp;

        TextComponent m_arenaColorTextCmp;
        ColorMaskedSprite m_arenaColorSpriteCmp;
        int m_arenaColorIndex;
        Asset<ColorSchemeData> m_colorSchemeDataAsset;

        public override void Start()
        {
            if (Game.GameSession.CurrentMatchInfo == null)
                Game.GameSession.CurrentMatchInfo = new MatchStartInfo();

            m_parameters = Engine.AssetManager.GetAsset<SelectArenaParameters>("Interface/SelectMapMenu.lua::Params");

            var backgroundCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/Background.png"), "MenuBackground");
            Menu.Owner.Attach(backgroundCmp);

            SpriteFont font = Engine.AssetManager.Get<SpriteFont>("Graphics/Menu/BigMacUnselectedPen");
            SpriteFont fontSelected = Engine.AssetManager.Get<SpriteFont>("Graphics/Menu/BigMacSelected");
            m_titleStyle = new TextStyle() { Scale = 0.75f, Color = Color.White, Font = fontSelected };
            m_contentStyle = new TextStyle() { Scale = 0.5f, Color = Color.White, Font = font };

            m_arenas = Engine.AssetManager.GetAsset<ArenaList>("Arenas/Arenas.lua::Arenas");

            m_itemPerScreen = Math.Min(4, m_arenas.Content.Arenas.Count());

            m_selectionIndex = 0;
            m_previewStartIndex = 0;
            m_previews = new ArenaPreview[m_arenas.Content.Arenas.Length];

            m_colorSchemeDataAsset = Engine.AssetManager.GetAsset<ColorSchemeData>("Game/Colors.lua::ColorShemeArenas");
            m_arenaColorIndex = -1;

            for (int i = 0; i < m_previews.Length; i++)
            {
                String arenaId = m_arenas.Content.Arenas[i];
                m_previews[i] = Engine.AssetManager.Get<ArenaPreview>("Arenas/" + arenaId + "/Arena.lua::Preview");

                //Set the arena to last selected/played
                if (Game.GameSession.CurrentMatchInfo.Arena.Name == arenaId)
                {
                    m_selectionIndex = i;

                    for (int j = 0; j < m_colorSchemeDataAsset.Content.ColorSchemes.Length; j++)
                    {
                        if ( Game.GameSession.CurrentMatchInfo.Arena.ColorScheme == m_colorSchemeDataAsset.Content.ColorSchemes[j])
                        {
                            m_arenaColorIndex = j;
                            break;
                        }
                    }
                }

                m_previewStartIndex = m_selectionIndex - 1;
                m_previewStartIndex = LBE.MathHelper.Clamp(0, m_previews.Length - m_itemPerScreen, m_previewStartIndex);
            }

            InitDescriptionPanel();
            InitSelectionPanel();

            m_separatorCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/ArenaSelectionSeparator.png"), "Menu");
            m_separatorCmp.Position = Parameters.SeparatorLineOffset * Vector2.UnitX;

            Menu.Owner.Attach(m_separatorCmp);

            m_contentStyle2 = new TextStyle() { Scale = 0.4f, Color = Color.White, Font = font };

            InitColorPanel();

            Game.GameMusic.PlayMenuMusic();

            UpdateMenu();
        }

        public void InitDescriptionPanel()
        {
            Vector2 position = Parameters.SeparatorLineOffset * Vector2.UnitX
                            - Parameters.SeparatorMargin * Vector2.UnitX
                            - Parameters.DescriptionPanelSize * Vector2.UnitX * 0.5f;

            m_descriptionPanel = new DescriptionPanel();
            m_descriptionPanel.Preview = new SpriteComponent("Menu");
            m_descriptionPanel.Preview.Position = position + Parameters.DescriptionPreviewHeight * Vector2.UnitY;
            Menu.Owner.Attach(m_descriptionPanel.Preview);

            m_descriptionPanel.Title = new TextComponent("Menu");
            m_descriptionPanel.Title.Style = m_titleStyle;
            m_descriptionPanel.Title.Position = position + Parameters.DescriptionPreviewHeight * Vector2.UnitY + Parameters.DescriptionPreviewSize.ProjectY() * 0.5f + Parameters.DescriptionPreviewMargin * Vector2.UnitY;
            m_descriptionPanel.Title.AlignementVertical = TextAlignementVertical.Down;
            m_descriptionPanel.Title.Alignement = TextAlignementHorizontal.Center;
            Menu.Owner.Attach(m_descriptionPanel.Title);

            m_descriptionPanel.Content = new TextComponent("Menu");
            m_descriptionPanel.Content.Style = m_contentStyle;
            m_descriptionPanel.Content.Position = position + Parameters.DescriptionPreviewHeight * Vector2.UnitY - Parameters.DescriptionPreviewSize.ProjectY() * 0.5f - Parameters.DescriptionPreviewMargin * Vector2.UnitY - Parameters.DescriptionPanelSize * 0.5f * Vector2.UnitX;
            m_descriptionPanel.Content.AlignementVertical = TextAlignementVertical.Up;
            m_descriptionPanel.Content.Alignement = TextAlignementHorizontal.Left;
            m_descriptionPanel.Content.Wrap = true;
            m_descriptionPanel.Content.WrapWidth = Parameters.DescriptionPanelSize;
            Menu.Owner.Attach(m_descriptionPanel.Content);
        }

        public void InitColorPanel()
        {
            if (m_arenaColorIndex == -1)
                m_arenaColorIndex = 0;
            
            Vector2 position = Parameters.SeparatorLineOffset * Vector2.UnitX
                            - Parameters.SeparatorMargin * Vector2.UnitX
                            - Parameters.DescriptionPanelSize * Vector2.UnitX;

            position += -Parameters.DescriptionPreviewHeight * Vector2.UnitY - Parameters.DescriptionPreviewSize.ProjectY() * 0.5f - Parameters.DescriptionPreviewMargin * Vector2.UnitY;

            m_arenaColorTextCmp = new TextComponent("Menu");
            m_arenaColorTextCmp.Text = "Color :";
            m_arenaColorTextCmp.Position = position;
            m_arenaColorTextCmp.Style = new TextStyle() { Font = m_contentStyle2.Font, Scale = m_contentStyle2.Scale, Color = Color.White };
            m_arenaColorTextCmp.Visible = true;
            Menu.Owner.Attach(m_arenaColorTextCmp);

            Sprite colorSprite = Sprite.CreateFromTexture("Graphics/Menu/ColorSelectionBase.png");
            var texAsset = Engine.AssetManager.GetAsset<Texture2D>("Graphics/Menu/ColorSelectionColorMask.png");


            m_arenaColorSpriteCmp = new ColorMaskedSprite(colorSprite, "Menu2");
            float arenaColorSpriteOffset = m_contentStyle2.Font.MeasureString(m_arenaColorTextCmp.Text + " ").X * m_contentStyle2.Scale;
            m_arenaColorSpriteCmp.Position = m_arenaColorTextCmp.Position + arenaColorSpriteOffset * Vector2.UnitX + colorSprite.Size.X * 0.5f * Vector2.UnitX;
            m_arenaColorSpriteCmp.Mask = texAsset.Content;
            m_arenaColorSpriteCmp.Color1 = m_colorSchemeDataAsset.Content.ColorSchemes[m_arenaColorIndex].Color2;
            m_arenaColorSpriteCmp.Color2 = Color.White;
            m_arenaColorSpriteCmp.Color3 = Color.White;
            m_arenaColorSpriteCmp.Color4 = Color.White;
            Menu.Owner.Attach(m_arenaColorSpriteCmp);
        }

        public void UpdateDescriptionPanel(ArenaPreview preview)
        {
            m_descriptionPanel.Preview.Sprite = Sprite.CreateFromTexture(preview.Preview);
            m_descriptionPanel.Preview.Sprite.ScaleToSizeFixedRatio(Parameters.DescriptionPreviewSize);

            m_descriptionPanel.Title.Text = preview.Name;
            m_descriptionPanel.Content.Text = preview.Description;
        }

        void InitSelectionPanel()
        {
            Vector2 position = Parameters.SeparatorLineOffset * Vector2.UnitX
                            + Parameters.SeparatorMargin * Vector2.UnitX
                            + Parameters.SelectionPanelSize * Vector2.UnitX * 0.5f;

            m_selectionPanel = new SelectionPanel();
            m_selectionPanel.Arrows = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/ArenaSelectionArrows.png"), "Menu");
            m_selectionPanel.Arrows.Position = position;
            Menu.Owner.Attach(m_selectionPanel.Arrows);

            m_selectionPanel.Previews = new SpriteComponent[m_itemPerScreen];
            for (int i = 0; i < m_itemPerScreen; i++)
            {
                float height = Parameters.SelectionPreviewSize.Y + Parameters.SelectionPreviewMargin * 2;
                float firstPreviewHeight = 0.5f * (m_itemPerScreen - 1) * height;

                m_selectionPanel.Previews[i] = new SpriteComponent("Menu");
                m_selectionPanel.Previews[i].Position = position + (firstPreviewHeight - i * height) * Vector2.UnitY;
                Menu.Owner.Attach(m_selectionPanel.Previews[i]);
            }
        }

        void UpdateSelectionPanel()
        {
            
            
            for (int i = 0; i < m_itemPerScreen; i++)
            {
                int mapIdx = m_previewStartIndex + i;

                m_selectionPanel.Previews[i].Sprite = Sprite.CreateFromTexture(m_previews[mapIdx].Preview);

                if (mapIdx == m_selectionIndex)
                {
                    float scale = 1.1f;
                    m_selectionPanel.Previews[i].Sprite.Color = Color.White;
                    m_selectionPanel.Previews[i].Sprite.ScaleToSizeFixedRatio(Parameters.SelectionPreviewSize * scale);
                }
                else
                {
                    m_selectionPanel.Previews[i].Sprite.Color = Color.Gray;
                    m_selectionPanel.Previews[i].Sprite.ScaleToSizeFixedRatio(Parameters.SelectionPreviewSize);
                }
            }
        }

        public override void Update()
        {
            foreach (var ctrl in Game.MenuManager.Controllers)
            {
                if (ctrl.DownCtrl.KeyPressed())
                {
                    m_selectionIndex++;
                    UpdateMenu();
                    if (Menu.AudioCmpSelect != null)
                        Menu.AudioCmpSelect.Play();
                    break;
                }

                if (ctrl.UpCtrl.KeyPressed())
                {
                    m_selectionIndex--;
                    UpdateMenu();
                    if (Menu.AudioCmpSelect != null)
                        Menu.AudioCmpSelect.Play();
                    break;
                }

                if (ctrl.ValidCtrl.KeyPressed())
                {
                    if (m_arenas.Content.Arenas.Length > 0 && m_selectionIndex < m_arenas.Content.Arenas.Length)
                    {
                        Game.GameSession.CurrentMatchInfo.Arena.Name = m_arenas.Content.Arenas[m_selectionIndex];
                        Game.GameSession.CurrentMatchInfo.Arena.ColorScheme = m_colorSchemeDataAsset.Content.ColorSchemes[m_arenaColorIndex];

                        var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/PlayMenu.lua::Menu");
                        Game.MenuManager.StartMenu(menuDef);
                        if (Menu.AudioCmpValid != null)
                            Menu.AudioCmpValid.Play();
                        break;
                    }
                }

                else if (ctrl.SelectPreviousCtrl.KeyPressed() || ctrl.SelectNextCtrl.KeyPressed())
                {
                    int colorIteration = ctrl.SelectNextCtrl.KeyPressed()?1:-1;
                   
                    int colorSchemesLength = m_colorSchemeDataAsset.Content.ColorSchemes.Length;
                    m_arenaColorIndex += colorIteration;
                    m_arenaColorIndex = (m_arenaColorIndex + colorSchemesLength) % colorSchemesLength;
                    m_arenaColorSpriteCmp.Color1 = m_colorSchemeDataAsset.Content.ColorSchemes[m_arenaColorIndex].Color2;
                }
            }
        }

        public void UpdateMenu()
        {
            if (m_selectionIndex < 0)
                m_selectionIndex = 0;
            if (m_selectionIndex >= m_previews.Count())
                m_selectionIndex = m_previews.Count() - 1;

            if (m_selectionIndex < m_previewStartIndex)
                m_previewStartIndex--;
            else if (m_selectionIndex >= m_previewStartIndex + 4)
                m_previewStartIndex++;

            UpdateDescriptionPanel(m_previews[m_selectionIndex]);
            UpdateSelectionPanel();
        }

        public override void OnCancel(string name, MenuController controller)
        {
            base.OnCancel(name, controller);
            
            var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/SelectTeamMenu.lua::Menu");
            Game.MenuManager.StartMenu(menuDef);
        }
    }
}
