using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class GraniteYoyoOrbit : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Content/Items/Weapons/Melee/GraniteYoyo/GraniteYoyoOrbit";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 0;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 HalfWidthHeight = new Vector2(Projectile.width / 2, Projectile.height / 2);
            for (int i = 0; Projectile.oldPos.Length > i; i++)
            {
                Vector2 pos = Projectile.oldPos[i] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + HalfWidthHeight;
                int ColorValue = ((255 / Projectile.oldPos.Length) * (Projectile.oldPos.Length - i));
                Color color = new Color(ColorValue, ColorValue, ColorValue, ColorValue);
                Main.spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("OvermorrowMod/Content/Items/Weapons/Melee/GraniteYoyo/GraniteYoyoOrbitTrail"), pos, new Rectangle(0, (Projectile.height + 2) * Projectile.frame, Projectile.width, Projectile.height), color, Projectile.rotation, HalfWidthHeight, (Projectile.scale / Projectile.oldPos.Length) * (Projectile.oldPos.Length - i), Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            return true;
        }
        public override void AI()
        {
            Projectile.timeLeft = 500;
        }
    }
}