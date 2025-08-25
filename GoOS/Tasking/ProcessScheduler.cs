using System;
using System.Collections.Generic;
using GoOS.GUI;
 
namespace GoOS.Tasking;

internal static class ProcessScheduler
{
    internal static List<Process> Processes = new List<Process>();

    internal static Process AddProcess(Process process)
    {
        Processes.Add(process);
        return process;
    }

    internal static void KillProcess(Process process)
        => Processes.Remove(process);

    internal static void KillProcess(int PID)
    {
        foreach (Process p in Processes)
        {
            if (p.PID != PID) continue;

            Processes.Remove(p);
            return;
        }

        throw new ArgumentException("Process not found!");
    }

    internal static void KillProcess(string name)
    {
        foreach (Process p in Processes)
        {
            if (p.Name != name) continue;

            Processes.Remove(p);
            return;
        }

        throw new ArgumentException("Process not found!");
    }

    internal static void HandleRun()
    {
        foreach (Process p in Processes)
        {
            try
            {
                if (!p.Closing) p.HandleRun();
                else
                {
                    if (p is Window)
                    {
                        WindowManager.RemoveWindow((Window)p);
                        WindowManager.Render();
                    }

                    KillProcess(p);
                }
            }
            catch (Exception) { KillProcess(p); }
        }
    }
}