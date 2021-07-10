using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects.Prim;

namespace OvermorrowMod.Projectiles
{
    public class Shiro : ModProjectile, ITrailEntity
    {
        public Type TrailType()
        {
            return typeof(ShiroTrail);
        }
        public Vector2 start;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulHostile;
        public override void SetDefaults()
        {
            projectile.timeLeft = 15;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.ai[1] == 0 && Main.myPlayer == projectile.owner)
            {
                projectile.ai[1] = 1;
                start = Main.MouseWorld - player.Center;
            }
            Vector2 trueStart = player.Center + start;
            bool channel = !player.CCed && !player.noItems && player.HeldItem.type == ModContent.ItemType<Items.Testing.DevGun>();
            if (channel)
            {
                float progress = (15f - projectile.timeLeft) / 15f;
                projectile.ai[0] = (progress - 0.5f) * (float)Math.PI;
                player.itemTime = 5;
                player.itemAnimation = 5;
                Vector2 direction = Vector2.Zero;
                if (Main.myPlayer == projectile.owner)
                {
                    direction = Vector2.Normalize(trueStart - player.Center);
                }
                if (direction.X > 0) player.direction = 1;
                else player.direction = -1;
                player.itemRotation = player.itemRotation = (float)Math.Atan2((double)(direction.Y * (float)player.direction), (double)(direction.X * (float)player.direction));
                projectile.Center = player.Center + direction.RotatedBy(projectile.ai[0] * player.direction) * 70f;
                if (++projectile.ai[0] % player.HeldItem.useTime == 0 && Main.myPlayer == projectile.owner)
                {
                    int type = Main.rand.NextBool() ? ProjectileID.SwordBeam : Main.rand.NextBool() ? ProjectileID.TerraBeam : ProjectileID.EnchantedBeam;
                    Vector2 vel = (trueStart - player.Center);
                    Vector2 pos = trueStart + vel.RotatedByRandom(Math.PI / 4).RotatedBy(Math.PI);
                    Projectile.NewProjectile(pos, Vector2.Normalize(trueStart - pos) * 10f, type, projectile.damage, 1f, projectile.owner);
                }
            }
            else
            {
                projectile.Kill();
            }
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            Vector2 trueStart = player.Center + start;
            if (Main.myPlayer == projectile.owner)
            {
                for (int i = 0; i < 10; i++)
                {
                    float startRot = (float)-Math.PI / 4;
                    float extraRot = (float)Math.PI / 2 * (float)i / 10f;
                    Vector2 direction = Vector2.Normalize(trueStart - player.Center).RotatedBy(startRot + extraRot);
                    Projectile.NewProjectile(player.Center, direction * 20, ProjectileID.SwordBeam, projectile.damage, 1f, projectile.owner);
                }
            }
        }
    }
}