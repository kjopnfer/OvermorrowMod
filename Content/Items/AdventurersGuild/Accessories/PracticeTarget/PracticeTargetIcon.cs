using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.AdventurersGuild.Accessories
{
    public class PracticeTargetIcon : ModProjectile
    {
        public override string Texture => AssetDirectory.GuildItems + "PracticeTarget";
        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }

        public ref float AICounter => ref Projectile.ai[0];
        public ref float FailCount => ref Projectile.ai[1]; // Determines whether or not the default behavior takes place and number of fail icons to spawn 
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (AICounter++ == 0 && FailCount != 0) Projectile.timeLeft = (int)(12 * FailCount);

            if (FailCount == 0)
            {
                Projectile.Center = Vector2.Lerp(player.Center + new Vector2(-4, -32), player.Center + new Vector2(-4, -32) - Vector2.UnitY * 32, (float)(Math.Sin(AICounter / 15f)) / 2 + 0.5f);
            }
            else
            {
                Projectile.Center = player.Center + new Vector2(-4, -32);


                if (AICounter % 12 == 0)
                {
                    float randomVelocity = Main.rand.Next(3, 5);
                    float randomAngle = Main.rand.Next(32, 38) * 5;
                    Vector2 velocity = Vector2.UnitY.RotatedBy(MathHelper.ToRadians(randomAngle)) * randomVelocity;
                    Projectile.NewProjectile(null, Projectile.Center, velocity, ModContent.ProjectileType<PracticeTargetIconFail>(), 0, 0f, player.whoAmI);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (FailCount != 0 && FailCount != -1) return false;

            //Texture2D texture = TextureAssets.Item[ModContent.ItemType<PracticeTarget>()].Value;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 scale = new Vector2(MathHelper.Lerp(0.5f, 1f, Utils.Clamp(AICounter, 0, 10) / 10f), 1f);
            float alpha = MathHelper.Lerp(0, 0.75f, (float)(Math.Sin(AICounter / 15f)) / 2 + 0.5f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, texture.Size() / 2f, scale * 0.9f, SpriteEffects.None, 1);

            return false;
        }
    }

    public class PracticeTargetIconFail : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }

        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            AICounter++;

            Projectile.velocity.Y += 0.15f;
            Projectile.velocity.X *= 0.99f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[ModContent.ProjectileType<PracticeTargetIcon>()].Value;

            Vector2 scale = new Vector2(MathHelper.Lerp(0.5f, 1f, Utils.Clamp(AICounter, 0, 10) / 10f), 1f);
            float alpha = MathHelper.Lerp(0, 0.55f, (float)(Math.Sin(AICounter / 15f)) / 2 + 0.5f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, texture.Size() / 2f, scale * 0.7f, SpriteEffects.None, 1);

            return base.PreDraw(ref lightColor);
        }
    }
}