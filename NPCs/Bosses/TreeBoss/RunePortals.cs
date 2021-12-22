using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public class EntrancePortal : ModProjectile
    {
        public bool RunOnce = true;

        public int MAX_TAILS = 8;
        public float TailDistance = 1f;

        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override bool CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune Portal");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 260;
        }

        public override void AI()
        {
            if (RunOnce)
            {
                int RADIUS = 20;

                for (int i = 0; i < 4; i++)
                {
                    Vector2 SpawnLocation = new Vector2(RADIUS, 0).RotatedBy(MathHelper.PiOver2 * i);

                    int proj = Projectile.NewProjectile(projectile.Center + SpawnLocation, Vector2.Zero, ModContent.ProjectileType<PortalSpinner>(), projectile.damage, 0f, Main.myPlayer, MathHelper.PiOver2 * i, RADIUS);
                    ((PortalSpinner)Main.projectile[proj].modProjectile).RotationCenter = projectile;
                }

                /*for (int i = 0; i < 2; i++)
                {
                    Vector2 SpawnLocation = new Vector2(RADIUS, 0).RotatedBy(MathHelper.Pi * i);

                    int proj = Projectile.NewProjectile(projectile.Center + SpawnLocation, Vector2.Zero, ModContent.ProjectileType<PortalSpinner>(), projectile.damage, 0f, Main.myPlayer, MathHelper.Pi * i, RADIUS);
                    ((PortalSpinner)Main.projectile[proj].modProjectile).RotationCenter = projectile;
                    ((PortalSpinner)Main.projectile[proj].modProjectile).RotateOpposite = true;
                }*/

                RunOnce = false;
            }

            projectile.localAI[0] += 6;

            if (projectile.ai[0]++ > 60 && projectile.ai[0] % 5 == 0 && TailDistance < 22)
            {
                TailDistance++;
            }

            // Outer for loop controls the strength
            for (int i = 0; i < 2; i++)
            {
                for (int iterations = 1; iterations <= MAX_TAILS; iterations++)
                {
                    Vector2 RotationalVelocity = Vector2.One.RotatedBy(MathHelper.ToRadians(360f / MAX_TAILS) * iterations).RotatedBy(MathHelper.ToRadians(projectile.localAI[0]));

                    Dust dust = Dust.NewDustDirect(projectile.Center, 0, 0, DustID.TerraBlade);
                    dust.velocity = RotationalVelocity * TailDistance;

                    dust.noGravity = true;
                    dust.scale *= 0.1f;
                    dust.scale += 1f;
                }
            }

            /*for (int i = 1; i <= MAX_TAILS; i++)
            {
                Vector2 RotationalVelocity = Vector2.One.RotatedBy(MathHelper.ToRadians(360f / MAX_TAILS) * i).RotatedBy(MathHelper.ToRadians(-projectile.localAI[0]));

                Dust dust = Dust.NewDustDirect(projectile.Center, 0, 0, DustID.TerraBlade);
                dust.velocity = RotationalVelocity * TailDistance;

                dust.noGravity = true;
                dust.scale *= 0.1f;
                dust.scale += 1f;
            }*/
        }
    }

    public class ExitPortal : ModProjectile
    {
        public bool RunOnce = true;

        public int MAX_TAILS = 8;
        public float TailDistance = 1f;

        public Player TrackPlayer = null;

        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override bool CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune Portal");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 260;
        }

        public override void AI()
        {
            if (RunOnce)
            {
                // This is to determine if the portal follows the player or not
                if (projectile.ai[0] != -1)
                {
                    TrackPlayer = Main.player[(int)projectile.ai[0]];
                }

                // Reset the ai counter afterwards
                projectile.ai[0] = 0;

                int RADIUS = 20;

                for (int i = 0; i < 4; i++)
                {
                    Vector2 SpawnLocation = new Vector2(RADIUS, 0).RotatedBy(MathHelper.PiOver2 * i);

                    int proj = Projectile.NewProjectile(projectile.Center + SpawnLocation, Vector2.Zero, ModContent.ProjectileType<PortalSpinner>(), projectile.damage, 0f, Main.myPlayer, MathHelper.PiOver2 * i, RADIUS);
                    ((PortalSpinner)Main.projectile[proj].modProjectile).RotationCenter = projectile;
                }

                RunOnce = false;
            }

            projectile.localAI[0] += 6;

            if (projectile.ai[0]++ > 60 && projectile.ai[0] % 5 == 0 && TailDistance < 22)
            {
                TailDistance++;
            }

            if (TailDistance == 22 && projectile.timeLeft == 30)
            {
                Main.npc[(int)projectile.ai[1]].Center = projectile.Center;
                Main.npc[(int)projectile.ai[1]].dontTakeDamage = false;
                Main.npc[(int)projectile.ai[1]].alpha = 0;

                if (TrackPlayer != null)
                {
                    ((TreeBossP2)Main.npc[(int)projectile.ai[1]].modNPC).PortalLaunched = true;
                    Main.npc[(int)projectile.ai[1]].velocity = Vector2.UnitY * 28;
                }

                for (int i = 0; i < 200; i++)
                {
                    Vector2 RandomVelocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(40, 60);
                    Dust.NewDust(projectile.Center, 2, 2, DustID.TerraBlade, RandomVelocity.X, RandomVelocity.Y);
                }
            }

            // Outer for loop controls the strength
            for (int i = 0; i < 2; i++)
            {
                for (int iterations = 1; iterations <= MAX_TAILS; iterations++)
                {
                    Vector2 RotationalVelocity = Vector2.One.RotatedBy(MathHelper.ToRadians(360f / MAX_TAILS) * iterations).RotatedBy(MathHelper.ToRadians(projectile.localAI[0]));

                    Dust dust = Dust.NewDustDirect(projectile.Center, 0, 0, DustID.TerraBlade);
                    dust.velocity = RotationalVelocity * TailDistance;

                    dust.noGravity = true;
                    dust.scale *= 0.1f;
                    dust.scale += 1f;
                }
            }
        }
    }
}