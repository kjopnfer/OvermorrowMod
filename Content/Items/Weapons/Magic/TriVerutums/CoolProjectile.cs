using System;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;

namespace OvermorrowMod.Content.Items.Weapons.Magic.TriVerutums
{
    public class CoolProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        bool StartInTile = false;
        public override void SetDefaults()
        {
            Projectile.width = 32; //16 orig: extra 2 blank pixels on right
            Projectile.height = 86;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 500;
        }

        float Scale = -1f;
        float ScaleIncrease = 0.005f;
        static int StaticAI = 0;
        bool IsFirstProjectile()
        {
            for (int i = 0; Main.projectile.Length > i; i++)
            {
                if (Main.projectile[i].type == Projectile.type && Main.projectile[i].active)
                {
                    if (Main.projectile[i].whoAmI == Projectile.whoAmI)
                    {
                        return true;
                    }
                    break;
                }
            }
            return false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0 && !TriVerutums.ReadyCoolProjectiles.Contains(Projectile.whoAmI))
            {
                TriVerutums.ReadyCoolProjectiles.Add(Projectile.whoAmI);
                StaticAI = 0;
            }

            if (Projectile.ai[1] == 1)
            {
                Projectile.ai[1] = 0;
                StaticAI = 0;
            }

            Projectile.timeLeft = 500;
            if (Projectile.ai[0] % 24 == 0)
                ScaleIncrease = -ScaleIncrease;
            Scale += ScaleIncrease;
            Projectile.ai[0]++;

            if (StaticAI >= 100 && !TriVerutums.ReadyCoolProjectiles.Contains(Projectile.whoAmI))
                TriVerutums.ReadyCoolProjectiles.Add(Projectile.whoAmI);
            if (TriVerutums.ReadyCoolProjectiles.Count != TriVerutums.MaxCoolThings && IsFirstProjectile())
                StaticAI++;

        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 Center = Projectile.Center + new Vector2(0, -Projectile.height / 4.5f).RotatedBy(Projectile.rotation);
            Vector2 Size = new Vector2(12, 12);
            hitbox = new Rectangle((int)Center.X - (int)Size.X, (int)Center.Y - (int)Size.Y, (int)Size.X * 2, (int)Size.Y * 2);
            base.ModifyDamageHitbox(ref hitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);
            Vector2 HalfWidthHeight = new Vector2(Projectile.width / 2, Projectile.height / 2);
            if (!TriVerutums.ReadyCoolProjectiles.Contains(Projectile.whoAmI))
            {
                for (int i = 0; Projectile.oldPos.Length > i; i++)
                {
                    Vector2 pos = Projectile.oldPos[i] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + HalfWidthHeight;
                    int ColorValue = ((255 / Projectile.oldPos.Length) * (Projectile.oldPos.Length - i));
                    Color color = new Color(ColorValue, ColorValue, ColorValue, ColorValue) /** 0.5f*/;
                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>(AssetDirectory.Magic + "TriVerutums/CoolProjectileTrail").Value, pos, new Rectangle(0, (Projectile.height + 2) * Projectile.frame, Projectile.width, Projectile.height), color, Projectile.rotation, HalfWidthHeight, (Projectile.scale / Projectile.oldPos.Length) * (Projectile.oldPos.Length - i), SpriteEffects.FlipVertically | (Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally), 0f);
                }
            }
            else
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>(AssetDirectory.Magic + "TriVerutums/CoolProjectileTrail").Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, (Projectile.height + 2) * Projectile.frame, Projectile.width, Projectile.height), Color.White, Projectile.rotation, HalfWidthHeight, Scale, SpriteEffects.FlipVertically | (Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally), 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.Transform);
            
            return true;
        }

        public override void Kill(int timeLeft)
        {
            if (!TriVerutums.ReadyCoolProjectiles.Contains(Projectile.whoAmI))
                TriVerutums.ReadyCoolProjectiles.Add(Projectile.whoAmI);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!TriVerutums.ReadyCoolProjectiles.Contains(Projectile.whoAmI))
                TriVerutums.ReadyCoolProjectiles.Add(Projectile.whoAmI);

            return false;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (!TriVerutums.ReadyCoolProjectiles.Contains(Projectile.whoAmI))
                TriVerutums.ReadyCoolProjectiles.Add(Projectile.whoAmI);
        }
    }
}