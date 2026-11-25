using OvermorrowMod.Core.Runs;
using OvermorrowMod.Core.UI;
using SubworldLibrary;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.WorldGeneration.Guild
{
    public class GuildSubworld : Subworld
    {
        public override int Width => 600;
        public override int Height => 600;

        public override bool ShouldSave => true;

        public override List<GenPass> Tasks => [new GuildGenPass()];

        /// <summary>
        /// Sends the player to the guild, whose portal then leads to <paramref name="nextBiome"/> ("ModName/SubworldName").
        /// </summary>
        public static void Enter(Player player, string nextBiome)
        {
            RunSavePlayer save = player.GetModPlayer<RunSavePlayer>();
            save.NextBiome = nextBiome;
            save.LastPosition.Remove(ModContent.GetInstance<GuildSubworld>().FullName);
            SubworldSystem.Enter<GuildSubworld>();
        }

        public static void EnterNextBiome(Player player)
        {
            string nextBiome = player.GetModPlayer<RunSavePlayer>().NextBiome;
            if (!string.IsNullOrEmpty(nextBiome)) SubworldSystem.Enter(nextBiome);
        }

        public override void Update()
        {
            foreach (TileEntity entity in TileEntity.ByID.Values.ToArray())
                entity.Update();
        }

        public override void OnEnter()
        {
            TitleCardManager.ShowTitle(DisplayName.Value);
            base.OnEnter();
        }

        public override void OnExit()
        {
            RunSave.WaitForPendingSave();
            TitleCardManager.Hide();
            base.OnExit();
        }
    }
}
