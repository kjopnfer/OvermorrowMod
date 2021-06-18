using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    class WaterStaffProj : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_1";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lacusite Bolt");
        }
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.tileCollide = true;
            projectile.magic = true;
        }
        int timer = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            knockback = 0;
        }
        int direction = 1;
        List<Projectile> owned = new List<Projectile>();
        public override void AI()
        {
            if (projectile.ai[1] != 1)
            {
                Projectile owner = Main.projectile[(int)projectile.ai[0]];
                double deg = projectile.ai[1]++ * 2 * direction; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / 180); //Convert degrees to radians
                double dist = 10 * projectile.knockBack; //Distance away from the player

                /*Position the player projectiled on where the player is, the Sin/Cos of the angle times the /
                /distance for the desired distance away from the player minus the projectile's width   /
                /and height divided by two so the center of the projectile is at the right place.     */
                projectile.position.X = owner.Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2;
                projectile.position.Y = owner.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;
            }
            if (projectile.ai[1] == 3)
            {
                direction = -1;
            }
            if (projectile.ai[0] == 0)
            {
                projectile.ai[0]++;
                for (int i = 0; i < 3; i++)
                {
                    int direction ;
                    if (i == 0 || i == 2)
                    {
                        direction = 2;
                    }
                    else
                    {
                        direction = 3;
                    }
                 owned.Add(Main.projectile[Projectile.NewProjectile(projectile.Center, Microsoft.Xna.Framework.Vector2.Zero, projectile.type, projectile.damage, i, projectile.owner, projectile.whoAmI, direction)]);
                }
            }
            foreach (Projectile proj in owned)
                proj.ai[0] = projectile.whoAmI;
        }
           public float TrailSize(float progress)
        {
            return 32f * (1f - progress);
        }
        public override void Kill(int timeLeft)
        {
            foreach (Projectile proj in owned)
                proj.Kill();
        }

        public Color TrailColor(float progress)
        {
            //return Main.hslToRgb(progress, 0.75f, 0.5f) * (1f - progress);
            //return Main.DiscoColor;
            return Color.Lerp(Color.Cyan, Color.Blue, progress) * (1f - progress);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //     if (Main.netMode != NetmodeID.Server)
            ///        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Effects/Trail2");
            int length = 16;
            TrailHelper helper = new TrailHelper(projectile, TrailColor, TrailSize, length, "Texture", texture);
            helper.Draw();
            //     }
            return base.PreDraw(spriteBatch, lightColor);
        }
        
    }
}
