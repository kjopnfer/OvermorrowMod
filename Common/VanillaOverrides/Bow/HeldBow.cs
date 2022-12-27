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
using Terraria.DataStructures;

namespace OvermorrowMod.Common.VanillaOverrides.Bow
{
    public abstract class HeldBow : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;


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

        public virtual void SafeSetDefaults() { }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1;
            Projectile.hide = true; 

            SafeSetDefaults();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public virtual int ArrowType => ProjectileID.None;
        public Projectile LoadedArrow;
        public int LoadedArrowType;

        public override void OnSpawn(IEntitySource source)
        {
            int ammoItemType = (source as EntitySource_ItemUse_WithAmmo)?.AmmoItemIdUsed ?? ItemID.None;
            Item ammoItem = ammoItemType != ItemID.None ? ContentSamples.ItemsByType[ammoItemType] : null;

            EntitySource_ItemUse_WithAmmo itemSource = source as EntitySource_ItemUse_WithAmmo;

            int arrowType = ArrowType;
            if (arrowType == ProjectileID.None)
            {
                arrowType = ammoItem?.shoot ?? ProjectileID.None;
            }

            /*LoadedArrow = Projectile.NewProjectileDirect(source, Projectile.Center, Vector2.Zero, arrowType, (itemSource?.Item.damage + (ammoItem?.damage ?? 0)) ?? 0, itemSource?.Item.knockBack ?? 0, player.whoAmI);
            LoadedArrowType = LoadedArrow.type;

            LoadedArrow.netUpdate = true;

            Main.NewText("Type: " + LoadedArrow.Name);*/

            base.OnSpawn(source);
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
                Projectile.timeLeft = 120;
            }
            else
            {
                if (drawCounter > 0) drawCounter -= 2;
                if (drawCounter < 0) drawCounter = 0;
            }
        }

        private void FindAmmo()
        {
            //Main.NewText("Inventory Type: " + player.inventory[54].Name);
            //Main.NewText("Inventory Type: " + player.inventory[55].Name);
            //Main.NewText("Inventory Type: " + player.inventory[56].Name);
            //Main.NewText("Inventory Type: " + player.inventory[57].Name);

            LoadedArrowType = player.inventory[54].shoot;
            //Main.NewText(LoadedArrowType);
        }

        public void DrawArrow(Color lightColor)
        {

            Vector2 arrowOffset = Vector2.Lerp(Vector2.UnitX * 20, Vector2.UnitX * 16, Utils.Clamp(drawCounter, 0, 40f) / 40f).RotatedBy(Projectile.rotation);
            Vector2 arrowPosition = player.MountedCenter + arrowOffset;

            //LoadedArrow.Center = arrowPosition;
            //LoadedArrow.rotation = Projectile.rotation + MathHelper.PiOver2;

            FindAmmo();

            //Texture2D texture = TextureAssets.Projectile[LoadedArrowType].Value;
            Texture2D texture = TextureAssets.Item[player.inventory[54].type].Value;

            Main.spriteBatch.Draw(texture, arrowPosition - Main.screenPosition, null, lightColor, Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2f, 0.75f, SpriteEffects.None, 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 topPosition = Projectile.Center + StringPositions.Item1.RotatedBy(Projectile.rotation);
            Vector2 bottomPosition = Projectile.Center + StringPositions.Item2.RotatedBy(Projectile.rotation);

            Vector2 armOffset = Vector2.Lerp(Vector2.UnitX * 10, Vector2.UnitX * 1, Utils.Clamp(drawCounter, 0, 40f) / 40f).RotatedBy(Projectile.rotation);
            Vector2 armPosition = player.MountedCenter + armOffset;

            Color color = StringGlow ? StringColor : lightColor.MultiplyRGBA(StringColor);

            Utils.DrawLine(Main.spriteBatch, topPosition, armPosition, color, StringColor, 1.25f);
            Utils.DrawLine(Main.spriteBatch, bottomPosition, armPosition, color, StringColor, 1.25f);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 1);

            DrawArrow(lightColor);

            return false;
        }
    }
}