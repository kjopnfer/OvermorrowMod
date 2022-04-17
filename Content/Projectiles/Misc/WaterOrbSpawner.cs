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
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] % 8 == 0)
            {
                radius--;
            }

            Vector2 origin = Projectile.Center;
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
            int item = Item.NewItem(Projectile.GetItemSource_DropAsItem(), (int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height, ModContent.ItemType<WaterOrb>());

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item);
        }
    }
}