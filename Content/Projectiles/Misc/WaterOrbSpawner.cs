using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using OvermorrowMod.Content.Items.Consumable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Misc
{
    public class WaterOrbSpawner : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        float radius = 15;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Water Orb Spawner");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 120;
            projectile.alpha = 255;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.ai[0]++;

            if (projectile.ai[0] % 8 == 0)
            {
                radius--;
            }

            Vector2 origin = projectile.Center;
            int numLocations = 30;
            for (int i = 0; i < 30; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, -2.5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                int dust = Dust.NewDust(position, 2, 2, DustID.MagnetSphere, dustvelocity.X, dustvelocity.Y, 0, default, 1);
                Main.dust[dust].noGravity = false;
            }
        }

        public override void Kill(int timeLeft)
        {
            int item = Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<WaterOrb>());

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item);
        }
    }
}