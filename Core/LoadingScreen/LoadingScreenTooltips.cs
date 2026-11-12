using OvermorrowMod.Common;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace OvermorrowMod.Core.LoadingScreen
{
    public enum TipSource
    {
        ArchiveTips,
        JermaQuotes
    }

    public static class LoadingScreenTooltips
    {
        private static readonly List<string> tooltipKeys = new List<string>
        {
            "CombatTargeting",
            "HiddenItems",
            "EliteEnemies",
            "EnemyPerception",
            "Scouting",
            "SupportEnemies",
            "Barrier",
            "StealthMechanics",
            "Rapiers"
        };

        private static readonly List<string> jermaQuotes = new List<string>
        {
            "If you can't handle me at my worst, obey your thirst.",
            "If I chop you up in a meat grinder, and the only thing that comes out is your eyeball, you're PROBABLY DEAD!",
            "There's Blood in the box, there's blood inside this box",
            "THE DREAM CAST FUCKING SUCKS",
            "That's some good apple cider",
            "Paperairplane… AAAAUGH ICOUGNTMA- THECHARACTERFUCKIN gasp THE CHARACT gasp THE CHARACTER IS LIKE WHYASAFLOSER gasp WHAYAWHA THE CHARACTER- that character is whayuhwhawhat",
            "WHEN IN DOUBT\n\nRADISH IT OUT",
            "Michelangelo, Leonardo, Da Vinci, all dead....I REMAIN",
            "Giving Aliens edibles is an act of war",
            "It's come and mop up"
        };

        private static (string Title, string Text) currentTip;
        private static bool hasGeneratedTip = false;
        private static TipSource currentSource = TipSource.ArchiveTips;

        public static (string Title, string Text) GetCurrentTip()
        {
            if (!hasGeneratedTip)
            {
                GenerateRandomTip();
            }
            return currentTip;
        }

        public static void GenerateRandomTip()
        {
            var random = new Random();
            if (currentSource == TipSource.JermaQuotes)
            {
                string quote = jermaQuotes[random.Next(jermaQuotes.Count)];
                currentTip = ("Pro tip", quote);
            }
            else
            {
                string selectedKey = tooltipKeys[random.Next(tooltipKeys.Count)];
                string description = Language.GetTextValue(LocalizationPath.LoadingTips + selectedKey + ".Description");
                currentTip = ("Pro tip", description);
            }
            hasGeneratedTip = true;
        }

        public static void Reset(TipSource source = TipSource.ArchiveTips)
        {
            currentSource = source;
            hasGeneratedTip = false;
        }
    }
}