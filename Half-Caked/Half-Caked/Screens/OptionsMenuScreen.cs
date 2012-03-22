#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using System.Collections.Generic;
#endregion

using KeybindingKV = System.Collections.Generic.KeyValuePair<string, Half_Caked.Keybinding[]>;
namespace Half_Caked
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            MenuEntry profileMenuEntry = new MenuEntry("Profile");
            MenuEntry resolutionMenuEntry = new MenuEntry("Resolution");
            MenuEntry soundMenuEntry = new MenuEntry("Sound");
            MenuEntry keybindingsMenuEntry = new MenuEntry("Keybindings");
            MenuEntry backMenuEntry = new MenuEntry("Back");

            // Hook up menu event handlers.
            profileMenuEntry.Selected += ProfileMenuEntrySelected;
            resolutionMenuEntry.Selected += ResolutionMenuEntrySelected;
            soundMenuEntry.Selected += SoundMenuEntrySelected;
            keybindingsMenuEntry.Selected += KeybindingsMenuEntrySelected;
            backMenuEntry.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(profileMenuEntry);
            MenuEntries.Add(resolutionMenuEntry);
            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(keybindingsMenuEntry);
            MenuEntries.Add(backMenuEntry);
        }

         #endregion

        #region Handle Input

        void ProfileMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            var device = (ScreenManager.Game as HalfCakedGame).Device;
            MessageBoxScreen msgbox;
            if (device != null)
                msgbox = new ProfileSelectionScreen((ScreenManager.Game as HalfCakedGame).Device);
            else
                msgbox = new MessageBoxScreen("Unable to write to your documents folder. Cannot save profiles.", new string[]{"Ok"}, 0);

            ScreenManager.AddScreen(msgbox, null);
        }

        void KeybindingsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            var scrn = new KeybindingsScreen((ScreenManager.Game as HalfCakedGame).CurrentProfile);

            ScreenManager.AddScreen(scrn, null);
        }

        void ResolutionMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            var scrn = new GraphicsScreen((ScreenManager.Game as HalfCakedGame).CurrentProfile);

            ScreenManager.AddScreen(scrn, null);
        }
        
        void SoundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            var scrn = new AudioOptionsScreen((ScreenManager.Game as HalfCakedGame).CurrentProfile);
            ScreenManager.AddScreen(scrn, null);
        }
        
        #endregion
    }

    class AudioOptionsScreen : MenuScreen
    {
        public AudioOptionsScreen(Profile curProfile)
            : base("Audio Settings")
        {
            MenuEntry backMenuEntry = new MenuEntry("Back");
            backMenuEntry.Selected += OnCancel;
            MenuEntries.Add(backMenuEntry);

            mProfile = curProfile;
        }

        private Profile mProfile;

        void SaveButton(object sender, PlayerIndexEventArgs e) { }
    }

    //This is just an example, the resolutions/display mode need to be made intelligable and the methods need to be implemented
    class GraphicsScreen : MenuScreen
    {
        public GraphicsScreen(Profile curProfile)
            : base("Graphics Settings")
        {
            // Create our menu entries.
            MenuEntry displayModeMenuEntry = new MenuEntry("Display Mode: ", new string[3]{"W", "W (NB)", "FS"});
            MenuEntry resolutionMenuEntry = new MenuEntry("Resolutions: ", new string[3]{"A", "B", "C"});
            MenuEntry testMenuEntry = new MenuEntry("Test");
            MenuEntry saveMenuEntry = new MenuEntry("Save");
            MenuEntry backMenuEntry = new MenuEntry("Back");

            // Hook up menu event handlers.
            testMenuEntry.Selected += TestButton;
            saveMenuEntry.Selected += SaveButton;
            backMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(displayModeMenuEntry);
            MenuEntries.Add(resolutionMenuEntry);
            MenuEntries.Add(testMenuEntry);
            MenuEntries.Add(saveMenuEntry);
            MenuEntries.Add(backMenuEntry);

            mProfile = curProfile;
        }

        private Profile mProfile;

        void SaveButton(object sender, PlayerIndexEventArgs e) { }
        void TestButton(object sender, PlayerIndexEventArgs e) { }
    }

    /* My thoughts: 
     *      -Have this menu just a list of different things to bind and their current bindings
     *      -Each one opens up a 'KeybindingDialog' at which time the user can bind the primary or secondary keys 
     *          -has two buttons, one for primary, one for secondary
     *          -pressing it changes the dialog's message to say "Press any key to bind it to Y (P or S)" at which point it waits for the next valid keypress
    */
    class KeybindingsScreen : MenuScreen
    {
        private Profile mProfile;
        List<KeybindingKV> menuList;
        public KeybindingsScreen(Profile curProfile) : base("Keybindings") {
            mProfile = curProfile;
            
            // Creates the keybindings menu...
            menuList = new List<KeybindingKV>() {
                new KeybindingKV("Move Forward",        curProfile.KeyBindings.MoveForward){},
                new KeybindingKV("Move Backwards",      curProfile.KeyBindings.MoveBackwards){},
                new KeybindingKV("Crouch",              curProfile.KeyBindings.Crouch){},
                new KeybindingKV("Jump",                curProfile.KeyBindings.Jump){},
                new KeybindingKV("Interact",            curProfile.KeyBindings.Interact){},
                new KeybindingKV("Pause",               curProfile.KeyBindings.Pause){},
                new KeybindingKV("Portal (Entry) Fire", curProfile.KeyBindings.Portal1){},
                new KeybindingKV("Portal (Exit) Fire",  curProfile.KeyBindings.Portal2){},
            };
            foreach (KeybindingKV keyItem in menuList) {
                string title    = keyItem.Key;
                MenuEntry item  = new MenuEntry(title);
                item.Selected  += OpenKeybindingDialog(keyItem);
                MenuEntries.Add(item);
            }

            // Menu Items that are special
            MenuEntry acceptMenuEntry   = new MenuEntry("Accept");
            MenuEntry cancelMenuEntry   = new MenuEntry("Cancel");

            // Event bindings
            acceptMenuEntry.Selected    += SaveButton;
            cancelMenuEntry.Selected    += OnCancel;


            // Menu entries on our list
            MenuEntries.Add(acceptMenuEntry);
            MenuEntries.Add(cancelMenuEntry);
            
            
        }

        // Keybindings Dialog event generator
        System.EventHandler<Half_Caked.PlayerIndexEventArgs> OpenKeybindingDialog(KeybindingKV s) {
            return (object sender, PlayerIndexEventArgs e) => {
                MessageBoxScreen dialog = new KeybindingDialog(s.Key, (InputState input, string whichBinding) => { this.SetKeybinding(s, input, whichBinding); });
                ScreenManager.AddScreen(dialog, ControllingPlayer);
            };
        }
        public void SetKeybinding(KeybindingKV s, InputState input, string whichBinding)
        {
            string displayName = s.Key;
            Keybinding[] key = s.Value;
            System.Console.Error.WriteLine("Request to set the {0} keybinding [{1}] to {2}", whichBinding, displayName, input.ToString());
            //TODO: Make this work.
            // Specifically, how do we translate an InputState object to a Keybinding? Iono.
        }
        
        void SaveButton(object sender, PlayerIndexEventArgs e) {
            HalfCakedGame game = ScreenManager.Game as HalfCakedGame;
            Profile.SaveProfile(mProfile, "default.sav", game.Device);
            ExitScreen();
        }
    }
}
