using System;
using GoOS.GUI.Apps;
using GoOS.Themes;

namespace GoOS.Virtualisation
{
    public abstract class VM
    {
        public string name;

        protected bool mStarted = false;

        protected bool mStopped = false;
        
        private Terminal terminal;

        public VM(Terminal terminal)
        {
            this.terminal = terminal;
        }

        public virtual void Start()
        {
            try
            {
                terminal.log(ThemeManager.WindowText, $"\nStarting virtual machine {name}...\n");

                if (mStarted)
                {
                    throw new Exception("A virtual machine cannot be started twice!");
                }
                mStarted = true;

                terminal.terminal.ForegroundColor = Gold.Graphics.Color.White;
                terminal.terminal.BackgroundColor = Gold.Graphics.Color.Black;

                OnBoot();
                BeforeRun();

                while (!mStopped)
                {
                    Run();
                }

                AfterRun();

                terminal.log(ThemeManager.WindowText, "\nReturning back to GoOS...\n");
            }
            catch (Exception ex)
            {
                terminal.log(ThemeManager.ErrorText, $"\nAn exception occured in the virtual machine {name}\n{ex}\n");
            }
        }

        protected virtual void OnBoot() { }

        protected virtual void BeforeRun() { }

        protected abstract void Run();

        protected virtual void AfterRun() { }

        public virtual void Stop() => mStopped = true;
    }
}
