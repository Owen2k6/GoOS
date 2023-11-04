using Cosmos.Core;
using IL2CPU.API.Attribs;
using PrismAPI.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoOS.GUI
{
    public static class MemoryWatch
    {
        [ManifestResourceStream(ResourceName = "GoOS.Resources.GUI.warning.bmp")] private static byte[] warningIconRaw;
        private static Canvas warningIcon = Image.FromBitmap(warningIconRaw, false);

        private const int CYCLES_PER_CHECK = 50;
        private const uint WARNING_THRESHOLD = 64;

        private static int cyclesUntilNextCheck = CYCLES_PER_CHECK;
        private static Dialogue warningDialogue = null;

        private static void RunCheck()
        {
            uint memTotal = Cosmos.Core.CPU.GetAmountOfRAM();
            uint memUnavail = memTotal - (uint)Cosmos.Core.GCImplementation.GetAvailableRAM();
            uint memUsed = (Cosmos.Core.GCImplementation.GetUsedRAM() / 1024 / 1024) + memUnavail;
            uint memFree = memTotal - memUsed;
            uint memPercentUsed = (uint)(((float)memUsed / (float)memTotal) * 100f);

            if (memFree <= WARNING_THRESHOLD)
            {
                if (warningDialogue != null)
                {
                    return;
                }

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
}
