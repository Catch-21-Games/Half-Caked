﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Half_Caked
{
    class KeybindingDialog : MessageBoxScreen
    {
        Keybinding newKey = null;
        string keybinding = null;
        string message = null;
        ReturnKeybindingInput return_method = null;

        public KeybindingDialog(string keybinding, ReturnKeybindingInput method)
            : base("", new string[] {"Accept", "Cancel"}, 0)
        {
            this.keybinding = keybinding;
            message += "Set the keybindings for: " + keybinding + "\n\n";
            
            IsPopup = true;
            this.return_method = method;
            
            // Hook up menu event handlers.
            Buttons[0].Pressed += AcceptSelected;

            this.mMessage += message;
        }

        public delegate void ReturnKeybindingInput(Keybinding input);

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            Keybinding tmpKey = input.GetNewestKeybindingPressed(this.ControllingPlayer);
            if (tmpKey != null) {
                // Reset the message to be our "default" + the new keybinding
                this.mMessage = this.message + "Key [" + tmpKey.ToString() + "] Pressed";
                newKey = tmpKey;
            }
        }

        void AcceptSelected(object sender, PlayerIndexEventArgs e)
        {
            // Tell the Keybinding screen what was selected...
            this.return_method.Invoke(this.newKey);
        }        
    }
}
