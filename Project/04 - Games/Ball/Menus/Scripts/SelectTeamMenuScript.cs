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
using LBE.Assets;
using Ball.Graphics;

namespace Ball.MainMenu.Scripts
{
    public enum ControllerState
    {
        None,
        Left,
        Right
    }

    public class ControllerInfo
    {
        public int ControllerIndex;
        public MenuController Controller;

        public SpriteComponent SpriteCmp;
        public TextComponent TextCmp;

        public ControllerState State;
        public int PlayerIndex;
    }

    public class SelectTeamMenuParamters
    {
        public float Offset;
        public float PanelWidth;

        public float ControllersHeight;
        public float ControllersHeightOffset;
        public float ControllerTextHeightOffset;
    }

    public class SelectTeamMenuScript : MenuScript
    {
        Asset<SelectTeamMenuParamters> m_parameters;
        public SelectTeamMenuParamters Parameters
        {
            get { return m_parameters.Content; }
        }

        Dictionary<MenuController, ControllerInfo> m_ctrlInfos;
        public Dictionary<MenuController, ControllerInfo> ControllersInfo
        {
            get { return m_ctrlInfos; }
            set { m_ctrlInfos = value; }
        }

        SpriteComponent[] m_playerSpotCmp;
        TextComponent[] m_playerSpotTextCmp;
        MenuController[] m_playerSlots;
        Vector2[] m_playerSpotPos;

        TextComponent m_pressStartCmp;

        TextComponent[] m_teamColorTextCmp;
        ColorMaskedSprite[] m_teamColorSpriteCmp;
        int[] m_teamColorIndexes;
        Asset<ColorSchemeData> m_colorSchemeDataAsset;

        float yPos = 80;
        float xOffset = 320;
        float yOffset = 200;

        float textYOffset = 30;

        float iconSpriteScale = 0.8f;

        TextStyle m_style;

        public override void Start()
        {
            m_teamColorIndexes = new int[2];
            m_colorSchemeDataAsset = Engine.AssetManager.GetAsset<ColorSchemeData>("Game/Colors.lua::ColorShemeTeams");
            m_teamColorIndexes[0] = -1;
            m_teamColorIndexes[1] = -1;

            if (Game.GameSession.CurrentMatchInfo == null)
            {
                Game.GameSession.CurrentMatchInfo = new MatchStartInfo();
                m_teamColorIndexes[0] = 0;
                m_teamColorIndexes[1] = m_colorSchemeDataAsset.Content.ColorSchemes.Length / 2;
            }
            else
            {
                for (int i = 0; i < Game.GameSession.CurrentMatchInfo.Teams.Length; i++)
                {
                    for (int j = 0; j < m_colorSchemeDataAsset.Content.ColorSchemes.Length; j++)
                    {
                        if (Game.GameSession.CurrentMatchInfo.Teams[i].ColorScheme == m_colorSchemeDataAsset.Content.ColorSchemes[j])
                        {
                            m_teamColorIndexes[i] = j;
                            break;
                        }
                    }
                }
            }

            m_parameters = Engine.AssetManager.GetAsset<SelectTeamMenuParamters>("Interface/SelectTeamMenu.lua::Params");

            var backgroundCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/Background.png"), "MenuBackground");
            Menu.Owner.Attach(backgroundCmp);

            var backgroundTeamCmp = new SpriteComponent(Sprite.CreateFromTexture("Graphics/Menu/TeamSelectionBackground.png"), "Menu");
            Menu.Owner.Attach(backgroundTeamCmp);

            m_ctrlInfos = new Dictionary<MenuController, ControllerInfo>();
            m_playerSlots = new MenuController[4];

            SpriteFont font = Engine.AssetManager.Get<SpriteFont>("Graphics/Menu/BigMacUnselectedPen");
            m_style = new TextStyle() { Scale = 0.4f, Color = Color.White, Font = font };

            m_playerSpotPos = new Vector2[4];
            m_playerSpotPos[0] = Parameters.Offset * Vector2.UnitX - Parameters.PanelWidth * Vector2.UnitX + Parameters.ControllersHeight * Vector2.UnitY;
            m_playerSpotPos[1] = Parameters.Offset * Vector2.UnitX - Parameters.PanelWidth * Vector2.UnitX + Parameters.ControllersHeight * Vector2.UnitY - Parameters.ControllersHeightOffset * Vector2.UnitY;
            m_playerSpotPos[2] = Parameters.Offset * Vector2.UnitX + Parameters.PanelWidth * Vector2.UnitX + Parameters.ControllersHeight * Vector2.UnitY;
            m_playerSpotPos[3] = Parameters.Offset * Vector2.UnitX + Parameters.PanelWidth * Vector2.UnitX + Parameters.ControllersHeight * Vector2.UnitY - Parameters.ControllersHeightOffset * Vector2.UnitY;

            m_pressStartCmp = new TextComponent("Menu");
            m_pressStartCmp.Text = "Press Start\n     to join";
            m_pressStartCmp.Alignement = TextAlignementHorizontal.Center;
            m_pressStartCmp.AlignementVertical = TextAlignementVertical.Down;
            m_pressStartCmp.Style = new TextStyle() { Font = m_style.Font, Scale = m_style.Scale, Color = Color.DarkGray };
            m_pressStartCmp.Visible = false;
            Menu.Owner.Attach(m_pressStartCmp);

            m_teamColorTextCmp = new TextComponent[2];

            m_teamColorTextCmp[0] = new TextComponent("Menu");
            m_teamColorTextCmp[0].Text = "Color :";
            m_teamColorTextCmp[0].Alignement = TextAlignementHorizontal.Center;
            m_teamColorTextCmp[0].Position = m_playerSpotPos[1] - Parameters.ControllersHeight * Vector2.UnitY;
            m_teamColorTextCmp[0].Style = new TextStyle() { Font = m_style.Font, Scale = m_style.Scale, Color = Color.White };
            m_teamColorTextCmp[0].Visible = true;
            Menu.Owner.Attach(m_teamColorTextCmp[0]);

            m_teamColorTextCmp[1] = new TextComponent("Menu");
            m_teamColorTextCmp[1].Text = "Color :";
            m_teamColorTextCmp[1].Alignement = TextAlignementHorizontal.Center;
            m_teamColorTextCmp[1].Position = m_playerSpotPos[3] - Parameters.ControllersHeight * Vector2.UnitY;
            m_teamColorTextCmp[1].Style = new TextStyle() { Font = m_style.Font, Scale = m_style.Scale, Color = Color.White };
            m_teamColorTextCmp[1].Visible = true;
            Menu.Owner.Attach(m_teamColorTextCmp[1]);

            //
            if (m_teamColorIndexes[0] == -1)
                m_teamColorIndexes[0] = 0;

            if (m_teamColorIndexes[1] == -1)
                m_teamColorIndexes[1] = m_colorSchemeDataAsset.Content.ColorSchemes.Length / 2;

            m_teamColorSpriteCmp = new ColorMaskedSprite[2];
            Sprite colorSprite = Sprite.CreateFromTexture("Graphics/Menu/ColorSelectionBase.png");
            var texAsset = Engine.AssetManager.GetAsset<Texture2D>("Graphics/Menu/ColorSelectionColorMask.png");

            m_teamColorSpriteCmp[0] = new ColorMaskedSprite(colorSprite, "Menu2");
            m_teamColorSpriteCmp[0].Position = m_teamColorTextCmp[0].Position - Parameters.ControllersHeight * Vector2.UnitY;
            m_teamColorSpriteCmp[0].Mask = texAsset.Content;
            m_teamColorSpriteCmp[0].Color1 = m_colorSchemeDataAsset.Content.ColorSchemes[m_teamColorIndexes[0]].Color1;
            m_teamColorSpriteCmp[0].Color2 = Color.White;
            m_teamColorSpriteCmp[0].Color3 = Color.White;
            m_teamColorSpriteCmp[0].Color4 = Color.White;
            Menu.Owner.Attach(m_teamColorSpriteCmp[0]);

            m_teamColorSpriteCmp[1] = new ColorMaskedSprite(colorSprite, "Menu2");
            m_teamColorSpriteCmp[1].Position = m_teamColorTextCmp[1].Position - Parameters.ControllersHeight * Vector2.UnitY;
            m_teamColorSpriteCmp[1].Mask = texAsset.Content;
            m_teamColorSpriteCmp[1].Color1 = m_colorSchemeDataAsset.Content.ColorSchemes[m_teamColorIndexes[1]].Color1;
            m_teamColorSpriteCmp[1].Color2 = Color.White;
            m_teamColorSpriteCmp[1].Color3 = Color.White;
            m_teamColorSpriteCmp[1].Color4 = Color.White;
            Menu.Owner.Attach(m_teamColorSpriteCmp[1]);

            //
            SpriteDefinition spriteDef = Engine.AssetManager.Get<SpriteDefinition>("Graphics/controlSprite.lua::Sprite");
            m_playerSpotCmp = new SpriteComponent[4];
            m_playerSpotTextCmp = new TextComponent[4];
            for (int i = 0; i < 4; i++)
            {
                m_playerSpotCmp[i] = new SpriteComponent(new Sprite(spriteDef), "Menu");
                m_playerSpotCmp[i].Position = m_playerSpotPos[i];
                m_playerSpotCmp[i].Sprite.Scale = iconSpriteScale * Vector2.One;
                Menu.Owner.Attach(m_playerSpotCmp[i]);

                m_playerSpotTextCmp[i] = new TextComponent("Menu");
                m_playerSpotTextCmp[i].Text = "AI";
                m_playerSpotTextCmp[i].Alignement = TextAlignementHorizontal.Center;
                m_playerSpotTextCmp[i].Style = m_style;
                m_playerSpotTextCmp[i].Position = m_playerSpotPos[i] - Parameters.ControllerTextHeightOffset * Vector2.UnitY;
                Menu.Owner.Attach(m_playerSpotTextCmp[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                var playerInfo = Game.GameSession.CurrentMatchInfo.Players[i];
                if (playerInfo.InputType != InputType.AI)
                {
                    foreach (var menuController in Game.MenuManager.Controllers)
                    {
                        if (menuController.InputIndex == playerInfo.InputIndex && menuController.Type == playerInfo.InputType)
                        {
                            AddController(menuController);
                            m_playerSlots[i] = menuController;
                            m_ctrlInfos[menuController].PlayerIndex = i;

                            if (i == 0 || i == 1)
                                m_ctrlInfos[menuController].State = ControllerState.Left;
                            else
                                m_ctrlInfos[menuController].State = ControllerState.Right;
                        }
                    }
                }
            }

            Menu.SelectItem(0);

            Game.GameMusic.PlayMenuMusic();
        }

        public override void Update()
        {
            bool readyToStart = true;
            foreach (var ctrl in Game.MenuManager.Controllers)
                if (m_ctrlInfos.ContainsKey(ctrl) && m_ctrlInfos[ctrl].PlayerIndex == -1) readyToStart = false;

            if (m_ctrlInfos.Count == 0)
            {
                QuitMenu();
                return;
            }

            //Check if a player wants to start the match
            bool startMatch = false;

            foreach (var ctrl in Game.MenuManager.Controllers)
            {
                //Start/Confirm: Activate controller
                if (!m_ctrlInfos.ContainsKey(ctrl) && 
                    (ctrl.StartCtrl.KeyPressed() || ctrl.ValidCtrl.KeyPressed()))
                {
                    AddController(ctrl);
                    readyToStart = false;

                    if (Menu.AudioCmpValid != null)
                        Menu.AudioCmpValid.Play();    
                }

                //Keyboard can always quit the menu
                if (ctrl.Type == InputType.Keyboard && !m_ctrlInfos.ContainsKey(ctrl) && ctrl.CancelCtrl.KeyPressed())
                {
                    m_ctrlInfos.Clear();
                }

                //If the controller is not active we stop here
                if (!m_ctrlInfos.ContainsKey(ctrl))
                    continue;

                var info = m_ctrlInfos[ctrl];

                //If the player has not selected a team
                if (info.PlayerIndex == -1)
                {
                    //Back: remove the controller
                    if (ctrl.CancelCtrl.KeyPressed())
                    {
                        RemoveController(ctrl);
                        readyToStart = false;
                        continue;
                    }
                }

                //If the player has a team
                if (info.PlayerIndex != -1)
                {
                    //Start/Confirm: Start the match if everybody is ready
                    if (ctrl.StartCtrl.KeyPressed() || ctrl.ValidCtrl.KeyPressed())
                    {
                        startMatch = true;
                    }
                    //Next/previous Cycle through team colors
                    else if (ctrl.SelectPreviousCtrl.KeyPressed() || ctrl.SelectNextCtrl.KeyPressed())
                    {
                        int colorIteration = ctrl.SelectNextCtrl.KeyPressed() ? 1 : -1;
                        ChangeTeamColor(info, colorIteration);
                    }
                    else if (ctrl.CancelCtrl.KeyPressed())
                    {
                        SetPlayerNeutral(ctrl);
                    }
                }

                //Left/Right: Change player team
                if (ctrl.LeftCtrl.KeyPressed())
                {
                    if (info.State == ControllerState.None)
                        SetPlayerLeft(ctrl);
                    else if (info.State == ControllerState.Right)
                        SetPlayerNeutral(ctrl);
                }
                else if (ctrl.RightCtrl.KeyPressed())
                {
                    if (info.State == ControllerState.None)
                        SetPlayerRight(ctrl);
                    else if (info.State == ControllerState.Left)
                        SetPlayerNeutral(ctrl);
                }
            }

            if (startMatch)
            {
                if (readyToStart)
                {
                    StartMatch();

                    if (Menu.AudioCmpValid != null)
                        Menu.AudioCmpValid.Play();
                }
                else
                {
                    foreach (var ctrlInfo in m_ctrlInfos.Values)
                    {
                        if (ctrlInfo.PlayerIndex == -1)
                        {
                            ctrlInfo.SpriteCmp.Owner.Attach(new MenuScaleFx(ctrlInfo.TextCmp));
                            ctrlInfo.SpriteCmp.Owner.Attach(new MenuScaleFx(ctrlInfo.SpriteCmp));
                        }
                    }

                    if (Menu.AudioCmpCancel != null)
                        Menu.AudioCmpCancel.Play();
                }
            }

            UpdateGraphics();
        }

        private void UpdateGraphics()
        {
            //Update player spots sprites
            for (int i = 0; i < 4; i++)
            {
                if (m_playerSlots[i] == null)
                    m_playerSpotCmp[i].Sprite.SetAnimation("AI");
                else if (m_playerSlots[i].Type == InputType.Gamepad)
                    m_playerSpotCmp[i].Sprite.SetAnimation("Gamepad");
                else if (m_playerSlots[i].Type == InputType.Keyboard)
                    m_playerSpotCmp[i].Sprite.SetAnimation("Keyboard");

                m_playerSpotCmp[i].Sprite.AnimationIndex = i + 2;
                m_playerSpotTextCmp[i].Text = GetControllerText(m_playerSlots[i]);
            }

            foreach (var info in m_ctrlInfos.Values)
            {
                Vector2 stringOffset = new Vector2(0, -yOffset * 0.5f);
                String controllerString = info.Controller.Type == InputType.Keyboard ?
                    "Keyboard" :
                    "Gamepad " + (info.ControllerIndex + 1);

                info.SpriteCmp.Position = Parameters.Offset * Vector2.UnitX + Parameters.ControllersHeight * Vector2.UnitY - info.ControllerIndex * Parameters.ControllersHeightOffset * Vector2.UnitY;
                info.TextCmp.Position = Parameters.Offset * Vector2.UnitX + Parameters.ControllersHeight * Vector2.UnitY - info.ControllerIndex * Parameters.ControllersHeightOffset * Vector2.UnitY - Parameters.ControllerTextHeightOffset * Vector2.UnitY;

                if (info.State == ControllerState.None)
                {
                    info.SpriteCmp.Visible = true;
                    info.SpriteCmp.Sprite.AnimationIndex = 1;
                    info.TextCmp.Visible = true;
                }
                else
                {
                    info.SpriteCmp.Visible = true;
                    info.SpriteCmp.Sprite.AnimationIndex = 0;
                    info.TextCmp.Visible = false;
                }
            }

            m_pressStartCmp.Visible = false;
            if (m_ctrlInfos.Values.Count < 4)
            {
                int posIndex = m_ctrlInfos.Values.Count;
                m_pressStartCmp.Visible = true;
                m_pressStartCmp.Position = Parameters.Offset * Vector2.UnitX + Parameters.ControllersHeight * Vector2.UnitY - posIndex * Parameters.ControllersHeightOffset * Vector2.UnitY;
            }
        }

        private void ChangeTeamColor(ControllerInfo info, int colorIteration)
        {
            int teamIndex = 0;
            int otherTeamIndex = 1;
            if (info.PlayerIndex == 2 || info.PlayerIndex == 3)
            {
                teamIndex = 1;
                otherTeamIndex = 0;
            }
            int colorSchemesLength = m_colorSchemeDataAsset.Content.ColorSchemes.Length;
            m_teamColorIndexes[teamIndex] += colorIteration;
            m_teamColorIndexes[teamIndex] = (m_teamColorIndexes[teamIndex] + colorSchemesLength) % colorSchemesLength;
            if (m_teamColorIndexes[teamIndex] == m_teamColorIndexes[otherTeamIndex])
            {
                m_teamColorIndexes[teamIndex] += colorIteration;
                m_teamColorIndexes[teamIndex] = (m_teamColorIndexes[teamIndex] + colorSchemesLength) % colorSchemesLength;
            }
            m_teamColorSpriteCmp[teamIndex].Color1 = m_colorSchemeDataAsset.Content.ColorSchemes[m_teamColorIndexes[teamIndex]].Color1;
        }

        public void AddController(MenuController ctrl)
        {
            if (m_ctrlInfos.ContainsKey(ctrl))
                return;

            ControllerInfo info = new ControllerInfo();
            info.State = ControllerState.None;
            info.Controller = ctrl;
            info.ControllerIndex = m_ctrlInfos.Keys.Count;
            info.PlayerIndex = -1;

            Sprite edgeSprite = Sprite.Create("Graphics/controlSprite.lua::Sprite");
            info.SpriteCmp = new SpriteComponent(edgeSprite, "Menu");
            if (ctrl.Type == InputType.Gamepad)
                edgeSprite.SetAnimation("Gamepad");
            else
                edgeSprite.SetAnimation("Keyboard");
            info.SpriteCmp.Visible = false;
            info.SpriteCmp.Sprite.Scale = iconSpriteScale * Vector2.One;
            Menu.Owner.Attach(info.SpriteCmp);

            info.TextCmp = new TextComponent("Menu");
            info.TextCmp.Style = m_style;
            info.TextCmp.Alignement = TextAlignementHorizontal.Center;
            info.TextCmp.Text = GetControllerText(ctrl);
            info.TextCmp.Visible = false;
            Menu.Owner.Attach(info.TextCmp);

            m_ctrlInfos.Add(ctrl, info);
        }

        public void RemoveController(MenuController ctrl)
        {
            if (!m_ctrlInfos.ContainsKey(ctrl))
                return;

            //Remove sprites
            var info = m_ctrlInfos[ctrl];
            Menu.Owner.Remove(info.SpriteCmp);
            Menu.Owner.Remove(info.TextCmp);

            //If controller has a playerSpot, release it
            if (info.PlayerIndex != -1)
            {
                var playerSpot = m_playerSlots[info.PlayerIndex];
                playerSpot.Type = InputType.AI;
            }

            //Remove the controller
            m_ctrlInfos.Remove(ctrl);

            //Update other controller index accordingly
            foreach (var otherCtrlInfo in m_ctrlInfos.Values)
                if (otherCtrlInfo.ControllerIndex > info.ControllerIndex) otherCtrlInfo.ControllerIndex--;
        }

        private String GetControllerText(MenuController ctrl)
        {
            String controllerTxt = "AI";
            if (ctrl != null)
            {
                if (ctrl.Type == InputType.Gamepad)
                    controllerTxt = "Gamepad " + (1 + (int)ctrl.InputIndex);
                else if (ctrl.Type == InputType.Keyboard)
                    controllerTxt = "Keyboard";
            }
            return controllerTxt;
        }

        private bool StartMatch()
        {
            if (m_ctrlInfos.Keys.Count == 0)
                return false;

            //Set the player controllers to MatchInfo
            for (int i = 0; i < 4; i++)
            {
                if (m_playerSlots[i] != null)
                {
                    Game.GameSession.CurrentMatchInfo.Players[i].InputType = m_playerSlots[i].Type;
                    Game.GameSession.CurrentMatchInfo.Players[i].InputIndex = m_playerSlots[i].InputIndex;
                }
                else
                {
                    Game.GameSession.CurrentMatchInfo.Players[i].InputType = InputType.AI;
                }
            }

            for (int i = 0; i < 2; i++)
            {
                Game.GameSession.CurrentMatchInfo.Teams[i].ColorScheme = m_colorSchemeDataAsset.Content.ColorSchemes[m_teamColorIndexes[i]];
            }

            var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/SelectMapMenu.lua::Menu");
            Game.MenuManager.StartMenu(menuDef);

            return true;
        }

        private void SetPlayerNeutral(MenuController ctrl)
        {
            if (m_ctrlInfos[ctrl].State != ControllerState.None)
            {
                if (Menu.AudioCmpSelect != null)
                    Menu.AudioCmpSelect.Play();

                m_playerSlots[m_ctrlInfos[ctrl].PlayerIndex] = null;
                m_ctrlInfos[ctrl].PlayerIndex = -1;
                m_ctrlInfos[ctrl].State = ControllerState.None;
            }
        }

        private bool SetPlayerLeft(MenuController ctrl)
        {
            if (m_playerSlots[0] == null)
            {
                if (Menu.AudioCmpSelect != null)
                    Menu.AudioCmpSelect.Play();

                m_playerSlots[0] = ctrl;
                m_ctrlInfos[ctrl].PlayerIndex = 0;
                m_ctrlInfos[ctrl].State = ControllerState.Left;
                return true;
            }
            else if (m_playerSlots[1] == null)
            {
                if (Menu.AudioCmpSelect != null)
                    Menu.AudioCmpSelect.Play();

                m_playerSlots[1] = ctrl;
                m_ctrlInfos[ctrl].PlayerIndex = 1;
                m_ctrlInfos[ctrl].State = ControllerState.Left;
                return true;
            }

            return false;
        }

        private bool SetPlayerRight(MenuController ctrl)
        {
            if (m_playerSlots[2] == null)
            {
                if (Menu.AudioCmpSelect != null)
                    Menu.AudioCmpSelect.Play();

                m_playerSlots[2] = ctrl;
                m_ctrlInfos[ctrl].PlayerIndex = 2;
                m_ctrlInfos[ctrl].State = ControllerState.Right;
                return true;
            }
            else if (m_playerSlots[3] == null)
            {
                if (Menu.AudioCmpSelect != null)
                    Menu.AudioCmpSelect.Play();

                m_playerSlots[3] = ctrl;
                m_ctrlInfos[ctrl].PlayerIndex = 3;
                m_ctrlInfos[ctrl].State = ControllerState.Right;
                return true;
            }

            return false;
        }

        private void QuitMenu()
        {
            m_ctrlInfos.Clear();
            var menuDef = Engine.AssetManager.Get<Menus.MenuDefinition>("Interface/MainMenu.lua::Menu");
            Game.GameSession.CurrentMatchInfo = new Gameplay.MatchStartInfo();
            Game.MenuManager.StartMenu(menuDef);
        }

        public override void OnItemValid(string name, MenuController controller)
        {
            //Sound for validation is handled differently
        }
    }
}
