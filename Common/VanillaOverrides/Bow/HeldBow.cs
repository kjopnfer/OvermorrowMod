using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.DataStructures;
using System;

namespace OvermorrowMod.Common.VanillaOverrides.Bow
{
    public abstract class HeldBow : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        /// <summary>
        /// Determines whether the bow consumes any ammo on use.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool CanConsumeAmmo(Player player) => true;

        /// <summary>
        /// The offsets for the top and bottom string endpoints, respectively.
        /// </summary>
        public virtual (Vector2, Vector2) StringPositions => (new Vector2(-5, 14), new Vector2(-5, -14));

        /// <summary>
        /// The offset for where the bow should be held. Defaults to <c>(13, 0)</c>.
        /// </summary>
        public virtual Vector2 PositionOffset => new Vector2(15, 0);

        /// <summary>
        /// The color of the bow's string.
        /// </summary>
        public virtual Color StringColor => Color.White;

        /// <summary>
        /// Determines whether the string of the bow ignores light.
        /// </summary>
        public virtual bool StringGlow => false;

        public virtual float ChargeSpeed => 1;
        public virtual float MaxChargeTime => 60;
        public virtual float ShootDelay => 30;
        public virtual float MaxSpeed => 12;

        /// <summary>
        /// Determines if the bow fires a unique type of arrow. Uses Projectile ID instead of Item ID.
        /// </summary>
        public virtual int ArrowType => ProjectileID.None;

        /// <summary>
        /// Determines what arrow type is needed in order to convert the arrows to if ArrowType is given.
        /// </summary>
        public virtual int ConvertArrow => ItemID.None;

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

        public Projectile LoadedArrow;
        public int LoadedArrowType;
        public int LoadedArrowItemType;
        private int AmmoSlotID;

        public Player player => Main.player[Projectile.owner];
        public ref float drawCounter => ref Projectile.ai[0];
        public ref float delayCounter => ref Projectile.ai[1];
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

            Player.CompositeArmStretchAmount stretchAmount = drawCounter < Math.Round(MaxChargeTime * 0.33) ? Player.CompositeArmStretchAmount.Full : Player.CompositeArmStretchAmount.Quarter;
            if (drawCounter > Math.Round(MaxChargeTime * 0.66f)) stretchAmount = Player.CompositeArmStretchAmount.None;

            player.SetCompositeArmFront(true, stretchAmount, Projectile.rotation - MathHelper.PiOver2);

            if (delayCounter > 0) delayCounter--;

            if (player.controlUseItem && drawCounter >= 0)
            {
                Projectile.timeLeft = 120;

                if (delayCounter != 0) return;

                if (drawCounter == 0) AutofillAmmoSlots();

                if (FindAmmo())
                {
                    if (drawCounter > MaxChargeTime) drawCounter = MaxChargeTime;
                    drawCounter += ChargeSpeed;
                }
            }
            else
            {
                if (drawCounter > 0)
                {
                    ShootArrow();
                    drawCounter = -6;
                }

                if (drawCounter < 0) drawCounter++;
            }
        }

        /// <summary>
        /// Loops through the ammo slots, loads in the first arrow found into the bow.
        /// </summary>
        /// <returns></returns>
        private bool FindAmmo()
        {
            #region Ammo Slots
            LoadedArrowItemType = -1;
            for (int i = 0; i <= 3; i++)
            {
                Item item = player.inventory[54 + i];
                if (item.type == ItemID.None || item.ammo != AmmoID.Arrow) continue;
                if (ConvertArrow != ItemID.None)
                {
                    if (item.type != ConvertArrow) continue;
                }

                if (ArrowType == ProjectileID.None)
                {
                    LoadedArrowType = item.shoot;
                    LoadedArrowItemType = item.type;
                }

                AmmoSlotID = 54 + i;

                return true;
            }
            #endregion


            if (LoadedArrowItemType == -1) Main.NewText("No ammo found.");

            return false;
        }

        /// <summary>
        /// Loops through the player's inventory and then places any suitable ammo types into the ammo slots if they are empty or the wrong ammo type.
        /// </summary>
        private void AutofillAmmoSlots()
        {
            for (int j = 0; j <= 3; j++) // Check if any of the ammo slots are empty or are not an arrow
            {
                Item ammoItem = player.inventory[54 + j];
                if (ammoItem.type != ItemID.None && ammoItem.ammo == AmmoID.Arrow) continue;

                // Loop through the player's inventory in order to find any useable ammo types to use
                for (int i = 0; i <= 49; i++)
                {
                    Item item = player.inventory[i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Arrow) continue;

                    //Main.NewText("Swapping " + i + " with " + (54 + j));

                    Item tempItem = ammoItem;
                    player.inventory[54 + j] = item;
                    player.inventory[i] = tempItem;

                    break;
                }
            }
        }

        private void ConsumeAmmo()
        {
            if (!CanConsumeAmmo(player)) return;

            player.inventory[AmmoSlotID].stack--;
        }

        private void ShootArrow()
        {
            float progress = Utils.Clamp(drawCounter, 0, MaxChargeTime) / MaxChargeTime;
            Vector2 arrowOffset = Vector2.Lerp(Vector2.UnitX * 20, Vector2.UnitX * 16, progress).RotatedBy(Projectile.rotation);
            Vector2 arrowPosition = player.MountedCenter + arrowOffset;

            Vector2 velocity = Vector2.Normalize(arrowPosition.DirectionTo(Main.MouseWorld));
            float speed = MathHelper.Lerp(1, MaxSpeed, progress);

            if (ArrowType == ProjectileID.None)
                Projectile.NewProjectile(null, arrowPosition, velocity * speed, LoadedArrowType, Projectile.damage, Projectile.knockBack, player.whoAmI);
            else
                Projectile.NewProjectile(null, arrowPosition, velocity * speed, ArrowType, Projectile.damage, Projectile.knockBack, player.whoAmI);

            ConsumeAmmo();

            delayCounter = ShootDelay;
            LoadedArrowItemType = -1;
        }

        private void DrawArrow(Color lightColor)
        {
            Vector2 arrowOffset = Vector2.Lerp(Vector2.UnitX * 20, Vector2.UnitX * 16, Utils.Clamp(drawCounter, 0, 40f) / 40f).RotatedBy(Projectile.rotation);
            Vector2 arrowPosition = player.MountedCenter + arrowOffset;

            Texture2D texture;
            if (ArrowType == ProjectileID.None)
            {
                if (LoadedArrowItemType == -1) return;

                texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + LoadedArrowType).Value;
                Main.spriteBatch.Draw(texture, arrowPosition - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2f, 0.75f, SpriteEffects.None, 1);
            }
            else
            {
                texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ArrowType).Value;
                Main.spriteBatch.Draw(texture, arrowPosition - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver2, texture.Size() / 2f, 0.75f, SpriteEffects.None, 1);
            }
        }

        private void DrawBow(Color lightColor)
        {
            Vector2 topPosition = Projectile.Center + StringPositions.Item1.RotatedBy(Projectile.rotation);
            Vector2 bottomPosition = Projectile.Center + StringPositions.Item2.RotatedBy(Projectile.rotation);

            Vector2 restingPosition = Vector2.UnitX * (drawCounter < 0 ? 12 : 10);
            Vector2 armOffset = Vector2.Lerp(restingPosition, Vector2.UnitX * -1, Utils.Clamp(drawCounter, 0, MaxChargeTime) / MaxChargeTime).RotatedBy(Projectile.rotation);
            Vector2 armPosition = player.MountedCenter + armOffset;

            Color color = StringGlow ? StringColor : StringColor * Lighting.Brightness((int)player.Center.X / 16, (int)player.Center.Y / 16);

            Utils.DrawLine(Main.spriteBatch, topPosition, armPosition, color, StringColor, 1.25f);
            Utils.DrawLine(Main.spriteBatch, bottomPosition, armPosition, color, StringColor, 1.25f);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBow(lightColor);
            DrawArrow(lightColor);

            return false;
        }
    }
}