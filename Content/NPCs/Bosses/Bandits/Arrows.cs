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
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Bandits
{
    public class FlameArrow : ModProjectile, ITrailEntity
    {


        public Color TrailColor(float progress) => ModUtils.Lerp3(new Color(216, 44, 4), new Color(254, 121, 2), new Color(253, 221, 3), progress) * progress;
        public float TrailSize(float progress) => 20;
        public Type TrailType() => typeof(TorchTrail);

        //public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(BuffID.OnFire, 120);
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCsAndTiles.Add(index);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 5;
            Projectile.hide = true;
        }

        private ref float AICounter => ref Projectile.ai[0];
        private bool CollideTile = false;

        public override void AI()
        {
            if (Main.rand.NextBool(3) && !CollideTile)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X + Projectile.velocity.X, Projectile.position.Y + Projectile.velocity.Y), Projectile.width, Projectile.height, DustID.RainbowMk2, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(253, 254, 255), Main.rand.NextFloat(0.5f, 0.7f));
                Main.dust[dust].noGravity = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.extraUpdates != 0)
            {
                Main.spriteBatch.Reload(BlendState.Additive);

                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01").Value;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(254, 121, 2) * 0.65f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * 0.7f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(216, 44, 4) * 0.3f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * 1.2f, SpriteEffects.None, 0f);

                texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;

                var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
                var trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
                var fadeMult = 1f / trailLength;
                for (int i = 1; i < trailLength; i++)
                {
                    Main.spriteBatch.Draw(texture, Projectile.oldPos[i] - Main.screenPosition + offset, null, new Color(254, 121, 2) * (1f - fadeMult * i) * 0.5f, Projectile.oldRot[i], texture.Size() / 2f, Projectile.scale * (trailLength - i) / trailLength * 1.5f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texture, Projectile.oldPos[i] - Main.screenPosition + offset, null, new Color(254, 121, 2) * (1f - fadeMult * i) * 0.2f, Projectile.oldRot[i], texture.Size() / 2f, Projectile.scale * (trailLength - i) / trailLength * 3f, SpriteEffects.None, 0);
                }

                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }

            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            float progress = 1;
            if (CollideTile) progress = (Utils.Clamp(AICounter++, 0, 120) / 120f) - 1f;

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(progress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            Texture2D arrow = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(arrow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, arrow.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Reload(SpriteSortMode.Deferred);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < Main.rand.Next(16, 24); i++)
            {
                float randomScale = Main.rand.NextFloat(0.5f, 0.85f);
                float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(80), MathHelper.ToRadians(80));
                Vector2 RandomVelocity = -Vector2.Normalize(Projectile.velocity).RotatedBy(randomAngle) * Main.rand.Next(8, 13);

                Color color = Color.Orange;

                Particle.CreateParticle(Particle.ParticleType<LightSpark>(), Projectile.Center, RandomVelocity, color, 1, randomScale);
            }

            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 0;
            Projectile.velocity = Projectile.velocity / 10000;

            CollideTile = true;

            return false;
        }
    }

    public class SplitArrow : ModProjectile, ITrailEntity
    {
        public override string Texture => AssetDirectory.Boss + "Bandits/FlameArrow";
        public Color TrailColor(float progress) => ModUtils.Lerp3(new Color(166, 3, 253), new Color(253, 3, 228), new Color(253, 3, 166), progress) * progress;
        public float TrailSize(float progress) => 20;
        public Type TrailType() => typeof(TorchTrail);
        public override bool? CanDamage() => !CollideTile;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCsAndTiles.Add(index);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 5;
            Projectile.hide = true;
        }

        private ref float FadeCounter => ref Projectile.localAI[0];
        private ref float SplitTime => ref Projectile.localAI[1];


        private ref float AICounter => ref Projectile.ai[0];
        private ref float ArrowGravity => ref Projectile.ai[1];

        private bool CollideTile = false;

        public override void OnSpawn(IEntitySource source)
        {
            SplitTime = Main.rand.Next(6, 15);

            // Sets the gravity for the initial arrow only
            if (ArrowGravity == 0)
            {
                ArrowGravity = Main.rand.NextFloat(0.18f, 0.28f);
            }
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3) && !CollideTile)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X + Projectile.velocity.X, Projectile.position.Y + Projectile.velocity.Y), Projectile.width, Projectile.height, DustID.RainbowMk2, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(253, 254, 255), Main.rand.NextFloat(0.5f, 0.7f));
                Main.dust[dust].noGravity = true;
            }

            if (AICounter == SplitTime)
            {
                float randomGravity = Main.rand.NextFloat(0.21f, 0.26f);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, Projectile.type, Projectile.damage, 0f, Main.myPlayer, -1, randomGravity);

                randomGravity = Main.rand.NextFloat(0.5f, 0.56f);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, Projectile.type, Projectile.damage, 0f, Main.myPlayer, -1, randomGravity);
            }

            if (!CollideTile)
            {
                if (AICounter == -1) // The child arrows will have passed in -1 to make sure they don't split AND use the provided gravity
                {
                    Projectile.velocity.Y += ArrowGravity;
                }
                else
                {
                    Projectile.velocity.Y += AICounter++ < 60 ? (ArrowGravity / 2f) : ArrowGravity;
                }

                if (Projectile.velocity.Y >= 5) Projectile.velocity.Y = 5;

                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.extraUpdates != 0)
            {
                Main.spriteBatch.Reload(BlendState.Additive);

                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01").Value;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(179, 124, 204) * 0.65f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * 0.7f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(153, 78, 173) * 0.3f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * 1.2f, SpriteEffects.None, 0f);

                texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;

                var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
                var trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
                var fadeMult = 1f / trailLength;
                for (int i = 1; i < trailLength; i++)
                {
                    Main.spriteBatch.Draw(texture, Projectile.oldPos[i] - Main.screenPosition + offset, null, new Color(253, 3, 166) * (1f - fadeMult * i) * 0.5f, Projectile.oldRot[i], texture.Size() / 2f, Projectile.scale * (trailLength - i) / trailLength * 1.5f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(texture, Projectile.oldPos[i] - Main.screenPosition + offset, null, new Color(253, 3, 166) * (1f - fadeMult * i) * 0.2f, Projectile.oldRot[i], texture.Size() / 2f, Projectile.scale * (trailLength - i) / trailLength * 3f, SpriteEffects.None, 0);
                }

                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }

            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            float progress = 1;
            if (CollideTile)
            {
                progress = 1f - (Utils.Clamp(FadeCounter - 30, 0, 120) / 120f);
                if (!Main.gamePaused) FadeCounter++;
            }

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(progress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            Texture2D arrow = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(arrow, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, arrow.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Reload(SpriteSortMode.Deferred);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!CollideTile)
            {
                for (int i = 0; i < Main.rand.Next(16, 24); i++)
                {
                    float randomScale = Main.rand.NextFloat(0.5f, 0.85f);
                    float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(80), MathHelper.ToRadians(80));
                    Vector2 RandomVelocity = -Vector2.UnitY.RotatedBy(randomAngle) * Main.rand.Next(8, 13);

                    Color color = Color.Purple;

                    Particle.CreateParticle(Particle.ParticleType<LightSpark>(), Projectile.Center, RandomVelocity, color, 1, randomScale);
                }

                Projectile.timeLeft = 150;
                Projectile.extraUpdates = 0;
                Projectile.velocity = Projectile.velocity / 10000;

                CollideTile = true;
            }

            return false;
        }
    }

}