using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.SoulSaber
{
    public class SoulSpin : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => Color.Lerp(Color.Blue, Color.Cyan, progress) * progress;
        public float TrailSize(float progress) => 80;
        public Type TrailType()
        {
            return typeof(SpinTrail);

        }

        public override string Texture => AssetDirectory.Melee + "SoulSaber/SoulSaber";
        public override void SetDefaults()
        {
            Projectile.timeLeft = 360;
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[0]++ == 0 && Projectile.ai[1] != -1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, -1f);
            }

            if (player.channel)
            {
                if (Main.mouseRight && Projectile.ai[1] == 1)
                {
                    if (Projectile.localAI[0]++ == 240)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, player.Center);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.Normalize(player.DirectionTo(Main.MouseWorld)) * 4, ModContent.ProjectileType<SoulFlameRing>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, -1f);
                        Projectile.localAI[0] = 0;
                    }

                    Dust dust = Main.dust[Terraria.Dust.NewDust(player.Center - new Vector2(92, 76), 184, 152, DustID.Frost, 0f, -10f, 0, new Color(255, 255, 255), 0.5f)];
                    dust.noGravity = true;
                }

                Vector2 ProjectileRotation = (Vector2.UnitY * 80).RotatedBy(MathHelper.ToRadians(Projectile.ai[0] += 15 * player.direction)) * Projectile.ai[1];
                Projectile.Center = player.MountedCenter + ProjectileRotation;
                Projectile.rotation = Projectile.DirectionTo(player.MountedCenter).ToRotation() + MathHelper.PiOver4 + MathHelper.Pi;

                //player.itemTime = 5;
                //player.itemAnimation = 5;
                player.direction = player.direction == 1 ? 1 : -1;

                Projectile.timeLeft = 5;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //target.AddHex(Hex.HexType<SoulFlame>(), 60 * 10);
            target.AddBuff(ModContent.BuffType<SoulFlame>(), 60 * 10);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var player = Main.player[Projectile.owner];

            Vector2 mountedCenter = player.Center;
            Texture2D chainTexture = ModContent.Request<Texture2D>(AssetDirectory.Chains + "Bones").Value;

            var drawPosition = Projectile.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

            if (Projectile.alpha == 0)
            {
                int direction = -1;

                if (Projectile.Center.X < mountedCenter.X)
                    direction = 1;

                player.itemRotation = (float)Math.Atan2(remainingVectorToPlayer.Y * direction, remainingVectorToPlayer.X * direction);
            }

            // This while loop draws the chain texture from the Projectile to the player, looping to draw the chain texture along the path
            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                if (length < 25f || float.IsNaN(length))
                    break;

                // drawPosition is advanced along the vector back to the player by 24 pixels
                // 24 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
                drawPosition += remainingVectorToPlayer * 24 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                // Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
                Main.EntitySpriteDraw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}