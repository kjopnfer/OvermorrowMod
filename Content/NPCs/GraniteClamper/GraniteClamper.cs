using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.GraniteClamper
{
    // This should not exist, probably
    public class GraniteClamper : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Clamper");
        }
        int timer = 0;

        public override void SetDefaults()
        {
            npc.width = 44;
            npc.height = 42;
            npc.damage = 40;
            npc.defense = 9;
            npc.lifeMax = 90;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath53;
            npc.value = 60f;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public override void AI()
        {

            timer++;
            if (timer == 1)
            {
                int proj = Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 10, mod.ProjectileType("GraniteClamperHook"), 13, 0.0f, Main.myPlayer, 0.0f, (float)npc.whoAmI);
                npc.netUpdate = true;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == 368 ? 0.2f : 0f;
        }


    }
}