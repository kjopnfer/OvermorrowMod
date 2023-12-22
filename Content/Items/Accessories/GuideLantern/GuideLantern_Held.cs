using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Common.VanillaOverrides.Bow;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.GuideLantern
{
    public class GuideLantern_Held : ModProjectile
    {
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanCutTiles() => false;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
        }

        public static bool IsPerformingAction(Player player)
        {
            return player.HeldItem.holdStyle == 1 || player.HeldItem.holdStyle == 2 || player.itemAnimation > 0
                || ModContent.GetModProjectile(player.HeldItem.shoot) is HeldGun || ModContent.GetModProjectile(player.HeldItem.shoot) is HeldBow || player.channel;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Vector2 offset = new Vector2(8 * player.direction, 10);
            Projectile.Center = player.Center + offset;

            if (IsPerformingAction(player) || player.wet || !player.GetModPlayer<OvermorrowModPlayer>().GuideLantern) Projectile.Kill();
            if (player.active) Projectile.timeLeft = 10;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (-MathHelper.PiOver2 + MathHelper.ToRadians(15))* player.direction);
            player.heldProj = Projectile.whoAmI;

            Lighting.AddLight(Projectile.Center, 0.85f, 0.7f, 0.7f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[ModContent.ProjectileType<GuideLantern_Held>()].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 1);

            return false;
        }
    }
}