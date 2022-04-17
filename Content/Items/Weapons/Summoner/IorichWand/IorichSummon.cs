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
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.IorichWand
{
    public class IorichSummon : ModProjectile
    {
        private int ShieldCooldown = 0;
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich's Avatar");
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 80;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 2f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;

            DrawOffsetX = -25;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }

            if (player.HeldItem.type == ModContent.ItemType<IorichWand>())
            {
                if (++Projectile.ai[1] % 60 == 0 && player.GetModPlayer<OvermorrowModPlayer>().IorichGuardianEnergy < 100 && !player.channel)
                {
                    for (int i = 0; i < Main.rand.Next(4); i++)
                    {
                        float RandomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                        Vector2 RandomPosition = player.Center + new Vector2(200, 0).RotatedBy(-RandomRotation);
                        int RotationDirection = Main.rand.NextBool() ? 1 : -1;
                        int RotationAngle = Main.rand.Next(6, 10);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), RandomPosition, Vector2.Zero, ModContent.ProjectileType<IorichSummonEnergy>(), 0, 0f, Projectile.owner, RotationDirection, RotationAngle);
                        }
                    }
                }

                if (player.channel && Main.myPlayer == Projectile.owner && !player.noItems && !player.CCed)
                {
                    // Check if there is a sufficient amount of energy to use
                    bool CheckEnergy = (player.GetModPlayer<OvermorrowModPlayer>().IorichGuardianEnergy - 3) >= 0;
                    if (++Projectile.ai[0] % 20 == 0 && CheckEnergy)
                    {
                        player.GetModPlayer<OvermorrowModPlayer>().IorichGuardianEnergy -= 5;

                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 RandomPosition = Projectile.Center + new Vector2(Main.rand.Next(-70, -60) * Projectile.direction, Main.rand.Next(-50, 50));
                            Vector2 RandomVelocity = Vector2.Normalize(Projectile.DirectionTo(Main.MouseWorld)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(20, 45))) * Main.rand.Next(6, 9);

                            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), RandomPosition, RandomVelocity, ModContent.ProjectileType<IorichSummonStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, -1);

                            RandomVelocity = Vector2.Normalize(Projectile.DirectionTo(Main.MouseWorld)).RotatedBy(MathHelper.ToRadians(-Main.rand.Next(20, 45))) * Main.rand.Next(6, 9);
                            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), RandomPosition, RandomVelocity, ModContent.ProjectileType<IorichSummonStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
                        }
                    }
                }

                if (ShieldCooldown == 0)
                {
                    // Check if there is a sufficient amount of energy to use
                    bool CheckEnergy = (player.GetModPlayer<OvermorrowModPlayer>().IorichGuardianEnergy - 20) >= 0;

                    if (Main.myPlayer == Projectile.owner && Main.mouseRight && CheckEnergy)
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
            if (++Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 7)
                {
                    Projectile.frame = 0;
                }
            }
        }

        private void Summon_Idle()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }

            if (player.HasBuff(ModContent.BuffType<IorichGuardian>()))  
            {
                Projectile.timeLeft = 2;
            }

            Vector2 vector = player.Center;
            vector.X -= (5 + player.width / 2) * player.direction;
            vector.Y -= 50f;

            Projectile.Center = Vector2.Lerp(Projectile.Center, vector, 0.05f);
            Projectile.velocity *= 0.5f;
            Projectile.direction = (Projectile.spriteDirection = player.direction);


            Projectile.velocity *= 0.7f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, vector, 0.2f);
        }
    }

    public class IorichSummonEnergy : ModProjectile
    {
        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Natural Energy");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 900;
        }

        public override void AI()
        {
            Player parent = Main.player[Projectile.owner];

            Vector2 Target = Projectile.Center - parent.Center;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Target.SafeNormalize(Vector2.UnitX) * -6, 0.1f);
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(Projectile.ai[1]) * Projectile.ai[0]);

            Dust dust = Terraria.Dust.NewDustPerfect(Projectile.Center, 107, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);

            if (Projectile.getRect().Intersects(parent.getRect()))
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact);
                Particle.CreateParticle(Particle.ParticleType<Pulse>(), Projectile.Center, Vector2.Zero, new Color(195, 255, 154), 0.5f, 0.25f);

                parent.GetModPlayer<OvermorrowModPlayer>().IorichGuardianEnergy += 3;

                Projectile.Kill();
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
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
            {
                DelegateMethods.v3_1 = new Vector3(0.6f, 1f, 1f) * 0.2f;
                Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 10f, 8f, DelegateMethods.CastLightOpen);
                float num108 = 16f;
                for (int num109 = 0; (float)num109 < num108; num109++)
                {
                    Vector2 spinningpoint5 = Vector2.UnitX * 0f;
                    spinningpoint5 += -Vector2.UnitY.RotatedBy((float)num109 * ((float)Math.PI * 2f / num108)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(Projectile.velocity.ToRotation());

                    int num110 = Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowMk2);
                    Main.dust[num110].scale = 1.5f;
                    Main.dust[num110].noGravity = true;
                    Main.dust[num110].position = Projectile.Center + spinningpoint5;
                    Main.dust[num110].velocity = Projectile.velocity * 0f + spinningpoint5.SafeNormalize(Vector2.UnitY) * 1f;
                }
            }

            if (Main.rand.NextBool(5))
            {
                int num98 = Dust.NewDust(new Vector2(Projectile.position.X + Projectile.velocity.X, Projectile.position.Y + Projectile.velocity.Y), Projectile.width, Projectile.height, DustID.RainbowMk2, Projectile.velocity.X, Projectile.velocity.Y, 100, ProjectileColor, 2f * Projectile.scale);
                Main.dust[num98].noGravity = true;
            }


            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0]++ < 120) Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(0.5f * Projectile.ai[1]));
        }

        //public override bool ShouldUpdatePosition() => Projectile.ai[1] > 60 ? true : false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;

            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), ProjectileColor, Projectile.rotation, drawOrigin, Projectile.scale * 0.4f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), ProjectileColor, Projectile.rotation, drawOrigin, Projectile.scale * 0.4f, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), ProjectileColor, Projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(0.15f, 1f), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), ProjectileColor, Projectile.rotation, drawOrigin, new Vector2(0.15f, 1f), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Particle.CreateParticle(Particle.ParticleType<Pulse>(), Projectile.Center, Vector2.Zero, ProjectileColor, 1, 0.5f, 0, 1f);

            Vector2 origin = Projectile.Center;
            float radius = 15;
            int numLocations = 6;

            for (int i = 0; i < 6; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 3f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));

                Particle.CreateParticle(Particle.ParticleType<Glow>(), position, dustvelocity, ProjectileColor, 1, 0.5f, MathHelper.ToRadians(360f / numLocations * i), 1f);
            }

            SoundEngine.PlaySound(SoundID.Item14);
        }
    }

    public class IorichSummonShield : ModProjectile
    {
        public override string Texture => AssetDirectory.Textures + "Perlin";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shield");
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            Projectile.Center = Main.player[Projectile.owner].Center;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Vector2 scale = new Vector2(1.5f, 1f);
            DrawData drawData = new DrawData(ModContent.Request<Texture2D>("Terraria/Misc/Perlin").Value,
                Projectile.Center - Main.screenPosition + Projectile.Size * scale * 0.5f,
                new Rectangle(0, 0, Projectile.width, Projectile.height),
                Color.LightGreen,
                Projectile.rotation,
                Projectile.Size,
                scale,
                SpriteEffects.None, 0);

            GameShaders.Misc["ForceField"].UseColor(Color.LightGreen);
            GameShaders.Misc["ForceField"].Apply(drawData);
            Main.EntitySpriteDraw(drawData);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}