using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class Crosshair : ModProjectile
    {
        public override bool CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crosshair");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.alpha = 255;
            projectile.timeLeft = 69420;
            projectile.penetrate = -1;
            projectile.extraUpdates = 200;
        }

        public override void AI()
        {
            if (projectile.velocity != Vector2.Zero && projectile.ai[0]++ == projectile.ai[1])
            {
                projectile.velocity = Vector2.Zero;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/SandstormBoss/Crosshair");
            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            projectile.rotation += 0.06f;

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTime * 2) * 0.1f);
            float scale = projectile.scale * 3 * mult;

            Color color = projectile.velocity == Vector2.Zero ? Color.Yellow : Color.Transparent;
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, drawRectangle, color, projectile.rotation, new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2), scale, SpriteEffects.None, 0f);

            return base.PreDraw(spriteBatch, lightColor);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.extraUpdates = 0;
            projectile.velocity = Vector2.Zero;
            return false;
        }
    }

    public class PlayerCrosshair : ModProjectile
    {
        protected NPC ParentNPC;
        private bool MouseFired = false;
        public override bool CanDamage() => false;
        public override string Texture => "OvermorrowMod/NPCs/Bosses/SandstormBoss/Crosshair";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crosshair");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.timeLeft = 5;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            ParentNPC = Main.npc[(int)projectile.ai[0]];

            if (!player.active || player.dead)
            {
                player.ClearBuff(ModContent.BuffType<Steal>());
                projectile.Kill();
            }

            if (player.HasBuff(ModContent.BuffType<Steal>()))
            {
                projectile.timeLeft = 5;
            }

            if (player == Main.LocalPlayer)
            {
                if (!MouseFired)
                {
                    projectile.Center = Main.MouseWorld;

                    if (Main.mouseLeft)
                    {
                        Main.NewText("fire artifact");

                        MouseFired = true;
                        ((DharuudMinion)ParentNPC.modNPC).FiredArtifact = true;
                        ((DharuudMinion)ParentNPC.modNPC).ShootPosition = projectile.Center;
                        ParentNPC.ai[3] = 0;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/SandstormBoss/Crosshair");
            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            projectile.rotation += 0.06f;

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTime * 2) * 0.1f);
            float scale = projectile.scale * 3 * mult;
            Color color = Color.Lerp(Color.Orange, Color.Yellow, (float)Math.Sin(projectile.localAI[0]++ / 5f));

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, drawRectangle, color, projectile.rotation, new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2), scale, SpriteEffects.None, 0f);

            return base.PreDraw(spriteBatch, lightColor);
        }
    }
}
