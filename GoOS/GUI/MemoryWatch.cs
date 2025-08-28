using Cosmos.Core;
using static GoOS.Resources;

namespace GoOS.GUI;

public static class MemoryWatch
{
    private const int CYCLES_PER_CHECK = 50;
    private const uint WARNING_THRESHOLD = 64;

    private static int cyclesUntilNextCheck = CYCLES_PER_CHECK;
    private static Dialogue warningDialogue;

    private static void RunCheck()
    {
        var memTotal = CPU.GetAmountOfRAM();
        var memUnavail = memTotal - (uint)GCImplementation.GetAvailableRAM();
        var memUsed = GCImplementation.GetUsedRAM() / 1024 / 1024 + memUnavail;
        var memFree = memTotal - memUsed;
        var memPercentUsed = (uint)(memUsed / (float)memTotal * 100f);

        if (memFree <= WARNING_THRESHOLD)
        {
            if (warningDialogue != null) return;

            warningDialogue = Dialogue.Show(
                "Warning",
                $"Only {memPercentUsed} MB of RAM is free.\nStability may be affected.",
                null, // default buttons
                warningIcon
            );
        }
        else
        {
            warningDialogue.Dispose();
            warningDialogue = null;
        }
    }

    public static void Watch()
    {
        cyclesUntilNextCheck--;
        if (cyclesUntilNextCheck == 0)
        {
            cyclesUntilNextCheck = CYCLES_PER_CHECK;
            RunCheck();
        }
    }
}