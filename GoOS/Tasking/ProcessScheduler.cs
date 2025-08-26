using System;
using System.Collections.Generic;
using Cosmos.HAL;
using GoOS.GUI;
 
namespace GoOS.Tasking;

internal static class ProcessScheduler
{
    internal static byte _lastSecond { get; private set; } = RTC.Second;
    internal static byte _lastMinute { get; private set; } = RTC.Minute;
    
    internal static List<Process> Processes = new List<Process>();
    
    internal static List<Process> PriorityProcesses = new List<Process>();

    internal static Process ExclusiveProcess { get; private set; }

    private static int ProcessIndex = 0;

    internal static Process AddProcess(Process process)
    {
        Processes.Add(process);
        return process;
    }
    
    internal static Process AddPriorityProcess(Process process)
    {
        PriorityProcesses.Add(process);
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
        // Handle priority processes, these run no matter what. Even if a Process has exclusivity.
        foreach (Process p in PriorityProcesses)
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
        
        // Handle exclusive process
        if (ExclusiveProcess != null)
        {
            CheckExclusivity();

            try
            {
                if (ExclusiveProcess.Closing)
                {
                    if (ExclusiveProcess is Window)
                    {
                        WindowManager.RemoveWindow((Window)ExclusiveProcess);
                        WindowManager.Render();
                    }

                    KillProcess(ExclusiveProcess);

                    ExclusiveProcess = null;
                }
                else ExclusiveProcess.HandleRun();
            } catch (Exception) { KillProcess(ExclusiveProcess); }

            _lastSecond = RTC.Second;
            _lastMinute = RTC.Minute;
            
            return;
        }
        
        if (Processes.Count == 0)
            return;
        
        // Handle processes, "batching" them according to how many there are.
        int endIndex = Math.Min(ProcessIndex + (Processes.Count / Processes.Count < 8 ? 1 : Processes.Count < 16 ? 2 : 4), Processes.Count);

        int i = 0;
        for (i = ProcessIndex; i < endIndex; i++)
        {
            Process p = Processes[i];
            
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
        ProcessIndex = i;

        if (ProcessIndex >= Processes.Count)
            ProcessIndex = 0;
        
        _lastSecond = RTC.Second;
        _lastMinute = RTC.Minute;
    }

    internal static void CheckExclusivity()
    {
        // Processes only get exclusivity for 1 minute at a time, at most. 
        // They are expected to return it after finished, but if they don't we cut them off after a minute.
        // Yes, regardless of if it is done.
        if (_lastMinute != RTC.Minute)
            ExclusiveProcess = null;
    }

    internal static bool RequestExclusivity(Process process)
    {
        if (ExclusiveProcess == null)
        {
            ExclusiveProcess = process;
            return true;
        }
        
        return false;
    }

    internal static bool ExitExclusivity(Process process)
    {
        if (ExclusiveProcess != null && ExclusiveProcess == process)
        {
            ExclusiveProcess = null;
            return true;
        }

        return false;
    }
}