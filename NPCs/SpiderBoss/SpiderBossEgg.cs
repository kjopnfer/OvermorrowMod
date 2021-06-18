using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class SpiderBossEgg : ModProjectile
    {
        public override bool CanDamage() => false;
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 18;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 80;
            projectile.light = 0.5f;
            projectile.extraUpdates = 1;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spider Egg");
        }
        public override void AI()
        {
            projectile.velocity.Y += 0.4f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void Kill(int timeLeft)
        {
            NPC.NewNPC((int)projectile.Center.X + 0, (int)projectile.Center.Y + 0, mod.NPCType("BombSpider"));

            Vector2 position = projectile.Center;
            Main.PlaySound(SoundID.Item51, (int)position.X, (int)position.Y);
        }
    }
}
