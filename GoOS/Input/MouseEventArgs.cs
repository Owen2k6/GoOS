using CitrineUI.Views;
using Cosmos.System;
using System;

namespace CitrineUI.Input
{
    /// <summary>
    /// Information about a mouse event.
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        /// <summary>
        /// The cursor that triggered the event.
        /// </summary>
        public Cursor Cursor { get; set; }

        /// <summary>
        /// The relative X position of the event.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The relative Y position of the event.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The mouse state of the event.
        /// </summary>
        public MouseState State { get; set; }
    }
}
