using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.NPCs.Bosses.Apollus
{
    public class ArrowRuneCircle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune Circle");
        }
        public override void SetDefaults()
        {
            projectile.width = 62;
            projectile.height = 62;
            projectile.tileCollide = false;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            projectile.scale = 1f;
        }
        //public bool kill = false;
        float rotationcounter;
        int directionalstore;
        int whoamistore;

        public override void AI()
        {
            if (projectile.ai[0] == -10)
            {
                projectile.ai[0] = 0;
                whoamistore = (int)projectile.ai[1];
                projectile.ai[1] = 0;
            }
            if (projectile.ai[0] == -20)
            {
                projectile.ai[0] = 2;
                whoamistore = (int)projectile.ai[1];
                projectile.ai[1] = 0;
            }
            NPC owner = Main.npc[whoamistore];
            if (!owner.active)
            {
                projectile.Kill();
                return;
            }
            if (projectile.damage == 15)
            {
                projectile.ai[0] = 2;
            }
            switch (projectile.ai[0])
            {
                case 0:
                    {
                        if (projectile.ai[1] == 0)
                        {
                            projectile.scale = 0.01f;
                        }
                        if (projectile.ai[1] > 2 && projectile.ai[1] < 45)
                        {
                            projectile.scale = MathHelper.Lerp(projectile.scale, 1, 0.05f);
                            rotationcounter = MathHelper.Lerp(0.001f, 5f, 0.05f);
                            projectile.rotation += rotationcounter;
                        }
                        if (projectile.ai[1] == 45)
                        {
                            projectile.ai[0] = 1;
                        }
                    }
                    break;
                case 1:
                    {
                        projectile.rotation += rotationcounter;
                        if (projectile.ai[1] % 45 == 0)
                        {
                            for (int i = 0; i < Main.rand.Next(2, 5); i++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ProjectileType<ApollusGravityArrow>(), 12, 10f, Main.myPlayer);
                                }
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        Player projectileowner = Main.player[projectile.owner];
                        projectile.position = projectileowner.Center + new Vector2(-35 + (566 * directionalstore), -35);
                        if (projectile.ai[1] == 0)
                        {
                            projectile.scale = 0.01f;
                            directionalstore = (int)projectile.knockBack;
                            projectile.knockBack = 10;
                            projectile.damage = 12;
                        }
                        if (projectile.ai[1] > 2 && projectile.ai[1] < 45)
                        {
                            projectile.scale = MathHelper.Lerp(projectile.scale, 1, 0.05f);
                            rotationcounter = MathHelper.Lerp(0.001f, 5f, 0.05f);
                            projectile.rotation += rotationcounter;
                        }
                        if (projectile.ai[1] == 45)
                        {
                            projectile.ai[0] = 3;
                        }
                    }
                    break;
                case 3:
                    {
                        Player projectileowner = Main.player[projectile.owner];
                        projectile.position = projectileowner.Center + new Vector2(-35 + (566 * directionalstore), -35);
                        projectile.rotation += rotationcounter;
                        if (projectile.ai[1] % 45 == 0)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(projectile.Center, new Vector2(Main.rand.Next(-5, -3) * directionalstore, Main.rand.Next(-3, 3) * directionalstore), ProjectileType<ApollusArrowNormal>(), 12, 2, Main.myPlayer);
                                }
                            }
                        }
                    }
                    break;
            }
            projectile.ai[1]++;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}
