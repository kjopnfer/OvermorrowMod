using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.NPCs.Bosses.SandstormBoss;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Artifact
{
    public class SafetyZone : ModProjectile
    {
        public bool hide = false;
        private bool isActive = false;
        private bool owneralive = false;
        private float radius = 450;

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
            projectile.friendly = false;
            projectile.hostile = false;
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
            if (projectile.ai[1] < radius)
            {
                projectile.ai[1] += 15;
            }
            else
            {
                isActive = true;
            }
            if (projectile.ai[1] > radius && hide)
            {
                projectile.ai[1] += -15;
            }
            else if (hide)
            {
                isActive = false;
            }
           

            Vector2 dustVelocity = Vector2.UnitX * 18f;
            dustVelocity = dustVelocity.RotatedBy(projectile.rotation - 1.57f);
            Vector2 spawnPos = projectile.Center + dustVelocity;
            Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f);
            Vector2 velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * 6 / 10f;

            Vector2 origin = projectile.Center;
            if (isActive)
            {
                for (int i = 0; i < 36; i++)
                {
                    Vector2 dustPos = projectile.Center + new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 10 + projectile.ai[0]));
                    Dust dust = Dust.NewDustPerfect(dustPos, 57 /* 32 */, Vector2.Zero, 0, new Color(255, 255, 255), 2.04f / 2);
                    dust.noGravity = true;
                }
            }

            if (isActive)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    float distance = Vector2.Distance(projectile.Center, Main.player[i].Center);
                    if (distance <= radius)
                    {
                        Main.player[i].buffImmune[194] = true;
                    }
                }
            }

            owneralive = Main.npc[(int)projectile.knockBack].active;

            if (((SandstormBoss)Main.npc[(int)projectile.knockBack].modNPC).safetyCircleSwitch)
            {
                hide = !hide;
                radius = hide ? 0 : 433;
            }

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