using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class Crosshair : ModProjectile
    {
        public override string Texture => AssetDirectory.Textures + "Crosshair";
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crosshair");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 69420;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 200;
            Projectile.scale = 0.3f;
        }

        public override void AI()
        {
            if (Projectile.velocity != Vector2.Zero && Projectile.ai[0]++ == Projectile.ai[1])
            {
                Projectile.velocity = Vector2.Zero;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Crosshair").Value;
            Projectile.rotation += 0.06f;

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.1f);
            float scale = Projectile.scale * 2 * mult;

            Color color = Projectile.velocity == Vector2.Zero ? Color.Yellow : Color.Transparent;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0);

            return base.PreDraw(ref lightColor);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.extraUpdates = 0;
            Projectile.velocity = Vector2.Zero;
            return false;
        }
    }

    public class PlayerCrosshair : Crosshair
    {
        protected NPC ParentNPC;
        private bool MouseFired = false;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 5;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            ParentNPC = Main.npc[(int)Projectile.ai[0]];

            if (!player.active || player.dead)
            {
                player.ClearBuff(ModContent.BuffType<Steal>());
                Projectile.Kill();
            }

            if (player.HasBuff(ModContent.BuffType<Steal>()))
            {
                Projectile.timeLeft = 5;
            }

            if (player == Main.LocalPlayer)
            {
                if (!MouseFired)
                {
                    Projectile.Center = Main.MouseWorld;

                    if (Main.mouseLeft)
                    {
                        Main.NewText("fire artifact");

                        MouseFired = true;
                        ((DharuudMinion)ParentNPC.ModNPC).FiredArtifact = true;
                        ((DharuudMinion)ParentNPC.ModNPC).ShootPosition = Projectile.Center;
                        ParentNPC.ai[3] = 0;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Crosshair").Value;
            Projectile.rotation += 0.06f;

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.1f);
            float scale = Projectile.scale * 2 * mult;
            Color color = Color.Lerp(Color.Orange, Color.Yellow, (float)Math.Sin(Projectile.localAI[0]++ / 5f));

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0);

            return base.PreDraw(ref lightColor);
        }
    }
}
