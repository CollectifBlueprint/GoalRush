using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ball.Menus
{
    public class MenuScript
    {
        public Menu Menu;

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void End() { }

        public virtual void OnItemValid(String name, MenuController controller)
        {
            if (Menu.AudioCmpValid != null)
                Menu.AudioCmpValid.Play();
        }

        public virtual void OnItemToggle(String name, MenuController controller) 
        {
            if (Menu.AudioCmpSelectAlt != null)
                Menu.AudioCmpSelectAlt.Play();
        }

        public virtual void OnItemSlider(String name, MenuController controller)
        {
            if (Menu.AudioCmpSelectAlt != null)
                Menu.AudioCmpSelectAlt.Play();
        }

        public virtual void OnCancel(String name, MenuController controller)
        {
            if (Menu.AudioCmpCancel != null)
                Menu.AudioCmpCancel.Play();
        }
        public virtual void OnItemSelect(String name, MenuController controller)
        {
            if (Menu.AudioCmpCancel != null)
                Menu.AudioCmpCancel.Play();
        }
    }
}
