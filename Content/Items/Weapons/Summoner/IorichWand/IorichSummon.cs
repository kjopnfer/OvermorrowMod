using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Content.Buffs.Summon;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.IorichWand
{
    public class IorichSummon : ModProjectile
    {
        private int ShieldCooldown = 0;
        public override bool CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich's Avatar");
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 80;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.friendly = true;
            projectile.minionSlots = 2f;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.netImportant = true;
            projectile.manualDirectionChange = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;

            drawOffsetX = -25;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (!player.active)
            {
                projectile.active = false;
                return;
            }

            if (player.HeldItem.type == ModContent.ItemType<IorichWand>())
            {
                if (++projectile.ai[1] % 60 == 0 && player.GetModPlayer<OvermorrowModPlayer>().IorichGuardianEnergy < 100 && !player.channel)
                {
                    for (int i = 0; i < Main.rand.Next(4); i++)
                    {
                        float RandomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                        Vector2 RandomPosition = player.Center + new Vector2(200, 0).RotatedBy(-RandomRotation);
                        int RotationDirection = Main.rand.NextBool() ? 1 : -1;
                        int RotationAngle = Main.rand.Next(6, 10);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(RandomPosition, Vector2.Zero, ModContent.ProjectileType<IorichSummonEnergy>(), 0, 0f, projectile.owner, RotationDirection, RotationAngle);
                        }
                    }
                }

                if (player.channel && Main.myPlayer == projectile.owner && !player.noItems && !player.CCed)
                {
                    // Check if there is a sufficient amount of energy to use
                    bool CheckEnergy = (player.GetModPlayer<OvermorrowModPlayer>().IorichGuardianEnergy - 3) >= 0;
                    if (++projectile.ai[0] % 20 == 0 && CheckEnergy)
                    {
                        player.GetModPlayer<OvermorrowModPlayer>().IorichGuardianEnergy -= 5;

                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 RandomPosition = projectile.Center + new Vector2(Main.rand.Next(-70, -60) * projectile.direction, Main.rand.Next(-50, 50));
                            Vector2 RandomVelocity = Vector2.Normalize(projectile.DirectionTo(Main.MouseWorld)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(20, 45))) * Main.rand.Next(6, 9);

                            Projectile.NewProjectile(RandomPosition, RandomVelocity, ModContent.ProjectileType<IorichSummonStar>(), projectile.damage, projectile.knockBack, projectile.owner, 0, -1);

                            RandomVelocity = Vector2.Normalize(projectile.DirectionTo(Main.MouseWorld)).RotatedBy(MathHelper.ToRadians(-Main.rand.Next(20, 45))) * Main.rand.Next(6, 9);
                            Projectile.NewProjectile(RandomPosition, RandomVelocity, ModContent.ProjectileType<IorichSummonStar>(), projectile.damage, projectile.knockBack, projectile.owner, 0, 1);
                        }
                    }
                }

                if (ShieldCooldown == 0)
                {
                    // Check if there is a sufficient amount of energy to use
                    bool CheckEnergy = (player.GetModPlayer<OvermorrowModPlayer>().IorichGuardianEnergy - 20) >= 0;

                    if (Main.myPlayer == projectile.owner && Main.mouseRight && CheckEnergy)
                    {
                        player.statLife += 25;
                        player.HealEffect(25);

                        player.GetModPlayer<OvermorrowModPlayer>().IorichGuardianEnergy -= 20;
                        player.AddBuff(ModContent.BuffType<ShieldBuff>(), 360);
                        ShieldCooldown = 480;
                    }
                }
                else
                {
                    ShieldCooldown--;
                }
            }

            Summon_Idle();

            // Loop through 8 frames
            if (++projectile.frameCounter >= 8)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame > 7)
                {
                    projectile.frame = 0;
                }
            }
        }

        private void Summon_Idle()
        {
            Player player = Main.player[projectile.owner];
            if (!player.active)
            {
                projectile.active = false;
                return;
            }

            if (player.HasBuff(ModContent.BuffType<IorichGuardian>()))  
            {
                projectile.timeLeft = 2;
            }

            Vector2 vector = player.Center;
            vector.X -= (5 + player.width / 2) * player.direction;
            vector.Y -= 50f;

            projectile.Center = Vector2.Lerp(projectile.Center, vector, 0.05f);
            projectile.velocity *= 0.5f;
            projectile.direction = (projectile.spriteDirection = player.direction);


            projectile.velocity *= 0.7f;
            projectile.Center = Vector2.Lerp(projectile.Center, vector, 0.2f);
        }
    }

    public class IorichSummonEnergy : ModProjectile
    {
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
            Player parent = Main.player[projectile.owner];

            Vector2 Target = projectile.Center - parent.Center;
            projectile.velocity = Vector2.Lerp(projectile.velocity, Target.SafeNormalize(Vector2.UnitX) * -6, 0.1f);
            projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(projectile.ai[1]) * projectile.ai[0]);

            Dust dust = Terraria.Dust.NewDustPerfect(projectile.Center, 107, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);

            if (projectile.getRect().Intersects(parent.getRect()))
            {
                Main.PlaySound(SoundID.DD2_DarkMageHealImpact);
                Particle.CreateParticle(Particle.ParticleType<Pulse>(), projectile.Center, Vector2.Zero, new Color(195, 255, 154), 0.5f, 0.25f);

                parent.GetModPlayer<OvermorrowModPlayer>().IorichGuardianEnergy += 3;

                projectile.Kill();
            }
        }
    }

    public class IorichSummonStar : ModProjectile, ITrailEntity
    {
        public Vector2 TargetPosition;
        public Color TrailColor(float progress) => ProjectileColor;
        public float TrailSize(float progress) => 20;
        public Type TrailType()
        {
            return typeof(TorchTrail);
        }

        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public Color ProjectileColor = Main.DiscoColor;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prisma Starstorm");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 240;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (projectile.localAI[0]++ == 0)
            {
                DelegateMethods.v3_1 = new Vector3(0.6f, 1f, 1f) * 0.2f;
                Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * 10f, 8f, DelegateMethods.CastLightOpen);
                float num108 = 16f;
                for (int num109 = 0; (float)num109 < num108; num109++)
                {
                    Vector2 spinningpoint5 = Vector2.UnitX * 0f;
                    spinningpoint5 += -Vector2.UnitY.RotatedBy((float)num109 * ((float)Math.PI * 2f / num108)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(projectile.velocity.ToRotation());

                    int num110 = Dust.NewDust(projectile.Center, 0, 0, DustID.RainbowMk2);
                    Main.dust[num110].scale = 1.5f;
                    Main.dust[num110].noGravity = true;
                    Main.dust[num110].position = projectile.Center + spinningpoint5;
                    Main.dust[num110].velocity = projectile.velocity * 0f + spinningpoint5.SafeNormalize(Vector2.UnitY) * 1f;
                }
            }

            if (Main.rand.NextBool(5))
            {
                int num98 = Dust.NewDust(new Vector2(projectile.position.X + projectile.velocity.X, projectile.position.Y + projectile.velocity.Y), projectile.width, projectile.height, DustID.RainbowMk2, projectile.velocity.X, projectile.velocity.Y, 100, ProjectileColor, 2f * projectile.scale);
                Main.dust[num98].noGravity = true;
            }


            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.ai[0]++ < 120) projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(0.5f * projectile.ai[1]));
        }

        //public override bool ShouldUpdatePosition() => projectile.ai[1] > 60 ? true : false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture(AssetDirectory.Textures + "Spotlight");

            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), ProjectileColor, projectile.rotation, drawOrigin, projectile.scale * 0.4f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), ProjectileColor, projectile.rotation, drawOrigin, projectile.scale * 0.4f, SpriteEffects.None, 0);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), ProjectileColor, projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(0.15f, 1f), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), ProjectileColor, projectile.rotation, drawOrigin, new Vector2(0.15f, 1f), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Particle.CreateParticle(Particle.ParticleType<Pulse>(), projectile.Center, Vector2.Zero, ProjectileColor, 1, 0.5f, 0, 1f);

            Vector2 origin = projectile.Center;
            float radius = 15;
            int numLocations = 6;

            for (int i = 0; i < 6; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 3f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));

                Particle.CreateParticle(Particle.ParticleType<Glow>(), position, dustvelocity, ProjectileColor, 1, 0.5f, MathHelper.ToRadians(360f / numLocations * i), 1f);
            }

            Main.PlaySound(SoundID.Item14);
        }
    }

    public class IorichSummonShield : ModProjectile
    {
        public override string Texture => AssetDirectory.Textures + "Perlin";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shield");
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 90;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.alpha = 0;
            projectile.timeLeft = 360;
        }

        public override void AI()
        {
            projectile.Center = Main.player[projectile.owner].Center;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Vector2 scale = new Vector2(1.5f, 1f);
            DrawData drawData = new DrawData(ModContent.GetTexture("Terraria/Misc/Perlin"),
                projectile.Center - Main.screenPosition + projectile.Size * scale * 0.5f,
                new Rectangle(0, 0, projectile.width, projectile.height),
                Color.LightGreen,
                projectile.rotation,
                projectile.Size,
                scale,
                SpriteEffects.None, 0);

            GameShaders.Misc["ForceField"].UseColor(Color.LightGreen);
            GameShaders.Misc["ForceField"].Apply(drawData);
            drawData.Draw(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}