using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.UI.ClassSelection;
using OvermorrowMod.Core.WorldGeneration;
using OvermorrowMod.Core.WorldGeneration.TestSubworld;
using SubworldLibrary;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Runs
{
    public class RunSavePlayer : ModPlayer
    {
        public string ActiveSubworld;
        public Guid MainWorldId;
        public string BiomeName;
        public int TimerTicks = -1;

        public bool PendingReset;
        public CharacterClass? PendingNewRun;

        public bool HasSavedRun => ActiveSubworld != null && MainWorldId == Main.ActiveWorldFileData.UniqueId;

        public void ClearRun()
        {
            ActiveSubworld = null;
            MainWorldId = Guid.Empty;
            BiomeName = null;
            TimerTicks = -1;
        }

        public override void OnEnterWorld()
        {
            if (RunState.InRun)
            {
                Subworld current = SubworldSystem.Current;
                ActiveSubworld = current.FullName;
                MainWorldId = Guid.Parse(Path.GetFileName(Path.GetDirectoryName(SubworldSystem.CurrentPath)));
                BiomeName = current is TestSubworld test ? test.TitleCardText : current.DisplayName.Value;
                return;
            }

            if (PendingReset && !SubworldSystem.AnyActive())
            {
                PendingReset = false;
                RunReset.Reset(Player);
            }
        }

        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer) return;

            if (PendingNewRun is CharacterClass characterClass && !SubworldSystem.AnyActive() && ClassSelection.BeginRun(Player, characterClass))
                PendingNewRun = null;
        }

        public override void SaveData(TagCompound tag)
        {
            if (ActiveSubworld == null) return;

            if (Player.GetModPlayer<SubworldPlayer>().inSubworldNow)
                TimerTicks = SubworldTimer.Running ? SubworldTimer.RemainingTicks : -1;

            tag["ActiveSubworld"] = ActiveSubworld;
            tag["MainWorldId"] = MainWorldId.ToString();
            tag["BiomeName"] = BiomeName ?? "";
            tag["TimerTicks"] = TimerTicks;
        }

        public override void LoadData(TagCompound tag)
        {
            string active = tag.GetString("ActiveSubworld");
            ActiveSubworld = active == "" ? null : active;
            Guid.TryParse(tag.GetString("MainWorldId"), out MainWorldId);
            BiomeName = tag.GetString("BiomeName");
            TimerTicks = tag.ContainsKey("TimerTicks") ? tag.GetInt("TimerTicks") : -1;
        }
    }
}
