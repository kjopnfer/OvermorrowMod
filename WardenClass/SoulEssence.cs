using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WardenClass
{
    public class SoulEssence : ModProjectile
    {
        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Essence");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 2;
        }

        public override void AI()
        {
            //Making player variable "p" set as the projectile's owner
            Player player = Main.player[projectile.owner];

            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            // Make sure the projectile does not naturally expire while active
            if (modPlayer.soulList.Count > 0)
            {
                projectile.timeLeft = 2;
            }

            /*projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int num1163 = 0; num1163 < 5; num1163++)
                {
                    int num1161 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 175, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num1161].noGravity = true;
                    Dust dust81 = Main.dust[num1161];
                    dust81.velocity *= 0f;
                }
            }*/

            if (player.dead)
            {
                projectile.Kill();
                return;
            }

            //Factors for calculations
            double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
            double rad = deg * (Math.PI / 180); //Convert degrees to radians
            double dist = projectile.ai[0]; //Distance away from the player

            /*Position the player projectiled on where the player is, the Sin/Cos of the angle times the /
			/distance for the desired distance away from the player minus the projectile's width   /
			/and height divided by two so the center of the projectile is at the right place.     */
            projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2;
            projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;

            //Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
            if (projectile.knockBack == 1)
            {
                projectile.ai[1] += 4f;
            }
            else
            {
                projectile.ai[1] -= 4f;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            /*Texture2D projTexture = ModContent.GetTexture("OvermorrowMod/Effects/Trail3");
            int length = 32;
            TrailHelper helper = new TrailHelper(projectile.oldPos, new TrailConfig
            {
                color = delegate (float progress)
                {
                    return Color.Lerp(Color.Cyan, Color.White, progress) * (1f - progress);
                },
                size = delegate (float progress)
                {
                    return 16f * (1f - progress);
                },
                Length = length,
                texture = projTexture,
                TAlpha = true
            });
            helper.Draw();*/
            PrimitivePacket packet = new PrimitivePacket();
            packet.type = PrimitiveType.TriangleList;
            packet.pass = "Basic";
            packet.Add(projectile.Center - Vector2.UnitX * 100f, Color.Red, Vector2.Zero);
            packet.Add(projectile.Center + Vector2.UnitX * 100f, Color.Red, Vector2.Zero);
            packet.Add(projectile.Center - Vector2.UnitY * 100f, Color.Red, Vector2.Zero);
            packet.Send();

            return base.PreDraw(spriteBatch, lightColor);
        }
    }
}