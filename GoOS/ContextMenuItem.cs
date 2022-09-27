using Cosmos.System.Graphics;
using System;

namespace CitrineUI
{
    /// <summary>
    /// An item in a context menu.
    /// </summary>
    public class ContextMenuItem
    {
        public ContextMenuItem(string name)
        {
            Name = name;
        }

        public ContextMenuItem(string name, Bitmap icon)
        {
            Name = name;
            Icon = icon;
        }

        /// <summary>
        /// The name of the context menu item.
        /// </summary>
        public string Name = "Item";

        /// <summary>
        /// The icon to be displayed to the left of the name of the context menu item.
        /// </summary>
        public Bitmap Icon = null;

        internal EventHandler ClickedHandler;
        /// <summary>
        /// Fires when the context menu item is clicked.
        /// </summary>
        public event EventHandler Clicked
        {
            add
            {
                ClickedHandler = value;
            }
            remove
            {
                ClickedHandler -= value;
            }
        }
    }
}
