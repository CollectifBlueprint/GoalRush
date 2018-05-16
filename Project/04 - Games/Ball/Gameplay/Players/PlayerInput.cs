using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LBE.Input;
using Microsoft.Xna.Framework;

namespace Ball.Gameplay
{
    [Flags]
    public enum InputType
    {
        AI = 0,
        Gamepad = 1,
        Keyboard = 2,
        GamepadAndKeyboard = 3,
        MouseKeyboard = 4,
        MouseKeyboardAndGamepad = 5,
    }

    public class PlayerInput
    {
        StickControl m_moveCtrl;
        public StickControl MoveCtrl
        {
            get { return m_moveCtrl; }
        }

        StickControl m_ballCtrl;
        public StickControl BallCtrl
        {
            get { return m_ballCtrl; }
        }

        KeyControl m_ballLaunchCtrl;
        public KeyControl BallLaunchCtrl
        {
            get { return m_ballLaunchCtrl; }
        }

        KeyControl m_specialCtrl;
        public KeyControl SpecialCtrl
        {
            get { return m_specialCtrl; }
        }

        KeyControl m_changePlayerCtrl;
        public KeyControl ChangePlayerCtrl
        {
            get { return m_changePlayerCtrl; }
        }

        KeyControl m_pauseCtrl;
        public KeyControl PauseCtrl
        {
            get { return m_pauseCtrl; }
        }

        PlayerIndex m_inputIndex;
        public PlayerIndex InputIndex
        {
            get { return m_inputIndex; }
        }

        InputType m_inputType;
        public InputType InputType
        {
            get { return m_inputType; }
        }

        public static PlayerInput CreateHuman(InputType type, PlayerIndex index)
        {
            PlayerInput pc = new PlayerInput();
            pc.m_inputType = type;

            pc.m_moveCtrl = new StickControl();
            pc.m_ballCtrl = new StickControl();

            pc.m_inputIndex = index;

            InputCombiner ballLaunchInput = new InputCombiner();
            pc.m_ballLaunchCtrl = new KeyControl(ballLaunchInput);

            InputCombiner specialInput = new InputCombiner();
            pc.m_specialCtrl = new KeyControl(specialInput);

            InputCombiner changeInput = new InputCombiner();
            pc.m_changePlayerCtrl = new KeyControl(changeInput);

            InputCombiner pauseInput = new InputCombiner();
            pc.m_pauseCtrl = new KeyControl(pauseInput);

            if ((type & InputType.Gamepad) == InputType.Gamepad)
            {
                pc.m_moveCtrl.Inputs.Add(new PadThumbstickInput(ThumbSticks.Left, index));
                pc.m_ballCtrl.Inputs.Add(new PadThumbstickInput(ThumbSticks.Right, index));
                ballLaunchInput.Add(new GamepadButtonInput(Microsoft.Xna.Framework.Input.Buttons.RightShoulder, index));
                specialInput.Add(new GamepadButtonInput(Microsoft.Xna.Framework.Input.Buttons.LeftShoulder, index));
                changeInput.Add(new GamepadButtonInput(Microsoft.Xna.Framework.Input.Buttons.A, index));
                pauseInput.Add(new GamepadButtonInput(Microsoft.Xna.Framework.Input.Buttons.Start, index));
                pauseInput.Add(new KeyboardInput(Microsoft.Xna.Framework.Input.Keys.Escape));
            }

            if ((type & InputType.Keyboard) == InputType.Keyboard)
            {
                pc.m_moveCtrl.Inputs.Add(KeyInput2D.FromKeyboardZQSD());
                pc.m_ballCtrl.Inputs.Add(KeyInput2D.FromKeyboardArrows());
                ballLaunchInput.Add(new KeyboardInput(Microsoft.Xna.Framework.Input.Keys.Space));
                specialInput.Add(new KeyboardInput(Microsoft.Xna.Framework.Input.Keys.LeftShift));
                pauseInput.Add(new KeyboardInput(Microsoft.Xna.Framework.Input.Keys.Escape));
            }

            if ((type & InputType.MouseKeyboard) == InputType.MouseKeyboard)
            {
                pc.m_moveCtrl.Inputs.Add(KeyInput2D.FromKeyboardZQSD());
                pc.m_ballCtrl.Inputs.Add(new CustomInput2D());
                ballLaunchInput.Add(new KeyboardInput(Microsoft.Xna.Framework.Input.Keys.Space));
                ballLaunchInput.Add(new MouseButtonInput(MouseButtons.Left));
                specialInput.Add(new KeyboardInput(Microsoft.Xna.Framework.Input.Keys.LeftShift));
                specialInput.Add(new MouseButtonInput(MouseButtons.Right));
                pauseInput.Add(new KeyboardInput(Microsoft.Xna.Framework.Input.Keys.Escape));
            }

            return pc;
        }
    }
}
