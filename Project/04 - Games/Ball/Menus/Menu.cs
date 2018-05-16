using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using LBE.Gameplay;
using LBE.Graphics.Sprites;
using LBE;
using Microsoft.Xna.Framework.Graphics;
using LBE.Assets;
using LBE.Audio;

namespace Ball.Menus
{
    public class MenuParameters
    {
        public float BlobPeriod;
        public float BlobScale;

        public Vector2 ItemPosition;
        public Vector2 ItemOffset;

        public Vector2 ItemToggleOnOffset;
        public Vector2 ItemToggleOffOffset;

        public Vector2 ItemSliderOffset;

         
    }

    public class MenuSoundDefinitions
    {
        public SoundDefinition Select;
        public SoundDefinition SelectAlt;
        public SoundDefinition Valid;
        public SoundDefinition Cancel;
  
  }

    public class MenuDefinition
    {
        public String Name;
        public TextAlignementHorizontal Alignement = TextAlignementHorizontal.Left;

        public MenuScript Script;
        public MenuItemDefinition[] Items = new MenuItemDefinition[0];
        public MenuSoundDefinitions Sounds;
    }

    public class Menu : GameObjectComponent
    {
        MenuDefinition m_definition;

        Asset<MenuParameters> m_parameters;
        public MenuParameters Parameters
        {
            get { return m_parameters.Content; }
        }

        MenuScript m_script;
        public MenuScript Script
        {
            get { return m_script; }
        }

        String m_name;
        Dictionary<String, MenuItem> m_items;
        public Dictionary<String, MenuItem> Items
        {
            get { return m_items; }
        }

        int m_itemIndex;
        public int SelectedItem
        {
            get { return m_itemIndex; }
        }

        TextStyle m_selectedStyle;
        TextStyle m_normalStyle;
        TextStyle m_disabledStyle;

        AudioComponent m_audioCmpSelect;
        public AudioComponent AudioCmpSelect
        {
            get { return m_audioCmpSelect; }
        }

        AudioComponent m_audioCmpSelectAlt;
        public AudioComponent AudioCmpSelectAlt
        {
            get { return m_audioCmpSelectAlt; }
        }

        AudioComponent m_audioCmpValid;
        public AudioComponent AudioCmpValid
        {
            get { return m_audioCmpValid; }
        }

        AudioComponent m_audioCmpCancel;
        public AudioComponent AudioCmpCancel
        {
            get { return m_audioCmpCancel; }
        }


        public Menu(MenuDefinition menuDef)
        {
            m_definition = menuDef;

            m_parameters = Engine.AssetManager.GetAsset<MenuParameters>("Game/Menu.lua::Menu");

            m_name = menuDef.Name;

            m_items = new Dictionary<string, MenuItem>();
            m_itemIndex = 0;
            if (m_definition.Sounds !=null)
            {
                if (m_definition.Sounds.Select != null)
                    m_audioCmpSelect = new AudioComponent(m_definition.Sounds.Select);
                if (m_definition.Sounds.SelectAlt != null)
                    m_audioCmpSelectAlt = new AudioComponent(m_definition.Sounds.SelectAlt);
                if (m_definition.Sounds.Valid != null)
                    m_audioCmpValid = new AudioComponent(m_definition.Sounds.Valid);
                if (m_definition.Sounds.Cancel != null)
                    m_audioCmpCancel = new AudioComponent(m_definition.Sounds.Cancel);
            }

            SpriteFont font = Engine.AssetManager.Get<SpriteFont>("Graphics/Menu/BigMacUnselectedPen");
            SpriteFont fontSelected = Engine.AssetManager.Get<SpriteFont>("Graphics/Menu/BigMacSelected");
            m_normalStyle = new TextStyle() { Scale = 1.0f, Color = Color.White, Font = font };
            m_selectedStyle = new TextStyle() { Scale = 1.0f, Color = Color.White, Font = fontSelected };

            float colorCoef = .5f;
            m_disabledStyle = new TextStyle() { Scale = 1.0f, Color = Color.White, Font = font };
            m_disabledStyle.Color = new Color(m_disabledStyle.Color.R / 255.0f * colorCoef, m_disabledStyle.Color.G / 255.0f * colorCoef, m_disabledStyle.Color.B / 255.0f * colorCoef, m_disabledStyle.Color.A);
     
        }

        public override void Start()
        {
            Vector2 basePos = Parameters.ItemPosition;
            Vector2 itemOffset = Parameters.ItemOffset;

            if (m_definition.Alignement == TextAlignementHorizontal.Center)
                basePos.X = 0;
            else if (m_definition.Alignement == TextAlignementHorizontal.Right)
                basePos.X = -basePos.X;

            Vector2 posAcc = Vector2.Zero;
            foreach (var itemDef in m_definition.Items)
            {
                MenuItem item = new MenuItem();
                item.Name = itemDef.Name;
                item.Position = basePos + posAcc;
                item.Text = (char)127 + " " + itemDef.Text;
                item.TextComponent = new TextComponent("Menu");
                item.Type = itemDef.Type;
                item.Enabled = itemDef.Enabled;
                m_items.Add(item.Name, item);

                item.TextComponent.Position = item.Position;
                item.TextComponent.AttachedToOwner = false;
                item.TextComponent.Text = item.Text;
                item.TextComponent.Alignement = m_definition.Alignement;
                item.TextComponent.AlignementVertical = TextAlignementVertical.Center;
                Owner.Attach(item.TextComponent);

                if (itemDef.Type == MenuItemType.Toggle)
                {
                    item.ToggleOffTextComponent = new TextComponent("Menu");
                    item.ToggleOffTextComponent.Position = item.Position;
                    item.ToggleOffTextComponent.AttachedToOwner = false;
                    item.ToggleOffTextComponent.Text = "off";
                    item.ToggleOffTextComponent.Alignement = m_definition.Alignement;
                    item.ToggleOffTextComponent.AlignementVertical = TextAlignementVertical.Center;
                    Owner.Attach(item.ToggleOffTextComponent);

                    item.ToggleOnTextComponent = new TextComponent("Menu");
                    item.ToggleOnTextComponent.Position = item.Position;
                    item.ToggleOnTextComponent.AttachedToOwner = false;
                    item.ToggleOnTextComponent.Text = "on";
                    item.ToggleOnTextComponent.Alignement = m_definition.Alignement;
                    item.ToggleOnTextComponent.AlignementVertical = TextAlignementVertical.Center;
                    Owner.Attach(item.ToggleOnTextComponent);
                }

                if (itemDef.Type == MenuItemType.Slider)
                {
                    int sliderCount = 8;
                    item.SliderComponents = new TextComponent[sliderCount];

                    var sliderChar = (char)127;
                    for (int i = 0; i < item.SliderComponents.Length; i++)
                    {
                        var txtCmp = new TextComponent("Menu");
                        txtCmp.Text = sliderChar.ToString();
                        txtCmp.Style = m_disabledStyle;
                        txtCmp.AlignementVertical = TextAlignementVertical.Center;
                        txtCmp.Alignement = TextAlignementHorizontal.Center;
                        txtCmp.Position = item.Position + i * m_disabledStyle.Font.MeasureString(txtCmp.Text).X * 1.5f * Vector2.UnitX;
                        Owner.Attach(txtCmp);

                        item.SliderComponents[i] = txtCmp;
                    }

                }

                posAcc += itemOffset;
            }

            m_script = m_definition.Script;
            m_script.Menu = this;
            m_script.Start();

            if (m_items.Count != 0 && GetItemByIndex(m_itemIndex).Enabled == false)
                SelectNextItem(1, false, null);

            UpdateItems();
        }

        public override void Update()
        {
            m_script.Update();

            //Update Item selection
            if (m_items.Count > 0)
            {
                int itemOffset = 0;
                MenuController ctrlSelect = null;
                foreach (var ctrl in Game.MenuManager.Controllers)
                    if (ctrl.UpCtrl.KeyPressed())
                    {
                        itemOffset = -1;
                        ctrlSelect = ctrl;
                    }

                foreach (var ctrl in Game.MenuManager.Controllers)
                    if (ctrl.DownCtrl.KeyPressed())
                    {
                        itemOffset = 1;
                        ctrlSelect = ctrl;
                    }

                SelectNextItem(itemOffset, true, ctrlSelect);

            }

            //Update menu interaction
            if (m_items.Count == 0)
            {
                foreach (var ctrl in Game.MenuManager.Controllers)
                {
                    if (ctrl.ValidCtrl.KeyPressed())
                        m_script.OnItemValid(null, ctrl);

                    if (ctrl.CancelCtrl.KeyPressed())
                        m_script.OnCancel(null, ctrl);
                }
            }
            else
            {
                //Update Item interaction
                foreach (var ctrl in Game.MenuManager.Controllers)
                {
                    var selectedItem = GetItemByIndex(m_itemIndex);

                    if (ctrl.CancelCtrl.KeyPressed())
                        m_script.OnCancel(selectedItem.Name, ctrl);

                    if (selectedItem.Type == MenuItemType.Button)
                    {
                        if (ctrl.ValidCtrl.KeyPressed())
                            m_script.OnItemValid(selectedItem.Name, ctrl);
                    }

                    if (selectedItem.Type == MenuItemType.Slider)
                    {
                        if (ctrl.ValidCtrl.KeyPressed() || ctrl.RightCtrl.KeyPressed())
                        {
                            var newSliderState = LBE.MathHelper.Clamp(0, selectedItem.SliderComponents.Length, selectedItem.SliderState + 1);
                            SetSlider(m_itemIndex, newSliderState);
                            m_script.OnItemSlider(selectedItem.Name, ctrl);
                        }
                        else if (ctrl.ValidCtrl.KeyPressed() || ctrl.LeftCtrl.KeyPressed())
                        {
                            var newSliderState = LBE.MathHelper.Clamp(0, selectedItem.SliderComponents.Length, selectedItem.SliderState - 1);
                            SetSlider(m_itemIndex, newSliderState);
                            m_script.OnItemSlider(selectedItem.Name, ctrl);
                        }
                    }

                    if (selectedItem.Type == MenuItemType.Toggle)
                    {
                        if (ctrl.ValidCtrl.KeyPressed())
                            m_script.OnItemToggle(selectedItem.Name, ctrl);
                        else if (ctrl.RightCtrl.KeyPressed() && selectedItem.ToggleState == true)
                            m_script.OnItemToggle(selectedItem.Name, ctrl);
                        else if (ctrl.LeftCtrl.KeyPressed() && selectedItem.ToggleState == false)
                            m_script.OnItemToggle(selectedItem.Name, ctrl);
                    }
                }
            }

            UpdateItems();
        }

        private void UpdateItems()
        {
            for (int i = 0; i < m_items.Count; i++)
            {
                var item = GetItemByIndex(i);
                var style = i == m_itemIndex ? m_selectedStyle : m_normalStyle;

                item.TextComponent.Style = style;

                if (item.Enabled == false)
                    item.TextComponent.Style = m_disabledStyle;

                if (item.ToggleOffTextComponent != null)
                {
                    item.ToggleOffTextComponent.Style = style;
                    item.ToggleOnTextComponent.Style = style;

                    if (item.ToggleState == false)
                        item.ToggleOnTextComponent.Style = m_disabledStyle;
                    else
                        item.ToggleOffTextComponent.Style = m_disabledStyle;

                    float offset = style.Font.MeasureString(" ").X * style.Scale;
                    item.ToggleOffTextComponent.Position = item.TextComponent.Position
                        + Parameters.ItemToggleOffOffset;

                    item.ToggleOnTextComponent.Position = item.TextComponent.Position
                        + Parameters.ItemToggleOnOffset;
                }

                if (item.SliderComponents != null)
                {
                    for (int iSlider = 0; iSlider < item.SliderComponents.Length; iSlider++)
                    {
                        if (iSlider == item.SliderState - 1 && i == m_itemIndex)
                            item.SliderComponents[iSlider].Style = m_selectedStyle;
                        else if (iSlider < item.SliderState)
                            item.SliderComponents[iSlider].Style = m_normalStyle;
                        else
                            item.SliderComponents[iSlider].Style = m_disabledStyle;

                        item.SliderComponents[iSlider].Position = item.TextComponent.Position
                            + Parameters.ItemSliderOffset
                            + iSlider * item.SliderComponents[iSlider].Style.Font.MeasureString(item.SliderComponents[iSlider].Text).X * 1.2f * Vector2.UnitX;
                    }
                }
            }
        }

        public override void End()
        {
            m_script.End();
        }

        public MenuItem GetItemByIndex(int index)
        {
            String itemName = m_definition.Items[index].Name;
            return m_items[itemName];
        }

        private void SelectNextItem(int direction, bool playSound, MenuController ctrl)
        {
            if (direction == 0)
                return;

            for (int i = 1; i < m_items.Count; i++)
            {
                int idx = (m_itemIndex + i * direction + 2 * m_items.Count) % m_items.Count;
                var item = GetItemByIndex(idx);
                if (item.Enabled)
                {
                    SelectItem(idx);
                    if (playSound && m_audioCmpSelect != null)
                        m_audioCmpSelect.Play();
                    var selectedItem = GetItemByIndex(m_itemIndex);
                    m_script.OnItemSelect(selectedItem.Name, ctrl);
                    break;
                }
            }
        }

        public void SelectItem(int index)
        {
            m_itemIndex = index;

            var item = GetItemByIndex(m_itemIndex);
            Owner.Attach(new MenuScaleFx(item.TextComponent));
        }

        public void SetSlider(int itemIndex, int sliderValue)
        {
            var item = GetItemByIndex(m_itemIndex);
            item.SliderState = sliderValue;

            if(sliderValue > 0)
                Owner.Attach(new MenuScaleFx(item.SliderComponents[sliderValue - 1]));
        }

        public void SetToggle(int index, bool value)
        {
            var item = GetItemByIndex(m_itemIndex);
            item.ToggleState = value;

            if (value == true)
                Owner.Attach(new MenuScaleFx(item.ToggleOnTextComponent));
            else
                Owner.Attach(new MenuScaleFx(item.ToggleOffTextComponent));
        }
    }
}
