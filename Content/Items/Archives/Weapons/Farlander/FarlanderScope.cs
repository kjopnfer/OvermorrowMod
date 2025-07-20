using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Globals;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class FarlanderScope : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;
        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 34;
            Projectile.penetrate = -1;
        }

        float maxChargeTime = 120;
        public ref float AICounter => ref Main.player[Projectile.owner].GetModPlayer<GunPlayer>().FarlanderCharge;
        public ref float DeathFlag => ref Projectile.ai[0];

        public override void AI()
        {
            if (DeathFlag == 1) return;

            if (Main.mouseRight && Main.player[Projectile.owner].active) Projectile.timeLeft = 2;

            float playerChargeBonus = 0.25f * Main.player[Projectile.owner].GetModPlayer<GunPlayer>().FarlanderSpeedBoost;
            float countRate = 1;

            // Apply rate penalty if the player is moving the mouse around very quickly
            float mouseSpeed = Math.Abs(Main.lastMouseX - Main.mouseX);
            if (AICounter < maxChargeTime) countRate = MathHelper.Lerp(1f, 0.05f, Utils.Clamp(mouseSpeed, 0, 35) / 35f);

            Projectile.Center = Main.MouseWorld;

            if (AICounter < maxChargeTime + 30) AICounter += countRate + playerChargeBonus;
            else AICounter++;

            if (Main.mouseLeft)
            {
                // Initially, the projectile would sometimes die before the gun was fired, resetting the counter
                // Thus, the gun would have a max inaccuracy shot even though it was fully charged
                // This fixes the issue by giving a brief window to always allow the gun to fire before resetting the counter
                DeathFlag = 1;
                Projectile.timeLeft = 10;
            }
        }

        public override void Kill(int timeLeft)
        {
            AICounter = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            float flashCounter = Utils.Clamp(AICounter - maxChargeTime, 0, 999999);
            float flashProgress = Utils.Clamp((float)Math.Sin(flashCounter / 8f), 0, 1);
            if (DeathFlag == 1) flashProgress = 0;

            Effect effect = OvermorrowModFile.Instance.ColorFill.Value;
            effect.Parameters["ColorFillColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["ColorFillProgress"].SetValue(flashProgress);
            effect.CurrentTechnique.Passes["ColorFill"].Apply();

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 yOffset = Vector2.UnitY * -12;

            if (AICounter < maxChargeTime)
            {
                float scale = MathHelper.Lerp(2f, 0.5f, Utils.Clamp(AICounter, 0, maxChargeTime) / maxChargeTime);
                Main.spriteBatch.Draw(texture, Projectile.Center + yOffset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 1);

                Texture2D outerScope = ModContent.Request<Texture2D>(AssetDirectory.ArchiveItems + "FarlanderOuterScope").Value;
                Main.spriteBatch.Draw(outerScope, Projectile.Center + yOffset - Main.screenPosition, null, Color.White * 0.75f, 0f, outerScope.Size() / 2f, scale, SpriteEffects.None, 1);
            }
            else
                Main.spriteBatch.Draw(texture, Projectile.Center + yOffset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 1);


            Main.spriteBatch.Reload(SpriteSortMode.Deferred);

            return false;
        }
    }
}