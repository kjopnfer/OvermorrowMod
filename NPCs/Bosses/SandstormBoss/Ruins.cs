using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using OvermorrowMod.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public abstract class Ruin : ModProjectile
    {
        private Vector2 SpawnPosition;
        protected Vector2 InitialPosition;
        public override bool CanDamage() => projectile.ai[0] > 120;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Buried Ruin");
        }

        public override void SetDefaults()
        {
            projectile.tileCollide = false;
            projectile.hide = true;
            projectile.hostile = true;
            projectile.friendly = false;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                //SpawnPosition = projectile.Center;

                Tile tile = Framing.GetTileSafely((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16);

                while (!tile.active() || tile.collisionType != 1)
                {
                    projectile.position.Y += 1;
                    tile = Framing.GetTileSafely((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16);
                }

                projectile.position.Y += Main.rand.Next(4, 8) * 20;
            }

            /*if (!Collision.CanHit(projectile.Center, projectile.width, projectile.height, projectile.Center + Vector2.UnitY, 2, 2))
            {
                if (Main.rand.NextBool(3))
                {
                    Tile tile = Framing.GetTileSafely((int)(SpawnPosition.X + (Main.rand.Next(-2, 2) * 15)) / 16, (int)SpawnPosition.Y / 16);

                    while (!tile.active() || tile.collisionType != 1)
                    {
                        SpawnPosition.Y += 1;
                        tile = Framing.GetTileSafely((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16);
                    }

                    Particle.CreateParticle(Particle.ParticleType<Smoke2>(), SpawnPosition * 16, Vector2.One.RotatedByRandom(MathHelper.TwoPi), new Color(182, 128, 70));
                }
            }*/

            if (projectile.ai[0]++ < 240)
            {
                projectile.Center -= Vector2.UnitY;
                InitialPosition = projectile.Center;
            }
            else
            {
                projectile.Center = Vector2.Lerp(InitialPosition, InitialPosition + Vector2.UnitY * 50, (float)Math.Sin(projectile.localAI[0]++ / 120f));
            }

            projectile.rotation = MathHelper.Lerp(0, MathHelper.PiOver4, (float)Math.Sin(projectile.localAI[0] / 240));
        }
    }

    public class Ruin1 : Ruin
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            projectile.width = 94;
            projectile.height = 102;
            projectile.timeLeft = 1200;
        }
    }

    public class Ruin2 : Ruin
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            projectile.width = 142;
            projectile.height = 132;
            projectile.timeLeft = 1200;
        }
    }

    public class Ruin3 : Ruin
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            projectile.width = 60;
            projectile.height = 60;
            projectile.timeLeft = 1200;
        }
    }
}