using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.DripplerBoss
{
    public class DripplerSpawner : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drippler Spawner");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 3f)
            {
                int dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 0, default, 1.84f);
                Main.dust[dust].noGravity = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[0] == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int spawnedNPC = NPC.NewNPC(Projectile.GetSource_Death(), (int)(Projectile.Center.X), (int)(Projectile.Center.Y), ModContent.NPCType<LoomingDrippler>(), 0, 0, Projectile.ai[1]);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawnedNPC);
                    }
                    Main.npc[spawnedNPC].netUpdate = true;
                }
            }
        }
    }
}