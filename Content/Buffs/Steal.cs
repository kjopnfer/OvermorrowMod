using OvermorrowMod.Common;
using OvermorrowMod.Content.NPCs.Bosses.SandstormBoss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs
{
    public class Steal : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Steal!");
            Description.SetDefault("You've taken something from the enemy, better put it to good use...");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<OvermorrowModPlayer>().StoleArtifact = true;

            // This does stuff when Dharuud is active
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.ModNPC is SandstormBoss)
                {
                    player.buffImmune[BuffID.WindPushed] = true;
                    player.moveSpeed += 0.85f;
                    player.jumpSpeedBoost += 3f;

                    // What is this? Needs migration.
                    // player.doubleJumpSandstorm = true;
                }
            }
        }
    }
}