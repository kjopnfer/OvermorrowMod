using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs;
using OvermorrowMod.NPCs.Bosses.SandstormBoss;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Artifact
{
    public class SafetyZone : ModProjectile
    {
        private bool isActive = false;
        private bool owneralive = false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => false;
        public override string Texture => "OvermorrowMod/NPCs/Bosses/StormDrake/LaserWarning";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sandstorm Safety");
        }

        public override void SetDefaults()
        {
            projectile.width = 144;
            projectile.height = 106;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 1.2f, 0f, 0f);

            Player player = Main.player[projectile.owner];
            if (player.dead || !player.active)
            {
                return;
            }

            projectile.ai[0] += 1;
            if (projectile.ai[1] < /*275*/ 290) // The radius
            {
                projectile.ai[1] += 15;
            }
            else
            {
                isActive = true;
            }

            Vector2 dustVelocity = Vector2.UnitX * 18f;
            dustVelocity = dustVelocity.RotatedBy(projectile.rotation - 1.57f);
            Vector2 spawnPos = projectile.Center + dustVelocity;
            Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f);
            Vector2 velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * 6 / 10f;

            Vector2 origin = projectile.Center;

            for (int i = 0; i < 36; i++)
            {
                Vector2 dustPos = projectile.Center + new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 10 + projectile.ai[0]));
                Dust dust = Main.dust[Dust.NewDust(dustPos, 15, 15, 32, 0f, 0f, 0, default, 2.04f / 2)];
                dust.noGravity = true;
            }

            if (isActive)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    float distance = Vector2.Distance(projectile.Center, Main.player[i].Center);
                    if (distance <= /*275*/ 290)
                    {
                        Main.player[i].buffImmune[194] = true;
                    }
                }
            }

            owneralive = Main.npc[(int)projectile.knockBack].active;

            if (projectile.timeLeft < 5 && !(player.dead || !player.active) && owneralive)
            {
                projectile.timeLeft = 5;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}