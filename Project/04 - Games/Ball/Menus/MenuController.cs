using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ball.Gameplay;
using Microsoft.Xna.Framework;
using LBE.Input;
using Microsoft.Xna.Framework.Input;

using InputType = Ball.Gameplay.InputType;

namespace Ball.Menus
{
    public class MenuController
    {
        public InputType Type;
        public PlayerIndex InputIndex;

        public KeyControl UpCtrl;
        public KeyControl DownCtrl;
        public KeyControl LeftCtrl;
        public KeyControl RightCtrl;

        public KeyControl StartCtrl;
        public KeyControl ValidCtrl;
        public KeyControl CancelCtrl;

        public KeyControl SelectNextCtrl;
        public KeyControl SelectPreviousCtrl;


        public static MenuController Keyboard()
        {
            MenuController ctrl = new MenuController();
            ctrl.Type = InputType.Keyboard;
            ctrl.InputIndex = PlayerIndex.One;
            
            ctrl.UpCtrl = new KeyControl(Keys.Up);
            ctrl.DownCtrl = new KeyControl(Keys.Down);
            ctrl.LeftCtrl = new KeyControl(Keys.Left);
            ctrl.RightCtrl = new KeyControl(Keys.Right);

            ctrl.StartCtrl = new KeyControl(Keys.Space);
            ctrl.ValidCtrl = new KeyControl(Keys.Enter);
            ctrl.CancelCtrl = new KeyControl(Keys.Escape);

            ctrl.SelectNextCtrl = new KeyControl(Keys.LeftShift);
            ctrl.SelectPreviousCtrl = new KeyControl(Keys.LeftControl);

            return ctrl;
        }

        public static MenuController Gamepad(PlayerIndex inputIndex)
        {
            MenuController ctrl = new MenuController();
            ctrl.Type = InputType.Gamepad;
            ctrl.InputIndex = inputIndex;

            ctrl.UpCtrl = new KeyControl(
                new InputCombiner(
                    new GamepadButtonInput(Buttons.DPadUp, inputIndex),
                    new GamepadButtonInput(Buttons.LeftThumbstickUp, inputIndex)));
            ctrl.DownCtrl = new KeyControl(
                new InputCombiner(
                    new GamepadButtonInput(Buttons.DPadDown, inputIndex),
                    new GamepadButtonInput(Buttons.LeftThumbstickDown, inputIndex)));
            ctrl.LeftCtrl = new KeyControl(
                new InputCombiner(
                    new GamepadButtonInput(Buttons.DPadLeft, inputIndex),
                    new GamepadButtonInput(Buttons.LeftThumbstickLeft, inputIndex)));
            ctrl.RightCtrl = new KeyControl(
                new InputCombiner(
                    new GamepadButtonInput(Buttons.DPadRight, inputIndex),
                    new GamepadButtonInput(Buttons.LeftThumbstickRight, inputIndex)));

            ctrl.StartCtrl = new KeyControl(new GamepadButtonInput(Buttons.Start, inputIndex));
            ctrl.ValidCtrl = new KeyControl(new GamepadButtonInput(Buttons.A, inputIndex));
            ctrl.CancelCtrl = new KeyControl(new GamepadButtonInput(Buttons.B, inputIndex));

            ctrl.SelectNextCtrl = new KeyControl(new GamepadButtonInput(Buttons.RightShoulder, inputIndex));
            ctrl.SelectPreviousCtrl = new KeyControl(new GamepadButtonInput(Buttons.LeftShoulder, inputIndex));

            return ctrl;
        }
    }
}
