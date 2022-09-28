namespace CitrineUI.Input
{
    /// <summary>
    /// A view that can internally receive mouse input.
    /// </summary>
    public interface IMouseListener
    {
        /// <summary>
        /// Called when the view is left clicked.
        /// </summary>
        /// <param name="rx">The relative X position of the event.</param>
        /// <param name="ry">The relative Y position of the event.</param>
        void OnClick(int relativeX, int relativeY);

        /// <summary>
        /// Called when the left mouse button is pressed on the view
        /// </summary>
        /// <param name="rx">The relative X position of the event.</param>
        /// <param name="ry">The relative Y position of the event.</param>
        void OnMouseDown(int relativeX, int relativeY);
    }
}
