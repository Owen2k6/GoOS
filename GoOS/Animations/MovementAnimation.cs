using CitrineUI.Views;
using System;
using System.Drawing;

namespace CitrineUI.Animations
{
    /// <summary>
    /// An animation that moves or resizes a view.
    /// </summary>
    internal class MovementAnimation : Animation
    {
        /// <summary>
        /// Initialise the animation.
        /// </summary>
        /// <param name="view">The view associated with the animation.</param>
        /// <param name="to">The goal of the animation.</param>
        public MovementAnimation(View view)
        {
            View = view;
            From = view.Rectangle;
        }

        /// <summary>
        /// The starting rectangle of the animation.
        /// </summary>
        public Rectangle From;

        /// <summary>
        /// The goal rectangle of the animation. 
        /// </summary>
        public Rectangle To;

        public override bool Advance()
        {
            if (From.IsEmpty || To.IsEmpty) throw new Exception("The From or To value of this MovementAnimation is empty.");
            Position++;
            if (Position == Duration)
            {
                View.Rectangle = To;
            }
            else
            {
                double t = Easing.Ease(Position / (double)Duration, EasingType, EasingDirection);
                View.Rectangle = new Rectangle(
                    (int)Easing.Lerp(From.X, To.X, t),
                    (int)Easing.Lerp(From.Y, To.Y, t),
                    (int)Easing.Lerp(From.Width, To.Width, t),
                    (int)Easing.Lerp(From.Height, To.Height, t)
                );
            }
            if (Finished)
            {
                Cancel();
            }
            return Finished;
        }
    }
}
