using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Projectiles.Accessory
{
    public class ArtemisRune : ModProjectile
    {
        public override string Texture => "OvermorrowMod/NPCs/Bosses/Apollus/ArrowRuneCircle";

        float rotationCounter;
        int directionalStore;
        int whoAmiStore;

        // public ref float Stage => ref projectile.ai[0];
        // public ref float Counter => ref projectile.ai[1];

        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Rune Circle");
        }

        public override void SetDefaults() {
            projectile.width = 62;
            projectile.height = 62;
            projectile.tileCollide = false;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            projectile.scale = 1f;
        }

        public override void AI() {

            Player player = Main.player[projectile.owner];
            if (!player.active)
            {
                projectile.Kill();
                return;
            }

            directionalStore = (int)projectile.knockBack;

            if (projectile.ai[1] == 0) {
                if (projectile.ai[0] == 0)
                    {
                        projectile.scale = 0.01f;
                    }
                    if (projectile.ai[0] > 2 && projectile.ai[0] < 45)
                    {
                        projectile.scale = MathHelper.Lerp(projectile.scale, 1, 0.05f);
                        rotationCounter = MathHelper.Lerp(0.001f, 5f, 0.05f);
                        projectile.rotation += rotationCounter;
                    }
                    if (projectile.ai[0] == 45)
                    {
                        projectile.ai[1] = 1;
                    }
            } else {
                rotationCounter = MathHelper.Lerp(0.001f, 5f, 0.05f);

                if (projectile.ai[0] % 45 == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ProjectileType<ArtemisArrow>(), 75, 3f, Main.myPlayer);
                        }
                    }
                }
            }

            projectile.ai[0]++;
            projectile.rotation += rotationCounter;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    }
}