using OvermorrowMod.Common;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace OvermorrowMod.Core.LoadingScreen
{
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

        private static (string Title, string Text) currentTip;
        private static bool hasGeneratedTip = false;

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
            string selectedKey = tooltipKeys[random.Next(tooltipKeys.Count)];

            string title = Language.GetTextValue(LocalizationPath.LoadingTips + selectedKey + ".DisplayName");
            string description = Language.GetTextValue(LocalizationPath.LoadingTips + selectedKey + ".Description");

            currentTip = (title, description);
            hasGeneratedTip = true;
        }

        public static void Reset()
        {
            hasGeneratedTip = false;
        }
    }
}