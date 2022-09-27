using CitrineUI.Views;

namespace CitrineUI.Animations
{
    /// <summary>
    /// A UI animation.
    /// </summary>
    public abstract class Animation
    {
        /// <summary>
        /// The easing type of the animation.
        /// </summary>
        public EasingType EasingType = EasingType.Sine;

        /// <summary>
        /// The direction of the easing of the animation.
        /// </summary>
        public EasingDirection EasingDirection = EasingDirection.Out;

        /// <summary>
        /// The duration of the animation, in frames.
        /// </summary>
        public int Duration = 100;

        /// <summary>
        /// How many frames of the animation have been completed.
        /// </summary>
        public int Position = 0;

        /// <summary>
        /// If the animation has finished.
        /// </summary>
        public bool Finished
        {
            get
            {
                return Position >= Duration;
            }
        }

        /// <summary>
        /// The view associated with the animation.
        /// </summary>
        public View View;

        /// <summary>
        /// Advance the animation by one frame.
        /// </summary>
        /// <returns>Whether or not the animation is now finished.</returns>
        public abstract bool Advance();

        /// <summary>
        /// Start the animation.
        /// </summary>
        public void Start()
        {
            foreach (Animation animation in View.Desktop.Animations)
            {
                if (animation == this)
                {
                    return;
                }
                // If an animation is currently running on this view, cancel it.
                if (animation.View == View)
                {
                    animation.Cancel();
                }
            }
            View.Desktop.Animations.Add(this);
        }

        /// <summary>
        /// Cancel the animation.
        /// </summary>
        public void Cancel()
        {
            View.Desktop.Animations.Remove(this);
        }
    }
}
