using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using OvermorrowMod.Effects.Prim;
using System;
using OvermorrowMod.Effects.Prim.Trails;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.WardenClass;
using OvermorrowMod.Effects.Prim.Trails.TorchVariants;

namespace OvermorrowMod.Projectiles.Artifact
{
    public class TorchBall : ArtifactProjectile, ITrailEntity
    {
        public Color projectileColor = new Color(255, 255, 255);
        public int dustId = 64;

        public Type TrailType()
        {
            if (projectile.ai[0] <= -1)
            {
                switch (projectile.ai[0])
                {
                    case -1:
                        return typeof(TorchTrail1);
                    case -2:
                        return typeof(TorchTrail2);
                    case -3:
                        return typeof(TorchTrail3);
                    case -4:
                        return typeof(TorchTrail4);
                    case -5:
                        return typeof(TorchTrail5);
                    case -6:
                        return typeof(TorchTrail6);
                    case -7:
                        return typeof(TorchTrail7);
                    case -8:
                        return typeof(TorchTrail8);
                    case -9:
                        return typeof(TorchTrail9);
                    case -10:
                        return typeof(TorchTrail10);
                    case -11:
                        return typeof(TorchTrail11);
                    case -12:
                        return typeof(TorchTrail12);
                }
            }
            
            return typeof(TorchTrail);
            
        }

        //public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Torch God's Wrath");
            Main.projFrames[projectile.type] = 9;
        }

        public override void SafeSetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 100;
            //projectile.alpha = 255;
            projectile.extraUpdates = 1;

            drawOffsetX = -15;
            drawOriginOffsetY = -2;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();

            if (projectile.ai[1] > 10)
            {
                if (projectile.ai[0] <= -1)
                {
                    projectile.velocity.Y += 0.17f;
                }
            }
            else
            {
                projectile.ai[1]++;
            }

            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Texture2D texture = Main.projectileTexture[projectile.type];
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/test");
            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), projectileColor, projectile.rotation, drawOrigin, projectile.scale * 0.8f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (/*projectile.ai[0] <= -1*/true)
            {
                projectile.alpha = 255;

                //Texture2D Texture = ModContent.GetTexture("OvermorrowMod/Projectiles/Artifact/GreyBall");
                Texture2D Texture = ModContent.GetTexture("OvermorrowMod/Textures/test");


                SpriteEffects spriteEffects = SpriteEffects.None;
                if (projectile.spriteDirection == -1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
                int startY = frameHeight * projectile.frame;
                Rectangle sourceRectangle = new Rectangle(0, startY, Texture.Width, frameHeight);
                Vector2 origin = sourceRectangle.Size() / 2f;

                /*Main.spriteBatch.Draw(Texture,
                    projectile.Center - Main.screenPosition,
                    sourceRectangle, projectileColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);*/
                /*Main.spriteBatch.Draw(Texture,
                    projectile.Center - Main.screenPosition,
                    null, projectileColor, projectile.rotation, Texture.Size() / 2, projectile.scale, spriteEffects, 0f);*/
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.ai[0] > -1)
            {
                return Color.White;
            }
            else
            {
                return null;
            }
        }

        public override void Kill(int timeLeft)
        {
            Vector2 origin = projectile.Center;
            float radius = 15;
            int numLocations = projectile.ai[0] > -1 ? 30 : 15;

            for (int i = 0; i < 30; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, (projectile.ai[0] > -1 ? 5f : 2f)).RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * (projectile.ai[0] > 1 ? Main.rand.Next(1, 4) : 1);

                Dust dust = Terraria.Dust.NewDustPerfect(position, projectile.ai[0] > -1 ? Main.rand.NextBool(3) ? 64 : 59 : dustId, new Vector2(dustvelocity.X, dustvelocity.Y), 0, projectileColor, projectile.ai[0] > -1 ? Main.rand.Next(1, 3) : 1);

            }

            if (projectile.ai[0] > -1)
            {
                for (int i = -1; i >= -12; i--)
                {
                    int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(-4, 4), Main.rand.Next(-10, -3), ModContent.ProjectileType<TorchBall>(), (projectile.damage / 2) - 1, 10f, Main.myPlayer, i);
                    Main.projectile[proj].tileCollide = false;
                    Main.projectile[proj].timeLeft = Main.rand.Next(60, 120);

                    switch (i)
                    {
                        case -1:
                            projectileColor = Color.Orange;
                            dustId = 127;
                            break;
                        case -2:
                            projectileColor = Color.Cyan;
                            dustId = 135;
                            break;
                        case -3:
                            projectileColor = Color.Purple;
                            dustId = 65;
                            break;
                        case -4:
                            projectileColor = Color.SpringGreen;
                            dustId = 61;
                            break;
                        case -5:
                            projectileColor = Color.Yellow;
                            dustId = 64;
                            break;
                        case -6:
                            projectileColor = Color.Red;
                            dustId = 60;
                            break;
                        case -7:
                            projectileColor = Color.Pink;
                            dustId = 69;
                            break;
                        case -8:
                            projectileColor = Color.LightCoral;
                            dustId = 59;
                            break;
                        case -9:
                            projectileColor = Color.Tan;
                            dustId = 64;
                            break;
                        case -10:
                            projectileColor = Color.LightCyan;
                            dustId = 56;
                            break;
                        case -11:
                            projectileColor = Color.Aqua;
                            dustId = 57;
                            break;
                        case -12:
                            projectileColor = Color.Lime;
                            dustId = 75;
                            break;

                    }

                    ((TorchBall)Main.projectile[proj].modProjectile).projectileColor = projectileColor;
                    ((TorchBall)Main.projectile[proj].modProjectile).dustId = dustId;
                }
            }

            Main.PlaySound(SoundID.Item14);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.ai[1] > 10 && projectile.ai[0] > -1)
            {
                return base.CanHitNPC(target);
            }
            else
            {
                return false;
            }
        }
    }
}