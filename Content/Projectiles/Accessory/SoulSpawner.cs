using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Consumable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Accessory
{
    public class SoulSpawner : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Spawner");
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 86;
            projectile.height = 60;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 120;
            projectile.tileCollide = false;
        }

        public override void AI()
        {

            // Spawn the flame during only one frame and only once
            if (projectile.frame == 4 && projectile.ai[0] == 0)
            {
                projectile.ai[0]++;

                int item = Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<ReaperFlame>());

                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item);
            }

            // Loop through the 8 animation frames, spending 4 ticks on each.
            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.Kill();
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}