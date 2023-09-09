/*using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.GraniteClamper
{
    // This should not exist, probably
    public class GraniteClamper : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Granite Clamper");
        }
        int timer = 0;

        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 42;
            NPC.damage = 40;
            NPC.defense = 9;
            NPC.lifeMax = 90;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath53;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void AI()
        {

            timer++;
            if (timer == 1)
            {
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 10, Mod.Find<ModProjectile>("GraniteClamperHook").Type, 13, 0.0f, Main.myPlayer, 0.0f, (float)NPC.whoAmI);
                NPC.netUpdate = true;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType == 368 ? 0.2f : 0f;
        }


    }
}*/