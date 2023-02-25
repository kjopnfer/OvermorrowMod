using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Interfaces;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.GraniteLauncher
{
    public class GraniteShard : ModProjectile, ITrailEntity, ITileOverlay
    {
        public Color TrailColor(float progress) => Color.Lerp(new Color(0, 137, 255), new Color(122, 232, 246), progress) * progress;
        public float TrailSize(float progress) => 24;
        public Type TrailType() => typeof(TorchTrail);

        public void DrawOverTiles(SpriteBatch spriteBatch)
        {
            if (CollideTile)
            {
                spriteBatch.Reload(BlendState.Additive);

                float offsetAmount = 15;
                Vector2 embedOffset = CollideTile ? Vector2.UnitX * offsetAmount : Vector2.Zero;

                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "shatter_temp").Value;

                float progress = 1 - Utils.Clamp(AICounter, 0, 90f) / 90f;

                Color color = Color.Cyan * progress;
                //color *= (float)Math.Cos(AICounter);
                spriteBatch.Draw(texture, Projectile.Center + embedOffset.RotatedBy(Projectile.rotation) - Main.screenPosition, null, color, randomCrackRotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

                spriteBatch.Reload(BlendState.AlphaBlend);
            }
        }

        public override bool? CanDamage() => !CollideTile;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.extraUpdates = 2;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        private float randomCrackRotation = MathHelper.ToRadians(Main.rand.Next(0, 18) * 20);
        float offsetAmount = 20;

        public bool CollideTile = false;
        public bool HasActivated = false;

        public ref float ShardID => ref Projectile.ai[0];
        public ref float AICounter => ref Projectile.ai[1];
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0, 0.7f, 0.7f));

            if (AICounter < 120 && CollideTile) AICounter++;

            Player player = Main.player[Projectile.owner];
            if (Projectile.Distance(player.Center) > 16 * 72)
            {
                GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
                gunPlayer.ShardList.Remove((int)ShardID);
                Projectile.Kill();
            }
            /*if (CollideTile)
            {
                Vector2 embedOffset = CollideTile ? Vector2.UnitX * offsetAmount : Vector2.Zero;

                Vector2 randomPosition = new Vector2(Main.rand.Next(-1, 1), Main.rand.Next(-1, 1)) * 12f;
                if (Main.rand.NextBool(20) && !Main.gamePaused)
                    Particle.CreateParticle(Particle.ParticleType<Electricity>(), Projectile.Center + randomPosition + embedOffset, Vector2.One.RotatedBy(Projectile.rotation) * 3f, Color.Cyan, 1, 1f);
            }*/

            if (!CollideTile)
            {
                if (Main.rand.NextBool(5))
                {
                    Vector2 positionOffset = new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 5)) * 4;
                    float randomScale = Main.rand.NextFloat(0.15f, 0.25f);
                    Vector2 RandomVelocity = -Vector2.Normalize(Projectile.velocity) * 7f;

                    Color color = Color.Cyan;
                    Particle.CreateParticle(Particle.ParticleType<LightSpark>(), Projectile.Center + positionOffset, RandomVelocity, color, 1, randomScale);
                }

                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public void ActivateNextChain(int projectileID)
        {
            Main.NewText(ShardID + " attempting to arm to " + projectileID);

            for (int i = 0; i < 18; i++)
            {
                float randomScale = Main.rand.NextFloat(0.25f, 0.35f);
                Vector2 RandomVelocity = -Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.ToRadians(20 * i)) * 6f;

                Color color = Color.Cyan;
                Particle.CreateParticle(Particle.ParticleType<LightSpark>(), Projectile.Center, RandomVelocity, color, 1, randomScale);
            }

            Projectile.timeLeft = 600;

            if (projectileID != -1)
                Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GraniteElectricity>(), Projectile.damage, 5f, Projectile.owner, Projectile.whoAmI, projectileID);

            /*foreach (Projectile projectile in Main.projectile)
            {
                if (!projectile.active) continue;

                if (projectile.ModProjectile is GraniteShard shard && !shard.HasActivated)
                {
                    if (shard.ShardID == (ShardID + 1))
                    {
                        Main.NewText(ShardID + " activating " + shard.ShardID);
                        Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GraniteElectricity>(), Projectile.damage, 5f, Projectile.owner, Projectile.whoAmI, projectile.whoAmI);
                    }
                }
            }*/

            HasActivated = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return !CollideTile;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 embedOffset = CollideTile ? Vector2.UnitX * offsetAmount : Vector2.Zero;

            if (Projectile.extraUpdates != 0)
            {
                Main.spriteBatch.Reload(BlendState.Additive);

                Texture2D trailTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01").Value;

                var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
                var trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
                var fadeMult = 1f / trailLength;
                for (int i = 1; i < trailLength; i++)
                {
                    Main.spriteBatch.Draw(trailTexture, Projectile.oldPos[i] - Main.screenPosition + offset, null, new Color(122, 232, 246) * (1f - fadeMult * i) * 0.6f, Projectile.oldRot[i] + MathHelper.PiOver2, trailTexture.Size() / 2f, Projectile.scale * (trailLength - i) / trailLength * 0.75f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(trailTexture, Projectile.oldPos[i] - Main.screenPosition + offset, null, new Color(0, 137, 255) * (1f - fadeMult * i) * 0.35f, Projectile.oldRot[i] + MathHelper.PiOver2, trailTexture.Size() / 2f, Projectile.scale * (trailLength - i) / trailLength * 1.5f, SpriteEffects.None, 0);
                }

                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }

            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            float progress = Utils.Clamp(AICounter, 0, 120f) / 120f;
            effect.Parameters["WhiteoutColor"].SetValue(new Color(122, 232, 246).ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(1 - progress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            if (CollideTile)
            {
                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
                Main.spriteBatch.Draw(texture, Projectile.Center + embedOffset.RotatedBy(Projectile.rotation) - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2f, 1, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Reload(SpriteSortMode.Deferred);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!CollideTile)
            {
                Vector2 embedOffset = CollideTile ? Vector2.UnitX * offsetAmount : Vector2.Zero;

                Projectile.extraUpdates = 0;
                Particle.CreateParticle(Particle.ParticleType<LightBurst>(), Projectile.Center + embedOffset.RotatedBy(Projectile.rotation), Vector2.Zero, Color.Cyan * 0.8f, 1, 0.75f, MathHelper.ToRadians(Main.rand.Next(0, 360)));

                for (int i = 0; i < Main.rand.Next(2, 4); i++)
                {
                    float randomScale = Main.rand.NextFloat(0.25f, 0.35f);

                    Color color = Color.Cyan;
                    Particle.CreateParticle(Particle.ParticleType<Pulse>(), Projectile.Center + embedOffset.RotatedBy(Projectile.rotation), Vector2.Zero, color, 1, randomScale);
                }

                for (int i = 0; i < Main.rand.Next(12, 18); i++)
                {
                    float randomScale = Main.rand.NextFloat(0.25f, 0.35f);
                    Vector2 RandomVelocity = -Vector2.Normalize(Projectile.velocity).RotatedByRandom(MathHelper.TwoPi) * 12f;

                    Color color = Color.Cyan;
                    Particle.CreateParticle(Particle.ParticleType<LightSpark>(), Projectile.Center + embedOffset.RotatedBy(Projectile.rotation), RandomVelocity, color, 1, randomScale);
                }

                CollideTile = true;
            }

            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact);

            for (int i = 0; i < 9; i++)
            {
                float randomScale = Main.rand.NextFloat(0.15f, 0.2f);
                Vector2 RandomVelocity = -Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.ToRadians(40 * i)) * 3f;

                Color color = Color.Cyan;
                Particle.CreateParticle(Particle.ParticleType<LightSpark>(), Projectile.Center, RandomVelocity, color, 1, randomScale);
            }

            for (int i = 0; i < 4; i++)
            {
                int textureNumber = i < 3 ? i : 0;
                int gore = Gore.NewGore(null, Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi), Mod.Find<ModGore>("GraniteShard_" + textureNumber).Type);
                Main.gore[gore].sticky = true;
            }
        }
    }
}