using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using OvermorrowMod.Common.Utilities;
using Terraria.ID;
using Terraria.Audio;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class FlowerSummon : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
        }

        int spriteVariant = 1;
        int hitboxWidth = 42;
        int hitboxHeight = 80;
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Grass with
            {
                MaxInstances = 1,
                PitchVariance = 0.9f,
                SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
            });
            spriteVariant = Main.rand.Next(0, 3);
        }

        public ref float SpawnRotation => ref Projectile.ai[0];
        public ref float AICounter => ref Projectile.ai[1];
        public override void AI()
        {
            Projectile.rotation = SpawnRotation;
            Projectile.velocity = Vector2.Zero;

            if (AICounter < 30) AICounter++;
            Projectile.Opacity = Math.Clamp(MathHelper.Lerp(0, 1f, Projectile.timeLeft / 60f), 0, 1f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, ModUtils.SecondsToTicks(5));
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Poisoned, ModUtils.SecondsToTicks(5));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox)) return true;

            float _ = float.NaN;
            Vector2 endPosition = Projectile.Bottom + new Vector2(0, -hitboxHeight).RotatedBy(Projectile.rotation);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPosition, hitboxWidth * Projectile.scale, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            float textureWidth = texture.Width / 3f;
            Rectangle drawRectangle = new Rectangle((int)textureWidth * spriteVariant, 0, (int)textureWidth, texture.Height);

            float drawHeight = MathHelper.Lerp(0, 1f, EasingUtils.EaseOutBack(AICounter / 30f));
            float drawWidth = MathHelper.Lerp(0.5f, 1f, AICounter / 30f);
            Vector2 scale = new Vector2(drawWidth, drawHeight);

            Vector2 drawOffset = new Vector2(0, 16).RotatedBy(Projectile.rotation);
            Main.spriteBatch.Draw(texture, Projectile.Bottom + drawOffset - Main.screenPosition, drawRectangle, lightColor * Projectile.Opacity, Projectile.rotation, new Vector2(drawRectangle.Width / 2f, texture.Height), scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}