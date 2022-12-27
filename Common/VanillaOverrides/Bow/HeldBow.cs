using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Overmorrow.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Content.UI;
using OvermorrowMod.Content.Projectiles.Misc;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace OvermorrowMod.Common.VanillaOverrides.Bow
{
    public abstract class HeldBow : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;


        /// <summary>
        /// The positions to endpoints of the string for top and bottom, respectively.
        /// </summary>
        public virtual (Vector2, Vector2) StringPositions => (new Vector2(3, 9), new Vector2(3, 50));

        /// <summary>
        /// The offset for where the bow should be held. Defaults to <c>(13, 0)</c>.
        /// </summary>
        public virtual Vector2 PositionOffset => new Vector2(13, 0);

        /// <summary>
        /// The color of the bow's string.
        /// </summary>
        public virtual Color StringColor => Color.White;

        /// <summary>
        /// Determines whether the string of the bow ignores light.
        /// </summary>
        public virtual bool StringGlow => false;

        public virtual void SafeSetDefaults() { }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1;

            SafeSetDefaults();
        }

        public Player player => Main.player[Projectile.owner];
        public ref float drawCounter => ref Projectile.ai[0];
        public override void AI()
        {
            if (Main.myPlayer != player.whoAmI) return;

            player.heldProj = Projectile.whoAmI;

            float bowRotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation();
            Projectile.rotation = bowRotation;
            Projectile.spriteDirection = bowRotation > MathHelper.PiOver2 || bowRotation < -MathHelper.PiOver2 ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            Vector2 positionOffset = PositionOffset.RotatedBy(bowRotation);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + positionOffset;

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            Player.CompositeArmStretchAmount stretchAmount = drawCounter < 20 ? Player.CompositeArmStretchAmount.Full : Player.CompositeArmStretchAmount.Quarter;
            if (drawCounter > 40) stretchAmount = Player.CompositeArmStretchAmount.None;

            player.SetCompositeArmFront(true, stretchAmount, Projectile.rotation - MathHelper.PiOver2);

            if (player.controlUseItem)
            {
                drawCounter++;
                Projectile.timeLeft = 60;
            }
            else
            {
                if (drawCounter > 0) drawCounter -= 2;
                if (drawCounter < 0) drawCounter = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 1);

            Texture2D stringTexture = TextureAssets.FishingLine.Value;

            Vector2 topPosition = Projectile.Center + new Vector2(-5, -20).RotatedBy(Projectile.rotation);
            Vector2 bottomPosition = Projectile.Center + new Vector2(-5, 20).RotatedBy(Projectile.rotation);
            Utils.DrawLine(Main.spriteBatch, topPosition, bottomPosition, StringColor, StringColor, 1.25f);

            /*var armPosition = player.Center;
            var remainingVector = StringPositions.Item1 - player.Center;

            float rotation = remainingVector.ToRotation();
            float SIZE = 2;
            while (true)
            {
                float length = remainingVector.Length();
                if (length < 2 || float.IsNaN(length)) break;

                armPosition += remainingVector * SIZE / length;
                remainingVector = player.MountedCenter - StringPositions.Item1;

                // Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
                Main.spriteBatch.Draw(stringTexture, armPosition - Main.screenPosition, null, StringColor, rotation, stringTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }*/

            //Main.spriteBatch.Draw(TextureAssets.FishingLine.Value, null, StringColor);
            /*float distance = Vector2.Distance(StringPositions.Item1, StringPositions.Item2);
            float iterations = distance / 2f;
            for (int i = 0; i < iterations; i++)
            {
                float progress = i / iterations;
                Main.EntitySpriteDraw(stringTexture, position - Main.screenPosition, null, StringColor, 0f, stringTexture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }*/

            return false;
        }
    }
}