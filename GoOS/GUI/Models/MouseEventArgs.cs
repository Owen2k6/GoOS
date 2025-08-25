using Cosmos.System;

namespace GoOS.GUI.Models
{
    public struct MouseEventArgs
    {
        public int X;
        public int Y;

        public MouseState MouseState;

        public MouseEventArgs()
        {
            X = (int)MouseManager.X;
            Y = (int)MouseManager.Y;

            MouseState = MouseManager.MouseState;
        }
    }
}