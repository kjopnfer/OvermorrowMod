using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.ID;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class GraniteYoyoProj : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Content/Items/Weapons/Melee/GraniteYoyo/GraniteYoyoProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unstable Throw");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; //15 :PoutingCat:
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 20f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 240f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 15f;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = 99;
            Projectile.extraUpdates = 0;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
        }
        public static bool HasOrbitProjectiles;
        const int OrbitingOrbs = 3;
        List<int> Orbs = new List<int>();
        double RotAdd = 0.0;
        public static double rot = 0.0;
        float distance = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 HalfWidthHeight = new Vector2(Projectile.width / 2, Projectile.height / 2);
            for (int i = 0; Projectile.oldPos.Length > i; i++)
            {
                Vector2 pos = Projectile.oldPos[i] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + HalfWidthHeight;
                int ColorValue = ((255 / Projectile.oldPos.Length) * (Projectile.oldPos.Length - i));
                Color color = new Color(ColorValue, ColorValue, ColorValue, ColorValue);
                Main.spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("OvermorrowMod/Content/Items/Weapons/Melee/GraniteYoyo/GraniteYoyoTrail"), pos, new Rectangle(0, (Projectile.height + 2) * Projectile.frame, Projectile.width, Projectile.height), color, Projectile.rotation, HalfWidthHeight, (Projectile.scale / Projectile.oldPos.Length) * (Projectile.oldPos.Length - i), Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            return true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
            decrease = 120;
            distance = MathHelper.Clamp(distance + 4f, 0, 36);
        }
        int decrease = 0;
        int framething;
        public override void PostAI()
        {
            Player player = Main.LocalPlayer;
            if (GraniteYoyo.ChannelTimer == 0)
            {
                rot = 0.0;
                for (int i = 0; OrbitingOrbs > i; i++)
                    Projectile.NewProjectileDirect(null, Main.LocalPlayer.Center, Vector2.Zero, ModContent.ProjectileType<GraniteYoyoOrbit>(), (int)(player.GetWeaponDamage(player.HeldItem) * 1.5f), 0f, player.whoAmI);
            }
            Orbs.Clear();
            framething = 0;
            for (int i = 0; Main.projectile.Length > i; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<GraniteYoyoOrbit>())
                {
                    Orbs.Add(i);
                    Main.projectile[i].frame = framething;
                    framething++;
                }
                if (framething == OrbitingOrbs)
                    break;
            }
            for (int i = 0; Orbs.Count > i; i++)
            {
                if (RotAdd < MathHelper.TwoPi)
                    RotAdd += 0.05;
                else
                    RotAdd = 0.0;
                rot = 0.0;
                rot += ((double)MathHelper.TwoPi / OrbitingOrbs) * (i + 1);
                rot += RotAdd;

                int Xpos = (int)(Projectile.Center.X + Math.Cos(rot) * distance);
                int Ypos = (int)(Projectile.Center.Y + Math.Sin(rot) * distance);

                Main.projectile[Orbs[i]].Center = new Vector2(Xpos, Ypos);
            }

            if (decrease == 0 || !player.channel)
            {
                if (distance > 0)
                    distance -= 2f;
            }
            else
                decrease--;

            /*Player player = Main.LocalPlayer;
            HasOrbitProjectiles = false;

            if (decrease == 0 || !player.channel)
            {
                if (distance > 0)
                    distance -= 2f;
            }
            else
                decrease--;
            for (int i = 0; Main.Projectile.Length > i; i++)
            {
                if (Main.Projectile[i].type == ModContent.ProjectileType<GraniteYoyoOrbit>() && Main.Projectile[i].active)
                {
                    HasOrbitProjectiles = true;
                    break;
                }
            }
            if (!HasOrbitProjectiles)
            {
                Orbs.Clear();
                HasOrbitProjectiles = true;
                for (int i = 0; OrbitingOrbs * (player.yoyoGlove ? 2 : 1) > i; i++)
                    Projectile.NewProjectileDirect(Main.LocalPlayer.Center, Vector2.Zero, ModContent.ProjectileType<GraniteYoyoOrbit>(), player.GetWeaponDamage(player.HeldItem), player.GetWeaponKnockback(player.HeldItem, 1f), player.whoAmI);

                Orbs.Clear();
                for (int i = 0; Main.Projectile.Length > i; i++)
                {
                    if (Main.Projectile[i].type == ModContent.ProjectileType<GraniteYoyoOrbit>())
                        Orbs.Add(i);
                }
            }
            for (int i = 0; Orbs.Count / (player.yoyoGlove ? 2 : 1) > i; i++)
            {
                if (RotAdd < MathHelper.TwoPi)
                    RotAdd += 0.05;
                else
                    RotAdd = 0.0;
                rot = 0.0;
                rot += ((double)MathHelper.TwoPi / 2) * (i + 1);
                rot += RotAdd;

                int Xpos = (int)(Projectile.Center.X + Math.Cos(rot) * distance);
                int Ypos = (int)(Projectile.Center.Y + Math.Sin(rot) * distance);

                Main.Projectile[Orbs[i]].Center = new Vector2(Xpos, Ypos);
                Main.Projectile[Orbs[i]].scale = 1.5f;
            }*/
        }
    }
}