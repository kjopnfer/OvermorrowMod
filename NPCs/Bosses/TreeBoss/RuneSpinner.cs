using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Effects.Prim.Trails;
using OvermorrowMod.Particles;
using OvermorrowMod.WardenClass;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public class RuneSpinner : ModProjectile
    {
        private bool RunOnce = true;
        private float Radius;
        private int MAX_TIME = 600;

        public NPC RotationCenter;
        public Vector2 OldPosition;
        public bool CanDespawn = false;
        public override bool CanDamage() => false;
        public override bool? CanCutTiles() => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune Spinner");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 25;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 42;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = MAX_TIME;
        }

        public override void AI()
        {
            //projectile.rotation = projectile.DirectionTo(RotationCenter.Center).ToRotation() + (MathHelper.PiOver2 * 3);
            projectile.rotation += 0.64f;

            if (RunOnce)
            {
                Main.PlaySound(SoundID.Item46, projectile.Center);

                Radius = projectile.ai[1];
                projectile.ai[1] = 0;

                RunOnce = false;

                #region Dust Code
                Vector2 vector23 = projectile.Center + Vector2.One * -20f;
                int num137 = 40;
                int num138 = num137;
                for (int num139 = 0; num139 < 4; num139++)
                {
                    int num140 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 100, default(Color), 0.25f);
                    Main.dust[num140].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                }

                for (int num141 = 0; num141 < 10; num141++)
                {
                    int num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 200, default(Color), 0.7f);
                    Main.dust[num142].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                    Main.dust[num142].noGravity = true;
                    Main.dust[num142].noLight = true;
                    Dust dust = Main.dust[num142];
                    dust.velocity *= 3f;
                    dust = Main.dust[num142];
                    dust.velocity += projectile.DirectionTo(Main.dust[num142].position) * (2f + Main.rand.NextFloat() * 4f);
                    num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 100, default(Color), 0.25f);
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

                for (int num143 = 0; num143 < 10; num143++)
                {
                    int num144 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 0, default(Color), 0.7f);
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
                    int num146 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 0, default(Color), 0.25f);
                    Main.dust[num146].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(projectile.velocity.ToRotation()) * num137 / 2f;
                    Main.dust[num146].noGravity = true;
                    Dust dust = Main.dust[num146];
                    dust.velocity *= 3f;
                    dust = Main.dust[num146];
                    dust.velocity += projectile.DirectionTo(Main.dust[num146].position) * 3f;
                }
                #endregion
            }

            projectile.alpha = (int)MathHelper.Lerp(255, 0, Utils.Clamp(projectile.timeLeft, 0, 60) / 60f);

            if (projectile.timeLeft < MAX_TIME - 60)
            {
                if (!CanDespawn && projectile.timeLeft < 60)
                {
                    projectile.timeLeft = 60;
                }

                projectile.Center = RotationCenter.Center + new Vector2(Radius, 0).RotatedBy(projectile.ai[0]);

                projectile.ai[0] += MathHelper.Lerp(0, 0.065f, Utils.Clamp(projectile.ai[1], 0, 90f) / 90f);

                projectile.ai[1]++;
                projectile.localAI[0]++;
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft < MAX_TIME - 60)
            {
                Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/TreeBoss/RuneSpinner_Trail");

                Color color = new Color(135, 255, 141);
                int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
                int y2 = num154 * projectile.frame;
                Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

                Vector2 origin2 = drawRectangle.Size() / 2f;
                var off = new Vector2(projectile.width / 2f, projectile.height / 2f);


                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
                {
                    Color color2 = color;
                    color2 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Vector2 value4 = projectile.oldPos[i];
                    float num165 = projectile.oldRot[i];
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] - Main.screenPosition + off, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), Color.Lerp(Color.Transparent, color2, Utils.Clamp(projectile.timeLeft, 0, 60) / 60f), num165, origin2, projectile.scale, SpriteEffects.None, 0f);
                }
            }

            return base.PreDraw(spriteBatch, lightColor);
        }
    }

    public class PortalSpinner : ModProjectile
    {
        private bool RunOnce = true;
        private float Radius;
        private int MAX_TIME = 260;

        public Projectile RotationCenter;
        public Vector2 OldPosition;
        public bool CanDespawn = false;
        public bool RotateOpposite = false;
        public override bool CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override string Texture => "OvermorrowMod/NPCs/Bosses/TreeBoss/RuneSpinner";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune Spinner");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 25;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 42;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = MAX_TIME;
        }

        public override void AI()
        {
            projectile.rotation = projectile.DirectionTo(RotationCenter.Center).ToRotation() + (MathHelper.PiOver2 * 3);
            //projectile.rotation += 0.64f;

            int ModifyDirection = RotateOpposite ? -1 : 1;

            if (RunOnce)
            {
                Main.PlaySound(SoundID.Item46, projectile.Center);

                Radius = projectile.ai[1];
                projectile.ai[1] = 0;
                RunOnce = false;

                #region Dust Code
                Vector2 vector23 = projectile.Center + Vector2.One * -20f;
                int num137 = 40;
                int num138 = num137;
                for (int num139 = 0; num139 < 4; num139++)
                {
                    int num140 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 100, default(Color), 0.25f);
                    Main.dust[num140].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                }

                for (int num141 = 0; num141 < 10; num141++)
                {
                    int num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 200, default(Color), 0.7f);
                    Main.dust[num142].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                    Main.dust[num142].noGravity = true;
                    Main.dust[num142].noLight = true;
                    Dust dust = Main.dust[num142];
                    dust.velocity *= 3f;
                    dust = Main.dust[num142];
                    dust.velocity += projectile.DirectionTo(Main.dust[num142].position) * (2f + Main.rand.NextFloat() * 4f);
                    num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 100, default(Color), 0.25f);
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

                for (int num143 = 0; num143 < 10; num143++)
                {
                    int num144 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 0, default(Color), 0.7f);
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
                    int num146 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 0, default(Color), 0.25f);
                    Main.dust[num146].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(projectile.velocity.ToRotation()) * num137 / 2f;
                    Main.dust[num146].noGravity = true;
                    Dust dust = Main.dust[num146];
                    dust.velocity *= 3f;
                    dust = Main.dust[num146];
                    dust.velocity += projectile.DirectionTo(Main.dust[num146].position) * 3f;
                }
                #endregion
            }

            projectile.alpha = (int)MathHelper.Lerp(255, 0, Utils.Clamp(projectile.timeLeft, 0, 60) / 60f);

            if (projectile.ai[1] <= 300) Radius = projectile.ai[1] += 2;

            projectile.Center = RotationCenter.Center + new Vector2(Radius, 0).RotatedBy(projectile.ai[0] * ModifyDirection);

            projectile.ai[0] += MathHelper.Lerp(0, 0.065f, Utils.Clamp(projectile.ai[1], 0, 90f) / 90f);
            projectile.localAI[0]++;

        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft < MAX_TIME - 60)
            {
                Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/TreeBoss/RuneSpinner_Trail");

                Color color = new Color(135, 255, 141);
                int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
                int y2 = num154 * projectile.frame;
                Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

                Vector2 origin2 = drawRectangle.Size() / 2f;
                var off = new Vector2(projectile.width / 2f, projectile.height / 2f);


                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
                {
                    Color color2 = color;
                    color2 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Vector2 value4 = projectile.oldPos[i];
                    float num165 = projectile.oldRot[i];

                    float scale = projectile.scale * (ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Main.spriteBatch.Draw(texture, projectile.oldPos[i] - Main.screenPosition + off, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), Color.Lerp(Color.Transparent, color2, Utils.Clamp(projectile.timeLeft, 0, 60) / 60f), num165, origin2, scale, SpriteEffects.None, 0f);
                }
            }

            return base.PreDraw(spriteBatch, lightColor);
        }
    }
}