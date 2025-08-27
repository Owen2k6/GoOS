using System;
using System.Collections.Generic;
using System.Threading;
using Cosmos.Core;
using Cosmos.HAL;
using GoOS.GUI;
 
namespace GoOS.Tasking;

internal class ProcessScheduler
{
    internal int IPS { get; private set; } = 0; // Iterations Per Second, how fast the backend (processes) are running.
    
    private int _iterations = 0; // For IPS timer
    
    public int Iterations { get; private set; } = 0; // For "public" usage
    
    internal List<Process> Processes = new List<Process>();
    
    internal List<Process> PriorityProcesses = new List<Process>();

    private int ProcessIndex = 0;

    internal ProcessScheduler()
    {
        Timer T = new((_) =>
        {
            IPS = _iterations * 2;
            _iterations = 0;
        }, null, 500, 0);
        
        Timer t = new((_) => Iterations = 0, null, 1000, 0);
    }

    internal Process AddProcess(Process process)
    {
        Processes.Add(process);
        return process;
    }
    
    internal Process AddPriorityProcess(Process process)
    {
        PriorityProcesses.Add(process);
        return process;
    }

    internal void KillProcess(Process process)
        => Processes.Remove(process);

    internal void KillProcess(int PID)
    {
        foreach (Process p in Processes)
        {
            if (p.PID != PID) continue;

            Processes.Remove(p);
            return;
        }

        throw new ArgumentException("Process not found!");
    }

    internal void KillProcess(string name)
    {
        foreach (Process p in Processes)
        {
            if (p.Name != name) continue;

            Processes.Remove(p);
            return;
        }

        throw new ArgumentException("Process not found!");
    }
    
    private void KillPriorityProcess(Process process)
        => PriorityProcesses.Remove(process);

    private void KillPriorityProcess(int PID)
    {
        foreach (Process p in PriorityProcesses)
        {
            if (p.PID != PID) continue;

            PriorityProcesses.Remove(p);
            return;
        }

        throw new ArgumentException("Process not found!");
    }

    internal void HandleRun()
    {
        // Handle priority processes, these run no matter what. Even if a Process has exclusivity.
        if (PriorityProcesses.Count > 0)
        {
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

                        KillPriorityProcess(p);
                    }
                }
                catch (Exception ex)
                {
                    KillPriorityProcess(p);
                    
                    WindowManager.Screen.IsEnabled = false;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The process scheduler crashed!: " + ex.Message);
                    Console.WriteLine("    at GoOS.Tasking.ProcessScheduler.HandleRun()");
                    Console.WriteLine("    at GoOS.Kernel.Run()");
                    Console.WriteLine("    at Cosmos.System.Kernel.Start()");

                    Console.ResetColor();
                    Console.WriteLine("\nPress any key to reboot...");
                    Console.ReadKey(true);
                    CPU.Reboot();
                }
            }
        }

        if (Processes.Count > 0)
        {
            // Handle processes, "batching" them according to how many there are.
            int endIndex =
                Math.Min(ProcessIndex + (Processes.Count / Processes.Count < 8 ? 1 : Processes.Count < 16 ? 2 : 4),
                    Processes.Count);

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
                catch (Exception ex)
                {
                    KillProcess(p);

                    WindowManager.Screen.IsEnabled = false;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The process scheduler crashed!: " + ex.Message);
                    Console.WriteLine("    at GoOS.Tasking.ProcessScheduler.HandleRun()");
                    Console.WriteLine("    at GoOS.Kernel.Run()");
                    Console.WriteLine("    at Cosmos.System.Kernel.Start()");

                    Console.ResetColor();
                    Console.WriteLine("\nPress any key to reboot...");
                    Console.ReadKey(true);
                    CPU.Reboot();
                }
            }

            ProcessIndex = i;

            if (ProcessIndex >= Processes.Count)
                ProcessIndex = 0;
        }

        Iterations++;
        _iterations++;
    }
}