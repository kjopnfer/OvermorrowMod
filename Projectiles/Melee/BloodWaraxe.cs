using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Debuffs;

namespace OvermorrowMod.Projectiles.Melee
{
    public class BloodWaraxe : ModProjectile
    {
        private const string ChainTexturePath = "OvermorrowMod/Projectiles/Melee/CrimeraDraw";
        int okay1 = 0;
        int okay2 = 0;
        float CircleArr = 0;
        public override void SetDefaults()
        {
            projectile.width = 44;
            projectile.height = 44;
            projectile.timeLeft = 40;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        // This AI code is adapted from the aiStyle 15. We need to re-implement this to customize the behavior of our flail
        public override void AI()
        {
            projectile.rotation = (Main.player[projectile.owner].Center - projectile.Center).ToRotation();
            if (Main.MouseWorld.X < Main.player[projectile.owner].Center.X && okay2 < 1)
            {
                okay1++;
            }
            if (Main.MouseWorld.X > Main.player[projectile.owner].Center.X && okay1 < 1)
            {
                okay2++;
            }
            if (okay1 > 0)
            {
                NPC npc = Main.npc[(int)projectile.ai[0]];
                projectile.Center = npc.Center;
                projectile.position.X = 75 * (float)Math.Cos(CircleArr) + Main.player[projectile.owner].Center.X - 11f * 1.3f;
                projectile.position.Y = 75 * (float)Math.Sin(CircleArr) + Main.player[projectile.owner].Center.Y - 11f * 1.3f;
                CircleArr -= (float)((2 * Math.PI) / (Math.PI * 2 * 60 / 10)); // 200 is the speed, god only knows what dividing by 10 does
            }
            if (okay2 > 0)
            {
                NPC npc = Main.npc[(int)projectile.ai[0]];
                projectile.Center = npc.Center;
                projectile.position.X = -75 * (float)Math.Cos(CircleArr) + Main.player[projectile.owner].Center.X - 11f * 1.3f;
                projectile.position.Y = -75 * (float)Math.Sin(CircleArr) + Main.player[projectile.owner].Center.Y - 11f * 1.3f;
                CircleArr -= (float)((2 * Math.PI) / (Math.PI * 2 * -60 / 10)); // 200 is the speed, god only knows what dividing by 10 does
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);
            projectile.alpha = 0;
            var drawPosition = projectile.Center;

            Vector2 unit = Main.player[projectile.owner].Center - projectile.Center; // changing all endpoints it just how you change it, dont change other stuff it wont go well
            float length = unit.Length();
            unit.Normalize();
            for (float k = 0; k <= length; k += 14f)
            {
                Vector2 drawPos = projectile.Center + unit * k - Main.screenPosition;
                Color alpha = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));

                spriteBatch.Draw(chainTexture, drawPos, null, alpha, (Main.player[projectile.owner].Center - projectile.Center).ToRotation(), new Vector2(4f, 4f), 1f, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}