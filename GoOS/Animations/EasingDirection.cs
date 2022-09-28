namespace CitrineUI.Animations
{
    /// <summary>
    /// Defines the direction of an easing type.
    /// </summary>
    public enum EasingDirection
    {
        /// <summary>
        /// Starts the animation slowly, and finishes at full speed.
        /// </summary>
        In,

        /// <summary>
        /// Starts the animation at full speed, and finishes slowly.
        /// </summary>
        Out,

        /// <summary>
        /// Starts the animation slowly, reaches full speed at the middle, and finishes slowly.
        /// </summary>
        InOut
    }
}
