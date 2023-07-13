using System;
using GoOS.Themes;
using Console = BetterConsole;
using static GoOS.Core;

namespace GoOS.Virtualisation
{
    public abstract class VM
    {
        public string name;

        protected bool mStarted = false;

        protected bool mStopped = false;

        public virtual void Start()
        {
            try
            {
                log(ThemeManager.WindowText, $"\nStarting virtual machine {name}...\n");

                if (mStarted)
                {
                    throw new Exception("A virtual machine cannot be started twice!");
                }
                mStarted = true;

                Console.ForegroundColor = ConsoleColorEx.White;
                Console.BackgroundColor = ConsoleColorEx.Black;

                OnBoot();
                BeforeRun();

                while (!mStopped)
                {
                    Run();
                }

                AfterRun();

                log(ThemeManager.WindowText, "\nReturning back to GoOS...\n");
            }
            catch (Exception ex)
            {
                log(ThemeManager.ErrorText, $"\nAn exception occured in the virtual machine {name}\n{ex}\n");
            }
        }

        protected virtual void OnBoot() { }

        protected virtual void BeforeRun() { }

        protected abstract void Run();

        protected virtual void AfterRun() { }

        public virtual void Stop() => mStopped = true;
    }
}
