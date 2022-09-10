using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Mercenary.Paladin
{
    public abstract class PaladinProjectile : ModProjectile
    {
        public Paladin owner;

        public override void SetDefaults()
        {
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public virtual void DoScreenShake()
        {
            owner.stunDuration = 30;
            owner.acceleration = 0;
            owner.NPC.velocity.X *= -2f;
            ScreenShake.ScreenShakeEvent(Projectile.Center, 15, 2, 100);
        }

        public void CheckCollision()
        {
            // If the paladin collides with a tile while spinning, send the paladin backwards and briefly stun the paladin
            Point checkTile = new Point(MathFunctions.AGF.Round(Projectile.Center.X / 16), MathFunctions.AGF.Round(Projectile.Center.Y / 16));
            Tile tile = Main.tile[checkTile.X, checkTile.Y];
            if (!WorldGen.TileEmpty(checkTile.X, checkTile.Y) && WorldGen.SolidOrSlopedTile(tile) && Main.tileSolid[tile.TileType])
            {
                DoScreenShake();
                Projectile.Kill();
            }
        }
    }

    public class PaladinHammer : PaladinProjectile
    {
        public float progress = 1;
        public int direction;
        bool initialize = false;
        float rotation;

        private bool isFarAway = false;
        private bool damageBoost = false;

        public Vector2 targetPosition;

        public float startValue;
        public float endValue;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mighty Hammer");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (!initialize)
            {
                progress = startValue;
                initialize = true;
            }

            if (owner != null && owner.NPC.active)
            {
                if (Projectile.ai[0]++ >= 60 && Projectile.Hitbox.Intersects(owner.NPC.Hitbox)) Projectile.Kill();

                Main.NewText("throwstyle: " + owner.throwStyle + " progress: " + progress + " id: " + Projectile.whoAmI);
                // Rotate the hammer forth, back, or forth but faster
                switch (owner.throwStyle)
                {
                    case 1:
                        rotation += 0.125f * direction;
                        break;
                    case 2:
                        rotation += 0.125f * direction;
                        break;
                    case 3:
                        rotation += 0.25f * direction;
                        break;
                }

                float distance = Vector2.Distance(owner.NPC.Center, targetPosition);
                if (progress < endValue) // 1st cycle ends at 3f, second cycle ends at 5f
                {
                    if (distance < 450)
                    {
                        // Move the hammer in a unique functioned movement; the cycle is based on the starting x
                        // See: https://www.desmos.com/calculator/8zryrzykns
                        Vector2 center = targetPosition - owner.NPC.Center;
                        float startRotation = (float)Math.Atan(center.Y / center.X);
                        float[] lerpRotation = new float[2] { -direction * (float)(Math.PI / 2) + startRotation, direction * (float)(Math.PI / 1) + startRotation };
                        progress += 2f / 75;
                        float lerpMult = (float)(Math.Cos(progress + Math.PI) / 2) + 0.5f;
                        float offsetDistance = (float)(Math.Cos(progress * Math.PI) / 2) + 0.5f;
                        offsetDistance *= (float)Math.Sqrt(distance / 16);
                        float offsetRotation = lerpRotation[0] + (lerpRotation[1] - lerpRotation[0]) * lerpMult;
                        Vector2 pos = new Vector2((float)(offsetDistance * Math.Sin(offsetRotation)), (float)((offsetDistance * -0.5f) * Math.Cos(offsetRotation)));
                        Projectile.Center = (pos * 100) + owner.NPC.Center;
                    }
                    else
                    {
                        isFarAway = true;
                        progress = 5f;
                    }
                }
                else if (progress >= 5f && progress <= 5f + (Math.PI))
                {
                    // 3rd hammer attack; lerp the position of the hammer towards the target; this is performed as the only attack if the target is very far away (slower, but more powerful)
                    progress += (float)Math.PI / (isFarAway ? 180 : 90);
                    if (!damageBoost)
                    {
                        Projectile.damage = MathFunctions.AGF.Round(Projectile.damage * 1.5f);
                        damageBoost = true;
                    }

                    float sin = progress - 5f;
                    float sine2 = (float)Math.Pow(Math.Sin(sin), 2);
                    Projectile.Center = new Vector2(MathHelper.Lerp(owner.NPC.Center.X, targetPosition.X, sine2), MathHelper.Lerp(owner.NPC.Center.Y, targetPosition.Y, sine2));
                }
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (owner != null && owner.NPC.active) ScreenShake.ScreenShakeEvent(Projectile.Center, 8, 4, 100);

            float randomScale = Main.rand.NextFloat(0.5f, 0.75f);
            float randomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            float particleTime = 120;

            Particle.CreateParticle(Particle.ParticleType<LightBurst>(), Projectile.Center, Vector2.Zero, Color.LightYellow, 1, randomScale, randomRotation, particleTime);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
        }

        public override void Kill(int timeLeft)
        {
            Main.NewText("die");
            base.Kill(timeLeft);
        }
    }

    public class PaladinHammerSpin : PaladinProjectile
    {
        public float sine = (float)Math.PI / 2;
        public override bool PreDraw(ref Color lightColor) => false;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) { Projectile.timeLeft = 180; }
        public override string Texture => AssetDirectory.NPC + "Mercenary/Paladin/PaladinHammer";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mighty Hammer");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 116;
            Projectile.height = 50;
            Projectile.timeLeft = 2;
            AIType = ProjectileID.Bullet;
        }

        public override void DoScreenShake()
        {
            owner.stunDuration = 30;
            owner.acceleration = 0;
            owner.NPC.velocity *= -1f;
            ScreenShake.ScreenShakeEvent(Projectile.Center, 15, 5, 100);
        }

        public override void AI()
        {
            if (owner != null && owner.NPC.active && !owner.DangerThreshold())
            {
                // Make the paladin more resistant, and lerp the position of this projectile back and forth
                CheckCollision();
                owner.NPC.defense = 90;
                owner.NPC.knockBackResist = 0;
                sine += (float)Math.PI / 60 * (owner.NPC.direction == -1 ? -1 : 1);
                Projectile.velocity = Vector2.Zero;
                //float pos = MathHelper.Lerp(-35, 35, (float)Math.Pow(Math.Sin(sine), 2));
                //Projectile.Center = owner.NPC.Center + new Vector2(pos, 0);
                Projectile.Center = owner.NPC.Center;
            }
            else
            {
                owner.spinCounter = 0;
                Projectile.Kill();
            }
        }
    }

    public class PaladinHammerHit : PaladinProjectile
    {
        public float sine;
        float initialVelocity;
        bool initialize;

        public override string Texture => AssetDirectory.NPC + "Mercenary/Paladin/PaladinHammer";
        public override bool PreDraw(ref Color lightColor) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shockwave");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.timeLeft = 180;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            // Dust offset
            Vector2 offset = new Vector2(0, 25);

            if (!initialize)
            {
                //Particle.CreateParticle(Particle.ParticleType<Pulse2>(), Projectile.Center + offset, Vector2.Zero, Color.Orange);

                /*for (int a = 0; a < 20; a++) // Create an oval dust shape
                {
                    float pi = (float)Math.PI / 10;
                    Dust dust = Dust.NewDustPerfect(new Vector2(20f * (float)Math.Cos(a * pi) + Projectile.Center.X, 5f * (float)Math.Sin(a * pi) + Projectile.Center.Y) + offset, 6);
                    dust.noGravity = true;
                    dust.velocity = Vector2.Zero;
                }*/

                initialize = true;
                initialVelocity = Projectile.velocity.X;
            }

            if (Projectile.ai[0]++ % 4 == 0)
            {
                //Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LightWave>(), Projectile.damage, 2f, Main.myPlayer);
            }

            // Create sine shaped dust formations as the projectile travels
            #region shockwave dust
            if (Projectile.timeLeft > 90)
            {
                for (int i = 0; i < 6; i++)
                {
                    float randomOffset = MathHelper.ToRadians(Main.rand.NextFloat(-10, 10));
                    float rotation = Projectile.velocity.X < 0 ? randomOffset + MathHelper.PiOver4 : -(randomOffset + MathHelper.PiOver4);

                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.Torch);
                    dust.noGravity = true;
                    dust.velocity = Projectile.velocity.RotatedBy(rotation);
                }
                /*float sin2 = (float)Math.Pow(Math.Abs(1.06f * Math.Sin(5f * sine)), 40) - 0.1f;
                if (sin2 > 0)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset + new Vector2(0, -sin2 * 2.5f), DustID.Torch);
                    dust.noGravity = true;
                    dust.velocity = Vector2.Zero;
                }*/
            }
            #endregion

            // Lerp the velocity of the projectile to 0
            Projectile.velocity = new Vector2(MathHelper.Lerp(initialVelocity, 0, 1 - (Projectile.timeLeft / 180f)), 0);

            // Makes the shockwave stick to the ground, if checks if there is a tile below the center and a tile at the center
            // If there is a tile below the projectile AND the projectile isn't in a tile, do nothing.
            // Otherwise, if there isn't a tile below the projectile, move downwards.
            // And if the projectile is inside of a tile, move upwards.
            Vector2 positionChange = Projectile.Bottom / 16;

            Tile tile = Framing.GetTileSafely((int)Projectile.Bottom.X / 16, (int)Projectile.Bottom.Y / 16);
            while ((tile.HasTile && Main.tileSolid[tile.TileType]))
            {
                // We are in a tile and the tile is solid (ie not a table or tree)
                if (tile.HasTile && Main.tileSolid[tile.TileType]) positionChange.Y -= 1;
                tile = Framing.GetTileSafely((int)positionChange.X, (int)positionChange.Y);
            }

            tile = Framing.GetTileSafely((int)positionChange.X, (int)positionChange.Y);
            while (!tile.HasTile || !Main.tileSolid[tile.TileType])
            {
                // The tile below doesnt exist or the tile is not solid
                if (!tile.HasTile || !Main.tileSolid[tile.TileType]) positionChange.Y += 1;
                tile = Framing.GetTileSafely((int)positionChange.X, (int)positionChange.Y);
            }

            Projectile.Center = new Vector2(0, -16 + -Projectile.height / 2f) + positionChange * 16;
        }
    }

    /// <summary>
    /// This projectile exists so that the texture can draw behind tiles which isn't currently supported in the particles.
    /// <br>Also handles the actual damage effect of the slam for the first half second.</br>
    /// </summary>
    public class LightExplosion : ModProjectile
    {
        float maxTime = 240;
        float maxSize = 0.5f;
        public override bool? CanDamage() => Projectile.ai[0] < 30;
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 224;
            Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.timeLeft = (int)maxTime;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.extraUpdates = 2;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1; // 1 hit per npc max
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            float progress = ModUtils.EaseOutQuad(1 - (Projectile.timeLeft / maxTime));
            Projectile.scale = MathHelper.SmoothStep(0, maxSize, progress);
            //Projectile.alpha = (int)MathHelper.SmoothStep(255, 0, (float)(1 - (Projectile.timeLeft / maxTime)));

            Projectile.rotation += 0.009f;
            Projectile.ai[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "LightBurst").Value;
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;

            Color color = Color.Lerp(Color.Orange, Color.Transparent, ModUtils.EaseOutQuad(Utils.Clamp(Projectile.ai[0] - 20, 0, 220) / 220f));
            Main.spriteBatch.Draw(texture, Projectile.Center + new Vector2(0, 32) - Main.screenPosition, null, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }
    }

    public class LightWave : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.extraUpdates = 1;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            Projectile.ai[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "LightSpike").Value;
            float heightLerp = MathHelper.Lerp(0, 0.5f, Projectile.ai[0] / 60f);
            //float widthLerp = MathHelper.Lerp(0, 0.25f, Projectile.ai[0] / 60f);

            //float widthLerp = MathHelper.Lerp(0, 0.25f, (float)(Math.Sin(Projectile.ai[0] / 30f)) / 2 + 0.5f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Orange, 0f, texture.Size() / 2f, new Vector2(heightLerp, 0.25f), SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Orange, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return true;
        }
    }

    public class LightBeam : ModProjectile
    {
        protected bool RunOnce = true;
        protected int RotationDirection = 1;

        protected const float MAX_TIME = 240;
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unity");
        }
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 5000;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)MAX_TIME;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            float progress = Utils.GetLerpValue(0, MAX_TIME, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.height, Projectile.width, ref a);
        }

        public Color BeamColor = Color.Yellow;

        public Texture2D TrailTexture1 = ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail0").Value;
        public Texture2D TrailTexture2 = ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail7").Value;
        public Texture2D TrailTexture3 = ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail1").Value;
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation += 0.3f;
            Main.spriteBatch.Reload(BlendState.Additive);
 
            // make the beam slightly change scale with time
            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly/* * 2*/) * 0.1f);
            // base scale for the flash so it actually connects with beam
            float scale = Projectile.scale * 3 * mult;
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.Yellow * 0.5f, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.5f, SpriteEffects.None, 0);

            //float scale = projectile.scale * 2 * mult;
            BeamPacket packet = new BeamPacket();
            packet.Pass = "Texture";
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity;
            float width = Projectile.width * Projectile.scale;
            // offset so i can make the triangles
            Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;

            BeamColor = new Color(240, 231, 113);
            BeamPacket.SetTexture(0, TrailTexture1);
            float off = -Main.GlobalTimeWrappedHourly % 1;
            // draw the flame part of the beam
            packet.Add(start + offset * 3 * mult, BeamColor, new Vector2(0 + off, 0));
            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));

            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end - offset * 3 * mult, BeamColor, new Vector2(1 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));
            packet.Send();

            BeamColor = new Color(240, 231, 113);
            BeamPacket packet2 = new BeamPacket();
            packet2.Pass = "Texture";
            BeamPacket.SetTexture(0, TrailTexture2);
            packet2.Add(start + offset * 2 * mult, BeamColor, new Vector2(0 + off, 0));
            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));

            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end - offset * 2 * mult, BeamColor, new Vector2(1 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));
            packet2.Send();

            BeamColor = Color.White;
            BeamPacket packet3 = new BeamPacket();
            packet3.Pass = "Texture";
            BeamPacket.SetTexture(0, TrailTexture3);
            float alpha = 1f;
            packet3.Add(start + offset * mult, BeamColor * alpha, new Vector2(0 + -off, 0));
            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));

            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end - offset * mult, BeamColor * alpha, new Vector2(1 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));
            packet3.Send();

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Sunlight").Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, BeamColor, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.25f, SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Sunlight").Value;
            for (int i = 0; i < 5; i++)
                Main.EntitySpriteDraw(texture, end - Main.screenPosition, null, BeamColor, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale * 0.15f, SpriteEffects.None, 0);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            DelegateMethods.v3_1 = new Color(240, 231, 113).ToVector3();
            Terraria.Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity, Projectile.width * Projectile.scale, new Utils.TileActionAttempt(DelegateMethods.CastLight));
        }
    }

    public class PaladinRamHitbox : PaladinProjectile
    {
        public override bool? CanCutTiles() => false;
        public override bool PreDraw(ref Color lightColor) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rookie Paladin");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 3000;
        }

        public override void AI()
        {
            // Set the position directly in front of the paladin
            if (owner != null && owner.NPC.active && owner.acceleration >= 0.4f)
                Projectile.Center = owner.NPC.Center + new Vector2(owner.NPC.direction == -1 ? -50 : 50, 4);
            else
                Projectile.Kill();

            CheckCollision();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //ram the non-boss npc and send them flying
            if (target.knockBackResist < 1 && !target.boss)
                target.velocity += new Vector2((owner.NPC.direction == -1 ? -7.5f : 7.5f) / target.knockBackResist, -8f / target.knockBackResist);
            //if the paladin is still alive and the above does not occur, shake the screen
            else if (owner != null && owner.NPC.active && owner.acceleration >= 0.4f)
                DoScreenShake();
        }
    }
}