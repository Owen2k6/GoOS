using System;
using System.Collections.Generic;
using System.IO;

namespace GoOS.GUI.Apps
{
    /// <summary>
    /// Provides standardised menu definitions for common application menus like File, Edit, etc.
    /// </summary>
    public static class StandardMenus
    {
        /// <summary>
        /// Creates a standard File menu with customisable options
        /// </summary>
        public static Menubar.MenuModel CreateFileMenu(
            Action newAction = null,
            Action openAction = null,
            Action saveAction = null,
            Action saveAsAction = null,
            Action closeAction = null,
            Action printAction = null,
            Action quitAction = null)
        {
            var items = new List<Menubar.MenuItem>();
            
            if (newAction != null)
                items.Add(new Menubar.MenuItem("New", newAction, "Ctrl+N"));
                
            if (openAction != null)
                items.Add(new Menubar.MenuItem("Open...", openAction, "Ctrl+O"));
                
            if (saveAction != null)
            {
                if (items.Count > 0) 
                    items.Add(Menubar.MenuItem.Separator());
                items.Add(new Menubar.MenuItem("Save", saveAction, "Ctrl+S"));
            }
                
            if (saveAsAction != null)
                items.Add(new Menubar.MenuItem("Save As...", saveAsAction, "Ctrl+Shift+S"));
                
            if (closeAction != null)
            {
                if (items.Count > 0) 
                    items.Add(Menubar.MenuItem.Separator());
                items.Add(new Menubar.MenuItem("Close", closeAction, "Ctrl+W"));
            }
                
            if (printAction != null)
            {
                if (items.Count > 0 && closeAction == null) 
                    items.Add(Menubar.MenuItem.Separator());
                items.Add(new Menubar.MenuItem("Print...", printAction, "Ctrl+P"));
            }
                
            if (quitAction != null)
            {
                if (items.Count > 0) 
                    items.Add(Menubar.MenuItem.Separator());
                items.Add(new Menubar.MenuItem("Quit", quitAction, "Ctrl+Q"));
            }
            
            return new Menubar.MenuModel("File", items);
        }
        
        /// <summary>
        /// Creates a standard Edit menu with customisable options
        /// </summary>
        public static Menubar.MenuModel CreateEditMenu(
            Action undoAction = null,
            Action redoAction = null,
            Action cutAction = null,
            Action copyAction = null,
            Action pasteAction = null,
            Action deleteAction = null,
            Action selectAllAction = null,
            Action findAction = null,
            Action findNextAction = null,
            Action replaceAction = null)
        {
            var items = new List<Menubar.MenuItem>();
            
            if (undoAction != null)
                items.Add(new Menubar.MenuItem("Undo", undoAction, "Ctrl+Z"));
                
            if (redoAction != null)
                items.Add(new Menubar.MenuItem("Redo", redoAction, "Ctrl+Y"));
                
            if ((undoAction != null || redoAction != null) && (cutAction != null || copyAction != null || pasteAction != null))
                items.Add(Menubar.MenuItem.Separator());
                
            if (cutAction != null)
                items.Add(new Menubar.MenuItem("Cut", cutAction, "Ctrl+X"));
                
            if (copyAction != null)
                items.Add(new Menubar.MenuItem("Copy", copyAction, "Ctrl+C"));
                
            if (pasteAction != null)
                items.Add(new Menubar.MenuItem("Paste", pasteAction, "Ctrl+V"));
                
            if (deleteAction != null)
                items.Add(new Menubar.MenuItem("Delete", deleteAction, "Del"));
                
            if (selectAllAction != null)
            {
                if (items.Count > 0) 
                    items.Add(Menubar.MenuItem.Separator());
                items.Add(new Menubar.MenuItem("Select All", selectAllAction, "Ctrl+A"));
            }
                
            if (findAction != null)
            {
                if (items.Count > 0 && selectAllAction == null) 
                    items.Add(Menubar.MenuItem.Separator());
                items.Add(new Menubar.MenuItem("Find...", findAction, "Ctrl+F"));
            }
                
            if (findNextAction != null)
                items.Add(new Menubar.MenuItem("Find Next", findNextAction, "F3"));
                
            if (replaceAction != null)
                items.Add(new Menubar.MenuItem("Replace...", replaceAction, "Ctrl+H"));
            
            return new Menubar.MenuModel("Edit", items);
        }
        
        /// <summary>
        /// Creates a standard View menu with customisable options
        /// </summary>
        public static Menubar.MenuModel CreateViewMenu(
            Action zoomInAction = null,
            Action zoomOutAction = null,
            Action resetZoomAction = null,
            Action fullScreenAction = null,
            Dictionary<string, Action> additionalViewOptions = null)
        {
            var items = new List<Menubar.MenuItem>();
            
            if (zoomInAction != null)
                items.Add(new Menubar.MenuItem("Zoom In", zoomInAction, "Ctrl++"));
                
            if (zoomOutAction != null)
                items.Add(new Menubar.MenuItem("Zoom Out", zoomOutAction, "Ctrl+-"));
                
            if (resetZoomAction != null)
                items.Add(new Menubar.MenuItem("Reset Zoom", resetZoomAction, "Ctrl+0"));
                
            if (fullScreenAction != null)
            {
                if (items.Count > 0) 
                    items.Add(Menubar.MenuItem.Separator());
                items.Add(new Menubar.MenuItem("Full Screen", fullScreenAction, "F11"));
            }
                
            if (additionalViewOptions != null && additionalViewOptions.Count > 0)
            {
                if (items.Count > 0) 
                    items.Add(Menubar.MenuItem.Separator());
                    
                foreach (var option in additionalViewOptions)
                {
                    items.Add(new Menubar.MenuItem(option.Key, option.Value));
                }
            }
            
            return new Menubar.MenuModel("View", items);
        }
        
        /// <summary>
        /// Creates a standard Help menu
        /// </summary>
        public static Menubar.MenuModel CreateHelpMenu(
            Action helpContentsAction = null,
            Action aboutAction = null)
        {
            var items = new List<Menubar.MenuItem>();
            
            if (helpContentsAction != null)
                items.Add(new Menubar.MenuItem("Help Contents", helpContentsAction, "F1"));
                
            if (aboutAction != null)
            {
                if (items.Count > 0) 
                    items.Add(Menubar.MenuItem.Separator());
                items.Add(new Menubar.MenuItem("About", aboutAction));
            }
            
            return new Menubar.MenuModel("Help", items);
        }
        
        /// <summary>
        /// Creates a GoOS system menu
        /// </summary>
        public static Menubar.MenuModel CreateGoOSMenu(Window parentWindow)
        {
            return new Menubar.MenuModel("GoOS", new List<Menubar.MenuItem>
            {
                new Menubar.MenuItem("About GoOS", () => WindowManager.AddWindow(new About())),
                new Menubar.MenuItem("Check for Updates", Menubar.CheckForUpdates),
                Menubar.MenuItem.Separator(),
                new Menubar.MenuItem("System Settings...", () => WindowManager.AddWindow(new Settings.Frame())),
                Menubar.MenuItem.Separator(),
                new Menubar.MenuItem("Restart Computer", () => 
                {
                    Dialogue.Show(
                        "GoOS",
                        "Are you sure you want to restart your computer?",
                        new List<DialogueButton> { new() { Text = "Reboot", Callback = () => Cosmos.System.Power.Reboot() } },
                        Resources.question
                    );
                }),
                new Menubar.MenuItem("Shutdown Computer", () => 
                {
                    Dialogue.Show(
                        "GoOS",
                        "Are you sure you want to shut down your computer?",
                        new List<DialogueButton> { new() { Text = "Shut Down", Callback = () => Cosmos.System.Power.Shutdown() } },
                        Resources.question
                    );
                })
            });
        }
    }
}