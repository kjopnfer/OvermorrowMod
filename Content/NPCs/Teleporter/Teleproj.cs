using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Teleporter
{
    public class Teleproj : ModProjectile
    {

        bool hashit = false;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Teleport");
        }

        public override void AI()
        {


            NPC npc = Main.npc[(int)Projectile.ai[1]];

            if (hashit)
            {
                npc.Center = Projectile.Center;
                Projectile.Kill();
                SoundEngine.PlaySound(SoundID.Item9, npc.position);
            }

            float num116 = 16f;
            for (int num117 = 0; (float)num117 < 16; num117++)
            {
                Vector2 spinningpoint7 = Vector2.UnitX * 0f;
                spinningpoint7 += -Vector2.UnitY.RotatedBy((float)num117 * ((float)Math.PI * 2f / num116)) * new Vector2(1f, 4f);
                spinningpoint7 = spinningpoint7.RotatedBy(Projectile.velocity.ToRotation());
                Vector2 position = Projectile.Center;
                Dust dust = Terraria.Dust.NewDustPerfect(position, 10, new Vector2(0f, 0f), 0, new Color(), 1f);
                dust.noLight = true;
                dust.noGravity = true;
                dust.position = Projectile.Center + spinningpoint7;
            }



            Projectile.velocity.X *= 0.97f;
            Projectile.velocity.Y += 0.3f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            hashit = true;
            return false;
        }
    }
}