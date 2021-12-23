using Microsoft.Xna.Framework;
using OvermorrowMod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public class AbsorbEnergy : ModNPC
    {
        public bool RunOnce = true;
        public Vector2 InitialDistance;
        public int RandomStopping;
        public bool HasStopped;
        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Natural Energy");
        }

        public override void SetDefaults()
        {
            npc.lifeMax = 10;
            npc.width = 10;
            npc.height = 10;
            npc.alpha = 255;
            npc.damage = 17;
            npc.friendly = false;
            npc.noTileCollide = true;
        }

        public override void AI()
        {
            // Get the ID of the Parent NPC that was passed in via ai[0]
            NPC parent = Main.npc[(int)npc.ai[0]];

            if (RunOnce)
            {
                // Grab the starting distance of when this NPC spawns
                InitialDistance = npc.Center;
                RandomStopping = Main.rand.Next(400, 600);

                npc.netUpdate = true;
            }

            Vector2 newMove = parent.Center - npc.Center;
            Vector2 move = newMove;
            float moveSpeed = Main.expertMode ? 180f : 210f;

            // Make the NPC stop midway from the initial spawnpoint to the boss for a few seconds
            if (npc.ai[1] < 120 && parent.Distance(npc.Center) <= RandomStopping)
            {
                npc.velocity = Vector2.Zero;
                npc.ai[1]++;

                HasStopped = true;
            }
            else
            {
                npc.velocity = move / (moveSpeed + (HasStopped ? 100 : 0));
            }

            #region Dust Code
            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.TerraBlade, npc.velocity.X, npc.velocity.Y, 50, default(Color), 0.4f);
                switch (num1103)
                {
                    case 0:
                        Main.dust[num1106].position = (Main.dust[num1106].position + npc.Center * 5f) / 6f;
                        break;
                    case 1:
                        Main.dust[num1106].position = (Main.dust[num1106].position + (npc.Center + npc.velocity / 2f) * 5f) / 6f;
                        break;
                }
                Dust dust81 = Main.dust[num1106];
                dust81.velocity *= 0.1f;
                Main.dust[num1106].noGravity = true;
                Main.dust[num1106].fadeIn = 1f;
            }
            #endregion

            if (npc.getRect().Intersects(parent.getRect()))
            {
                ((TreeBoss)Main.npc[parent.whoAmI].modNPC).AbsorbedEnergies += 1;

                if (parent.life < parent.lifeMax)
                {
                    parent.life += Main.expertMode ? 20 : 10;
                }

                npc.life = 0;
                npc.active = false;
            }
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            #region Dust Code
            Vector2 vector23 = projectile.Center + Vector2.One * -20f;
            int num137 = 40;
            int num138 = num137;
            for (int num139 = 0; num139 < 2; num139++)
            {
                int num140 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 100, default(Color), 0.125f);
                Main.dust[num140].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
            }

            for (int num141 = 0; num141 < 5; num141++)
            {
                int num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 200, default(Color), 0.35f);
                Main.dust[num142].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                Main.dust[num142].noGravity = true;
                Main.dust[num142].noLight = true;
                Dust dust = Main.dust[num142];
                dust.velocity *= 3f;
                dust = Main.dust[num142];
                dust.velocity += projectile.DirectionTo(Main.dust[num142].position) * (2f + Main.rand.NextFloat() * 4f);
                num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 100, default(Color), 0.125f);
                Main.dust[num142].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                dust = Main.dust[num142];
                dust.velocity *= 2f;
                Main.dust[num142].noGravity = true;
                Main.dust[num142].fadeIn = 1f;
                Main.dust[num142].color = Color.Crimson * 0.5f;
                Main.dust[num142].noLight = true;
                dust = Main.dust[num142];
                dust.velocity += projectile.DirectionTo(Main.dust[num142].position) * 8f;
            }

            for (int num143 = 0; num143 < 5; num143++)
            {
                int num144 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 0, default(Color), 0.35f);
                Main.dust[num144].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(projectile.velocity.ToRotation()) * num137 / 2f;
                Main.dust[num144].noGravity = true;
                Main.dust[num144].noLight = true;
                Dust dust = Main.dust[num144];
                dust.velocity *= 3f;
                dust = Main.dust[num144];
                dust.velocity += projectile.DirectionTo(Main.dust[num144].position) * 2f;
            }

            for (int num145 = 0; num145 < 50; num145++)
            {
                int num146 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 0, default(Color), 0.125f);
                Main.dust[num146].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(projectile.velocity.ToRotation()) * num137 / 2f;
                Main.dust[num146].noGravity = true;
                Dust dust = Main.dust[num146];
                dust.velocity *= 3f;
                dust = Main.dust[num146];
                dust.velocity += projectile.DirectionTo(Main.dust[num146].position) * 3f;
            }
            #endregion

            base.OnHitByProjectile(projectile, damage, knockback, crit);
        }
    }

    // I literally don't know how to check for the type of the NPC
    public class AbsorbEnergyP2 : ModNPC
    {
        public bool RunOnce = true;
        public Vector2 InitialDistance;
        public int RandomStopping;
        public bool HasStopped;
        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Natural Energy");
        }

        public override void SetDefaults()
        {
            npc.lifeMax = 10;
            npc.width = 10;
            npc.height = 10;
            npc.alpha = 255;
            npc.damage = 17;
            npc.friendly = false;
            npc.noTileCollide = true;
        }

        public override void AI()
        {
            // Get the ID of the Parent NPC that was passed in via ai[0]
            NPC parent = Main.npc[(int)npc.ai[0]];

            if (RunOnce)
            {
                // Grab the starting distance of when this NPC spawns
                InitialDistance = npc.Center;
                RandomStopping = Main.rand.Next(400, 600);

                npc.netUpdate = true;
            }

            Vector2 newMove = parent.Center - npc.Center;
            Vector2 move = newMove;
            float moveSpeed = Main.expertMode ? 180f : 210f;

            // Make the NPC stop midway from the initial spawnpoint to the boss for a few seconds
            if (npc.ai[1] < 120 && parent.Distance(npc.Center) <= RandomStopping)
            {
                npc.velocity = Vector2.Zero;
                npc.ai[1]++;

                HasStopped = true;
            }
            else
            {
                npc.velocity = move / (moveSpeed + (HasStopped ? 100 : 0));
                npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(45f));
            }

            #region Dust Code
            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.TerraBlade, npc.velocity.X, npc.velocity.Y, 50, default(Color), 0.4f);
                switch (num1103)
                {
                    case 0:
                        Main.dust[num1106].position = (Main.dust[num1106].position + npc.Center * 5f) / 6f;
                        break;
                    case 1:
                        Main.dust[num1106].position = (Main.dust[num1106].position + (npc.Center + npc.velocity / 2f) * 5f) / 6f;
                        break;
                }
                Dust dust81 = Main.dust[num1106];
                dust81.velocity *= 0.1f;
                Main.dust[num1106].noGravity = true;
                Main.dust[num1106].fadeIn = 1f;
            }
            #endregion

            if (npc.getRect().Intersects(parent.getRect()))
            {
                ((TreeBossP2)Main.npc[parent.whoAmI].modNPC).AbsorbedEnergies += 1;

                if (parent.life < parent.lifeMax)
                {
                    parent.life += Main.expertMode ? 20 : 10;
                }

                npc.life = 0;
                npc.active = false;
            }
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            #region Dust Code
            Vector2 vector23 = projectile.Center + Vector2.One * -20f;
            int num137 = 40;
            int num138 = num137;
            for (int num139 = 0; num139 < 2; num139++)
            {
                int num140 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 100, default(Color), 0.125f);
                Main.dust[num140].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
            }

            for (int num141 = 0; num141 < 5; num141++)
            {
                int num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 200, default(Color), 0.35f);
                Main.dust[num142].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                Main.dust[num142].noGravity = true;
                Main.dust[num142].noLight = true;
                Dust dust = Main.dust[num142];
                dust.velocity *= 3f;
                dust = Main.dust[num142];
                dust.velocity += projectile.DirectionTo(Main.dust[num142].position) * (2f + Main.rand.NextFloat() * 4f);
                num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 100, default(Color), 0.125f);
                Main.dust[num142].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                dust = Main.dust[num142];
                dust.velocity *= 2f;
                Main.dust[num142].noGravity = true;
                Main.dust[num142].fadeIn = 1f;
                Main.dust[num142].color = Color.Crimson * 0.5f;
                Main.dust[num142].noLight = true;
                dust = Main.dust[num142];
                dust.velocity += projectile.DirectionTo(Main.dust[num142].position) * 8f;
            }

            for (int num143 = 0; num143 < 5; num143++)
            {
                int num144 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 0, default(Color), 0.35f);
                Main.dust[num144].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(projectile.velocity.ToRotation()) * num137 / 2f;
                Main.dust[num144].noGravity = true;
                Main.dust[num144].noLight = true;
                Dust dust = Main.dust[num144];
                dust.velocity *= 3f;
                dust = Main.dust[num144];
                dust.velocity += projectile.DirectionTo(Main.dust[num144].position) * 2f;
            }

            for (int num145 = 0; num145 < 50; num145++)
            {
                int num146 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 0, default(Color), 0.125f);
                Main.dust[num146].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(projectile.velocity.ToRotation()) * num137 / 2f;
                Main.dust[num146].noGravity = true;
                Dust dust = Main.dust[num146];
                dust.velocity *= 3f;
                dust = Main.dust[num146];
                dust.velocity += projectile.DirectionTo(Main.dust[num146].position) * 3f;
            }
            #endregion

            base.OnHitByProjectile(projectile, damage, knockback, crit);
        }
    }

    public class AbsorbEnergyCinematic : ModProjectile
    {
        public bool RunOnce = true;
        public Vector2 InitialDistance;
        public float TravelDistance;
        public int RandomStopping;
        public bool HasStopped;
        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override bool CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Natural Energy");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 900;
        }

        public override void AI()
        {
            // Get the ID of the Parent NPC that was passed in via ai[0]
            NPC parent = Main.npc[(int)projectile.ai[0]];

            Vector2 move = Vector2.Zero;
            Vector2 newMove = parent.Center - projectile.Center;
            move = newMove;
            float launchSpeed = 30f;
            projectile.velocity = (move) / launchSpeed;

            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 0.4f);
                switch (num1103)
                {
                    case 0:
                        Main.dust[num1106].position = (Main.dust[num1106].position + projectile.Center * 5f) / 6f;
                        break;
                    case 1:
                        Main.dust[num1106].position = (Main.dust[num1106].position + (projectile.Center + projectile.velocity / 2f) * 5f) / 6f;
                        break;
                }
                Dust dust81 = Main.dust[num1106];
                dust81.velocity *= 0.1f;
                Main.dust[num1106].noGravity = true;
                Main.dust[num1106].fadeIn = 1f;
            }

            if (projectile.getRect().Intersects(parent.getRect()))
            {
                Particle.CreateParticle(Particle.ParticleType<Shockwave>(), projectile.Center, Vector2.Zero, new Color(195, 255, 154), 0.5f, 0.25f);
                projectile.Kill();  
            }
        }
    }
}