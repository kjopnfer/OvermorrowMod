using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Effects.Prim.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public class NatureScythe : ModProjectile
    {
        private Vector2 storeVelocity;
        private bool reverseDirection = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;        //The recording mode, this tracks rotation
        }

        public override void SetDefaults()
        {
            projectile.width = 106;
            projectile.height = 124;
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 540;
        }

        public override void AI()
        {
            // Spawning dust
            if (projectile.ai[1] < 15)
            {
                Vector2 origin = projectile.Center;
                float radius = 15;
                int numLocations = 30;
                for (int i = 0; i < 30; i++)
                {
                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 15f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(position, 2, 2, DustID.TerraBlade, dustvelocity.X, dustvelocity.Y, 0, default, 1);
                    Main.dust[dust].noGravity = true;
                }
                projectile.ai[1]++;
            }

            projectile.rotation += .55f;

            if (projectile.ai[0] == 0)
            {
                int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<Shiro>(), 0, 0f, Main.myPlayer);
                ((Shiro)Main.projectile[proj].modProjectile).RotationCenter = projectile;

                proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<Shiro>(), 0, 0f, Main.myPlayer, 270);
                ((Shiro)Main.projectile[proj].modProjectile).RotationCenter = projectile;
                ((Shiro)Main.projectile[proj].modProjectile).Offset = MathHelper.Pi;

                storeVelocity = projectile.velocity;
                projectile.velocity = Vector2.Zero;
            }

            projectile.ai[0]++;

            if (!reverseDirection)
            {
                if (projectile.ai[0] == 60) // Begin accelerating
                {
                    projectile.velocity = storeVelocity;
                }

                if (projectile.ai[0] > 66)
                {
                    if (projectile.ai[0] % 15 == 0) // Increase velocity by 25% every 6 ticks
                    {
                        projectile.velocity *= 1.25f;
                    }
                }

                if (projectile.ai[0] == 140)
                {
                    reverseDirection = true;
                    projectile.velocity = Vector2.Zero;
                }
            }
            else
            {
                if (projectile.ai[0] == 161)
                {
                    // Reverse the direction
                    projectile.velocity = storeVelocity * -1;
                }

                if (projectile.ai[0] > 226)
                {
                    if (projectile.ai[0] % 15 == 0) // Increase velocity by 25% every 6 ticks
                    {
                        projectile.velocity *= 1.25f;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D16 = mod.GetTexture("NPCs/Bosses/TreeBoss/NatureScythe_Trail");

            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            Vector2 origin2 = drawRectangle.Size() / 2f;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = new Color(144, 255, 0);
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                //Main.spriteBatch.Draw(texture2D16, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(drawRectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            return false;
            //return base.PreDraw(spriteBatch, lightColor);
        }

        /*public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }*/
    }



    public class Shiro : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => Color.LightGreen;
        public float TrailSize(float progress) => 35;
        public Type TrailType()
        {
            return typeof(SpinTrail);

        }

        public Vector2 start;
        public Projectile RotationCenter;
        public float Offset = 0;

        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulHostile;
        public override void SetDefaults()
        {
            projectile.timeLeft = 540;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.friendly = false;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            //Player player = Main.player[projectile.owner];
            if (projectile.ai[1] == 0 && Main.myPlayer == projectile.owner)
            {
                projectile.ai[1] = 1;
                start = Main.MouseWorld - RotationCenter.Center;
            }

            Vector2 trueStart = RotationCenter.Center + start;
            //bool channel = !player.CCed && !player.noItems && player.HeldItem.type == ModContent.ItemType<Items.Testing.DevGun>();
            //if (channel)
            //{
            float progress = (15f - projectile.timeLeft) / 15f;
            projectile.ai[0] = (progress) * MathHelper.TwoPi;

            //player.itemTime = 5;
            //player.itemAnimation = 5;
            Vector2 direction = Vector2.Zero;
            /*if (Main.myPlayer == projectile.owner)
            {
                direction = Vector2.Normalize(trueStart - RotationCenter.Center);
            }*/
            direction = Vector2.Normalize(trueStart - RotationCenter.Center);

            //if (direction.X > 0) player.direction = 1;
            //else player.direction = -1;
            //player.itemRotation = player.itemRotation = (float)Math.Atan2((double)(direction.Y * (float)player.direction), (double)(direction.X * (float)player.direction));
            projectile.Center = RotationCenter.Center + direction.RotatedBy(projectile.ai[0] * RotationCenter.direction + Offset) * 70f;

            /*if (++projectile.ai[0] % player.HeldItem.useTime == 0 && Main.myPlayer == projectile.owner)
            {
                int type = Main.rand.NextBool() ? ProjectileID.SwordBeam : Main.rand.NextBool() ? ProjectileID.TerraBeam : ProjectileID.EnchantedBeam;
                Vector2 vel = (trueStart - player.Center);
                Vector2 pos = trueStart + vel.RotatedByRandom(Math.PI / 4).RotatedBy(Math.PI);
                Projectile.NewProjectile(pos, Vector2.Normalize(trueStart - pos) * 10f, type, projectile.damage, 1f, projectile.owner);
            }*/
            /*}
            else
            {
                projectile.Kill();
            }*/
        }
        public override void Kill(int timeLeft)
        {
            /*Player player = Main.player[projectile.owner];
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
            }*/
        }
    }

}