using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class Fragment : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Fragment");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            projectile.hide = true;
            projectile.extraUpdates = 1;

            drawOffsetX = -5;
            //drawOriginOffsetY = -7;
            drawOriginOffsetY = 2;
        }

        public override void AI()
        {
            if (projectile.ai[0]++ == 0)
            {
                float radius = 15;
                int numLocations = 10;
                for (int i = 0; i < numLocations; i++)
                {
                    Vector2 position = projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(position, 2, 2, DustID.Enchanted_Gold, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            if (projectile.ai[0] < projectile.ai[1])
            {
                return false;
            }

            return base.ShouldUpdatePosition();
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/Fragment_Trail");
            var off = new Vector2(projectile.width / 2f, projectile.height / 2f) + new Vector2(0, 10);
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            var frame = new Rectangle(0, frameHeight * projectile.frame, texture.Width, frameHeight - 2);
            var orig = frame.Size() / 2f;

            Color color = Color.Yellow;
            var trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            var fadeMult = 1f / trailLength;
            for (int i = 1; i < trailLength; i++)
            {
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] - Main.screenPosition + off, frame, color * (1f - fadeMult * i), projectile.oldRot[i], orig, projectile.scale * (trailLength - i) / trailLength, SpriteEffects.None, 0f);
            }

            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTime * 2) * 0.1f);
            float scale = projectile.scale * 2 * mult;

            spriteBatch.Draw(texture, projectile.Center + new Vector2(0, 10) - Main.screenPosition, drawRectangle, color, projectile.rotation, new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2), scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(SpriteSortMode.Immediate);

            Effect effect = OvermorrowModFile.Instance.Whiteout;
            float progress = Utils.Clamp(projectile.localAI[0]++, 0, 15f) / 15f;
            effect.Parameters["WhiteoutColor"].SetValue(Color.Yellow.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(1 - progress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            texture = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(texture, projectile.Center + new Vector2(0, 10) - Main.screenPosition, null, Color.White, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, 1, SpriteEffects.None, 0f);

            spriteBatch.Reload(SpriteSortMode.Deferred);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            float radius = 15;
            int numLocations = 10;
            for (int i = 0; i < numLocations; i++)
            {
                Vector2 position = projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                int dust = Dust.NewDust(position, 2, 2, DustID.Enchanted_Gold, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.extraUpdates = 0;
            projectile.velocity = Vector2.Zero;
            return false;
        }
    }

    public class HorizontalFragment : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => Color.Lerp(new Color(253, 254, 255), new Color(244, 188, 91), progress) * progress;
        public float TrailSize(float progress) => 12;
        public Type TrailType() => typeof(TorchTrail);

        public override string Texture => "OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/Fragment";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light Warping Bullets");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 1200;
            projectile.penetrate = -1;
            projectile.extraUpdates = 10;

            drawOffsetX = -5;
            //drawOriginOffsetY = -7;
            drawOriginOffsetY = 2;
        }

        public ref float AICounter => ref projectile.ai[0];
        public ref float Delay => ref projectile.ai[1];

        public override void AI()
        {
            if (AICounter++ == 0)
            {
                float radius = 15;
                int numLocations = 10;
                for (int i = 0; i < numLocations; i++)
                {
                    Vector2 position = projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(position, 2, 2, DustID.Enchanted_Gold, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                    Main.dust[dust].noGravity = true;
                }
            }

            Tile tile = Framing.GetTileSafely((int)(projectile.Center.X / 16), (int)(projectile.Center.Y / 16));
            if (tile.active() && tile.type == TileID.Gold)
            {
                projectile.Kill();
            }

            projectile.localAI[0]++;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool ShouldUpdatePosition()
        {
            if (AICounter < Delay)
            {
                return false;
            }

            return base.ShouldUpdatePosition();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            /*
            Texture2D texture = ModContent.GetTexture(AssetDirectory.Boss + "SandstormBoss/Fragment_Trail");
            var off = new Vector2(projectile.width / 2f, projectile.height / 2f) + new Vector2(0, 10);
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            var frame = new Rectangle(0, frameHeight * projectile.frame, texture.Width, frameHeight - 2);
            var orig = frame.Size() / 2f;

            var trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            var fadeMult = 1f / trailLength;
            for (int i = 1; i < trailLength; i++)
            {
                Color color = Color.Lerp(Color.Yellow, Color.Orange, i / trailLength);
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] - Main.screenPosition + off, frame, color * (1f - fadeMult * i), projectile.oldRot[i], orig, projectile.scale * (trailLength - i) / trailLength, SpriteEffects.None, 0f);
            }

            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTime * 2) * 0.1f);
            float scale = projectile.scale * 2 * mult;

            //Color pulseColor = Color.Yellow;
            //spriteBatch.Draw(texture, projectile.Center + new Vector2(0, 10) - Main.screenPosition, drawRectangle, pulseColor, projectile.rotation, new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2), scale, SpriteEffects.None, 0f);

            */

            if (projectile.localAI[0] < Delay)
            {
                spriteBatch.Reload(SpriteSortMode.Immediate);

                Effect effect = OvermorrowModFile.Instance.Whiteout;

                float progress = Utils.Clamp(projectile.localAI[0], 0, Delay) / Delay;
                effect.Parameters["WhiteoutColor"].SetValue(new Color(244, 188, 91).ToVector3());
                // effect.Parameters["WhiteoutProgress"].SetValue(1 - progress); Fade out of a color
                effect.Parameters["WhiteoutProgress"].SetValue(progress);

                effect.CurrentTechnique.Passes["Whiteout"].Apply();

                Texture2D texture = Main.projectileTexture[projectile.type];
                spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, 1, SpriteEffects.None, 0f);

                spriteBatch.Reload(SpriteSortMode.Deferred);
            }
            else
            {
                spriteBatch.Reload(BlendState.Additive);

                Texture2D texture = ModContent.GetTexture(AssetDirectory.Boss + "SandstormBoss/Fragment_Trail");
                spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, new Color(244, 188, 91), projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, 1, SpriteEffects.None, 0f);

                spriteBatch.Reload(BlendState.AlphaBlend);
            }

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Particle.CreateParticle(Particle.ParticleType<Pulse2>(), projectile.Center, Vector2.Zero, Color.Yellow);

            float radius = 15;
            int numLocations = 10;
            for (int i = 0; i < numLocations; i++)
            {
                Vector2 position = projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                int dust = Dust.NewDust(position, 2, 2, DustID.Enchanted_Gold, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                Main.dust[dust].noGravity = true;
            }
        }
    }

    public class FragmentCenter : ModProjectile
    {
        public override bool CanDamage() => false;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Fragment");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 1200;
            projectile.penetrate = -1;
            projectile.hide = true;
            projectile.extraUpdates = 1;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            if (projectile.ai[0]++ == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 RotationPosition = projectile.Center + new Vector2(0, 250).RotatedBy(i * (360 / 8));
                    Projectile.NewProjectile(RotationPosition, Vector2.Zero, ModContent.ProjectileType<SpinningFragment>(), projectile.damage, 4f, Main.myPlayer, projectile.whoAmI, i * (360 / 8));
                }
            }
        }

        public override bool ShouldUpdatePosition() => projectile.ai[0] > 90;
    }

    public class SpinningFragment : ModProjectile
    {
        private Projectile ParentProjectile;
        private bool RunOnce = true;
        public override string Texture => "OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/Fragment";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Fragment");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 1200;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;

            drawOffsetX = -5;
            //drawOriginOffsetY = -7;
            drawOriginOffsetY = 2;
        }

        public override void AI()
        {
            if (RunOnce)
            {
                ParentProjectile = Main.projectile[(int)projectile.ai[0]];

                float radius = 15;
                int numLocations = 10;
                for (int i = 0; i < numLocations; i++)
                {
                    Vector2 position = projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(position, 2, 2, DustID.Enchanted_Gold, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                    Main.dust[dust].noGravity = true;
                }

                projectile.ai[0] = 0;
                RunOnce = false;
            }

            projectile.rotation = projectile.DirectionTo(ParentProjectile.Center).ToRotation() + (MathHelper.PiOver2 * 3);
            projectile.Center = ParentProjectile.Center + new Vector2(0, 250).RotatedBy(MathHelper.ToRadians(projectile.ai[1] + (projectile.ai[0] += 0.5f)));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/Fragment_Trail");
            Color color = Color.Yellow;

            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTime * 2) * 0.1f);
            float scale = projectile.scale * 2 * mult;

            spriteBatch.Draw(texture, projectile.Center + new Vector2(0, 10) - Main.screenPosition, drawRectangle, color, projectile.rotation, new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2), scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(SpriteSortMode.Immediate);

            Effect effect = OvermorrowModFile.Instance.Whiteout;
            float progress = Utils.Clamp(projectile.localAI[0]++, 0, 15f) / 15f;
            effect.Parameters["WhiteoutColor"].SetValue(Color.Yellow.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(1 - progress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            texture = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(texture, projectile.Center + new Vector2(0, 10) - Main.screenPosition, null, Color.White, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, 1, SpriteEffects.None, 0f);

            spriteBatch.Reload(SpriteSortMode.Deferred);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            float radius = 15;
            int numLocations = 10;
            for (int i = 0; i < numLocations; i++)
            {
                Vector2 position = projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                int dust = Dust.NewDust(position, 2, 2, DustID.Enchanted_Gold, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
