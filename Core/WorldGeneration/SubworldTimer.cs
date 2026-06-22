using OvermorrowMod.Common.Utilities;
using Terraria.Audio;

namespace OvermorrowMod.Core.WorldGeneration
{
    public static class SubworldTimer
    {
        public static readonly int Duration = ModUtils.SecondsToTicks(420);
        public static readonly int WarnThreshold = ModUtils.SecondsToTicks(60);
        public static readonly int EndSoundLead = 50;

        private static readonly SoundStyle WarnSound = new SoundStyle("OvermorrowMod/Sounds/TimerWarn");
        private static readonly SoundStyle EndSound = new SoundStyle("OvermorrowMod/Sounds/TimerEnd");

        public static int RemainingTicks { get; private set; } = Duration;
        public static bool Running { get; private set; }

        public static void Start()
        {
            RemainingTicks = Duration;
            Running = true;
        }

        public static void Tick()
        {
            if (!Running || RemainingTicks <= 0) return;

            RemainingTicks--;

            if (RemainingTicks == WarnThreshold)
                SoundEngine.PlaySound(WarnSound);
            else if (RemainingTicks == EndSoundLead)
                SoundEngine.PlaySound(EndSound);
        }

        public static void Reset()
        {
            Running = false;
            RemainingTicks = Duration;
        }

        public static string Display()
        {
            int totalSeconds = (RemainingTicks + 59) / 60;
            return $"{totalSeconds / 60}:{totalSeconds % 60:00}";
        }
    }
}
