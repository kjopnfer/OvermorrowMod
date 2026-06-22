using Microsoft.Xna.Framework;
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

        private static float displayTicks = Duration;
        private static float scrollFrom;
        private static int scrollElapsed;
        private static bool scrolling;
        private static readonly int ScrollDuration = ModUtils.SecondsToTicks(0.4f);

        public static int BonusCount { get; private set; }
        public static int LastBonusSeconds { get; private set; }

        public static void Start()
        {
            RemainingTicks = Duration;
            displayTicks = Duration;
            scrolling = false;
            BonusCount = 0;
            Running = true;
        }

        public static void AddTime(int seconds)
        {
            if (!Running) return;

            scrollFrom = displayTicks;
            RemainingTicks += ModUtils.SecondsToTicks(seconds);
            scrollElapsed = 0;
            scrolling = true;

            LastBonusSeconds = seconds;
            BonusCount++;
        }

        public static void Tick()
        {
            if (!Running || RemainingTicks <= 0) return;

            RemainingTicks--;

            if (RemainingTicks == WarnThreshold)
                SoundEngine.PlaySound(WarnSound);
            else if (RemainingTicks == EndSoundLead)
                SoundEngine.PlaySound(EndSound);

            if (scrolling)
            {
                scrollElapsed++;
                float t = EasingUtils.EaseOutQuart(MathHelper.Clamp(scrollElapsed / (float)ScrollDuration, 0f, 1f));
                displayTicks = MathHelper.Lerp(scrollFrom, RemainingTicks, t);
                if (scrollElapsed >= ScrollDuration) scrolling = false;
            }
            else
            {
                displayTicks = RemainingTicks;
            }
        }

        public static void Reset()
        {
            Running = false;
            RemainingTicks = Duration;
            displayTicks = Duration;
            scrolling = false;
            BonusCount = 0;
        }

        public static string Display()
        {
            int totalSeconds = ((int)displayTicks + 59) / 60;
            return $"{totalSeconds / 60}:{totalSeconds % 60:00}";
        }
    }
}
