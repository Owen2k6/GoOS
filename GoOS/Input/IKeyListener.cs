using Cosmos.System;

namespace CitrineUI.Input
{
    /// <summary>
    /// A view that can internally receive keyboard input.
    /// </summary>
    public interface IKeyListener
    {
        /// <summary>
        /// Called when a key is typed.
        /// </summary>
        /// <param name="key">The key that was typed.</param>
        void OnKeyPressed(KeyEvent key) { }
    }
}
